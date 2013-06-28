using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Observes an Entity2D and broadcasts a message when any rendering related component changes
	/// </summary>
	public class ObserveEntity2D : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			if (entity.Contains<SavedProperties>())
				BroadcastIfChanged(entity);
			else
				BroadcastChanged(entity);
		}

		public class SavedProperties
		{
			public Rectangle DrawArea { get; set; }
			public float Rotation { get; set; }
			public int RenderLayer { get; set; }
			public Visibility Visibility { get; set; }
			public bool IsRefreshRequired { get; set; }
		}

		private static void BroadcastIfChanged(Entity entity)
		{
			var entity2D = entity as Entity2D;
			var savedProperties = entity2D.Get<SavedProperties>();
			if (ArePropertiesUnchanged(savedProperties, entity2D))
				return;

			StoreChangedProperties(savedProperties, entity2D);
			BroadcastChanged(entity);
		}

		private static bool ArePropertiesUnchanged(SavedProperties savedProperties, Entity2D entity2D)
		{
			return entity2D.DrawArea == savedProperties.DrawArea &&
				entity2D.Rotation == savedProperties.Rotation &&
				entity2D.RenderLayer == savedProperties.RenderLayer &&
				entity2D.Visibility == savedProperties.Visibility && !savedProperties.IsRefreshRequired;
		}

		private static void StoreChangedProperties(SavedProperties savedProperties, Entity2D entity2D)
		{
			savedProperties.DrawArea = entity2D.DrawArea;
			savedProperties.Rotation = entity2D.Rotation;
			savedProperties.RenderLayer = entity2D.RenderLayer;
			savedProperties.Visibility = entity2D.Visibility;
			savedProperties.IsRefreshRequired = false;
		}

		private static void BroadcastChanged(Entity entity)
		{
			entity.MessageAllListeners(new HasChanged());
		}

		public class HasChanged {}

		public override Priority Priority
		{
			get
			{
				return Priority.High;
			}
		}
	}
}