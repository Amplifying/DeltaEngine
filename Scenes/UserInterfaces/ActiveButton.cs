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
			Add<UpdateSize>();
		}

		private class UpdateSize : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				var control = entity as Entity2D;
				if (message is Interact.ControlEntered || message is Interact.ControlClicked)
					Grow(control);
				else if (message is Interact.ControlPressed)
					Shrink(control);
				else if (message is Interact.ControlExited)
					Normalize(control);
			}

			private static void Grow(Entity2D control)
			{
				control.Add<Transition>();
				control.AddOrSet(new Transition.Size(control.Size, control.Get<Size>() * Growth));
				control.AddOrSet(new Transition.Duration(TransitionSlowly));
			}

			private const float Growth = 1.05f;
			private const float TransitionSlowly = 0.15f;

			private static void Shrink(Entity2D control)
			{
				control.Add<Transition>();
				control.AddOrSet(new Transition.Size(control.Size, control.Get<Size>() / Growth));
				control.AddOrSet(new Transition.Duration(TransitionQuickly));
			}

			private const float TransitionQuickly = 0.075f;

			private static void Normalize(Entity2D control)
			{
				control.Add<Transition>();
				control.AddOrSet(new Transition.Size(control.Size, control.Get<Size>()));
				control.AddOrSet(new Transition.Duration(TransitionSlowly));
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Low; }
			}
		}

		public Size BaseSize
		{
			get { return Get<Size>(); }
			set { Set(value); }
		}
	}
}