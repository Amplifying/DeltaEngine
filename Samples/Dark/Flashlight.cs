using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace Dark
{
	public class Flashlight : Character
	{
		public Flashlight(Image image, Vector position, ContentLoader content)
			: base(image, position)
		{
			Position = position;
			Facing = upVector;
			Center = Point.Zero;
			DrawArea = Rectangle.FromCenter(position.X, position.Y, 2.4f, 2.0f);
			RenderLayer = 128;
			for (int i = 1; i <= ImageCount; i++)
				images.Add(ContentLoader.Load<Image>("Flashlight0" + i));
		}

		private const int ImageCount = 7;

		protected override void SelectImage()
		{
			if (State == CharacterState.Stopped)
			{
				Image = images[3];
				return;
			}

			Image = images[animationStep];
			if (timeSinceLastImageUpdate >= TimePerImage)
			{
				timeSinceLastImageUpdate = 0.0f;
				animationStep++;
				if (animationStep == ImageCount)
					animationStep = 0;
			}			
		}
	}
}