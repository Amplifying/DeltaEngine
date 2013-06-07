using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace Snake
{
	/// <summary>
	/// This class holds data about the snake body and checks for snake collisions with either 
	/// itself or with the borders and whether the snake must grow in size.
	/// </summary>
	public class Snake : Entity
	{
		public Snake(int gridSize)
		{
			Add(new Body(gridSize));
			Add<SnakeHandler>();
		}

		public void Dispose()
		{
			foreach (var bodyPart in Get<Body>().BodyParts)
				bodyPart.IsActive = false;

			Get<Body>().BodyParts.Clear();
			Remove<Body>();
			Remove<SnakeHandler>();
		}

		internal class SnakeHandler : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				if (!Time.Current.CheckEvery(0.15f))
					return;

				var body = entity.Get<Body>();
				body.MoveBody();
				body.CheckSnakeCollidesWithChunk();
				body.CheckSnakeCollisionWithBorderOrItself();
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Normal; }
			}
		}
	}
}