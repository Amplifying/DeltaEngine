using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A simple UI button which can change image and color, and exposes a Clicked event
	/// </summary>
	public class Button : Label, Interact.Clickable
	{
		public Button(Theme theme, Rectangle drawArea, string text = "")
			: base(theme.Button, theme.Font, drawArea)
		{
			Text = text;
			Add(theme);
			Add(new Interact.State());
			Add<Interact, Interact.Clicking, UpdateAppearance>();
		}

		private class UpdateAppearance : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				if (!interactions.Contains(message.GetType()))
					return;

				var isInside = entity.Get<Interact.State>().IsInside;
				var isPressed = entity.Get<Interact.State>().IsPressed;
				if (isInside && isPressed)
					SetAppearance(entity, entity.Get<Theme>().ButtonPressed);
				else if (isInside)
					SetAppearance(entity, entity.Get<Theme>().ButtonMouseover);
				else
					SetAppearance(entity, entity.Get<Theme>().Button);
			}

			private readonly List<Type> interactions = new List<Type>(typeof(Interact).GetNestedTypes());

			private static void SetAppearance(Entity entity, Theme.Appearance appearance)
			{
				entity.Set(appearance.Image);
				entity.Set(appearance.Color);
			}
		}

		public void Clicking()
		{
			if (Clicked != null)
				Clicked();
		}

		public event Action Clicked;
	}
}