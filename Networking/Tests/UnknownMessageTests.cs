using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class UnknownMessageTests
	{
		[Test]
		public void UnknownMessageToString()
		{
			var message = new UnknownMessage("error");
			Assert.AreEqual("UnknownMessage(error)", message.ToString());
		}
	}
}