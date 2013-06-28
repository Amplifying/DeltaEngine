using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class SliderTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderSliderZeroToOneHundred()
		{
			var slider = CreateSlider();
			RunCode = () => { Window.Title = slider.Value.ToString(CultureInfo.InvariantCulture); };
		}

		private static Slider CreateSlider()
		{
			var theme = new Theme
			{
				Slider = new Theme.Appearance("DefaultButtonBackground"),
				SliderPointer = new Theme.Appearance("DefaultSlider"),
				SliderPointerMouseover = new Theme.Appearance("DefaultSliderHover")
			};
			var slider = new Slider(theme, Center);
			EntitySystem.Current.Run();
			return slider;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);

		[Test]
		public void RenderSliderMinusFiveToFive()
		{
			var slider = CreateSlider();
			slider.MinValue = -5;
			slider.Value = 0;
			slider.MaxValue = 5;
			RunCode = () => { Window.Title = slider.Value.ToString(CultureInfo.InvariantCulture); };
		}

		[Test]
		public void RenderGrowingSlider()
		{
			var slider = CreateSlider();
			RunCode = () =>
			{
				Window.Title = slider.Value.ToString(CultureInfo.InvariantCulture);
				var center = slider.DrawArea.Center + new Point(0.02f, 0.02f) * Time.Current.Delta;
				var size = slider.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
				slider.DrawArea = Rectangle.FromCenter(center, size);
			};
		}

		[Test]
		public void DefaultValues()
		{
			var slider = CreateSlider();
			Assert.AreEqual(0, slider.MinValue);
			Assert.AreEqual(100, slider.Value);
			Assert.AreEqual(100, slider.MaxValue);
			Window.CloseAfterFrame();
		}

		[Test]
		public void UpdateValues()
		{
			var slider = CreateSlider();
			slider.MinValue = 1;
			slider.Value = 2;
			slider.MaxValue = 3;
			Assert.AreEqual(1, slider.MinValue);
			Assert.AreEqual(2, slider.Value);
			Assert.AreEqual(3, slider.MaxValue);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ValidatePointerSize()
		{
			var slider = CreateSlider();
			var pointer = ContentLoader.Load<Image>("DefaultSlider");
			var width = pointer.PixelSize.AspectRatio * 0.1f;
			var pointerSize = new Size(width, 0.1f);
			Assert.AreEqual(pointerSize, slider.Get<Slider.Pointer>().DrawArea.Size);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ValidatePointerCenter()
		{
			var slider = CreateSlider();
			var position = new Point(0.42f, 0.52f);
			DragMouse(position);
			Assert.AreEqual(new Point(0.7f, 0.5f), slider.Get<Slider.Pointer>().DrawArea.Center);
			Window.CloseAfterFrame();
		}

		private void DragMouse(Point position)
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(position + new Point(0.1f, 0.1f));
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
			Resolve<MockMouse>().SetMousePositionNextFrame(position);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners();
		}
	}
}