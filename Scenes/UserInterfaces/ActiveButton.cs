using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A button which changes size depending on its state 
	/// - eg. grows on mouseover and shrinks on being clicked
	/// </summary>
	public class ActiveButton : Button
	{
		public ActiveButton(Theme theme, Rectangle drawArea, string text = "")
			: base(theme, drawArea, text)
		{
			Add(drawArea.Size);
			Start<UpdateSize>();
		}

		private class UpdateSize : EventListener2D
		{
			public override void ReceiveMessage(Entity2D control, object message)
			{
				if (message is Interact.ControlEntered || message is Interact.ControlClicked)
					Grow(control);
				else if (message is Interact.ControlPressed)
					Shrink(control);
				else if (message is Interact.ControlExited)
					Normalize(control);
			}

			private static void Grow(Entity2D control)
			{
				control.Start<Transition>();
				control.Set(new Transition.Size(control.Size, control.Get<Size>() * Growth));
				control.Set(new Transition.Duration(TransitionSlowly));
			}

			private const float Growth = 1.05f;
			private const float TransitionSlowly = 0.15f;

			private static void Shrink(Entity2D control)
			{
				control.Start<Transition>();
				control.Set(new Transition.Size(control.Size, control.Get<Size>() / Growth));
				control.Set(new Transition.Duration(TransitionQuickly));
			}

			private const float TransitionQuickly = 0.075f;

			private static void Normalize(Entity2D control)
			{
				control.Start<Transition>();
				control.Set(new Transition.Size(control.Size, control.Get<Size>()));
				control.Set(new Transition.Duration(TransitionSlowly));
			}

			public override Priority Priority
			{
				get { return Priority.Low; }
			}
		}

		public Size BaseSize
		{
			get { return Get<Size>(); }
			set { Set(value); }
		}
	}
}