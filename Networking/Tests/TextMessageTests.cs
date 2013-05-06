using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class TextMessageTests
	{
		[Test]
		public void NewTextMessageStoresText()
		{
			var message = new TextMessage("hello");
			Assert.AreEqual("hello", message.Text);
		}

		[Test]
		public void TextMessagesWithTheSameTextAreEqual()
		{
			var message1 = new TextMessage("hello");
			var message2 = new TextMessage("hello");
			var message3 = new TextMessage("goodbye");
			Assert.IsTrue(message1.Equals((object)message2));
			Assert.IsFalse(message1.Equals(message3));
		}

		[Test]
		public void TextMessagesWithTheSameTextHaveTheSameHashCodes()
		{
			var message1 = new TextMessage("hello");
			var message2 = new TextMessage("hello");
			var message3 = new TextMessage("goodbye");
			Assert.IsTrue(message1.GetHashCode() == message2.GetHashCode());
			Assert.IsFalse(message1.GetHashCode() == message3.GetHashCode());
		}
	}
}