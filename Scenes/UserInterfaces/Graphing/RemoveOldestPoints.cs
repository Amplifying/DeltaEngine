using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces.Graphing
{
	/// <summary>
	/// Restricts a graph to a limited number of points. Once more than that number are added
	/// points are removed from the start of the graph and all points are shifted backwards.
	/// </summary>
	public class RemoveOldestPoints : EventListener2D
	{
		public override void ReceiveMessage(Entity2D entity, object message)
		{
			var addPoint = message as Graph.PointAdded;
			if (addPoint != null)
				RemoveOldestPointsIfNecessary(entity);
		}

		private void RemoveOldestPointsIfNecessary(Entity entity)
		{
			var data = entity.Get<Graph.Data>();
			int maximumNumberOfPoints = data.MaximumNumberOfPoints;
			isRefreshNeeded = false;
			foreach (GraphLine line in data.Lines)
				PrunePointsFromLine(line, maximumNumberOfPoints);

			if (isRefreshNeeded)
				((Graph)entity).Refresh();
		}

		private bool isRefreshNeeded;

		private void PrunePointsFromLine(GraphLine line, int maximumNumberOfPoints)
		{
			var numberOfPointsToRemove = line.points.Count - maximumNumberOfPoints;
			if (numberOfPointsToRemove <= 0)
				return;

			for (int i = 0; i < numberOfPointsToRemove; i++)
				RemoveFirstPointAndShiftOthersBack(line);

			isRefreshNeeded = true;
		}

		private static void RemoveFirstPointAndShiftOthersBack(GraphLine line)
		{
			float interval = line.points[1].X - line.points[0].X;
			line.points.RemoveAt(0);
			for (int i = 0; i < line.points.Count; i++)
				line.points[i] = new Point(line.points[i].X - interval, line.points[i].Y);
		}
	}
}