using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Sorts all Entities into RenderLayer order; Then, for each, messages any listeners attached 
	/// that it's time to render it.
	/// </summary>
	public class SortAndRenderEntity2D : EntityHandler
	{
		public void Handle(List<Entity> entities)
		{
			var sortedEntities =
				entities.OfType<Entity2D>().Where(entity => entity.Visibility == Visibility.Show).OrderBy(
					entity => entity.RenderLayer);
			foreach (var entity in sortedEntities)
				entity.MessageAllListeners(new TimeToRender());
			NumberOfActiveRenderableObjects = sortedEntities.Count();
		}

		public class TimeToRender {}

		public int NumberOfActiveRenderableObjects { get; private set; }

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.High; }
		}
	}
}