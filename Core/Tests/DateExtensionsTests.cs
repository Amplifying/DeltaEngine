using System;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	internal class DateExtensionsTests
	{
		[Test]
		public void GetIsoDateTime()
		{
			Assert.AreEqual("2009-11-17 13:06:01",
				new DateTime(2009, 11, 17, 13, 6, 1).GetIsoDateTime());
			var testTime = new DateTime(2009, 11, 17, 13, 6, 1);
			Assert.AreEqual(testTime, DateTime.Parse(testTime.GetIsoDateTime()));
		}
	}
}