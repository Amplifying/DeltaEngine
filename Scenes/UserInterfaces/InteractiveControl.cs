using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Is the base class for an interactive control like Button
	/// </summary>
	public abstract class InteractiveControl : Control
	{
		public virtual void Press()
		{
			IsPressed = true;
			if (Pressed != null)
				Pressed();
		}

		public bool IsPressed { get; private set; }
		public event Action Pressed;

		public virtual void Tap(Point position)
		{
			Release();
			if (Tapped != null)
				Tapped(position);
		}

		public virtual void Release()
		{
			IsPressed = false;
		}

		public event Action<Point> Tapped;

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

		public void Hover()
		{
			IsHovering = true;
			if (Hovered != null)
				Hovered();
		}

		public bool IsHovering { get; private set; }
		public event Action Hovered;

		public void StopHover()
		{
			IsHovering = false;
			if (StoppedHover != null)
				StoppedHover();
		}

		public event Action StoppedHover;

		internal abstract bool Contains(Point point);
	}
}