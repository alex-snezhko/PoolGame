using System;
using System.Numerics;
using static PoolGame.Side;

namespace PoolGame
{
	class Wall : ICollider
	{
		// individual pieces of each wall
		readonly (Vector2, Vector2) diagWall1;
		readonly (Vector2, Vector2) mainWall;
		readonly (Vector2, Vector2) diagWall2;

		public const float BANK_WIDTH = 0.05f;

		public Wall(Side side)
		{
			const float W = GameManager.TABLE_WIDTH;
			const float H = GameManager.TABLE_HEIGHT;

			Vector2 tlPoint, trPoint, blPoint, brPoint;
			switch(side)
			{
				case Top:
					tlPoint = new Vector2(0.083f, H);
					trPoint = new Vector2(W - 0.083f, H);
					blPoint = new Vector2(0.083f + BANK_WIDTH, H - BANK_WIDTH);
					brPoint = new Vector2(W - 0.083f - BANK_WIDTH, H - BANK_WIDTH);

					diagWall1 = (trPoint, brPoint);
					mainWall = (brPoint, blPoint);
					diagWall2 = (blPoint, tlPoint);
					break;
				case Top | Left:
					tlPoint = new Vector2(0f, H - 0.083f);
					trPoint = new Vector2(BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					blPoint = new Vector2(0f, H / 2 + 0.04f);
					brPoint = new Vector2(BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);

					diagWall1 = (tlPoint, trPoint);
					mainWall = (trPoint, brPoint);
					diagWall2 = (brPoint, blPoint);
					break;
				case Top | Right:
					tlPoint = new Vector2(W - BANK_WIDTH, H - 0.083f - BANK_WIDTH);
					trPoint = new Vector2(W, H - 0.083f);
					blPoint = new Vector2(W - BANK_WIDTH, H / 2 + 0.04f + BANK_WIDTH);
					brPoint = new Vector2(W, H / 2 + 0.04f);

					diagWall1 = (brPoint, blPoint);
					mainWall = (blPoint, tlPoint);
					diagWall2 = (tlPoint, trPoint);
					break;
				case Bottom | Left:
					tlPoint = new Vector2(0f, H / 2 - 0.04f);
					trPoint = new Vector2(BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					blPoint = new Vector2(0f, 0.083f);
					brPoint = new Vector2(BANK_WIDTH, 0.083f + BANK_WIDTH);

					diagWall1 = (tlPoint, trPoint);
					mainWall = (trPoint, brPoint);
					diagWall2 = (brPoint, blPoint);
					break;
				case Bottom | Right:
					tlPoint = new Vector2(W - BANK_WIDTH, H / 2 - 0.04f - BANK_WIDTH);
					trPoint = new Vector2(W, H / 2 - 0.04f);
					blPoint = new Vector2(W - BANK_WIDTH, 0.083f + BANK_WIDTH);
					brPoint = new Vector2(W, 0.083f);

					diagWall1 = (brPoint, blPoint);
					mainWall = (blPoint, tlPoint);
					diagWall2 = (tlPoint, trPoint);
					break;
				case Bottom:
					tlPoint = new Vector2(0.083f + BANK_WIDTH, BANK_WIDTH);
					trPoint = new Vector2(W - 0.083f - BANK_WIDTH, BANK_WIDTH);
					blPoint = new Vector2(0.083f, 0f);
					brPoint = new Vector2(W - 0.083f, 0f);

					diagWall1 = (blPoint, tlPoint);
					mainWall = (tlPoint, trPoint);
					diagWall2 = (trPoint, brPoint);
					break;
			}
		}

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		public float CollisionDistance(Ball ball)
		{
			(Vector2, Vector2) traj = ball.GetTrajectoryVector();			

			// distance to each wall
			float dD1 = VectorFuncs.SmallestDistanceTwoLines(diagWall1, traj);
			float dM = VectorFuncs.SmallestDistanceTwoLines(mainWall, traj);
			float dD2 = VectorFuncs.SmallestDistanceTwoLines(diagWall2, traj);

			float shortest = Math.Min(dD1, Math.Min(dM, dD2));

			// no collision detected between this wall and specified ball
			if (shortest > Ball.RADIUS)
			{
				return float.NaN;
			}

			float uD1 = VectorFuncs.PathCompletedAtDFromWall(traj, diagWall1, Ball.RADIUS);
			float uM = VectorFuncs.PathCompletedAtDFromWall(traj, mainWall, Ball.RADIUS);
			float uD2 = VectorFuncs.PathCompletedAtDFromWall(traj, diagWall2, Ball.RADIUS);

			/* if any of u values are NaN, then set their values to arbitrary number (that is larger
			 * than the max accepted u-value of 1) to make the min calculation work properly */
			uD1 = float.IsNaN(uD1) ? 2f : uD1;
			uM = float.IsNaN(uM) ? 2f : uM;
			uD2 = float.IsNaN(uD2) ? 2f : uD2;

			float minU;
			if (uM <= uD1 && uM <= uD2)
			{
				minU = uM;
				collidingWall = mainWall;
			}
			else if (uD1 < uM && uD1 < uD2)
			{
				minU = uD1;
				collidingWall = diagWall1;
			}
			else
			{
				minU = uD2;
				collidingWall = diagWall2;
			}

			return minU;
		}

		// used for keeping track of which portion of the wall the ball will collide with in this frame
		(Vector2, Vector2) collidingWall;

		public void Collide(Ball ball)
		{
			Vector2 toWallUnit = Vector2.Normalize(VectorFuncs.SmallestVectorLinePoint(collidingWall, ball.Position));
			collidingWall = (Vector2.Zero, Vector2.Zero);

			// finds vector normal to the wall which can be added to current ball velocity to obtain new velocity vector
			float len = -2 * (ball.Velocity.X * toWallUnit.X + ball.Velocity.Y * toWallUnit.Y) / toWallUnit.LengthSquared();
			Vector2 normal = toWallUnit * len;

			ball.ApplyDeltaV(normal);
		}
	}
}
