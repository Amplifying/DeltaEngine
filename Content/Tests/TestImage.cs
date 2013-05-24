using System.IO;

namespace DeltaEngine.Content.Tests
{
	internal class TestImage : ContentData
	{
		public TestImage(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData) {}
		protected override void DisposeData() {}
	}
}