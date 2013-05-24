using System.Collections.Generic;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuildRequestTests
	{
		[Test]
		public void CompareBuildRequests()
		{
			var request = GetBuildRequestForWP7WithFakeData();
			var sameRequest = GetBuildRequestForWP7WithFakeData();
			Assert.AreEqual(request, sameRequest);
			Assert.AreEqual(request.GetHashCode(), sameRequest.GetHashCode());
		}

		private static BuildRequest GetBuildRequestForWP7WithFakeData()
		{
			return new BuildRequest("FakeRequestForTest", PlatformName.WindowsPhone7, new byte[1]);
		}

		[Test]
		public void CheckContainsInList()
		{
			var list = new List<BuildRequest>();
			list.Add(GetBuildRequestForWP7WithFakeData());
			Assert.IsTrue(list.Contains(GetBuildRequestForWP7WithFakeData()));
		}

		[Test]
		public void SaveAndLoadRequest()
		{
			var request = GetBuildRequestForWP7WithFakeData();
			var loadedRequest = request.CloneViaBinaryData();
			Assert.AreEqual(request, loadedRequest);
		}
	}
}
