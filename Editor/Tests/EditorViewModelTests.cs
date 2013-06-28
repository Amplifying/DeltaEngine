using System;
using System.Threading;
using System.Windows;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Common.Properties;
using DeltaEngine.Editor.Helpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	[Category("Slow"), Ignore]
	public class EditorViewModelTests
	{
		//ncrunch: no coverage start
		[Test]
		public void LoginWithInvalidApiKeyShouldFail()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			model.OnApiKeyChanged.Execute("invalid");
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			Assert.AreEqual(Resources.NotConnectedToDeltaEngine, model.Error);
		}

		private static EditorViewModel CreateModelAndTryToLoginWithEmptyApiKey()
		{
			var model = new EditorViewModel(new EditorPluginLoader(".."));
			model.OnApiKeyChanged.Execute("");
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			return model;
		}

		[Test]
		public void LoginWithValidApiKeyShouldPass()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			Assert.AreEqual(Resources.ObtainApiKey, model.Error);
			model.LoadApiKey();
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			Assert.AreEqual("", model.Error);
		}

		[Test]
		public void LoginPanelShouldBeVisibleWhenNotLoggedIn()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			if (model.Error == "")
			{
				Assert.AreEqual(Visibility.Hidden, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Visible, model.EditorPanelVisibility);
			}
			else
			{
				Assert.AreEqual(Visibility.Visible, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Hidden, model.EditorPanelVisibility);
			}
		}

		[Test]
		public void LoadApiKeyFromRegistry()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			var rememberKey = model.ApiKey;
			Console.WriteLine("Current API Key from Registry: " + rememberKey);
			model.OnApiKeyChanged.Execute("123");
			model.SaveApiKey();
			Assert.AreEqual("123", model.ApiKey);
			model.OnApiKeyChanged.Execute(rememberKey);
			model.SaveApiKey();
		}

		[Test]
		public void WebsiteWhereApiKeyCanBeObtainedShouldBeOpened()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			model.OnGetApiKeyClicked.Execute(null);
		}

		[Test]
		public void CreateEditorPluginEntryFromLoadedPlugins()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins);
			model.AddAllPlugins();
			Assert.AreEqual(1, model.EditorPlugins.Count);
			Assert.AreEqual("MockEditorPlugin", model.EditorPlugins[0].ShortName);
			Assert.AreEqual("Mock.png", model.EditorPlugins[0].Icon);
			Assert.AreEqual(EditorPluginCategory.Settings, model.EditorPlugins[0].Category);
			Assert.AreEqual(typeof(MockEditorPluginView), model.EditorPlugins[0].GetType());
		}

		private static EditorPluginLoader GetEditorPluginLoaderMock()
		{
			var mockPlugins = new EditorPluginLoader("..");
			mockPlugins.UserControlsType.Clear();
			mockPlugins.UserControlsType.Add(typeof(MockEditorPluginView));
			return mockPlugins;
		}
	}
}