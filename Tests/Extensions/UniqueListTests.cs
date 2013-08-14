using System.Collections.Generic;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class UniqueListTests
	{
		[Test]
		public void TestUniqueList()
		{
			var list = new UniqueList<int> { 2, 3, 4, 2 };
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2, list[0]);
			Assert.AreEqual("2, 3, 4", list.ToText());
			list.Remove(2);
			list.Remove(5);
			Assert.AreEqual("3, 4", list.ToText());
		}

		[Test]
		public void ToArray()
		{
			var list = new UniqueList<int> { 3, 4 };
			int[] intList = list.ToArray();
			Assert.AreEqual(3, intList[0]);
			Assert.AreEqual(4, intList[1]);
			Assert.AreEqual(2, intList.Length);
		}

		[Test]
		public void TestConstructorWithDuplicatesToBeRemoved()
		{
			var listWithDuplicates = new List<int> { 3, 5, 7, 3 };
			var copiedUniqueList = new UniqueList<int>(listWithDuplicates);
			Assert.AreEqual(listWithDuplicates.Count, 4);
			Assert.AreEqual(copiedUniqueList.Count, 3);
			Assert.AreEqual(copiedUniqueList[0], 3);
			Assert.AreEqual(copiedUniqueList[1], 5);
			Assert.AreEqual(copiedUniqueList[2], 7);
		}
	}
}