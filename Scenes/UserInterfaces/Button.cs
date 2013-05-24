using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A simple UI button which changes image and color on mouseover and press, and raises a 
	/// Clicked event on clicking.
	/// </summary>
	public class Button : Sprite, Interact.Clickable
	{
		public Button(Image image)
			: this(image, Rectangle.Zero) {}

		public Button(Image image, Rectangle drawArea)
			: this(image, drawArea, Color.White) { }

		public Button(Image image, Rectangle drawArea, Color color)
			: base(image, drawArea)
		{
			Color = color;
			Add(new Interact.State());
			Add(new Interact.Images(image));
			Add(new Interact.Colors(color));
			Add<Interact, Interact.RaiseClickEvent, UpdateImageAndColor>();
		}

		public Point RelativePointerPosition
		{
			get { return Get<Interact.State>().RelativePointerPosition; }
		}

		public void InvokeClickEvent()
		{
			if (Clicked != null)
				Clicked();
		}

		public event Action Clicked;

		public class UpdateImageAndColor : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				var state = entity.Get<Interact.State>();
				if (state.IsInside && state.IsPressed)
					SetPressedImageAndColor(entity);
				else if (state.IsInside)
					SetMouseoverImageAndColor(entity);
				else
					SetNormalImageAndColor(entity);
			}

			private static void SetPressedImageAndColor(Entity entity)
			{
				if (entity.Contains<Image, Interact.Images>())
					entity.Set(entity.Get<Interact.Images>().Pressed);

				if (entity.Contains<Color, Interact.Colors>())
					entity.Set(entity.Get<Interact.Colors>().Pressed);
			}

			private static void SetMouseoverImageAndColor(Entity entity)
			{
				if (entity.Contains<Image, Interact.Images>())
					entity.Set(entity.Get<Interact.Images>().Mouseover);

				if (entity.Contains<Color, Interact.Colors>())
					entity.Set(entity.Get<Interact.Colors>().Mouseover);
			}

			private static void SetNormalImageAndColor(Entity entity)
			{
				if (entity.Contains<Image, Interact.Images>())
					entity.Set(entity.Get<Interact.Images>().Normal);

				if (entity.Contains<Color, Interact.Colors>())
					entity.Set(entity.Get<Interact.Colors>().Normal);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.High; }
			}
		}

		public Color NormalColor
		{
			get { return Get<Interact.Colors>().Normal; }
			set { Get<Interact.Colors>().Normal = value; }
		}

		public Color PressedColor
		{
			get { return Get<Interact.Colors>().Pressed; }
			set { Get<Interact.Colors>().Pressed = value; }
		}

		public Color MouseoverColor
		{
			get { return Get<Interact.Colors>().Mouseover; }
			set { Get<Interact.Colors>().Mouseover = value; }
		}

		public Image NormalImage
		{
			get { return Get<Interact.Images>().Normal; }
			set { Get<Interact.Images>().Normal = value; }
		}

		public Image PressedImage
		{
			get { return Get<Interact.Images>().Pressed; }
			set { Get<Interact.Images>().Pressed = value; }
		}

		public Image MouseoverImage
		{
			get { return Get<Interact.Images>().Mouseover; }
			set { Get<Interact.Images>().Mouseover = value; }
		}

		public bool IsHovering
		{
			get { return Get<Interact.State>().IsHovering; }
		}

		public bool IsInside
		{
			get { return Get<Interact.State>().IsInside; }
		}

		public bool IsPressed
		{
			get { return Get<Interact.State>().IsPressed; }
		}
	}
}