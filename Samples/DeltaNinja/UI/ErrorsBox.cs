using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaNinja
{
	class ErrorsBox
	{
		public ErrorsBox(ContentLoader content, float right, float top)
		{
			var errImage = content.Load<Image>("ErrorIcon");

			var offsets = new[,] { { 0.108f, 0.025f }, { 0.083f, 0.033f }, { 0.05f, 0.05f } };

			errorIcons = new Sprite[3];

			for (int i = 0; i < errorIcons.Length; i++)
			{
				errorIcons[i] = new Sprite(errImage, new Rectangle(right - offsets[i, 0], top, offsets[i, 1], offsets[i, 1]))
				{
					RenderLayer = (int)GameRenderLayer.Score,
					Visibility = Visibility.Hide
				};
			}
		}

		private Sprite[] errorIcons;

		public void Show()
		{
			foreach (var errIcon in errorIcons)
			{
				errIcon.Color = DefaultColors.Gray;
				errIcon.Visibility = Visibility.Show;
			}
		}

		public void SetError(int count)
		{
			errorIcons[count - 1].Color = DefaultColors.Red;
		}

		public void Hide()
		{
			foreach (var errIcon in errorIcons)
				errIcon.Visibility = Visibility.Hide;
		}

		public float Top
		{
			set
			{
				foreach (var errIcon in errorIcons)
					errIcon.TopLeft = new Point(errIcon.TopLeft.X, value);
			}
		}
	}
}
