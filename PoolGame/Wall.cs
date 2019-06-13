using System;
using System.Numerics;

namespace PoolGame
{
	enum Side { Top, TopLeft, TopRight, BottomLeft, BottomRight, Bottom }

	class Wall : ICollider
	{
		Side side;
		Vector2 tlPoint;
		Vector2 trPoint;
		Vector2 blPoint;
		Vector2 brPoint;

		public Wall(Side side)
		{
			this.side = side;
			const float BANK_WIDTH = 0.05f;
			const float W = GameManager.WIDTH;
			const float H = GameManager.HEIGHT;
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

		public float? CollisionDistance(Ball other)
		{
			Tuple<Vector2, Vector2> traj = other.GetTrajectoryVector();

			Tuple<Vector2, Vector2> diag1, main, diag2;
			float dDiag1, dMain, dDiag2;

			Tuple<Vector2, Vector2> nearestWall;
			float shortest = float.MaxValue;

			switch(side)
			{
				case Side.Top:
					diag1 = new Tuple<Vector2, Vector2>(trPoint, brPoint);
					main = new Tuple<Vector2, Vector2>(brPoint, blPoint);
					diag2 = new Tuple<Vector2, Vector2>(blPoint, tlPoint);
					break;
				case Side.TopLeft:
				case Side.BottomLeft:
					diag1 = new Tuple<Vector2, Vector2>(tlPoint, trPoint);
					main = new Tuple<Vector2, Vector2>(trPoint, brPoint);	
					diag2 = new Tuple<Vector2, Vector2>(brPoint, blPoint);
					break;
				case Side.TopRight:
				case Side.BottomRight:
					diag1 = new Tuple<Vector2, Vector2>(brPoint, blPoint);
					main = new Tuple<Vector2, Vector2>(blPoint, tlPoint);	
					diag2 = new Tuple<Vector2, Vector2>(tlPoint, trPoint);
					break;
				default:
					diag1 = new Tuple<Vector2, Vector2>(blPoint, tlPoint);
					main = new Tuple<Vector2, Vector2>(tlPoint, trPoint);
					diag2 = new Tuple<Vector2, Vector2>(trPoint, brPoint);
					break;
			}

			dDiag1 = MathFuncs.SmallestDistance(traj, diag1);
			dMain = MathFuncs.SmallestDistance(traj, main);
			dDiag2 = MathFuncs.SmallestDistance(traj, diag2);

			if (dMain < dDiag1 && dMain < dDiag2) { nearestWall = main; shortest = dMain; }
			else if (dDiag1 < dMain && dDiag1 < dDiag2) { nearestWall = diag1; shortest = dDiag1; }
			else { nearestWall = diag2; shortest = dDiag2; }

			if (shortest > Ball.RADIUS)
			{
				return null;
			}

			return MathFuncs.PathCompletedWhenCollidedWithLine(traj, nearestWall, Ball.RADIUS);
		}
	}
}
