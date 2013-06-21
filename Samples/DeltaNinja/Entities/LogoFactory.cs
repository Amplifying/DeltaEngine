using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;
using System;

namespace DeltaNinja
{
	class LogoFactory
	{
		public LogoFactory(ContentLoader content, ScreenSpace screen)
		{
			this.screen = screen;	
			logoImage = content.Load<Image>("DeltaEngineLogo");
		}

		private readonly ScreenSpace screen;		
		private readonly Image logoImage;

		public Logo Create()
		{
			Randomizer random = Randomizer.Current;

			Size size = new Size(random.Get(0.02f, 0.08f));
			var halfWidth = size.Width / 2f;
			var doubleWidth = size.Width * 2f;

			Rectangle view = screen.Viewport;

			Point position = new Point(random.Get(doubleWidth, view.Width - doubleWidth), view.Bottom - size.Height / 2);

			float direction = position.X > 0.5f ? -1 : 1;
			if (random.Get(1, 100) >= 30) direction *= -1;

			float r;
			if (direction > 0)
				r = random.Get(0, view.Width - position.X - doubleWidth);
			else
				r = random.Get(0, position.X - doubleWidth);

			var h = random.Get(0.3f, view.Height - 0.05f);

			var angle = Math.Atan((4 * h) / r);
			if (angle == 0) angle = 1.57079f;
			var v0 = Math.Sqrt(r * MovingSprite.Gravity / Math.Sin(2 * angle));

			var v_x = (float)(v0 * Math.Cos(angle));
			var v_y = (float)(v0 * Math.Sin(angle));
			v_x *= direction;

			SimplePhysics.Data data = new SimplePhysics.Data()
			{
				Gravity = new Point(0f, MovingSprite.Gravity),
				Velocity = new Point(v_x, -v_y),
				RotationSpeed = random.Get(10, 50) * direction
			};

			return new Logo(logoImage, Color.GetRandomBrightColor(), position, size, data);
		}
	}
}
