using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Multimedia;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.Core
{
	/// <summary>
	/// This represents the viewport part of the Editor, holding a Screenspace, ensuring that needed 
	/// Commands are present and allowing plugins to control the viewport up to a degree.
	/// Plugins that want to destroy all entities except for the viewport controls should call
	/// "DestroyRenderedEntities".
	/// </summary>
	public class EditorOpenTkViewport
	{
		public EditorOpenTkViewport(Window window)
		{
			screenSpace = new Camera2DScreenSpace(window);
			Settings.Current.LimitFramerate = 60;
			CreateViewportCommands();
		}

		private void CreateViewportCommands()
		{
			panningCommand = new Command("ViewportPanning", ViewportPanning);
			zoomCommand = new Command("ViewportZooming", ViewPortZooming);
			panningCommand.AddTag("ViewControl");
			panningStart = new Command("ViewportPanningStart", position =>
			{
				lastDragPosition = position;
				dragAcceleration = 0.5f;
			});
			panningStart.AddTag("ViewControl");
			zoomCommand.AddTag("ViewControl");
		}

		private readonly Camera2DScreenSpace screenSpace;
		private Command panningCommand;
		private Command zoomCommand;
		private Command panningStart;

		private void ViewportPanning(Vector2D currentDragPosition)
		{
			screenSpace.LookAt += (lastDragPosition - currentDragPosition) * dragAcceleration;
			lastDragPosition = currentDragPosition;
		}

		private Vector2D lastDragPosition;
		private float dragAcceleration;

		private void ViewPortZooming(float zoomDifference)
		{
			var changeByAmount = zoomDifference * 0.1f;
			if (screenSpace.Zoom + changeByAmount > 0.0f)
				screenSpace.Zoom += changeByAmount;
		}

		public void CenterViewOn(Vector2D centerPosition)
		{
			screenSpace.LookAt = centerPosition;
		}

		public void ZoomViewTo(float zoomAmount)
		{
			if (zoomAmount >= 0)
				screenSpace.Zoom = zoomAmount;
		}

		public void DestroyRenderedEntities()
		{
			var allEntities = EntitiesRunner.Current.GetAllEntities();
			foreach (var entity in allEntities)
				if (!entity.GetType().IsSubclassOf(typeof(SoundDevice)) &&
					!(entity.GetTags().Contains("ViewControl")))
					entity.IsActive = false;
		}

		public void ResetViewportArea()
		{
			screenSpace.LookAt = Vector2D.Half;
			screenSpace.Zoom = 1.0f;
		}
	}
}