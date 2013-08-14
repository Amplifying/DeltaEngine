using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Updates an Entity2Ds position and rotation from the associated PhysicsBody
	/// </summary>
	public class AffixToPhysics : UpdateBehavior
	{
		public AffixToPhysics()
			: base(Priority.First)
		{
			screen = ScreenSpace.Current;
		}

		private readonly ScreenSpace screen;

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Entity2D entity in entities)
				UpdateCenterAndRotation(entity, entity.Get<PhysicsBody>());
		}

		private void UpdateCenterAndRotation(Entity2D entity, PhysicsBody body)
		{
			entity.Center = screen.FromPixelSpace(body.Position);
			entity.Rotation = body.Rotation;
		}
	}
}