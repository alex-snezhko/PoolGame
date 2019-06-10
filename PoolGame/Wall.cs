using System;
using System.Numerics;

namespace PoolGame
{
	enum Side { Top, TopLeft, TopRight, BottomLeft, BottomRight, Bottom }

	class Wall : ICollider
	{ 
		public Wall(Side side)
		{
			switch(side)
			{
				case Side.Top:
					break;
				case Side.TopLeft:
					break;
				case Side.TopRight:
					break;
				case Side.BottomLeft:
					break;
				case Side.BottomRight:
					break;
				case Side.Bottom:
					break;
			}
		}

		public bool WillCollideWith(Ball other)
		{
			Tuple<Vector2, Vector2> trajectory = other.GetTrajectoryVector();
			// TODO

			return false;
		}

		public float DistanceToCollision(Ball ball)
		{
			throw new NotImplementedException();
		}
	}
}
