using System.IO;

namespace DeltaEngine.Content.Tests
{
	public class MockContent : ContentData
	{
		public MockContent(string filename)
			: base(filename)
		{
			ContentChanged += () => changeCounter++;
		}

		public int changeCounter;

		protected override void LoadData(Stream fileData)
		{
			LoadCounter++;
		}
		public int LoadCounter { get; private set; }

		protected override void DisposeData() {}
	}
}