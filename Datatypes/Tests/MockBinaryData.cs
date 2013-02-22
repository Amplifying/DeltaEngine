using System.IO;

namespace DeltaEngine.Datatypes.Tests
{
	public class MockBinaryData : BinaryData
	{
		public MockBinaryData(string text = "")
		{
			Text = text;
		}

		public string Text { get; private set; }

		public void Save(BinaryWriter writer)
		{
			writer.Write(Text);
		}

		public void Load(BinaryReader reader)
		{
			Text = reader.ReadString();
		}
	}
}