using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class WarningTests
	{
		[Test]
		public void CheckTextWarningForEquality()
		{
			var firstTextWarning = new Warning(TestWarningText);
			var secondTextWarning = new Warning(TestWarningText);
			Assert.AreEqual(firstTextWarning, secondTextWarning);
			Assert.AreEqual(firstTextWarning.GetHashCode(), secondTextWarning.GetHashCode());
		}

		private const string TestWarningText = "A simple text warning";

		[Test]
		public void CheckExceptionWarningForEquality()
		{
			var firstTextWarning = new Warning(new NullReferenceException());
			var secondTextWarning = new Warning(new NullReferenceException());
			Assert.AreEqual(firstTextWarning, secondTextWarning);
			Assert.AreEqual(firstTextWarning.GetHashCode(), secondTextWarning.GetHashCode());
		}

		[Test]
		public void CheckListContains()
		{
			var list = new List<Warning>();
			list.Add(new Warning());
			Assert.IsTrue(list.Contains(new Warning()));
		}

		[Test]
		public void CheckSaveAndLoadWithBinaryDataFactory()
		{
			var warning = new Warning(TestWarningText);
			using (var dataFactory = new BinaryDataFactory())
			using (var dataStream = new MemoryStream())
			{
				dataFactory.Save(warning, new BinaryWriter(dataStream));
				dataStream.Position = 0;
				var loadedWarning = (Warning)dataFactory.Load(new BinaryReader(dataStream));
				Assert.AreEqual(warning, loadedWarning);
			}
		}
	}
}