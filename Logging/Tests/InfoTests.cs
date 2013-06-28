using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class InfoTests
	{
		[Test]
		public void CheckInfoForEquality()
		{
			var firstInfo = new Info(TestInfoText);
			var secondInfo = new Info(TestInfoText);
			Assert.AreEqual(firstInfo, secondInfo);
			Assert.AreEqual(firstInfo.GetHashCode(), secondInfo.GetHashCode());
		}

		private const string TestInfoText = "A test info message";

		[Test]
		public void CheckListContains()
		{
			var list = new List<Info> { new Info(TestInfoText) };
			Assert.IsTrue(list.Contains(new Info(TestInfoText)));
		}

		[Test]
		public void SaveAndLoad()
		{
			var info = new Info(TestInfoText);
			var data = info.SaveToMemoryStream();
			var loadedInfo = data.CreateFromMemoryStream() as Info;
			Assert.AreEqual(info, loadedInfo);
		}
	}
}