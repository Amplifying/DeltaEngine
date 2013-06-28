using System.Collections.Generic;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	class BuildServiceUserTests
	{
		[Test]
		public void CompareUserData()
		{
			BuildServiceUser firstUser = GetDummyUserData();
			BuildServiceUser secondUser = GetDummyUserData();
			Assert.AreEqual(firstUser, secondUser);
			Assert.AreEqual(firstUser.GetHashCode(), secondUser.GetHashCode());
		}

		private static BuildServiceUser GetDummyUserData()
		{
			return new BuildServiceUser
			{
				//TODO: ApiKey = "01234567-abcd-bcde-cdef-0123456789ab",
				AllowedPlaforms = new[] { PlatformName.WindowsPhone7 },
			};
		}

		[Test]
		public void ListContains()
		{
			var list = new List<BuildServiceUser>();
			list.Add(GetDummyUserData());
			Assert.IsTrue(list.Contains(GetDummyUserData()));
		}

		[Test]
		public void SaveAndLoadLogin()
		{
			BuildServiceUser user = GetDummyUserData();
			BuildServiceUser loadedUser = user.CloneViaBinaryData();
			Assert.AreEqual(user, loadedUser);
			// This property can't be currently used because of a BinaryDataExtensions bug for enum arrays
			Assert.AreEqual(user.AllowedPlaforms, loadedUser.AllowedPlaforms);
		}
	}
}