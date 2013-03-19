using System;
using System.Collections.Generic;
using System.IO;
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
			var list = new List<Error>();
			list.Add(new Error(TestException));
			Assert.IsTrue(list.Contains(new Error(TestException)));
		}

		[Test]
		public void CheckSaveAndLoadWithBinaryDataFactory()
		{
			var error = new Error(TestException);
			using (var dataFactory = new BinaryDataFactory())
			using (var dataStream = new MemoryStream())
			{
				dataFactory.Save(error, new BinaryWriter(dataStream));
				dataStream.Position = 0;
				var loadedError = (Error)dataFactory.Load(new BinaryReader(dataStream));
				Assert.AreEqual(error, loadedError);
			}
		}
	}
}