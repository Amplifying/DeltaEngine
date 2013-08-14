using System;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	internal class DateExtensionsTests
	{
		[Test]
		public void GetIsoDateTime()
		{
			Assert.AreEqual("2009-11-17 13:06:01", new DateTime(2009, 11, 17, 13, 6, 1).GetIsoDateTime());
			var testTime = new DateTime(2009, 11, 17, 13, 6, 1);
			Assert.AreEqual(testTime, DateTime.Parse(testTime.GetIsoDateTime()));
		}

		[Test]
		public void GetDateTimeFromString()
		{
			var isoDateTime = DateExtensions.Parse("2013-08-07 22:37:46Z");
			var germanDateTime = DateExtensions.Parse("7.8.2013 22:37:46");
			var englishDateTime = DateExtensions.Parse("08/07/2013 10:37:46 PM");
			Assert.AreEqual(isoDateTime, germanDateTime);
			Assert.AreEqual(isoDateTime, englishDateTime);
			var currentTime = DateTime.Now + "";
			Assert.AreEqual(DateExtensions.Parse(currentTime), Convert.ToDateTime(currentTime));
		}

		[Test]
		public void IncorrectRequestResultsInCurrentDateTime()
		{
			const string IncorrectDateTime = "2013[08]05";
			var incorrectDateTime = DateExtensions.Parse(IncorrectDateTime);
			Assert.AreEqual(DateTime.Now, incorrectDateTime);
		}
	}
}