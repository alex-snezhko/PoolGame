using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Numerics;
using PoolGame.Properties;
using System.Drawing;

namespace PoolGame
{
	static class GameManager
	{
		static MainForm form;

		// relevant measurements from form (in pixels)
		public const int START_X = 30, START_Y = 30, // table offset from top left of form
			PLAYAREA_W_PIX = 400, PLAYAREA_H_PIX = 800, BORDER_WIDTH = 33; //4"

		// relevant figures for game simulation calculations (in SI units)
		public const float TABLE_WIDTH = 1.2192f; // width of playing area of table in m (4')
		public const float TABLE_HEIGHT = 2.4384f; // height of playing area of table in m (8')
		public const float COEFF_FRICTION = 0.2f; // coefficient of kinetic friction between ball and table

		// interval between which game events are processed (also directly related to framerate)
		public static float TickInterval { get; private set; }	

		// cue object which controls striking cue ball at beginning of play
		public static PoolCue Cue { get; private set; }
		// [0]: cue ball, [1-15]: num balls, [16-21]: walls
		public static List<ICollider> Colliders { get; private set; }
		// keeps list of balls actively in play (not pocketed)
		public static List<Ball> ActiveBalls { get; private set; }
		// reference to cue ball object; same as Colliders[0] if cue ball is not scratched
		static CueBall cueBall;

		// true when balls are moving, false when the player must shoot
		public static bool BallsMoving { get; set; }
		// true if cue ball was scratched this shot
		public static bool Scratched { get; set; }
		// number of balls that have been pocketed this game
		public static int BallsPocketed { get; set; }

		// pool ball pictureboxes to be moved for UI 
		static PictureBox[] ballImages;
		// image of crosshair that appears when pointing at location to place scratched cue ball
		static PictureBox imgCrosshair;

		// begins or resets game
		public static void BeginGame(MainForm f, int tickInt)
		{
			form = f;
			TickInterval = tickInt / 1000f;

			// create GUI images
			CreateBallImages();
			imgCrosshair = new PictureBox()
			{
				Image = Resources.crosshair,
				Enabled = false,
				Size = new Size(20, 20),
				SizeMode = PictureBoxSizeMode.Zoom,
				BackColor = Color.FromArgb(145, 196, 125),
				Visible = false
			};
			form.Controls.Add(imgCrosshair);

			// 16 balls, 6 walls, 6 pockets in Colliders
			Colliders = new List<ICollider>();
			ActiveBalls = new List<Ball>();

			// add balls to colliders (and list of balls)
			cueBall = new CueBall(ballImages[0]);
			Colliders.Add(cueBall);
			ActiveBalls.Add(cueBall);
			for (int i = 1; i <= 15; i++)
			{
				NumberBall nb = new NumberBall(i, ballImages[i]);
				Colliders.Add(nb);
				ActiveBalls.Add(nb);
			}

			// add walls to colliders
			Colliders.Add(new Wall(Side.Top));
			Colliders.Add(new Wall(Side.Top | Side.Left));
			Colliders.Add(new Wall(Side.Top | Side.Right));
			Colliders.Add(new Wall(Side.Bottom | Side.Left));
			Colliders.Add(new Wall(Side.Bottom | Side.Right));
			Colliders.Add(new Wall(Side.Bottom));

			// add pockets to colliders
			Colliders.Add(new Pocket(Side.Top | Side.Left));
			Colliders.Add(new Pocket(Side.Top | Side.Right));
			Colliders.Add(new Pocket(Side.Left));
			Colliders.Add(new Pocket(Side.Right));
			Colliders.Add(new Pocket(Side.Bottom | Side.Left));
			Colliders.Add(new Pocket(Side.Bottom | Side.Right));

			Cue = new PoolCue(cueBall);
		}

		private static void CreateBallImages()
		{
			ballImages = new PictureBox[16];
			ballImages[0] = new PictureBox() { Name = "cueBall", Image = Resources.cueball };
			ballImages[1] = new PictureBox() { Name = "ball1", Image = Resources._1ball };
			ballImages[2] = new PictureBox() { Name = "ball2", Image = Resources._2ball };
			ballImages[3] = new PictureBox() { Name = "ball3", Image = Resources._3ball };
			ballImages[4] = new PictureBox() { Name = "ball4", Image = Resources._4ball };
			ballImages[5] = new PictureBox() { Name = "ball5", Image = Resources._5ball };
			ballImages[6] = new PictureBox() { Name = "ball6", Image = Resources._6ball };
			ballImages[7] = new PictureBox() { Name = "ball7", Image = Resources._7ball };
			ballImages[8] = new PictureBox() { Name = "ball8", Image = Resources._8ball };
			ballImages[9] = new PictureBox() { Name = "ball9", Image = Resources._9ball };
			ballImages[10] = new PictureBox() { Name = "ball10", Image = Resources._10ball };
			ballImages[11] = new PictureBox() { Name = "ball11", Image = Resources._11ball };
			ballImages[12] = new PictureBox() { Name = "ball12", Image = Resources._12ball };
			ballImages[13] = new PictureBox() { Name = "ball13", Image = Resources._13ball };
			ballImages[14] = new PictureBox() { Name = "ball14", Image = Resources._14ball };
			ballImages[15] = new PictureBox() { Name = "ball15", Image = Resources._15ball };

			const int DIMENSION = (int)(2 * Ball.RADIUS * (PLAYAREA_W_PIX / TABLE_WIDTH));
			Color tableColor = Color.FromArgb(145, 196, 125);
			foreach (PictureBox b in ballImages)
			{
				b.Enabled = false;
				b.Size = new Size(DIMENSION, DIMENSION);
				b.SizeMode = PictureBoxSizeMode.Zoom;
				b.BackColor = tableColor;
				form.Controls.Add(b);
			}
		}

