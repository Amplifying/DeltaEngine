using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class RadioButtonTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderRadioButton(Type resolver)
		{
			Start(resolver,
				(Scene s, ContentLoader content) => CreateRadioButton(content, Center, "Hello"));
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);

		private static void CreateRadioButton(ContentLoader content, Rectangle drawArea, string text)
		{
			var theme = new Theme
			{
				RadioButtonBackground = new Theme.Appearance(content.Load<Image>("DefaultLabel")),
				RadioButtonNotSelected = new Theme.Appearance(content.Load<Image>("DefaultRadiobuttonOff")),
				RadioButtonNotSelectedMouseover =
					new Theme.Appearance(content.Load<Image>("DefaultRadioButtonOffHover")),
				RadioButtonSelected = new Theme.Appearance(content.Load<Image>("DefaultRadiobuttonOn")),
				RadioButtonSelectedMouseover =
					new Theme.Appearance(content.Load<Image>("DefaultRadioButtonOnHover")),
				Font = new Font(content, "Verdana12")
			};
			new RadioButton(theme, drawArea, text);
		}

		[VisualTest]
		public void RenderThreeRadioButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				CreateRadioButton(content, Top, "Top Button");
				CreateRadioButton(content, Center, "Middle Button");
				CreateRadioButton(content, Bottom, "Bottom Button");
			});
		}

		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.5f, 0.1f);
		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.5f, 0.1f);
	}
}