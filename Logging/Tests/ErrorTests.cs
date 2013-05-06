using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class ErrorTests
	{
		[Test]
		public void CheckErrorForEquality()
		{
			var firstError = new Error(TestException);
			var secondError = new Error(TestException);
			Assert.AreEqual(firstError, secondError);
			Assert.AreEqual(firstError.GetHashCode(), secondError.GetHashCode());
		}

		private static readonly Exception TestException = new NotFiniteNumberException();

		[Test]
		public void CheckListContains()
		{
			var list = new List<Error> { new Error(TestException) };
			Assert.IsTrue(list.Contains(new Error(TestException)));
		}

		[Test]
		public void SaveAndLoad()
		{
			var error = new Error(TestException);
			var data = error.SaveToMemoryStream();
			var loadedError = data.CreateFromMemoryStream() as Error;
			Assert.AreEqual(error, loadedError);
		}
	}
}