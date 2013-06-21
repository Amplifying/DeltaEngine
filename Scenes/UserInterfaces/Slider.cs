using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Allows someone to select from a range of values via a visible slider
	/// </summary>
	public class Slider : Sprite
	{
		public Slider(Theme theme, Rectangle drawArea)
			: base(theme.Slider.Image, drawArea, theme.Slider.Color)
		{
			Add(new Values { MinValue = 0, Value = 100, MaxValue = 100 });
			Add(new Pointer(theme));
			Add(new Interact.State());
			Add<Interact, UpdateSlider>();
		}

		private class Values
		{
			public int MinValue { get; set; }
			public int Value { get; set; }
			public int MaxValue { get; set; }
		}

		internal class Pointer : Sprite
		{
			public Pointer(Theme theme)
				: base(theme.SliderPointer.Image, theme.SliderPointer.Color)
			{
				Add(new Interact.State());
				Add(theme);
				Add<Interact, UpdateAppearance>();
			}
		}

		private class UpdateAppearance : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				if (!interactions.Contains(message.GetType()))
					return;

				var state = entity.Get<Interact.State>();
				var theme = entity.Get<Theme>();
				SetAppearance(entity, state.IsInside ? theme.SliderPointerMouseover : theme.SliderPointer);
			}

			private readonly List<Type> interactions = new List<Type>(typeof(Interact).GetNestedTypes());

			private static void SetAppearance(Entity entity, Theme.Appearance appearance)
			{
				entity.Set(appearance.Image);
				entity.Set(appearance.Color);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.High; }
			}
		}

		private class UpdateSlider : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				var slider = entity as Slider;
				var pointer = entity.Get<Pointer>();
				UpdatePointerState(slider, pointer);
				UpdateSliderValue(slider, pointer);
				UpdatePointerDrawArea(slider, pointer);
				pointer.RenderLayer = slider.RenderLayer + 1;
			}

			private static void UpdatePointerState(Entity slider, Entity pointer)
			{
				var sliderState = slider.Get<Interact.State>();
				if (!sliderState.IsInside || !sliderState.IsPressed)
					return;

				var pointerState = pointer.Get<Interact.State>();
				pointerState.IsInside = true;
				pointerState.IsPressed = true;
				pointer.MessageAllListeners(new Interact.ControlPressed());
			}

			private static void UpdateSliderValue(Slider slider, Sprite pointer)
			{
				var state = slider.Get<Interact.State>();
				if (!state.IsPressed)
					return;

				var percentage = state.RelativePointerPosition.X.Clamp(0.0f, 1.0f);
				var unusable = pointer.Image.PixelSize.AspectRatio / slider.DrawArea.Aspect;
				var expandedPercentage = ((percentage - 0.5f) * (1.0f + unusable) + 0.5f).Clamp(0.0f, 1.0f);
				slider.Value =
					(int)(slider.MinValue + expandedPercentage * (slider.MaxValue - slider.MinValue));
			}

			private static void UpdatePointerDrawArea(Slider slider, Sprite pointer)
			{
				var drawArea = slider.DrawArea;
				var size = new Size(pointer.Image.PixelSize.AspectRatio * drawArea.Height, drawArea.Height);
				var percentage = (slider.Value - slider.MinValue) /
					(float)(slider.MaxValue - slider.MinValue);
				var pos = MathExtensions.Lerp(drawArea.Left + size.Width / 2,
					drawArea.Right - size.Width / 2, percentage);
				var center = new Point(pos, drawArea.Center.Y);
				pointer.DrawArea = Rectangle.FromCenter(center, size);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
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
	}
}