using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

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
			Start<Interact, InteractWithKeyboard>();
			Start<UpdateAppearance, UpdateText>();
		}

		private class UpdateAppearance : Behavior2D
		{
			public override void Handle(Entity2D entity)
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