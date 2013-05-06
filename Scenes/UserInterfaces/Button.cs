using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A simple UI button to which events can be attached when being pressed and released.
	/// Can also change color and/or image on mouseover and press.
	/// </summary>
	public class Button : InteractiveControl
	{
		public Button(Image image, Rectangle initialDrawArea)
		{
			Sprite = new Sprite(image, initialDrawArea);
			pressedImage = image;
			normalImage = image;
			mouseoverImage = image;
		}

		public Sprite Sprite { get; private set; }
		private readonly Image pressedImage;
		private readonly Image normalImage;
		private readonly Image mouseoverImage;

		private Button() {}

		internal override bool Contains(Point point)
		{
			return Sprite.DrawArea.Contains(point);
		}

		public override void Press()
		{
			base.Press();
			SetImageAndColor();
		}

		private void SetImageAndColor()
		{
			if (IsInside && IsPressed)
				SetPressedImageAndColor();
			else if (IsInside)
				SetMouseoverImageAndColor();
			else
				SetNormalImageAndColor();
		}

		private void SetPressedImageAndColor()
		{
			Sprite.Image = pressedImage;
			Sprite.Color = PressedColor;
		}

		public Color PressedColor
		{
			get { return pressedColor; }
			set
			{
				pressedColor = value;
				SetImageAndColor();
			}
		}

		private Color pressedColor = Color.White;

		private void SetMouseoverImageAndColor()
		{
			Sprite.Image = mouseoverImage;
			Sprite.Color = MouseoverColor;
		}

		public Color MouseoverColor
		{
			get { return mouseoverColor; }
			set
			{
				mouseoverColor = value;
				SetImageAndColor();
			}
		}

		private Color mouseoverColor = Color.White;

		private void SetNormalImageAndColor()
		{
			Sprite.Image = normalImage;
			Sprite.Color = NormalColor;
		}

		public Color NormalColor
		{
			get { return normalColor; }
			set
			{
				normalColor = value;
				SetImageAndColor();
			}
		}

		private Color normalColor = Color.White;

		public override void Release()
		{
			base.Release();
			SetImageAndColor();
		}

		public override void Enter()
		{
			base.Enter();
			SetImageAndColor();
		}

		public override void Exit()
		{
			base.Exit();
			SetImageAndColor();
		}

		internal override void Show()
		{
			Sprite.Visibility = Visibility.Show;
		}

		internal override void Hide()
		{
			Sprite.Visibility = Visibility.Hide;
		}

		public override void Dispose()
		{
			Sprite.IsActive = false;
		}
	}
}