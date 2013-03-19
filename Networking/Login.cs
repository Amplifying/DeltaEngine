using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking
{
	public abstract class Login : BinaryData
	{
		public abstract void Save(BinaryWriter writer);
		public abstract void Load(BinaryReader reader);
	}
}