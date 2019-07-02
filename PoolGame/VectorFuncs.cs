using System;
using System.Numerics;
using static System.Math;

namespace PoolGame
{
	static class VectorFuncs
	{
		// finds how much of its trajectory a moving point crossed through when the distance separating it from line was d
		// precondition: ball collides with wall at some point
		public static float PathCompletedAtDFromWall((Vector2, Vector2) path, (Vector2, Vector2) wall, float d)
		{
			Vector2 l1 = path.Item1, l2 = path.Item2, w1 = wall.Item1, w2 = wall.Item2;

			// finds vector of d
			double perpWallAngle = Atan2(w2.Y - w1.Y, w2.X - w1.X) + PI / 2;
			Vector2 dVec = new Vector2(d * (float)Cos(perpWallAngle), d * (float)Sin(perpWallAngle));

			float num = (w1.X + dVec.X - l1.X) * (w2.Y - w1.Y) - (w1.Y + dVec.Y - l1.Y) * (w2.X - w1.X);
			float den = (l2.X - l1.X) * (w2.Y - w1.Y) - (l2.Y - l1.Y) * (w2.X - w1.X);
			// moving point's u value; if NaN then lines are parallel
			float ul = num / den;

			// u value along wall
			float uw = (w2.Y - w1.Y != 0) ?
				(l1.Y - w1.Y - dVec.Y + ul * (l2.Y - l1.Y)) / (w2.Y - w1.Y) :
				(l1.X - w1.X - dVec.X + ul * (l2.X - l1.X)) / (w2.X - w1.X);

			// alternative u checked if trajectory is believed to touch the corner of a wall
			float altU;
			if (uw < 0f)
			{
				altU = PathCompletedAtDFromPoint(path, w1, d);
				return altU;
			}
			else if (uw > 1f)
			{
				altU = PathCompletedAtDFromPoint(path, w2, d);
				return altU;
			}

			return ul;
		}

		// finds how much of its trajectory a moving point crossed through when the distance separating it from point was d
		public static float PathCompletedAtDFromPoint((Vector2, Vector2) path, Vector2 p, float d)
		{
			// i = li + u(lf - li)
			// i = p + d(dUnitVec)
			// dUx^2 + dUy^2 = 1

			Vector2 li = path.Item1, lf = path.Item2;
			// quadratic formula- derived

			double a = Pow(lf.X - li.X, 2) + Pow(lf.Y - li.Y, 2);
			double b = 2 * ((li.X - p.X) * (lf.X - li.X) + (li.Y - p.Y) * (lf.Y - li.Y));
			double c = -Pow(d, 2) + Pow(li.X - p.X, 2) + Pow(li.Y - p.Y, 2);

			double plus = (-b + Sqrt(b * b - 4 * a * c)) / (2 * a);
			double minus = (-b - Sqrt(b * b - 4 * a * c)) / (2 * a);

			float u = (float)Min(plus, minus);
			return u;
		}

		// finds how much of their trajectories 2 moving points crossed through when the distance separating them is d
		public static float PathCompletedAtDFromTrajectories((Vector2, Vector2) line1, (Vector2, Vector2) line2, float d)
		{
			Vector2 l1i = line1.Item1, l1f = line1.Item2, l2i = line2.Item1, l2f = line2.Item2;
			// deltas
			float dL1x = l1f.X - l1i.X, dL2x = l2f.X - l2i.X;
			float dL1y = l1f.Y - l1i.Y, dL2y = l2f.Y - l2i.Y;
			float dLix = l2i.X - l1i.X, dLiy = l2i.Y - l1i.Y;

			// quadratic formula- derived
			double a = Pow(dL2x - dL1x, 2) + Pow(dL2y - dL1y, 2);
			double b = 2 * ((dL2x - dL1x) * dLix + (dL2y - dL1y) * dLiy);
			double c = Pow(dLix, 2) + Pow(dLiy, 2) - Pow(d, 2);

			double plus = (-b + Sqrt(b * b - 4 * a * c)) / (2 * a);
			double minus = (-b - Sqrt(b * b - 4 * a * c)) / (2 * a);

			float u = (float)Min(plus, minus);
			return u;
		}

		// finds shortest scalar distance between two lines
		public static float SmallestDistanceTwoLines((Vector2, Vector2) line1, (Vector2, Vector2) line2)
		{
			// initial points and outward vector components
			Vector2 p1 = line1.Item1, p2 = line2.Item1, 
				v1 = line1.Item2 - p1, v2 = line2.Item2 - p2;

			// found at https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
			float det = Cross2D(v1, v2);
			float u1 = Cross2D(p2 - p1, v2) / det;
			float u2 = Cross2D(p2 - p1, v1) / det;

			// checks to see if lines ever intersect
			bool linesIntersect = Abs(det) > 0.00001f
				&& u1 > 0f && u1 < 1f
				&& u2 > 0f && u2 < 1f;
			if (linesIntersect)
			{
				return 0f;
			}

			// if lines never intersected then finds shortest distance between line and point for both verteces and takes shorter
			float p1Dist = SmallestVectorLinePoint(line1, line2.Item1).Length();
			float p2Dist = SmallestVectorLinePoint(line1, line2.Item2).Length();

			return Min(p1Dist, p2Dist);

			// returns 2D cross product (determinant) of two 2D vectors
			float Cross2D(Vector2 a, Vector2 b)
			{
				return a.X * b.Y - a.Y * b.X;
			}
		}

		// finds shortest vector that can be drawn between a line and a point; found from http://paulbourke.net/geometry/pointlineplane/
		public static Vector2 SmallestVectorLinePoint((Vector2, Vector2) line, Vector2 point)
		{
			// p1 is farther from point than p2
			Vector2 p1 = (point - line.Item1).Length() > (point - line.Item2).Length() ? line.Item1 : line.Item2;
			Vector2 p2 = p1 == line.Item1 ? line.Item2 : line.Item1;

			// 0 -> intersection at p1, 1 -> intersection at p2, else -> fraction of dist between p1 and p2
			float u = ((point.X - p1.X) * (p2.X - p1.X) + (point.Y - p1.Y) * (p2.Y - p1.Y)) /
				(p2 - p1).LengthSquared();

			if (u > 1)
			{
				return point - p2;
			}

			Vector2 intersection = new Vector2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
			return point - intersection;
		}

		// gives resulting velocities of balls in collision (elastic collision); m = mass, v = velocity, x = position at impact
		// https://en.wikipedia.org/wiki/Elastic_collision (2 dimensional)
		public static (Vector2, Vector2) ElasticCollisionVels(float m1, Vector2 v1, Vector2 x1, float m2, Vector2 v2, Vector2 x2)
		{
			Vector2 v1f = v1 - 2 * m2 / (m1 + m2) * (Vector2.Dot(v1 - v2, x1 - x2) / Vector2.DistanceSquared(x1, x2)) * (x1 - x2);
			Vector2 v2f = v2 - 2 * m1 / (m1 + m2) * (Vector2.Dot(v2 - v1, x2 - x1) / Vector2.DistanceSquared(x1, x2)) * (x2 - x1);

			return (v1f, v2f);
		}
	}
}
