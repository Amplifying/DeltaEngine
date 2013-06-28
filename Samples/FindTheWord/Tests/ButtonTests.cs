using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class ButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowHoverState()
		{
			var button = new Button(Resolve<InputCommands>(), "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			RunCode = () => { button.AlphaValue = button.IsHovered ? 0.5f : 1.0f; };
		}

		[Test]
		public void ShowClickableButton()
		{
			var button = new Button(Resolve<InputCommands>(), "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			button.Clicked += b => button.AlphaValue = button.AlphaValue == 1.0f ? 0.5f : 1.0f;
		}

		[Test]
		public void CreateButton()
		{
			var button = new Button(Resolve<InputCommands>(), "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			Assert.IsNotNull(button);
		}
	}
}