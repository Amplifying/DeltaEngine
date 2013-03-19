using System.Collections.Generic;
using System.IO;
using DeltaEngine.Datatypes;
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
			var list = new List<Info>();
			list.Add(new Info(TestInfoText));
			Assert.IsTrue(list.Contains(new Info(TestInfoText)));
		}

		[Test]
		public void CheckSaveAndLoadWithBinaryDataFactory()
		{
			var info = new Info(TestInfoText);
			using (var dataFactory = new BinaryDataFactory())
			using (var dataStream = new MemoryStream())
			{
				dataFactory.Save(info, new BinaryWriter(dataStream));
				dataStream.Position = 0;
				var loadedInfo = (Info)dataFactory.Load(new BinaryReader(dataStream));
				Assert.AreEqual(info, loadedInfo);
			}
		}
	}
}