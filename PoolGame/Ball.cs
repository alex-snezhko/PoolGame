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
		public void Move(float completed = 0f)
		{
			ICollider collidingWithFirst = null;
			// shortest distance collision to compare to
			float shortestU = float.MaxValue;

			foreach(ICollider c in GameManager.Colliders)
			{
				if(c != this)
				{
					// collision will occur
					float? pathCompleted = c.CollisionDistance(this);
					// finds object whose collision occurs before any other collider analyzed
					if (pathCompleted == null && pathCompleted.Value < shortestU)
					{
						collidingWithFirst = c;
						shortestU = pathCompleted.Value;
					}
				}
			}

			// no collision
			if (collidingWithFirst == null)
			{
				// adjusts velocity
				Velocity = ApplyFriction(Velocity, GameManager.tickInterval);

				Position += Velocity * GameManager.tickInterval;
			}
			else
			{
				Tuple<Vector2, Vector2> t = GetTrajectoryVector();
				// position of ball at collision
				Vector2 collisionPos = t.Item1 + shortestU * (t.Item2 - t.Item1);
				// velocity of ball at collision
				Vector2 vAtCollision = ApplyFriction(Velocity, GameManager.tickInterval * shortestU);

				if (collidingWithFirst is Ball otherBall)
				{
					Tuple<Vector2, Vector2> otherT = otherBall.GetTrajectoryVector();
					Vector2 otherCollisionPos = otherT.Item1 + shortestU * (otherT.Item2 - otherT.Item1);
					Vector2 otherVAtCollision = ApplyFriction(otherBall.Velocity, GameManager.tickInterval * shortestU);

					// velocities of balls after collision
					Tuple<Vector2, Vector2> newVels = MathFuncs.ElasticCollisionVels(Mass, vAtCollision, otherBall.Mass, otherVAtCollision);

					// velocities at end of frame
					Velocity = ApplyFriction(newVels.Item1, GameManager.tickInterval * (1 - shortestU));
					otherBall.Velocity = ApplyFriction(newVels.Item2, GameManager.tickInterval * (1 - shortestU));


					// TODO: use new 'completed' parameter and only move ball to impact point
					Position = collisionPos + Velocity * GameManager.tickInterval * (1 - shortestU);
					otherBall.Position = otherCollisionPos + otherBall.Velocity * GameManager.tickInterval * (1 - shortestU);

					otherBall.Move();
				}
				else if (collidingWithFirst is Wall wall)
				{
					//Vector2 newVel = wall.vAtCollision
				}
			}

			Vector2 ApplyFriction(Vector2 vel, float time)
			{
				Vector2 newVel = vel;
				float deltaV = GameManager.COEFF_FRICTION * 9.81f * time;
				return deltaV > Velocity.Length() ? Vector2.Zero : vel - deltaV * Vector2.Normalize(vel);
			}
		}

		public Tuple<Vector2, Vector2> GetTrajectoryVector() => new Tuple<Vector2, Vector2>(Position, Position + Velocity * GameManager.tickInterval);

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		// note: this -> 'victim' ball, other -> likely the ball doing the colliding; moves the ball if a collision is detected
		public float? CollisionDistance(Ball ball)
		{
			Tuple<Vector2, Vector2> traj = GetTrajectoryVector();
			Tuple<Vector2, Vector2> otherTraj = ball.GetTrajectoryVector();

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

		public void Collide(Ball ball, float pathCompleted)
		{
			Tuple<Vector2, Vector2> t = GetTrajectoryVector();
			// position of ball at collision
			Vector2 collisionPos = t.Item1 + pathCompleted * (t.Item2 - t.Item1);
			// velocity of ball at collision
			Vector2 vAtCollision = ApplyFriction(Velocity, GameManager.tickInterval * pathCompleted);

			Tuple<Vector2, Vector2> otherT = ball.GetTrajectoryVector();
			Vector2 otherCollisionPos = otherT.Item1 + pathCompleted * (otherT.Item2 - otherT.Item1);
			Vector2 otherVAtCollision = ApplyFriction(ball.Velocity, GameManager.tickInterval * pathCompleted);

			// velocities of balls after collision
			Tuple<Vector2, Vector2> newVels = MathFuncs.ElasticCollisionVels(Mass, vAtCollision, ball.Mass, otherVAtCollision);

			// velocities at end of frame
			Velocity = ApplyFriction(newVels.Item1, GameManager.tickInterval * (1 - pathCompleted));
			ball.Velocity = ApplyFriction(newVels.Item2, GameManager.tickInterval * (1 - pathCompleted));


			// TODO: use new 'completed' parameter and only move ball to impact point
			Position = collisionPos + Velocity * GameManager.tickInterval * (1 - pathCompleted);
			ball.Position = otherCollisionPos + ball.Velocity * GameManager.tickInterval * (1 - pathCompleted);

			ball.Move();

			Vector2 ApplyFriction(Vector2 vel, float time)
			{
				Vector2 newVel = vel;
				float deltaV = GameManager.COEFF_FRICTION * 9.81f * time;
				return deltaV > Velocity.Length() ? Vector2.Zero : vel - deltaV * Vector2.Normalize(vel);
			}
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
