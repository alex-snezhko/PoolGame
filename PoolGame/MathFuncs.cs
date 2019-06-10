using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PoolGame
{
	static class MathFuncs
	{
		// finds the locations to put 2 points based on trajectories and a specified distance away (given that the lines have the same u-value)
		public static Tuple<Vector2, Vector2> FindPointsAtDistance(Tuple<Vector2, Vector2> line1, Tuple<Vector2, Vector2> line2, float d)
		{
			Vector2 l1i = line1.Item1, l1f = line1.Item2, l2i = line2.Item1, l2f = line2.Item2;
			// deltas
			float dL1x = l1f.X - l1i.X, dL2x = l2f.X - l2i.X, 
				dL1y = l1f.Y - l1i.Y, dL2y = l2f.Y - l2i.Y,
				dLix = l2i.X - l1i.X, dLiy = l2i.Y - l1i.Y;

			// quadratic formula- derived
			double a = Math.Pow(dL2x - dL1x, 2) + Math.Pow(dL2y - dL1y, 2);
			double b = 2 * ((dL2x - dL1x) * dLix + (dL2y - dL1y) * dLiy);
			double c = Math.Pow(dLix, 2) + Math.Pow(dLiy, 2) - Math.Pow(d, 2);

			double plus = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
			double minus = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
			float u = (float)Math.Min(plus, minus);

			return new Tuple<Vector2, Vector2>(l1i + u * (l1f - l1i), l2i + u * (l2f - l2i));
		}

		// finds shortest distance between two lines
		public static float SmallestDistance(Tuple<Vector2, Vector2> line1, Tuple<Vector2, Vector2> line2)
		{
			// finds shortest distance between line and point for both verteces and takes shorter
			float p1Dist = SmallestDistance(line1, line2.Item1);
			float p2Dist = SmallestDistance(line1, line2.Item2);

			return Math.Max(p1Dist, p2Dist);
		}

		// finds shortest distance between a line and a point
		public static float SmallestDistance(Tuple<Vector2, Vector2> line, Vector2 point)
		{
			// p1 is farther from point than p2
			Vector2 p1 = (point - line.Item1).Length() > (point - line.Item2).Length() ? line.Item1 : line.Item2;
			Vector2 p2 = p1 == line.Item1 ? line.Item2 : line.Item1;

			// 0 -> intersection at p1, 1 -> intersection at p2, else -> fraction of dist between p1 and p2
			float u = ((point.X - p1.X) * (p2.X - p1.X) + (point.Y - p1.Y) * (p2.Y - p1.Y)) /
				(p2 - p1).LengthSquared();

			if(u > 1)
			{
				return (point - p2).Length();
			}

			Vector2 intersection = new Vector2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
			return (point - intersection).Length();
		}

		// gives resulting velocities of balls in collision (elastic collision)
		public static Tuple<Vector2, Vector2> ElasticCollisionVels(float m1, Vector2 v1, float m2, Vector2 v2)
		{
			Vector2 v1f = (m1 - m2) / (m1 + m2) * v1 + 
				2 * m2 / (m1 + m2) * v2;
			Vector2 v2f = 2 * m1 / (m1 + m2) * v1 + 
				(m2 - m1) / (m1 + m2) * v2;

			return new Tuple<Vector2, Vector2>(v1f, v2f);
		}
	}
}
