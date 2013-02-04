using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class ArrayExtensionsTests
	{
		[Test]
		public void Contains()
		{
			var numbers = new[] { 1, 2, 5 };
			Assert.IsTrue(numbers.Contains(1));
			Assert.IsFalse(numbers.Contains(3));
		}

		[Test]
		public void ToText()
		{
			var texts = new List<string> { "Hi", "there", "whats", "up?" };
			Assert.AreEqual("Hi, there, whats, up?", texts.ToText());
		}
	}
}
