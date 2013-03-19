using System.IO;

namespace DeltaEngine.Networking.Tests
{
	public class LoginMock : Login
	{
		public override void Save(BinaryWriter writer)
		{
		}

		public override void Load(BinaryReader reader)
		{
		}
	}
}