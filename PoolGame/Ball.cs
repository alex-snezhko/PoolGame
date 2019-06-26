using System.Windows.Forms;
using System.Numerics;
using System.Drawing;

namespace PoolGame
{
	abstract class Ball : ICollider
	{
		protected readonly PictureBox ballImage;

		// x, y; using pool table size of 1.2192m x 2.4384m (4'x8') with short side as x
		public Vector2 Position { get; protected set; }
		public Vector2 Velocity { get; protected set; } // in m/s

		protected abstract float Mass { get; } // in kg
		public const float RADIUS = 0.028575f; // in m

		// how much of game frame's trajectory path this ball has completed
		float pathCompletedInFrame = 0f;

		protected Ball(PictureBox pic)
		{
			ballImage = pic;
			Velocity = Vector2.Zero;
		}

		public void ApplyForce(Vector2 force)
		{
			Velocity += force;
		}

		public (Vector2, Vector2) GetTrajectoryVector() => (Position, Position + Velocity * GameManager.tickInterval);

		// moves ball distance it would travel in one frame; only called if no collision was detected by CollidingWith()
		// parameter determines how much of its single-frame trajectory this ball should move
		public void Move(float completed)
		{
			if (Velocity != Vector2.Zero)
			{
				(Vector2, Vector2) t = GetTrajectoryVector();
				Vector2 finalPos = t.Item1 + (t.Item2 - t.Item1) * (completed - pathCompletedInFrame);

				Position = finalPos;
				Velocity = ApplyFriction(Velocity, GameManager.tickInterval * (completed - pathCompletedInFrame));	
			}

			pathCompletedInFrame = completed == 1f ? 0f : completed;

			Vector2 ApplyFriction(Vector2 vel, float time)
			{
				float deltaV = GameManager.COEFF_FRICTION * 9.81f * time;
				return deltaV > Velocity.Length() ?
					Vector2.Zero :
					vel - deltaV * Vector2.Normalize(vel);
			}
		}

		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		// note: this -> 'victim' ball, other -> likely the ball doing the colliding; moves the ball if a collision is detected
		public float? CollisionDistance(Ball ball)
		{
			(Vector2, Vector2) traj = GetTrajectoryVector();
			(Vector2, Vector2) otherTraj = ball.GetTrajectoryVector();

			// smallest distance between the trajectories of the two balls in collision
			float smallestDist = Velocity == Vector2.Zero ?
				VectorFuncs.SmallestVectorLinePoint(otherTraj, Position).Length() :
				VectorFuncs.SmallestDistanceTwoLines(otherTraj, traj);

			if (smallestDist > 2 * RADIUS)
			{
				return null;
			}

			float completed = VectorFuncs.PathCompletedAtDFromTrajectories(otherTraj, traj, 2 * RADIUS);
			return completed;
		}

		public void Collide(Ball other)
		{
			(Vector2 v1, Vector2 v2) = VectorFuncs.ElasticCollisionVels(other.Mass, other.Velocity, other.Position, Mass, Velocity, Position);
			other.Velocity = v1;
			Velocity = v2;
		}

		// init parameter allows for placing this ball on the screen at game startup
		public void MovePictureBox()
		{			
			Point ballCenter = GameManager.PositionToFormPoint(Position);
			// picturebox location based off top left point; make correction from position (which is center of ball)
			const int TL_PIX_OFFSET = (int)(RADIUS * GameManager.PLAYAREA_W_PIX / GameManager.TABLE_WIDTH);
			ballImage.Location = ballCenter - new Size(TL_PIX_OFFSET, TL_PIX_OFFSET);
			
			ballImage.Refresh();
		}

		public virtual void SetPictureBox(bool enabled)
		{
			ballImage.Visible = enabled;
			MovePictureBox();
		}		
	}
}
