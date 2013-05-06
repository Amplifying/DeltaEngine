using System.IO;

namespace DeltaEngine.Content.Tests
{
	public class MockContentFromName : ContentData
	{
		public MockContentFromName(string contentName)
			: base(contentName) {}

		protected override bool CanLoadDataFromStream
		{
			get { return false; }
		}

		//ncrunch: no coverage start
		protected override void DisposeData() {}
		protected override void LoadData(Stream fileData) {}
	}
}