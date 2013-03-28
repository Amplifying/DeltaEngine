using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Is the base class for an interactive control like Button
	/// </summary>
	public abstract class InteractiveControl : Label
	{
		protected InteractiveControl(Image image, Rectangle initialDrawArea)
			: base(image, initialDrawArea) {}

		public virtual void Press()
		{
			IsPressed = true;
			if (Pressed != null)
				Pressed();
		}

		public bool IsPressed { get; private set; }
		public event Action Pressed;

		public virtual void Tap()
		{
			Release();
			if (Tapped != null)
				Tapped();
		}

		public virtual void Release()
		{
			IsPressed = false;
		}

		public event Action Tapped;

		public virtual void Enter()
		{
			IsInside = true;
			if (Entered != null)
				Entered();
		}

		public bool IsInside { get; private set; }
		public event Action Entered;

		public virtual void Exit()
		{
			IsInside = false;
			if (Exited != null)
				Exited();
		}

		public event Action Exited;

		public virtual void Hover()
		{
			IsHovering = true;
			if (Hovered != null)
				Hovered();
		}

		public bool IsHovering { get; private set; }
		public event Action Hovered;

		public virtual void StopHover()
		{
			IsHovering = false;
			if (StoppedHover != null)
				StoppedHover();
		}

		public event Action StoppedHover;
	}
}