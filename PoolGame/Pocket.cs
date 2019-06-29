using System.Numerics;
using static PoolGame.Side;

namespace PoolGame
{
	class Pocket : ICollider
	{
		// pocket represented by circle
		readonly Vector2 center;

		public Pocket(Side side)
		{
			const float W = GameManager.TABLE_WIDTH;
			const float H = GameManager.TABLE_HEIGHT;
			const float CORNER_OFFSET = 0.043f;
			const float SIDE_OFFSET = -0.02f;
			switch(side)
			{
				case Top | Left:
					center = new Vector2(CORNER_OFFSET, H - CORNER_OFFSET);
					break;
				case Top | Right:
					center = new Vector2(W - CORNER_OFFSET, H - CORNER_OFFSET);
					break;
				case Left:
					center = new Vector2(SIDE_OFFSET, H / 2);
					break;
				case Right:
					center = new Vector2(W - SIDE_OFFSET, H / 2);
					break;
				case Bottom | Left:
					center = new Vector2(CORNER_OFFSET, CORNER_OFFSET);
					break;
				case Bottom | Right:
					center = new Vector2(W - CORNER_OFFSET, CORNER_OFFSET);
					break;
			}
		}

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		public float CollisionDistance(Ball ball)
		{
			(Vector2, Vector2) traj = ball.GetTrajectoryVector();

			const float POCKET_RADIUS = 0.057f;
			float completed = VectorFuncs.PathCompletedAtDFromPoint(traj, center, POCKET_RADIUS + Ball.RADIUS);
			return completed;
		}

		public void Collide(Ball other)
		{
			GameManager.PocketBall(other);
		}
	}
}
