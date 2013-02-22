using System.IO;

namespace DeltaEngine.Datatypes
{
	public class UnknownBinaryData : BinaryData
	{
		public UnknownBinaryData() { }

		public UnknownBinaryData(string error)
		{
			Error = error;
		}

		public string Error { get; private set; }

		public void Save(BinaryWriter writer)
		{
			writer.Write(Error);
		}

		public void Load(BinaryReader reader)
		{
			Error = reader.ReadString();
		}
	}
}