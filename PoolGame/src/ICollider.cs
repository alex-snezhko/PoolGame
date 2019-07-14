namespace PoolGame
{
	interface ICollider
	{
		// how much of its single-frame path the colliding ball completed at the time of collision; in [0-1] if collision detected
		float CollisionDistance(Ball ball);
		// handles collision of this collider with ball in question
		void Collide(Ball ball);
	}
}