		// moves balls to next frame (taking collisions that occur during the frame into account)
		public static void MoveBalls()
		{
			// does not calculate movement and returns game to shooting phase if all balls have stopped moving			
			foreach (Ball b in ActiveBalls)
			{
				if (b.Velocity != Vector2.Zero)
				{
					// a ball has been detected to be moving
					goto ballMoving;
				}
			}
			BallsMoving = false;
			return;

		ballMoving:

			// portion of one frame's trajectory which each ball has passed through
			float u = 0f;
			/* continuously finds earliest collision (if there is one), 
			 * moves all balls to the positions they were in during collision, 
			 * then adjusts velocities of balls involved in collision */
			do
			{ 				
				// 'attacker' ball and 'victim' object in collision
				(Ball colliderBall, ICollider objectCollidedWith) = FindEarliestCollision(ref u);

				// move every ball to shortest u
				foreach (Ball b in ActiveBalls)
				{
					b.Move(u);
				}

				// collision detected
				if (objectCollidedWith != null)
				{
					objectCollidedWith.Collide(colliderBall);
				}
			}
			while (u < 1f);

			// move all ball picture boxes once full trajectory has been completed
			foreach (Ball b in ActiveBalls)
			{
				b.MovePictureBox();
			}
		}

		// finds earliest collision between 2 objects (if there is a collision); if no collision this frame returns null references
		// minU represents the u value that the balls have all already traveled through
		private static (Ball, ICollider) FindEarliestCollision(ref float completedU)
		{
			// shortest portion of balls' single-frame trajectories crossed when a collision is detected anywhere
			float shortestU = 1f;
			// ball which collides with something the earliest
			Ball colliderBall = null;
			// object which participates in the earliest collision
			ICollider objectCollidedWith = null;

			// creates list of moving balls (only these need to be checked for collisions)
			List<Ball> movingBalls = new List<Ball>();
			foreach (Ball b in ActiveBalls)
			{
				if(b.Velocity != Vector2.Zero)
				{
					movingBalls.Add(b);
				}
			}

			// checks every ball against every other object to see if it will incur a collision this frame
			foreach (Ball ball in movingBalls)
			{
				foreach (ICollider obstacle in Colliders)
				{
					// prevents checking same collision twice and checking collision of ball against itself
					bool alreadyChecked = obstacle == colliderBall && ball == objectCollidedWith;
					if (obstacle == ball || alreadyChecked)
					{
						continue;
					}

					float u = obstacle.CollisionDistance(ball);
					// finds object whose collision occurs before any other collider analyzed
					bool thisFrame = !float.IsNaN(u) && u > 0f && u <= 1f;
					// collision will occur
					if (thisFrame && u < shortestU && u > completedU)
					{
						shortestU = u;
						colliderBall = ball;
						objectCollidedWith = obstacle;
					}
					// TODO: balls still sometimes pass through each other (although rare)
				}
			}

			completedU = shortestU;
			return (colliderBall, objectCollidedWith);
		}

		public static void MoveCrossHair(Point newLocation)
		{
			imgCrosshair.Visible = true;
			Vector2 mouseTablePos = FormToTablePoint(newLocation);

			// do nothing if mouse position is not in bounds of table where ball may be placed
			const float MAX_OFFSET = Ball.RADIUS + Wall.BANK_WIDTH;
			if (mouseTablePos.X < MAX_OFFSET || mouseTablePos.X >= TABLE_WIDTH - MAX_OFFSET
				|| mouseTablePos.Y < MAX_OFFSET || mouseTablePos.Y >= TABLE_HEIGHT - MAX_OFFSET)
			{
				imgCrosshair.Visible = false;
				return;
			}			

			imgCrosshair.Location = newLocation - new Size(imgCrosshair.Width / 2, imgCrosshair.Height / 2);
		}

		// place cue ball in new location after scratch
		public static void PlaceCueBall(Point newLocation)
		{
			// makes sure the ball is being placed in a valid location
			if (imgCrosshair.Visible)
			{
				ActiveBalls.Insert(0, cueBall);
				cueBall.PlaceBall(FormToTablePoint(newLocation));

				Scratched = false;
				imgCrosshair.Visible = false;
				return;
			}
		}

		// converts location on pool table to point on form
		public static Vector2 FormToTablePoint(Point screenPoint)
		{
			Vector2 pointVector = new Vector2(screenPoint.X, screenPoint.Y);
			// ratio of one pixel length to one game unit (in meters)
			const float P2M_COEFF = TABLE_WIDTH / PLAYAREA_W_PIX;

			// mouse location relative to top-left of pool table playing area
			Vector2 fromTableTL = pointVector - new Vector2(START_X + BORDER_WIDTH, START_Y + BORDER_WIDTH);

			Vector2 tablePoint = new Vector2(fromTableTL.X * P2M_COEFF, (PLAYAREA_H_PIX - fromTableTL.Y) * P2M_COEFF);
			return tablePoint;
		}

		// converts pixel location on form to position on table in game units; (0,0) = bottom left of board
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
