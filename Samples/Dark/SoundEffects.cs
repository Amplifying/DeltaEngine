using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Multimedia;
using DeltaEngine.Content;

namespace Dark
{
	public class SoundEffects : Runner
	{
		public SoundEffects()
		{
			//mainTheme = content.Load<Sound>("AsylumMainTheme");
			//footSteps = content.Load<Sound>("AsylumFootSteps");
			heartBeat = ContentLoader.Load<Sound>("AsylumHeartbeat");

			for (uint i = 5; i <= SoundsCount; ++i)
				ambient.Add(ContentLoader.Load<Sound>("AsylumSound" + i));
		}

		private Sound mainTheme;
		private Sound heartBeat;
		private Sound footSteps;
		private readonly List<Sound> ambient = new List<Sound>();
		private const int SoundsCount = 12;

		public void Run()
		{
			elapsedTime += Time.Current.Delta;
			if (elapsedTime > nextTimeStep)
			{
				ambient[Randomizer.Current.Get(0, ambient.Count)].Play();
				nextTimeStep += Randomizer.Current.Get(10.0f, 20.0f);
			}

			if (mainTheme != null)
				if (!mainTheme.IsAnyInstancePlaying)
					mainTheme.Play();
		}

		float nextTimeStep = 5.0f;
		private float elapsedTime;
	}
}