using System;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class EnumExtensionTests
	{
		[Test]
		public void GetValues()
		{
			Array enumValues = TestEnum.SomeValue.GetEnumValues();
			Assert.AreEqual(2, enumValues.Length);
			Assert.AreEqual(TestEnum.SomeValue, enumValues.GetValue(0));
			Assert.AreEqual(TestEnum.AnotherValue, enumValues.GetValue(1));
		}

		[Test]
		public void GetCount()
		{
			Assert.AreEqual(2, TestEnum.SomeValue.GetCount());
			Assert.AreEqual(2, TestEnum.AnotherValue.GetCount());
		}

		private enum TestEnum
		{
			SomeValue,
			AnotherValue,
		}
	}
}