using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace GameOfDeath
{
	/// <summary>
	/// Moves the rabbit and bounces it if it goes too far from home
	/// </summary>
	public class MoveRabbit : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			var sprite = entity.Get<RabbitSprite>();
			Point velocity = sprite.Velocity;
			velocity.ReflectIfHittingBorder(sprite.DrawArea, sprite.BoundingBox);
			sprite.Velocity = velocity;
			sprite.Center += velocity * Time.Current.Delta;
			entity.MessageAllListeners(new HasMoved());
		}

		public class HasMoved {}

		public override Priority Priority
		{
			get { return Priority.First; }
		}
	}
}