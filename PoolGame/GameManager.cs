using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PoolGame
{
	class GameManager
	{
		public const float WIDTH = 1.2192f; // width of table in m
		public const float HEIGHT = 2.4384f; // height of table in m
		public static int tickInterval;
		public const float COEFF_FRICTION = 0.25f;

		public static ICollider[] Colliders { get; private set; } // [0]: cue ball, [1-15]: num balls, [16-21]: walls
		public event EventHandler<CollisionEventArgs> Move;

		public GameManager(int tickInt)
		{
			tickInterval = tickInt;

			Colliders = new ICollider[16 + 6];

			Colliders[0] = new CueBall();
			for (int i = 1; i <= 15; i++)
			{
				Colliders[i] = new NumberBall(i);
			}

			for (int i = 16; i <= 21; i++)
			{
				Colliders[i] = new Wall((Side)(i - 16));
			}
		}

		public void LookForCollisions()
		{
			float u = 0f;
			do
			{
				// finds earliest collision between 2 objects (if there is a collision)
				(Ball colliderBall, ICollider objectCollidedWith) = FindEarliestCollision(ref u);

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

		private (Ball, ICollider) FindEarliestCollision(ref float minCompletion)
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
	}
}
