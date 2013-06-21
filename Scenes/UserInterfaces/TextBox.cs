using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A Label into which text can be typed
	/// </summary>
	public class TextBox : Label
	{
		public TextBox(Theme theme, Rectangle drawArea, string text = "")
			: base(theme.TextBox, theme.Font, drawArea)
		{
			Text = text;
			Add(theme);
			Add(new Interact.State { CanHaveFocus = true });
			Add<Interact, InteractWithKeyboard>();
			Add<UpdateAppearance, UpdateText>();
		}

		private class UpdateAppearance : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				var theme = entity.Get<Theme>();
				var hasFocus = entity.Get<Interact.State>().HasFocus;
				var appearance = hasFocus ? theme.TextBoxFocussed : theme.TextBox;
				entity.Set(appearance.Image);
				entity.Set(appearance.Color);
			}
		}
	}
}