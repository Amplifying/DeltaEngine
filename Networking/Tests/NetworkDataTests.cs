using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class NetworkDataTests
	{
		[Test]
		public void SerializeNetworkData()
		{
			NetworkData networkData = GetNetworkTestData();
			byte[] serializedData = networkData.ToByte();
			var deserializedNetworkData = new NetworkData(serializedData);
			// TODO: implement IEquatable<>
			AssertThatDataAreEqual(GetNetworkTestData(), deserializedNetworkData);
		}

		private NetworkData GetNetworkTestData()
		{
			return new NetworkData
			{
				Command = NetworkCommand.Login,
				UserId = "TestUser",
				AppName = "TestApp",
				Platform = PlatformName.WindowsPhone7,
				CommandDetails = "Hi Server.",
				BinaryData = new byte[] { 120, 240, },
			};
		}

		private void AssertThatDataAreEqual(NetworkData expectedData, NetworkData actualData)
		{
			Assert.AreEqual(expectedData.Command, actualData.Command);
			Assert.AreEqual(expectedData.UserId, actualData.UserId);
			Assert.AreEqual(expectedData.AppName, actualData.AppName);
			Assert.AreEqual(expectedData.Platform, actualData.Platform);
			Assert.AreEqual(expectedData.CommandDetails, actualData.CommandDetails);
			Assert.AreEqual(expectedData.BinaryData, actualData.BinaryData);
		}
	}
}
