using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Allows someone to select from a range of values via a visible slider
	/// </summary>
	public class Slider : Sprite
	{
		public Slider(Image background, Image pointer, Rectangle drawArea)
			: base(background, drawArea)
		{
			var pointerSize = new Size(pointer.PixelSize.AspectRatio * drawArea.Height, drawArea.Height);
			Add(new Sprite(pointer, new Rectangle(Point.Zero, pointerSize)));
			Add(new Values { MinValue = 0, Value = 100, MaxValue = 100 });
			Add(new Interact.State());
			Add<Interact, UpdateSlider>();
		}

		public class Values
		{
			public int MinValue { get; set; }
			public int Value { get; set; }
			public int MaxValue { get; set; }
		}

		public int MinValue
		{
			get { return Get<Values>().MinValue; }
			set { Get<Values>().MinValue = value; }
		}

		public int Value
		{
			get { return Get<Values>().Value; }
			set { Get<Values>().Value = value; }
		}

		public int MaxValue
		{
			get { return Get<Values>().MaxValue; }
			set { Get<Values>().MaxValue = value; }
		}

		public class UpdateSlider : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (Slider slider in entities.OfType<Slider>())
					UpdateSliderAndPointer(slider, slider.Get<Sprite>());
			}

			private static void UpdateSliderAndPointer(Slider slider, Entity2D pointer)
			{
				UpdateSliderValue(slider, pointer);
				UpdatePointerDrawArea(slider, pointer);
			}

			private static void UpdateSliderValue(Slider slider, Entity2D pointer)
			{
				var state = slider.Get<Interact.State>();
				if (!state.IsPressed)
					return;

				var percentage = state.RelativePointerPosition.X.Clamp(0.0f, 1.0f);
				var unusable = pointer.DrawArea.Aspect / slider.DrawArea.Aspect;
				var expandedPercentage = ((percentage - 0.5f) * (1.0f + unusable) + 0.5f).Clamp(0.0f, 1.0f);
				slider.Value =
					(int)(slider.MinValue + expandedPercentage * (slider.MaxValue - slider.MinValue));
			}

			private static void UpdatePointerDrawArea(Slider slider, Entity2D pointer)
			{
				var drawArea = slider.DrawArea;
				var size = new Size(pointer.DrawArea.Aspect * drawArea.Height, drawArea.Height);
				var percentage = (slider.Value - slider.MinValue) /
					(float)(slider.MaxValue - slider.MinValue);
				var pos = MathExtensions.Lerp(drawArea.Left + size.Width / 2,
					drawArea.Right - size.Width / 2, percentage);
				var center = new Point(pos, drawArea.Center.Y);
				pointer.DrawArea = Rectangle.FromCenter(center, size);
			}

			public override EntityHandlerPriority Priority
			{
				get
				{
					return EntityHandlerPriority.First;
				}
			}
		}
	}
}