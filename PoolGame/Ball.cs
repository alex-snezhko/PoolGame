using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing;

namespace PoolGame
{
	abstract class Ball : ICollider
	{
		private readonly PictureBox ballPic;

		// x, y; using pool table size of 1.2192m x 2.4384m (4'x8') with short side as x
		public Vector2 Position { get; protected set; }
		public Vector2 Velocity { get; protected set; } // in m/s

		protected abstract float Mass { get; } // in kg
		public const float RADIUS = 0.028575f; // in m

		// how much of game frame's trajectory path this ball has completed
		private float pathCompletedInFrame = 0f;

		protected Ball(PictureBox pic)
		{
			ballPic = pic;
			Velocity = Vector2.Zero;
		}

		public ValueTuple<Vector2, Vector2> GetTrajectoryVector() => new ValueTuple<Vector2, Vector2>(Position, Position + Velocity * GameManager.tickInterval);

		// moves ball distance it would travel in one frame; only called if no collision was detected by CollidingWith()
		// parameter determines how much of its single-frame trajectory this ball should move
		public void Move(float completed)
		{
			if(Velocity == Vector2.Zero)
			{
				return;
			}

			ValueTuple<Vector2, Vector2> t = GetTrajectoryVector();
			Vector2 finalPos = t.Item1 + (t.Item2 - t.Item1) * (pathCompletedInFrame - completed);

			Position = finalPos;
			Velocity = ApplyFriction(Velocity, GameManager.tickInterval * (pathCompletedInFrame - completed));

			pathCompletedInFrame = completed == 1f ? 0f : completed;

			Vector2 ApplyFriction(Vector2 vel, float time)
			{
				Vector2 newVel = vel;
				float deltaV = GameManager.COEFF_FRICTION * 9.81f * time;
				return deltaV > Velocity.Length() ? Vector2.Zero : vel - deltaV * Vector2.Normalize(vel);
			}
		}

		public void Collide(ICollider other)
		{
			if (other is Ball otherBall)
			{
				ValueTuple<Vector2, Vector2> vels = MathFuncs.ElasticCollisionVels(Mass, Velocity, otherBall.Mass, otherBall.Velocity);
				Velocity = vels.Item1;
				otherBall.Velocity = vels.Item2;
			}
			else if (other is Wall wall)
			{
				(ValueTuple<Vector2, Vector2> diag1,
					ValueTuple<Vector2, Vector2> main,
					ValueTuple<Vector2, Vector2> diag2) = wall.GetWallComponents();

				// find direction from wall piece hit to ball
				Vector2 toWallUnit;
				Vector2 fromD1 = MathFuncs.SmallestDistanceVector(diag1, Position);
				Vector2 fromMain = MathFuncs.SmallestDistanceVector(main, Position);
				Vector2 fromD2 = MathFuncs.SmallestDistanceVector(diag2, Position);
				float dD1 = fromD1.Length(), dM = fromMain.Length(), dD2 = fromD2.Length();
				if (dD1 < dM && dD1 < dD2) { toWallUnit = Vector2.Normalize(fromD1); }
				else if(dM < dD1 && dM < dD2) { toWallUnit = Vector2.Normalize(fromMain); }
				else { toWallUnit = Vector2.Normalize(fromD2); }

				// finds vector normal to the wall which can be added to current ball velocity to obtain new velocity vector
				float len = -2 * (Velocity.X * toWallUnit.X + Velocity.Y * toWallUnit.Y) / toWallUnit.LengthSquared();
				Vector2 normal = toWallUnit * len;

				Velocity += normal;
			}
		}

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		// note: this -> 'victim' ball, other -> likely the ball doing the colliding; moves the ball if a collision is detected
		public float? CollisionDistance(Ball ball)
		{
			ValueTuple<Vector2, Vector2> traj = GetTrajectoryVector();
			ValueTuple<Vector2, Vector2> otherTraj = ball.GetTrajectoryVector();

			// smallest distance between the trajectories of the two balls in collision
			float smallestDist = Velocity == Vector2.Zero ?
				MathFuncs.SmallestDistance(otherTraj, Position) :
				MathFuncs.SmallestDistance(otherTraj, traj);

			if (smallestDist > 2 * RADIUS)
			{
				return null;
			}

			float completed = MathFuncs.PathCompletedWhenMovingPointsCollided(otherTraj, traj, 2 * RADIUS);
			return completed;
		}

		// init parameter allows for placing this ball on the screen at game startup
		public void MovePictureBox(bool init = false)
		{
			// skips this chunk if this ball is being moved for game initialization
			if (!init)
			{
				// if ball has not moved this frame then return to avoid excess calculations
				(Vector2 initial, Vector2 final) = GetTrajectoryVector();
				if (initial == final)
				{
					return;
				}
			}

			// picturebox location based off top left point; make correction from position (which is center of ball)
			Point ballTopLeft = GameManager.TableToFormPoint(Position - new Vector2(-RADIUS, RADIUS));
			ballPic.Location = ballTopLeft;
			
			ballPic.Refresh();
			ballPic.Visible = true;
		}
	}

	class CueBall : Ball
	{
		protected override float Mass { get => 0.17f; } // in kg

		public CueBall(PictureBox pic) : base(pic)
		{
			Position = new Vector2(0.6096f, 0.6096f);
		}

		public void Shoot(Vector2 power)
		{
			Velocity += power;
		}
	}

	class NumberBall : Ball
	{
		public readonly int number;
		public readonly bool solid; // true if solid, false if striped
		protected override float Mass { get => 0.16f; } // in kg

		public NumberBall(PictureBox pic, int num) : base(pic)
		{
			number = num;

			// finds appropriate starting position and solid/striped based on number on the ball
			solid = num <= 7 ? true : false;
			float eightBallX = GameManager.TABLE_WIDTH / 2;
			float eightBallY = 3 * GameManager.TABLE_HEIGHT / 4;
			float ballSeparation = 2 * RADIUS + 0.001f;
			float cos60Deg = (float)Math.Cos(Math.PI / 3);
			float sin60Deg = (float)Math.Sin(Math.PI / 3);
			switch (num)
			{
				case 1:
					Position = new Vector2(eightBallX, eightBallY - 2 * ballSeparation * sin60Deg);
					break;
				case 2:
					Position = new Vector2(eightBallX + ballSeparation * cos60Deg, eightBallY - ballSeparation * sin60Deg);
					break;
				case 3:
					Position = new Vector2(eightBallX - ballSeparation, eightBallY);
					break;
				case 4:
					Position = new Vector2(eightBallX - ballSeparation - ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 5:
					Position = new Vector2(eightBallX, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 6:
					Position = new Vector2(eightBallX + 2 * ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 7:
					Position = new Vector2(eightBallX + ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 8:
					Position = new Vector2(eightBallX, eightBallY);
					break;
				case 9:
					Position = new Vector2(eightBallX - ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 10:
					Position = new Vector2(eightBallX - ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 11:
					Position = new Vector2(eightBallX + ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 12:
					Position = new Vector2(eightBallX + ballSeparation, eightBallY);
					break;
				case 13:
					Position = new Vector2(eightBallX - 2 * ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 14:
					Position = new Vector2(eightBallX - ballSeparation * cos60Deg, eightBallY - ballSeparation * sin60Deg);
					break;
				case 15:
					Position = new Vector2(eightBallX + ballSeparation + ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
			}
		}
	}
}
