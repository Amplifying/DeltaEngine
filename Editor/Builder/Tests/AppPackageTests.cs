using System.Collections.Generic;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class AppPackageTests
	{
		[Test]
		public void CompareBuildPackage()
		{
			AppPackage package = PlatformName.WindowsPhone7.GetTestPackageForWP7();
			AppPackage samePackage = PlatformName.WindowsPhone7.GetTestPackageForWP7();
			Assert.AreEqual(package, samePackage);
			Assert.AreNotEqual(package, new object());
			Assert.AreEqual(package.GetHashCode(), samePackage.GetHashCode());
		}

		[Test]
		public void CheckContainsInList()
		{
			var list = new List<AppPackage>();
			list.Add(PlatformName.WindowsPhone7.GetTestPackageForWP7());
			Assert.IsTrue(list.Contains(PlatformName.WindowsPhone7.GetTestPackageForWP7()));
		}

		[Test]
		public void SaveAndLoadRequest()
		{
			var package = PlatformName.WindowsPhone7.GetTestPackageForWP7();
			var loadedPackage = package.CloneViaBinaryData();
			Assert.AreEqual(package, loadedPackage);
		}
	}
}