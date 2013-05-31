using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A simple UI control which creates and groups Radio Buttons.
	/// </summary>
	public class RadioDialog : Entity2D
	{
		public RadioDialog(Theme theme, Rectangle drawArea)
			: base(drawArea)
		{
			Add(theme);
			Add(new List<RadioButton>());
			Add<ArrangeButtons>();
		}

		public void AddButton(string text)
		{
			var button = new RadioButton(Get<Theme>(), text);
			button.Clicked += () => ButtonClicked(button);
			var buttons = Get<List<RadioButton>>();
			buttons.Add(button);
			if (buttons.Count == 1)
				button.IsSelected = true;
		}

		private void ButtonClicked(RadioButton clicked)
		{
			var buttons = Get<List<RadioButton>>();
			foreach (RadioButton button in buttons)
				button.IsSelected = (button == clicked);
		}

		public class ArrangeButtons : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (Entity2D entity in entities.OfType<Entity2D>())
					ArrangeEntityButtons(entity);
			}

			private static void ArrangeEntityButtons(Entity2D entity)
			{
				var buttons = entity.Get<List<RadioButton>>();
				if (buttons.Count == 0)
					return;

				float height = entity.DrawArea.Height / buttons.Count;
				var background = entity.Get<Theme>().RadioButtonBackground.Image;
				float width = height * background.PixelSize.AspectRatio;
				for (int i = 0; i < buttons.Count; i++)
					buttons[i].DrawArea = new Rectangle(entity.DrawArea.Left, entity.DrawArea.Top + i * height,
						width, height);
			}
		}

		public RadioButton SelectedButton
		{
			get { return Get<List<RadioButton>>().FirstOrDefault(button => button.IsSelected); }
		}
	}
}