using System;
using System.Numerics;

namespace PoolGame
{
	enum Side { Top, TopLeft, TopRight, BottomLeft, BottomRight, Bottom }

	class Wall : ICollider
	{
		readonly Side side;
		readonly Vector2 tlPoint;
		readonly Vector2 trPoint;
		readonly Vector2 blPoint;
		readonly Vector2 brPoint;

		public Wall(Side side)
		{
			this.side = side;
			const float BANK_WIDTH = 0.05f;
			const float W = GameManager.TABLE_WIDTH;
			const float H = GameManager.TABLE_HEIGHT;
			switch(side)
			{
				case Side.Top:
					tlPoint = new Vector2(0.083f, H);
					trPoint = new Vector2(W - 0.083f, H);
					blPoint = new Vector2(0.083f + BANK_WIDTH, H - BANK_WIDTH);
					brPoint = new Vector2(W - 0.083f - BANK_WIDTH, H - BANK_WIDTH);
					break;
				case Side.TopLeft:
					tlPoint = new Vector2(0f, H - 0.083f);
					trPoint = new Vector2(BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					blPoint = new Vector2(0f, H / 2 + 0.04f);
					brPoint = new Vector2(BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);
					break;
				case Side.TopRight:
					tlPoint = new Vector2(W - BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					trPoint = new Vector2(W, H - 0.083f);
					blPoint = new Vector2(W - BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);
					brPoint = new Vector2(W, H / 2 + 0.04f);
					break;
				case Side.BottomLeft:
					tlPoint = new Vector2(0f, H / 2 - 0.04f);
					trPoint = new Vector2(BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					blPoint = new Vector2(0f, 0.083f);
					brPoint = new Vector2(BANK_WIDTH, 0.083f + BANK_WIDTH);
					break;
				case Side.BottomRight:
					tlPoint = new Vector2(W - BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					trPoint = new Vector2(W, H / 2 - 0.04f);
					blPoint = new Vector2(W - BANK_WIDTH, 0.083f + BANK_WIDTH);
					brPoint = new Vector2(W, 0.083f);
					break;
				case Side.Bottom:
					tlPoint = new Vector2(0.083f + BANK_WIDTH, BANK_WIDTH);
					trPoint = new Vector2(W - 0.083f - BANK_WIDTH, BANK_WIDTH);
					blPoint = new Vector2(0.083f, 0f);
					brPoint = new Vector2(W - 0.083f, 0f);
					break;
			}
		}

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		public float? CollisionDistance(Ball other)
		{
			ValueTuple<Vector2, Vector2> traj = other.GetTrajectoryVector();

			// distinct pieces of this wall
			(ValueTuple<Vector2, Vector2> diag1, 
				ValueTuple<Vector2, Vector2> main, 
				ValueTuple<Vector2, Vector2> diag2) = GetWallComponents();

			ValueTuple<Vector2, Vector2> nearestWall;
			float shortest;

			// distance to each wall
			float dDiag1 = MathFuncs.SmallestDistance(diag1, traj);
			float dMain = MathFuncs.SmallestDistance(main, traj);
			float dDiag2 = MathFuncs.SmallestDistance(diag2, traj);

			// finds which wall the ball will collide with first
			if (dMain < dDiag1 && dMain < dDiag2) { nearestWall = main; shortest = dMain; }
			else if (dDiag1 < dMain && dDiag1 < dDiag2) { nearestWall = diag1; shortest = dDiag1; }
			else { nearestWall = diag2; shortest = dDiag2; }

			// no collision detected between this wall and specified ball
			if (shortest > Ball.RADIUS)
			{
				return null;
			}

			return MathFuncs.PathCompletedWhenCollidedWithLine(traj, nearestWall, Ball.RADIUS);
		}

		// returns the locations of the pieces of this wall e.g. the diagonal parts and main part of the wall
		public (ValueTuple<Vector2, Vector2> diag1, ValueTuple<Vector2, Vector2> main, ValueTuple<Vector2, Vector2> diag2) GetWallComponents()
		{
			ValueTuple<Vector2, Vector2> diag1, main, diag2;

			switch (side)
			{
				case Side.Top:
					diag1 = (trPoint, brPoint);
					main = (brPoint, blPoint);
					diag2 = (blPoint, tlPoint);
					break;
				case Side.TopLeft:
				case Side.BottomLeft:
					diag1 = (tlPoint, trPoint);
					main = (trPoint, brPoint);
					diag2 = (brPoint, blPoint);
					break;
				case Side.TopRight:
				case Side.BottomRight:
					diag1 = (brPoint, blPoint);
					main = (blPoint, tlPoint);
					diag2 = (tlPoint, trPoint);
					break;
				default:
					diag1 = (blPoint, tlPoint);
					main = (tlPoint, trPoint);
					diag2 = (trPoint, brPoint);
					break;
			}

			return (diag1, main, diag2);
		}
	}
}
