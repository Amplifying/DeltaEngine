using System.Collections.Generic;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class LoginTests
	{
		[Test]
		public void CompareLogins()
		{
			ApiKeyLogin firstLogin = GetDummyLogin();
			ApiKeyLogin secondLogin = GetDummyLogin();
			Assert.AreEqual(firstLogin, secondLogin);
			Assert.AreEqual(firstLogin.GetHashCode(), secondLogin.GetHashCode());
		}

		private static ApiKeyLogin GetDummyLogin()
		{
			return new ApiKeyLogin { ApiKey = "01234567-abcd-bcde-cdef-0123456789ab" };
		}

		[Test]
		public void ListContains()
		{
			var list = new List<ApiKeyLogin>();
			list.Add(GetDummyLogin());
			Assert.IsTrue(list.Contains(GetDummyLogin()));
		}

		[Test]
		public void SaveAndLoadLogin()
		{
			ApiKeyLogin login = GetDummyLogin();
			ApiKeyLogin loadedLogin = login.CloneViaBinaryData();
			Assert.AreEqual(login.ApiKey, loadedLogin.ApiKey);
		}
	}
}