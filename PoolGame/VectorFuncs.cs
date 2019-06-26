using System;
using System.Numerics;

namespace PoolGame
{
	static class VectorFuncs
	{
		// finds how much of its trajectory a moving point crossed through when the distance separating it from line was d
		// precondition: ball collides with wall at some point
		public static float PathCompletedAtDFromWall((Vector2, Vector2) path, (Vector2, Vector2) wall, float d)
		{
			Vector2 l1 = path.Item1, l2 = path.Item2, w1 = wall.Item1, w2 = wall.Item2;
			Vector2 unitWall = Vector2.Normalize(new Vector2(w2.X - w1.X, w2.Y - w1.Y));

			double perpWallAngle = Math.Atan2(unitWall.Y, unitWall.X) + Math.PI / 2;
			Vector2 dVec = new Vector2(d * (float)Math.Cos(perpWallAngle), d * (float)Math.Sin(perpWallAngle));

			float Z = (w2.Y - w1.Y) / (w2.X - w1.X);
			float u3 = (w1.Y - l1.Y - dVec.Y + Z * (l1.X + dVec.X - w1.X)) /
				(l2.Y - l1.Y - Z * (l2.X - l1.X));

			float u2 = ((l1.X + dVec.X) * (w2.Y - w1.Y) + (w1.X - w2.X) * (dVec.Y + l1.Y) + w1.Y * w2.X - w2.Y * w1.X) /
				((l2.Y - l1.Y) * (w2.X - w1.X) + (w1.Y - w2.Y) * (l2.X - l1.X));





			float A = (w2.Y - w1.Y) / (w2.X - w1.X); // TODO: returning infinity
			float C = d * (A * A - 1) / (A * (float)Math.Sqrt(A * A + 1));

			float u = (w1.X - l1.X + (l1.Y - w1.Y) / A + C) /
				(l2.X - l1.X - (l2.Y - l1.Y) / A);

			return u;
		}

		// finds how much of its trajectory a moving point crossed through when the distance separating it from point was d
		public static float PathCompletedAtDFromPoint((Vector2, Vector2) path, Vector2 p, float d)
		{
			Vector2 ti = path.Item1, tf = path.Item2;
			// quadratic formula- derived
			double a = Math.Pow(tf.X - ti.X, 2) + Math.Pow(tf.Y - ti.Y, 2);
			double b = 2 * ((ti.X - p.X) * (tf.X - ti.X) + (ti.Y - p.Y) * (tf.Y - ti.Y));
			double c = -Math.Pow(d, 2) + Math.Pow(ti.Y - p.Y, 2) + Math.Pow(ti.X - p.X, 2);

			double plus = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
			double minus = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);

			float u = (float)Math.Min(plus, minus);
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
			double a = Math.Pow(dL2x - dL1x, 2) + Math.Pow(dL2y - dL1y, 2);
			double b = 2 * ((dL2x - dL1x) * dLix + (dL2y - dL1y) * dLiy);
			double c = Math.Pow(dLix, 2) + Math.Pow(dLiy, 2) - Math.Pow(d, 2);

			double plus = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
			double minus = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);

			float u = (float)Math.Min(plus, minus);
			return u;
		}

		// finds shortest scalar distance between two lines
		public static float SmallestDistanceTwoLines((Vector2, Vector2) line1, (Vector2, Vector2) line2)
		{
			// finds shortest distance between line and point for both verteces and takes shorter
			float p1Dist = SmallestVectorLinePoint(line1, line2.Item1).Length();
			float p2Dist = SmallestVectorLinePoint(line1, line2.Item2).Length();

			return Math.Min(p1Dist, p2Dist);
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
