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
			Dictionary<Ball, Ball[]> ballCollidingWith = new Dictionary<Ball, Ball[]>();

			foreach(Ball b in Colliders)
			{
				if(b.Velocity != Vector2.Zero)
				{
					ballCollidingWith.Add(b, null);
				}
			}




			List<Ball> movingBalls = new List<Ball>();
			foreach(Ball b in Colliders)
			{
				if(b.Velocity != Vector2.Zero)
				{
					movingBalls.Add(b);
				}
			}

			foreach(ICollider c in Colliders)
			{
				for(int i = 0; i < movingBalls.Count; i++)
				{
					if (c.CollidingWith(b) && c is Ball)
					{
						movingBalls.Add((Ball)c);
					}
				}
			}

			Move.
		}
	}
}
