using System;
using System.Threading;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	[Ignore]
	public class OnlineServiceTests
	{
		[Test]
		public void CheckOnlineService()
		{
			var service = new OnlineService();
			object result = null;
			var connection = new OnlineServiceConnection(new MockSettings(),
				() => { throw new ConnectionTimedOut(); });
			connection.Connected +=
				() => connection.Send(new LoginRequest(LoadApiKeyFromRegistry(), "LogoApp"));
			connection.DataReceived += message => result = message;
			service.Connect("John Doe", connection);
			Thread.Sleep(500);
			Console.WriteLine("User Name: " + service.UserName);
			CheckService(service, "LogoApp", result);
			service.RequestChangeProject("Asteroids");
			Thread.Sleep(500);
			CheckService(service, "Asteroids", result);
		}

		private static string LoadApiKeyFromRegistry()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					return (string)key.GetValue("ApiKey");
			return null;
		}

		private class ConnectionTimedOut : Exception {}

		private static void CheckService(OnlineService service, string projectName, object message)
		{
			Assert.AreEqual(projectName, service.ProjectName);
			Assert.AreNotEqual(ProjectPermissions.None, service.Permissions);
			Assert.IsInstanceOf<SetProject>(message);
		}
	}
}