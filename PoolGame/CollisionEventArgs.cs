using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PoolGame
{
	class CollisionEventArgs : EventArgs
	{
		public Vector2 Momentum { get; set; }
	}
}
