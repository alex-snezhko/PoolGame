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

		// maximum allowable distance that cue striker is allowed to be from ball
		const float MAX_DIST = 0.5f;

		public PoolCue(CueBall cb)
		{
			cueBall = cb;
		}

		// changes position of cue striker based on mouse location
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

		// how strong a strike at this moment would be (from 0-1)
		public float ShotPower()
		{
			float dist = posRelativeToBall.Length();
			float power = dist / MAX_DIST;
			return power;
		}

		// shoots cue ball based on distance of cue striker relative to ball
		public void Shoot(Point mouseLocation)
		{
			ChangePos(mouseLocation);
			if (posRelativeToBall == Vector2.Zero)
			{
				return;
			}

			// complete shot; max speed is 5 m/s (when cue is MAX_DIST away from cue ball)
			Vector2 deltaV = 5 / MAX_DIST * posRelativeToBall;
			cueBall.ApplyDeltaV(deltaV);
		}
	}
}
