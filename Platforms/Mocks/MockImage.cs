using System.IO;
using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockImage : Image
	{
		public MockImage(string contentName)
			: base(contentName) {}

		protected override void LoadImage(Stream fileData) {}
		protected override void SetSamplerState() {}
		protected override void DisposeData() {}
	}
}