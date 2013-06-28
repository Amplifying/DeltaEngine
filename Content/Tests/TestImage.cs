using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Content.Tests
{
	internal class TestImage : ContentData
	{
		public TestImage(string contentName)
			: base(contentName) {}

		public Size PixelSize
		{
			get { return MetaData.Get("PixelSize", Size.One); }
		}

		protected override void LoadData(Stream fileData) {}
		protected override void DisposeData() {}
	}
}