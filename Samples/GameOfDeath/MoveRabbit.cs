using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace GameOfDeath
{
	/// <summary>
	/// Moves the rabbit and bounces it if it goes too far from home
	/// </summary>
	public class MoveRabbit : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var sprite = entity.Get<RabbitSprite>();
			Point velocity = sprite.Velocity;
			velocity.ReflectIfHittingBorder(sprite.DrawArea, sprite.BoundingBox);
			sprite.Velocity = velocity;
			sprite.Center += velocity * Time.Current.Delta;
			entity.MessageAllListeners(new HasMoved());
		}

		public class HasMoved {}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}