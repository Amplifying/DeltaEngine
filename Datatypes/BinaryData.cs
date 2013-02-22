using System.IO;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Basic functionality to load and save byte data for networking and file operations. This is
	/// used to reconstruct objects from the binary data on the load side via the Factory.
	/// </summary>
	public interface BinaryData
	{
		void Save(BinaryWriter writer);
		void Load(BinaryReader reader);
	}
}