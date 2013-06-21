using System;
using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class SliderTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void DefaultValues(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var slider = CreateSlider(content);
				Assert.AreEqual(0, slider.MinValue);
				Assert.AreEqual(100, slider.Value);
				Assert.AreEqual(100, slider.MaxValue);
			});
		}

		private static Slider CreateSlider(ContentLoader content)
		{
			var theme = new Theme
			{
				Slider = new Theme.Appearance(content.Load<Image>("DefaultButtonBackground")),
				SliderPointer = new Theme.Appearance(content.Load<Image>("DefaultSlider")),
				SliderPointerMouseover = new Theme.Appearance(content.Load<Image>("DefaultSliderHover"))
			};
			var slider = new Slider(theme, Center);
			EntitySystem.Current.Run();
			return slider;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);

		[IntegrationTest]
		public void UpdateValues(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var slider = CreateSlider(content);
				slider.MinValue = 1;
				slider.Value = 2;
				slider.MaxValue = 3;
				Assert.AreEqual(1, slider.MinValue);
				Assert.AreEqual(2, slider.Value);
				Assert.AreEqual(3, slider.MaxValue);
			});
		}

		[IntegrationTest]
		public void ValidatePointerSize(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var slider = CreateSlider(content);
				var pointer = content.Load<Image>("DefaultSlider");
				var width = pointer.PixelSize.AspectRatio * 0.1f;
				var pointerSize = new Size(width, 0.1f);
				Assert.AreEqual(pointerSize, slider.Get<Slider.Pointer>().DrawArea.Size);
			});
		}

		[IntegrationTest]
		public void ValidatePointerCenter(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var slider = CreateSlider(content);
				var position = new Point(0.42f, 0.52f);
				DragMouse(position);
				Assert.AreEqual(new Point(0.42f, 0.5f), slider.Get<Slider.Pointer>().DrawArea.Center);
			});
		}

		private void DragMouse(Point position)
		{
			mockResolver.input.SetMousePosition(position + new Point(0.1f, 0.1f));
			mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			mockResolver.AdvanceTimeAndExecuteRunners();
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		[VisualTest]
		public void RenderSliderZeroToOneHundred(Type resolver)
		{
			Slider slider = null;
			Start(resolver, (Scene s, ContentLoader content) => { slider = CreateSlider(content); },
				(Window window) => { window.Title = slider.Value.ToString(CultureInfo.InvariantCulture); });
		}

		[VisualTest]
		public void RenderSliderMinusFiveToFive(Type resolver)
		{
			Slider slider = null;
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				slider = CreateSlider(content);
				slider.MinValue = -5;
				slider.Value = 0;
				slider.MaxValue = 5;
			},
				(Window window) => { window.Title = slider.Value.ToString(CultureInfo.InvariantCulture); });
		}

		[VisualTest]
		public void RenderGrowingSlider(Type resolver)
		{
			Slider slider = null;
			Start(resolver, (Scene s, ContentLoader content) => { slider = CreateSlider(content); },
				(Window window) =>
				{
					window.Title = slider.Value.ToString(CultureInfo.InvariantCulture);
					var center = slider.DrawArea.Center + new Point(0.02f, 0.02f) * Time.Current.Delta;
					var size = slider.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
					slider.DrawArea = Rectangle.FromCenter(center, size);
				});
		}
	}
}