using System.IO;

namespace DeltaEngine.Content.Tests
{
	internal class TestSound : ContentData
	{
		public TestSound(string contentName)
			: base(contentName) { }

		protected override void LoadData(Stream fileData) { }
		protected override void DisposeData() { }
	}
}