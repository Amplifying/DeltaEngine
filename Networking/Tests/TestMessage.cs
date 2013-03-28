using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Tests
{
	public class TestMessage : BinaryData
	{
		public TestMessage(string text)
		{
			Text = text;
		}

		public string Text { get; set; }

		private TestMessage() {}

		public void LoadData(BinaryReader reader)
		{
			Text = reader.ReadString();
		}

		public void SaveData(BinaryWriter writer)
		{
			writer.Write(Text);
		}
	}
}