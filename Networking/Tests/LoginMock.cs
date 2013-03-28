using System.IO;

namespace DeltaEngine.Networking.Tests
{
	public class LoginMock : Login
	{
		public override void SaveData(BinaryWriter writer) {}

		public override void LoadData(BinaryReader reader) {}
	}
}