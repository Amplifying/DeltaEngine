namespace DeltaEngine.Platforms.Mocks
{
	internal class MockSettings : Settings
	{
		public MockSettings()
		{
			SetFallbackSettings();
		}

		public override void Save() {}
	}
}