using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Groups Entities such that they can be activated and deactivated together. 
	/// </summary>
	public class Scene
	{
		public void Add(Entity control)
		{
			if (!controls.Contains(control))
				controls.Add(control);

			control.IsActive = isShown;
		}

		private readonly List<Entity> controls = new List<Entity>();
		private bool isShown = true;

		public List<Entity> Controls
		{
			get { return controls; }
		}

		public void Remove(Entity control)
		{
			controls.Remove(control);
			control.IsActive = false;
		}

		public void Show()
		{
			foreach (Entity control in controls)
				control.IsActive = true;

			isShown = true;
		}

		public void Hide()
		{
			foreach (Entity control in controls)
				control.IsActive = false;

			isShown = false;
		}

		public virtual void Clear()
		{
			foreach (Entity control in controls)
				control.IsActive = false;

			controls.Clear();
		}

		public void SetBackground(Image image)
		{
				if (background != null)
					Remove(background);

				background = new Sprite(image, Rectangle.One) { RenderLayer = int.MinValue };
				Add(background);
		}

		protected Sprite background;
	}
}