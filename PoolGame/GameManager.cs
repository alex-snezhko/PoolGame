using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PoolGame
{
	static class GameManager
	{
		public const float TABLE_WIDTH = 1.2192f; // width of table in m
		public const float TABLE_HEIGHT = 2.4384f; // height of table in m
		public static int tickInterval;
		public const float COEFF_FRICTION = 0.25f;
		
		public static PoolCue Cue { get; private set; }
		// [0]: cue ball, [1-15]: num balls, [16-21]: walls
		public static ICollider[] Colliders { get; private set; }
		// true when balls are moving, false when the player must shoot
		public static bool InPlay { get; set; }

		private static int numMovingBalls = 0;

		public static void BeginGame(int tickInt)
		{
			tickInterval = tickInt;

			Colliders = new ICollider[16 + 6];

			Colliders[0] = new CueBall();
			Cue = new PoolCue((CueBall)Colliders[0]);
			for (int i = 1; i <= 15; i++)
			{
				Colliders[i] = new NumberBall(i);
			}

			for (int i = 16; i <= 21; i++)
			{
				Colliders[i] = new Wall((Side)(i - 16));
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
	}
}
