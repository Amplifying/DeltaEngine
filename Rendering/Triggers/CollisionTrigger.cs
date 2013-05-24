using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	internal class CollisionTrigger : Trigger2D
	{
		public override void Update(Entity entity)
		{
			var data = entity.Get<CollisionTriggerData>();
			var foundEntities = GetEntitiesFromSearchTags(data);
			foreach (var otherEntity in foundEntities)
				if (entity != otherEntity)
					entity.Set(IsEntityRectCollidingWithOtherEntityRect(entity, otherEntity)
						? data.TriggeredColor : data.DefaultColor);
		}

		private static IEnumerable<Entity> GetEntitiesFromSearchTags(CollisionTriggerData data)
		{
			var foundEntities = new List<Entity>();
			foreach (var tag in data.SearchTags)
				foreach (var foundEntity in EntitySystem.Current.GetEntitiesWithTag(tag))
					foundEntities.Add(foundEntity);

			return foundEntities;
		}

		private static bool IsEntityRectCollidingWithOtherEntityRect(Entity entity,
			Entity otherEntity)
		{
			return entity.Get<Rectangle>().IsColliding(entity.Get<float>(), otherEntity.Get<Rectangle>(),
				otherEntity.Get<float>());
		}
	}
}