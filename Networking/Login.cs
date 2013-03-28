using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking
{
	public abstract class Login : BinaryData
	{
		public abstract void SaveData(BinaryWriter writer);
		public abstract void LoadData(BinaryReader reader);
	}
}