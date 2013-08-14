using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Messages
{
	public sealed class SupportedPlatformsResult
	{
		private SupportedPlatformsResult() { }

		public SupportedPlatformsResult(PlatformName[] platforms)
		{
			Platforms = platforms;
		}

		public PlatformName[] Platforms { get; private set; }
	}
}