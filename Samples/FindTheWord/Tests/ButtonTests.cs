using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class ButtonTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void ShowHoverState(Type resolver)
		{
			Button button = null;
			Start(resolver, (InputCommands input, ContentLoader content) =>
			{
				button = new Button(input, content, "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			}, () =>
			{
				button.AlphaValue = button.IsHovered ? 0.5f : 1.0f;
			});
		}

		[VisualTest]
		public void ShowClickableButton(Type resolver)
		{
			Start(resolver, (InputCommands input, ContentLoader content) =>
			{
				var button = new Button(input, content, "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
				button.Clicked += b => button.AlphaValue =  button.AlphaValue == 1.0f ? 0.5f : 1.0f;
			});
		}

		[Test]
		public void CreateButton()
		{
			Start(typeof(MockResolver), (InputCommands input, ContentLoader content) =>
			{
				var button = new Button(input, content, "Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
				Assert.IsNotNull(button);
			});
		}
	}
}