using System;
using System.Collections.Generic;
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
			var data = warning.SaveToMemoryStream();
			var loadedWarning = data.CreateFromMemoryStream<Warning>();
			Assert.AreEqual(warning, loadedWarning);
		}
	}
}