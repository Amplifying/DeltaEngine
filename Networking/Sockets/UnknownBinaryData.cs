using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Sockets
{
	public class UnknownBinaryData : BinaryData
	{
		public UnknownBinaryData() { }

		public UnknownBinaryData(string error)
		{
			Error = error;
		}

		public string Error { get; private set; }

		public void SaveData(BinaryWriter writer)
		{
			writer.Write(Error);
		}

		public void LoadData(BinaryReader reader)
		{
			Error = reader.ReadString();
		}
	}
}