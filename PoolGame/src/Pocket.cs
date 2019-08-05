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

		// how much of its single-frame path the colliding ball completed at the time of collision; in [0-1] if collision detected
		public float CollisionDistance(Ball ball)
		{
			(Vector2, Vector2) traj = ball.GetTrajectoryVector();

			const float POCKET_RADIUS = 0.057f;
			float u = VectorFuncs.PathCompletedAtDFromPoint(traj, center, POCKET_RADIUS + Ball.RADIUS);

			if(u > 0f && u < 1f)
			{
				int i = 3;
			}
			if(float.IsNaN(u))
			{
				int a = 3;
			}
			return u;
		}

		public void Collide(Ball ball)
		{
			ball.Pocket(); // TODO: game freezing because when ball touches pocket it never even makes it here for some reason
		}
	}
}
