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
		public void Compare()
		{
			var numbers1 = new[] { 1, 2, 5 };
			var numbers2 = new[] { 1, 2, 5 };
			var numbers3 = new[] { 1, 2, 5, 7 };
			Assert.IsTrue(numbers1.Compare(numbers2));
			Assert.IsFalse(numbers1.Compare(null));
			Assert.IsFalse(numbers1.Compare(numbers3));
			Assert.IsFalse(numbers3.Compare(numbers1));

			byte[] optionalData = null;
			Assert.IsTrue(optionalData.Compare(null));
		}

		[Test]
		public void ToText()
		{
			var texts = new List<string> { "Hi", "there", "whats", "up?" };
			Assert.AreEqual("Hi, there, whats, up?", texts.ToText());
		}
	}
}
