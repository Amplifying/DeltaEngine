using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Tests
{
	public class TestMessage : BinaryData
	{
		public TestMessage() { }

		public TestMessage(string text)
		{
			Text = text;
		}

		public string Text { get; set; }

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