using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using PoolGame.Properties;
using System.Drawing;

namespace PoolGame
{
	static class GameManager
	{
		private static MainForm form;

		// in pixels (for the form)
		private const int START_X = 25, START_Y = 25, // table offset from top left of form
			TABLE_W_PIX = 444, TABLE_H_PIX = 834, BORDER_WIDTH = 15,
			PLAYAREA_W_PIX = TABLE_W_PIX - 2 * BORDER_WIDTH, PLAYAREA_H_PIX = TABLE_H_PIX - 2 * BORDER_WIDTH;

		// pool ball pictureboxes to be moved for UI 
		public static PictureBox[] ballPics;

		// in SI units (for the game simulation)
		public const float TABLE_WIDTH = 1.2192f; // width of playing area of table in m
		public const float TABLE_HEIGHT = 2.4384f; // height of playing area of table in m
		public const float COEFF_FRICTION = 0.25f;
		public static int tickInterval;	

		public static PoolCue Cue { get; private set; }
		// [0]: cue ball, [1-15]: num balls, [16-21]: walls
		public static ICollider[] Colliders { get; private set; }
		// true when balls are moving, false when the player must shoot
		public static bool InPlay { get; set; }

		public static void BeginGame(MainForm f, int tickInt)
		{
			form = f;
			tickInterval = tickInt;
			CreateBalls();

			Colliders = new ICollider[16 + 6];

			Colliders[0] = new CueBall(ballPics[0]);
			((Ball)Colliders[0]).MovePictureBox(true);
			
			Cue = new PoolCue((CueBall)Colliders[0]);
			for (int i = 1; i <= 15; i++)
			{
				Colliders[i] = new NumberBall(ballPics[i], i);
				((Ball)Colliders[i]).MovePictureBox(true);
			}

			for (int i = 16; i <= 21; i++)
			{
				Colliders[i] = new Wall((Side)(i - 16));
			}
		}

		private static void CreateBalls()
		{
			ballPics = new PictureBox[16];
			ballPics[0] = new PictureBox() { Name = "cueBall", Image = Resources.cueball };
			ballPics[1] = new PictureBox() { Name = "ball1", Image = Resources._1ball };
			ballPics[2] = new PictureBox() { Name = "ball2", Image = Resources._2ball };
			ballPics[3] = new PictureBox() { Name = "ball3", Image = Resources._3ball };
			ballPics[4] = new PictureBox() { Name = "ball4", Image = Resources._4ball };
			ballPics[5] = new PictureBox() { Name = "ball5", Image = Resources._5ball };
			ballPics[6] = new PictureBox() { Name = "ball6", Image = Resources._6ball };
			ballPics[7] = new PictureBox() { Name = "ball7", Image = Resources._7ball };
			ballPics[8] = new PictureBox() { Name = "ball8", Image = Resources._8ball };
			ballPics[9] = new PictureBox() { Name = "ball9", Image = Resources._9ball };
			ballPics[10] = new PictureBox() { Name = "ball10", Image = Resources._10ball };
			ballPics[11] = new PictureBox() { Name = "ball11", Image = Resources._11ball };
			ballPics[12] = new PictureBox() { Name = "ball12", Image = Resources._12ball };
			ballPics[13] = new PictureBox() { Name = "ball13", Image = Resources._13ball };
			ballPics[14] = new PictureBox() { Name = "ball14", Image = Resources._14ball };
			ballPics[15] = new PictureBox() { Name = "ball15", Image = Resources._15ball };

			const int DIMENSION = (int)(2 * Ball.RADIUS * (PLAYAREA_W_PIX / TABLE_WIDTH));
			Color tableColor = Color.FromArgb(76, 176, 80);
			foreach (PictureBox b in ballPics)
			{
				b.Size = new Size(DIMENSION, DIMENSION);
				b.SizeMode = PictureBoxSizeMode.Zoom;
				b.Location = new Point(0, 0);
				b.BackColor = tableColor;
				b.Visible = true;
				form.Controls.Add(b);
			}
		}

		// moves balls to next frame
		public static void MoveBalls()
		{
			foreach (Ball b in Colliders)
			{
				if(b.Velocity != Vector2.Zero)
				{
					// a ball has been detected to be moving
					goto completed;
				}
			}

			// no balls are moving
			foreach(Ball b in Colliders)
			{
				// correct ball positions for next frame
				b.MovePictureBox();
			}
			InPlay = false;
			return;

		completed:

			// portion of one frame's trajectory which each ball has passed through
			float u = 0f;
			do
			{ 				
				Ball colliderBall; // 'attacker' ball which collides with another object					   
				ICollider objectCollidedWith; // 'victim' object in collision
				(colliderBall, objectCollidedWith) = FindEarliestCollision(ref u);

				// move every ball to shortest u
				foreach (Ball ball in Colliders)
				{
					ball.Move(u);
				}

				// collision detected
				if (objectCollidedWith != null)
				{
					colliderBall.Collide(objectCollidedWith);
				}
			}
			while (u < 1f);
		}

		// finds earliest collision between 2 objects (if there is a collision); if no collision returns null references
		private static (Ball, ICollider) FindEarliestCollision(ref float minCompletion)
		{
			// shortest portion of balls' single-frame trajectories crossed when a collision is detected anywhere
			float shortest = 1f;
			// ball which collides with something the earliest
			Ball colliderBall = null;
			// object which participates in the earliest collision
			ICollider objectCollidedWith = null;

			foreach (Ball ball in Colliders)
			{
				if (ball.Velocity != Vector2.Zero)
				{
					foreach (ICollider obstacle in Colliders)
					{
						if (obstacle != ball)
						{
							// collision will occur
							float? pathCompleted = obstacle.CollisionDistance(ball);
							// finds object whose collision occurs before any other collider analyzed
							if (pathCompleted != null && pathCompleted.Value < shortest && pathCompleted.Value > minCompletion)
							{
								shortest = pathCompleted.Value;
								colliderBall = ball;
								objectCollidedWith = obstacle;
							}
						}
					}
				}
			}

			minCompletion = shortest;
			return (colliderBall, objectCollidedWith);
		}

		public static Vector2 FormToTablePoint(Point screenPoint)
		{
			Vector2 pointVector = new Vector2(screenPoint.X, screenPoint.Y);
			// ratio of one pixel length to one game unit (in meters)
			const float P2M_COEFF = TABLE_WIDTH / PLAYAREA_W_PIX;

			// mouse location relative to top-left of pool table playing area
			Vector2 fromTL = pointVector - new Vector2(START_X + BORDER_WIDTH, START_Y + BORDER_WIDTH);

			Vector2 tablePoint = new Vector2(fromTL.X * P2M_COEFF, (PLAYAREA_H_PIX - fromTL.Y) * P2M_COEFF);
			return tablePoint;
		}

		// converts pixel location on form to position on board in game units; (0,0) = bottom left of board
		public static Point TableToFormPoint(Vector2 location)
		{
			// ratio of one game unit (in meters) to pixel length
			const float M2P_COEFF = PLAYAREA_W_PIX / TABLE_WIDTH;
			// pixel equivalent of vector location (not accounting for table offset from form)
			int tableXPix = (int)(location.X * M2P_COEFF), tableYPix = (int)(location.Y * M2P_COEFF);

			Point screenPoint = new Point(tableXPix + START_X + BORDER_WIDTH, PLAYAREA_H_PIX - tableYPix + START_Y + BORDER_WIDTH);
			return screenPoint;
		}
	}
}
