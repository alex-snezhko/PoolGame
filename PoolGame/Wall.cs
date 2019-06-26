using System.Numerics;
using static PoolGame.Side;

namespace PoolGame
{
	class Wall : ICollider
	{
		readonly Side side;
		readonly Vector2 tlPoint;
		readonly Vector2 trPoint;
		readonly Vector2 blPoint;
		readonly Vector2 brPoint;

		public const float BANK_WIDTH = 0.05f;

		public Wall(Side side)
		{
			this.side = side;
			const float W = GameManager.TABLE_WIDTH;
			const float H = GameManager.TABLE_HEIGHT;
			switch(side)
			{
				case Top:
					tlPoint = new Vector2(0.083f, H);
					trPoint = new Vector2(W - 0.083f, H);
					blPoint = new Vector2(0.083f + BANK_WIDTH, H - BANK_WIDTH);
					brPoint = new Vector2(W - 0.083f - BANK_WIDTH, H - BANK_WIDTH);
					break;
				case Top | Left:
					tlPoint = new Vector2(0f, H - 0.083f);
					trPoint = new Vector2(BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					blPoint = new Vector2(0f, H / 2 + 0.04f);
					brPoint = new Vector2(BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);
					break;
				case Top | Right:
					tlPoint = new Vector2(W - BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					trPoint = new Vector2(W, H - 0.083f);
					blPoint = new Vector2(W - BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);
					brPoint = new Vector2(W, H / 2 + 0.04f);
					break;
				case Bottom | Left:
					tlPoint = new Vector2(0f, H / 2 - 0.04f);
					trPoint = new Vector2(BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					blPoint = new Vector2(0f, 0.083f);
					brPoint = new Vector2(BANK_WIDTH, 0.083f + BANK_WIDTH);
					break;
				case Bottom | Right:
					tlPoint = new Vector2(W - BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					trPoint = new Vector2(W, H / 2 - 0.04f);
					blPoint = new Vector2(W - BANK_WIDTH, 0.083f + BANK_WIDTH);
					brPoint = new Vector2(W, 0.083f);
					break;
				case Bottom:
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
			(Vector2, Vector2) traj = other.GetTrajectoryVector();

			// distinct pieces of this wall
			((Vector2, Vector2) diag1,
				(Vector2, Vector2) main,
				(Vector2, Vector2) diag2) = GetWallComponents();

			(Vector2, Vector2) nearestWall;
			float shortest;

			// distance to each wall
			float dDiag1 = VectorFuncs.SmallestDistanceTwoLines(diag1, traj);
			float dMain = VectorFuncs.SmallestDistanceTwoLines(main, traj);
			float dDiag2 = VectorFuncs.SmallestDistanceTwoLines(diag2, traj);

			// finds which wall the ball will collide with first
			if (dMain < dDiag1 && dMain < dDiag2) { nearestWall = main; shortest = dMain; }
			else if (dDiag1 < dMain && dDiag1 < dDiag2) { nearestWall = diag1; shortest = dDiag1; }
			else { nearestWall = diag2; shortest = dDiag2; }

			// no collision detected between this wall and specified ball
			if (shortest > Ball.RADIUS)
			{
				return null;
			}

			return VectorFuncs.PathCompletedAtDFromWall(traj, nearestWall, Ball.RADIUS);
		}

		// returns the locations of the pieces of this wall e.g. the diagonal parts and main part of the wall
		public ((Vector2, Vector2) diag1, (Vector2, Vector2) main, (Vector2, Vector2) diag2) GetWallComponents()
		{
			(Vector2, Vector2) diag1, main, diag2;

			switch (side)
			{
				case Top:
					diag1 = (trPoint, brPoint);
					main = (brPoint, blPoint);
					diag2 = (blPoint, tlPoint);
					break;
				case Top | Left:
				case Bottom | Left:
					diag1 = (tlPoint, trPoint);
					main = (trPoint, brPoint);
					diag2 = (brPoint, blPoint);
					break;
				case Top | Right:
				case Bottom | Right:
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

		public void Collide(Ball other)
		{
			((Vector2, Vector2) diag1,
					(Vector2, Vector2) main,
					(Vector2, Vector2) diag2) = GetWallComponents();

			// find direction from wall piece hit to ball
			Vector2 toWallUnit;
			Vector2 fromD1 = VectorFuncs.SmallestVectorLinePoint(diag1, other.Position);
			Vector2 fromMain = VectorFuncs.SmallestVectorLinePoint(main, other.Position);
			Vector2 fromD2 = VectorFuncs.SmallestVectorLinePoint(diag2, other.Position);

			float dD1 = fromD1.Length(), dM = fromMain.Length(), dD2 = fromD2.Length();
			if (dD1 < dM && dD1 < dD2) { toWallUnit = Vector2.Normalize(fromD1); }
			else if (dM < dD1 && dM < dD2) { toWallUnit = Vector2.Normalize(fromMain); }
			else { toWallUnit = Vector2.Normalize(fromD2); }

			// finds vector normal to the wall which can be added to current ball velocity to obtain new velocity vector
			float len = -2 * (other.Velocity.X * toWallUnit.X + other.Velocity.Y * toWallUnit.Y) / toWallUnit.LengthSquared();
			Vector2 normal = toWallUnit * len;

			other.ApplyForce(normal);
		}
	}
}
