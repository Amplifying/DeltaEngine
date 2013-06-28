using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class RadioButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderRadioButton()
		{
			CreateRadioButton(Center, "Hello");
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);

		private static void CreateRadioButton(Rectangle drawArea, string text)
		{
			var theme = new Theme
			{
				RadioButtonBackground = new Theme.Appearance("DefaultLabel"),
				RadioButtonNotSelected = new Theme.Appearance("DefaultRadiobuttonOff"),
				RadioButtonNotSelectedMouseover = new Theme.Appearance("DefaultRadioButtonOffHover"),
				RadioButtonSelected = new Theme.Appearance("DefaultRadiobuttonOn"),
				RadioButtonSelectedMouseover = new Theme.Appearance("DefaultRadioButtonOnHover"),
				Font = new Font("Verdana12")
			};
			new RadioButton(theme, drawArea, text);
		}

		[Test]
		public void RenderThreeRadioButtons()
		{
			CreateRadioButton(Top, "Top Button");
			CreateRadioButton(Center, "Middle Button");
			CreateRadioButton(Bottom, "Bottom Button");
		}

		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.5f, 0.1f);
		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.5f, 0.1f);
	}
}