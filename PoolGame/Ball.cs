using System.Windows.Forms;
using System.Numerics;
using System.Drawing;

namespace PoolGame
{
	abstract class Ball : ICollider
	{
		// GUI image which represents this ball
		protected readonly PictureBox ballImage;

		// (x, y); using pool table size of 1.2192m x 2.4384m (4'x8') with short side as x; all values in SI units
		public Vector2 Position { get; protected set; }
		public Vector2 Velocity { get; protected set; }

		protected abstract float Mass { get; }
		public const float RADIUS = 0.028575f;

		// how much of this frame's trajectory this ball has already completed
		public float PathCompleted { get; private set; } = 0f;

		protected Ball(PictureBox pic)
		{
			ballImage = pic;
			Velocity = Vector2.Zero;
		}

		// changes ball's velocity by specified value
		public void ApplyDeltaV(Vector2 force)
		{
			Velocity += force;
		}

		// vector which reaches from current position to position at the end of this ball's single-frame path completed
		public (Vector2, Vector2) GetTrajectoryVector() => (Position, Position + Velocity * (1f - PathCompleted) * GameManager.TickInterval); 

		// moves ball to new position at which the ball's total single-frame path completed is that specified by parameter
		public void Move(float newU)
		{
			if (Velocity != Vector2.Zero)
			{
				float uToMove = newU - PathCompleted;
				// small offset added if balls collide to make sure the balls dont overlap one another unintentionally
				uToMove -= newU < 1f ? 0.0001f : 0f;

				// new position to which this ball must move
				Vector2 finalPos = Position + Velocity * uToMove * GameManager.TickInterval;

				Position = finalPos;
				Velocity = ApplyFriction(Velocity, GameManager.TickInterval * uToMove);	
			}

			// resets this ball's u completed if it has fully completed frame's movement
			PathCompleted = newU == 1f ? 0f : newU;

			Vector2 ApplyFriction(Vector2 vel, float time)
			{
				float deltaV = GameManager.COEFF_FRICTION * 9.81f * time;
				return deltaV > Velocity.Length() ?
					Vector2.Zero :
					vel - deltaV * Vector2.Normalize(vel);
			}
		}

		// how much of its single-frame path the colliding ball completed at the time of collision; in [0-1] if collision detected
		// note: this -> 'victim' ball, other -> ball doing the colliding
		public float CollisionDistance(Ball other)
		{
			(Vector2, Vector2) traj = GetTrajectoryVector();
			(Vector2, Vector2) otherTraj = other.GetTrajectoryVector();

			float u = Velocity == Vector2.Zero ?
				VectorFuncs.PathCompletedAtDFromPoint(otherTraj, Position, 2 * RADIUS) :
				VectorFuncs.PathCompletedAtDFromTrajectories(otherTraj, traj, 2 * RADIUS);

			// correctly calculates new u regardless of how many collisions have already occurred this frame
			float netCompleted = other.PathCompleted + u * (1f - other.PathCompleted);
			return netCompleted;
		}

		public void Collide(Ball other)
		{
			(Vector2 v1, Vector2 v2) = VectorFuncs.ElasticCollisionVels(other.Mass, other.Velocity, other.Position, Mass, Velocity, Position);
			other.Velocity = v1;
			Velocity = v2;
		}

		// moves location of ballImage picturebox to this ball's current position on the table
		public void MovePictureBox()
		{			
			Point ballCenter = GameManager.TableToFormPoint(Position);
			// picturebox location based off top left point; make correction from position (which is center of ball)
			const int TL_PIX_OFFSET = (int)(RADIUS * GameManager.PLAYAREA_W_PIX / GameManager.TABLE_WIDTH);
			ballImage.Location = ballCenter - new Size(TL_PIX_OFFSET, TL_PIX_OFFSET);
			
			ballImage.Refresh();
		}

		// called when this ball lands in a pocket
		public virtual void Pocket()
		{
			GameManager.Colliders.Remove(this);
			GameManager.ActiveBalls.Remove(this);
		}
	}
}
