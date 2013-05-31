using System.Collections.Generic;
using System.Linq;
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
		public override void Handle(List<Entity> entities)
		{
			foreach (Rabbit rabbit in entities.OfType<Rabbit>())
				Move(rabbit);
		}

		private static void Move(Rabbit rabbit)
		{
			var sprite = rabbit.RabbitSprite;
			Point velocity = sprite.Velocity;
			velocity.ReflectIfHittingBorder(sprite.DrawArea, sprite.BoundingBox);
			sprite.Velocity = velocity;
			sprite.Center += velocity * Time.Current.Delta;
			rabbit.MessageAllListeners(new HasMoved());
		}

		public class HasMoved {}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}