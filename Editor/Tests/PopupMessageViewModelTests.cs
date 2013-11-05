using System.Threading;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class PopupMessageViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeMessageViewModelAndService()
		{
			service = new MockService("TestUser", "TestProject");
			messageViewModel = new PopupMessageViewModel(service, MessageDisplayTime);	
		}

		private const int MessageDisplayTime = 3001;
		private MockService service;
		private PopupMessageViewModel messageViewModel;

		[Test]
		public void InitialPopupMessageShouldBeInvisibleAndHasNoText()
		{
			Assert.IsEmpty(messageViewModel.Text);
			Assert.AreEqual(Visibility.Hidden, messageViewModel.Visiblity);
		}

		[Test]
		public void RaiseUpdateContentEventShouldShowUpdateText()
		{
			service.RecieveData(ContentType.Image);
			Assert.AreEqual("MockContent Image Updated!", messageViewModel.Text);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void PopupMessageShouldBeHiddenAfterThreeSeconds()
		{
			service.RecieveData(ContentType.Image);
			Thread.Sleep(MessageDisplayTime);
			Assert.AreEqual(Visibility.Hidden, messageViewModel.Visiblity);
		}
	}
}