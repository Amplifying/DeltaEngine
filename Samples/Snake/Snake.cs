using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace Snake
{
	/// <summary>
	/// This class holds data about the snake body and checks for snake collisions with either 
	/// itself or with the borders and whether the snake must grow in size.
	/// </summary>
	public class Snake : Entity2D
	{
		public Snake(int gridSize) : base(Rectangle.Zero)
		{
			Add(new Body(gridSize));
			Start<SnakeHandler>();
		}

		public void Dispose()
		{
			var body = Get<Body>();
			foreach (var bodyPart in body.BodyParts)
				bodyPart.IsActive = false;

			Get<Body>().BodyParts.Clear();
			Remove<Body>();
			Stop<SnakeHandler>();
		}

		internal class SnakeHandler : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				if (!Time.Current.CheckEvery(0.15f))
					return;

				var body = entity.Get<Body>();
				body.MoveBody();
				body.CheckSnakeCollidesWithChunk();
				body.CheckSnakeCollisionWithBorderOrItself();
			}

			public override Priority Priority
			{
				get { return Priority.Normal; }
			}
		}
	}
}