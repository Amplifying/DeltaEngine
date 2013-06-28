using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace Dark
{
	public class MainCharacter : Character
	{
		public MainCharacter(Image image, Vector position)
			: base(image, position)
		{
			RenderLayer = 256;
			for (int i = 1; i <= ImageCount; i++)
				images.Add(ContentLoader.Load<Image>("AsylumChar112px0" + i));
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