using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PoolGame
{
	abstract class Ball : ICollider
	{
		// x, y; using pool table size of 1.2192m x 2.4384m (4'x8') with short side as x
		public Vector2 Position { get; protected set; }
		public Vector2 Velocity { get; protected set; } // in m/s

		protected abstract float Mass { get; } // in kg
		public const float RADIUS = 0.028575f; // in m

		protected Ball()
		{
			Velocity = Vector2.Zero;
		}

		// moves ball distance it would travel in one frame; only called if no collision was detected by CollidingWith()
		public void Move()
		{
			ICollider collidingWithFirst;

			foreach(ICollider c in GameManager.Colliders)
			{
				if(c != this)
				{
					collisions.Add(c);
				}
			}

			Position += Velocity * GameManager.tickInterval;

			float deltaV = GameManager.COEFF_FRICTION * 9.81f * GameManager.tickInterval;
			if(deltaV > Velocity.Length())
			{
				Velocity = Vector2.Zero;
			}
			else
			{
				Velocity -= deltaV * Vector2.Normalize(Velocity);
			}
		}

		public Tuple<Vector2, Vector2> GetTrajectoryVector() => new Tuple<Vector2, Vector2>(Position, Position + Velocity * GameManager.tickInterval);

		// true -> re-adds this ball to list of balls that are moving to check for further collisions
		// note: this -> 'victim' ball, other -> likely the ball doing the colliding; moves the ball if a collision is detected
		public bool WillCollideWith(Ball ball)
		{
			Tuple<Vector2, Vector2> traj = GetTrajectoryVector();
			Tuple<Vector2, Vector2> otherTraj = ball.GetTrajectoryVector();

			// smallest distance between the trajectories of the two balls in collision
			float smallestDist = Velocity == Vector2.Zero ? 
				MathFuncs.SmallestDistance(otherTraj, Position) : 
				MathFuncs.SmallestDistance(otherTraj, traj);

			return smallestDist <= 2 * RADIUS;


			/*// place other ball into position just touching this ball if they have collided
			if (smallestDist <= 2 * RADIUS)
			{
				// positions of two balls right as they collided
				Tuple<Vector2, Vector2> collisionPos = MathFuncs.FindPointsAtDistance(otherTraj, traj, 2 * RADIUS);
				// portion of path completed when balls collided
				float completed = (traj.Item2 - traj.Item1).Length() / (collisionPos.Item2 - traj.Item1).Length();

				Tuple<Vector2, Vector2> newVels = MathFuncs.ElasticCollisionVels(ball.Mass, ball.Velocity, Mass, Velocity);
				ball.Velocity = newVels.Item1;
				Velocity = newVels.Item2;

				ball.Position = collisionPos.Item1 + ball.Velocity * (1 - completed) * GameManager.tickInterval;
				Position = collisionPos.Item2 + Velocity * (1 - completed) * GameManager.tickInterval;

				return true;
			}

			return false;*/
		}

		public float DistanceToCollision(Ball ball)
		{
			return (ball.Position - MathFuncs.FindPointsAtDistance(ball.GetTrajectoryVector(), GetTrajectoryVector(), 2 * RADIUS).Item1).Length();
		}
	}

	class CueBall : Ball
	{
		protected override float Mass { get => 0.17f; } // in kg

		public CueBall()
		{
			Position = new Vector2(0.6096f, 0.6096f);
		}

		
	}

	class NumberBall : Ball
	{
		public readonly int number;
		public readonly bool solid; // true if solid, false if striped
		protected override float Mass { get => 0.16f; } // in kg

		public NumberBall(int num)
		{
			number = num;

			// finds appropriate starting position and solid/striped based on number on the ball
			solid = num <= 7 ? true : false;
			float eightBallX = GameManager.WIDTH / 2;
			float eightBallY = 3 * GameManager.HEIGHT / 4;
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
					Position = new Vector2(eightBallX + ballSeparation + ballSeparation * cos60Deg, eightBallY + 2 * ballSeparation * sin60Deg);
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
					Position = new Vector2(eightBallX - ballSeparation - ballSeparation * cos60Deg, eightBallY + 2 * ballSeparation * sin60Deg);
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
