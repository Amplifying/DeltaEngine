using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuildMessagesListViewModelTests
	{
		[Test]
		public void AddDifferentMessages()
		{
			var messagesList = new AppBuildMessagesListViewModel();
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestWarning("A test warning for this test"));
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestError("A test error for this test"));
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestError("Just another test error for this test"));
			Assert.AreEqual(1, messagesList.Warnings.Count);
			Assert.AreEqual("1 Warning", messagesList.TextOfWarningCount);
			Assert.AreEqual(2, messagesList.Errors.Count);
			Assert.AreEqual("2 Errors", messagesList.TextOfErrorCount);
		}

		[Test]
		public void OnlyShowingErrorFilter()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = false;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		private static AppBuildMessagesListViewModel GetViewModelWithOneMessageForEachType()
		{
			var messagesList = new AppBuildMessagesListViewModel();
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestWarning("Test warning"));
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestError("Test error"));
			return messagesList;
		}

		[Test]
		public void OnlyShowingWarningFilter()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = false;
			messagesList.IsShowingWarningsAllowed = true;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void ShowingAllKindsOfMessages()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;
			Assert.AreEqual(2, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void CheckMessagesMatchingCurrentFilterOrder()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;

			IList<AppBuildMessageViewModel> messages = messagesList.MessagesMatchingCurrentFilter;
			DateTime timeStampOfFirstElement = messages[0].MessageData.TimeStamp;
			for (int i = 1; i < messages.Count; i++)
				Assert.IsTrue(messages[i].MessageData.TimeStamp >= timeStampOfFirstElement);
		}

		[Test]
		public void ClearMessages()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.ClearMessages();
			Assert.AreEqual(0, messagesList.MessagesMatchingCurrentFilter.Count);
		}
	}
}