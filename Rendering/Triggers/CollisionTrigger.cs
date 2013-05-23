using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	internal class CollisionTrigger : Trigger2D
	{
		public CollisionTrigger(EntitySystem system)
		{
			this.system = system;
		}

		private readonly EntitySystem system;

		public override void Update(Entity entity)
		{			
			var data = entity.Get<CollisionTriggerData>();
			var foundEntities = GetEntitiesFromSearchTags(data);
			foreach (var otherEntity in foundEntities)
				if (entity != otherEntity)
					entity.Set(IsEntityRectCollidingWithOtherEntityRect(entity, otherEntity)
						? data.TriggeredColor : data.DefaultColor);
		}

		private IEnumerable<Entity> GetEntitiesFromSearchTags(CollisionTriggerData data)
		{
			List<Entity> foundEntities = new List<Entity>();
			foreach (var tag in data.SearchTags)
				foreach (var foundEntity in system.GetEntitiesWithTag(tag))
					foundEntities.Add(foundEntity);
			return foundEntities;
		}

		private static bool IsEntityRectCollidingWithOtherEntityRect(Entity entity,
			Entity otherEntity)
		{
			return entity.Get<Rectangle>().IsColliding(otherEntity.Get<Rectangle>(),
				entity.Get<Rotation>().Value, otherEntity.Get<Rotation>().Value);
		}
	}
}