using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

namespace PoolGame
{
	class PoolCue
	{
		readonly CueBall cueBall;
		Vector2 posRelativeToBall = Vector2.Zero;

		const float MAX_DIST = 0.5f;

		public PoolCue(CueBall cb)
		{
			cueBall = cb;
		}

		// changes position of cue based on mouse location
		public void ChangePos(Point mousePos)
		{
			Vector2 mouseTablePoint = GameManager.FormToTablePoint(mousePos);
			posRelativeToBall = mouseTablePoint - cueBall.Position;

			// pool cue striker can only be a maximum of MAX_DIST meters away from the cue ball
			float dist = posRelativeToBall.Length();
			if(dist > MAX_DIST)
			{
				posRelativeToBall *= MAX_DIST / dist;
			}
		}

		// returns how much power is being charged
		public float ChargeShot()
		{
			float dist = posRelativeToBall.Length();
			float power = dist / MAX_DIST;
			return power;
		}

		// shoots cue ball based on distance of cue striker relative to ball
		public void Shoot(Point mouseLocation)
		{
			Vector2 mouseTablePoint = GameManager.FormToTablePoint(mouseLocation);
			posRelativeToBall = mouseTablePoint - cueBall.Position;

			// pool cue striker can only be a maximum of MAX_DIST meters away from the cue ball
			float dist = posRelativeToBall.Length();
			if (dist > MAX_DIST)
			{
				posRelativeToBall *= MAX_DIST / dist;
			}

			if (posRelativeToBall == Vector2.Zero)
			{
				return;
			}

			// complete shot; max speed is 5 m/s (when cue is MAX_DIST away from cue ball)
			cueBall.ApplyDeltaV(5 / MAX_DIST * posRelativeToBall);
		}
	}
}
