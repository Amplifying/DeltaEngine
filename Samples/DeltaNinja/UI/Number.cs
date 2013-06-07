using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DeltaNinja
{
	class Number
	{
		public Number(Image[] images, float left, float top, float height, Alignment align, int digitCount, GameRenderLayer layer = GameRenderLayer.Score, Color? color = null)
		{
			this.images = images;
			this.sprites = new List<Sprite>();
			this.align = align;
			this.digitCount = digitCount;

			this.layer = layer;
			this.color = color.GetValueOrDefault(DefaultColors.Gray);

			this.digitHeight = height;
			this.digitWidth = height / images[0].PixelSize.AspectRatio;
						
			this.left = left;
			this.top = top;
		}

		private readonly Image[] images;
		private readonly List<Sprite> sprites;
		private readonly Alignment align;
		private readonly int digitCount;

		private readonly GameRenderLayer layer ;
		private readonly Color color;

		private readonly float digitWidth;
		private readonly float digitHeight;

		private float left;
		public float Left
		{
			get
			{
				return left;
			}
			set
			{
				this.left = value;
				RefreshPosition();
			}
		}

		private float top;
		public float Top
		{
			get
			{
				return top;
			}
			set
			{
				this.top = value;
				RefreshPosition();
			}
		}

		public float Width { get { return digitWidth * sprites.Count;  } }
		public float Height { get { return digitHeight; } }

		public void Show(int value = 0)
		{
			Hide();
			AddDigits(digitCount);
			SetValue(value);
		}

		public void Hide()
		{
			foreach (var sprite in sprites)
				sprite.IsActive = false;
			sprites.Clear();
		}

		public void Fade()
		{
			foreach (var sprite in sprites)
				sprite.AlphaValue -= GameSettigs.FadeStep;
		}

		public void SetValue(int value)
		{
			var digits = new List<int>();
			var letters = value.ToString(CultureInfo.InvariantCulture).ToCharArray();
			digits.AddRange(letters.Select(letter => letter - '0')); //.Reverse());

			if (digits.Count > sprites.Count)
				AddDigits(digits.Count);

			int n = sprites.Count - digits.Count;
			for (int i = 0; i < n; i++)
				digits.Insert(0, digitCount>0 ? 0 : 10);
							
			for (int i = 0; i < sprites.Count; i++)
			{
				sprites[i].Image = images[digits[i]];
			}
		}

		private void AddDigits(int count)
		{
			var empty = images.Last();

			for (int i = sprites.Count; i < count; i++)
			{
				var digitArea = new Rectangle(left, top, digitWidth, digitHeight);
				var digit = new Sprite(empty, digitArea, color)
				{
					Visibility = DeltaEngine.Rendering.Visibility.Show,
					RenderLayer = (int) layer
				};
				sprites.Insert(0, digit);
			}

			RefreshPosition();
		}

		private void RefreshPosition()
		{
			float x = left;

			switch (align)
			{
				case Alignment.Center:
					x -= (digitWidth * sprites.Count) / 2f;
					break;
				case Alignment.Right:
					x -= (digitWidth * sprites.Count);
					break;
			}

			foreach (var sprite in sprites)
			{
				sprite.TopLeft = new Point(x, top);
				x += digitWidth;
			}						
		}
	}
}
