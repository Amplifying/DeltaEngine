using System.IO;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Basic functionality to save and reconstruct byte data for networking and file operations.
	/// </summary>
	public interface BinaryData
	{
		void SaveData(BinaryWriter writer);
		void LoadData(BinaryReader reader);
	}
}