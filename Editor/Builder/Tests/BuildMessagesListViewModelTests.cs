using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuildMessagesListViewModelTests
	{
		[Test]
		public void AddDifferentMessages()
		{
			var messagesList = new BuildMessagesListViewModel();
			messagesList.AddMessage("Just a info message for this test".AsBuildTestInfoMessage());
			messagesList.AddMessage("Just another info message for this test".AsBuildTestInfoMessage());
			messagesList.AddMessage("A test warning for this test".AsBuildTestWarning());
			messagesList.AddMessage("A test error for this test".AsBuildTestError());
			messagesList.AddMessage("Just another test error for this test".AsBuildTestError());
			Assert.AreEqual(2, messagesList.Infos.Count);
			Assert.AreEqual("2 Messages", messagesList.TextOfInfoCount);
			Assert.AreEqual(1, messagesList.Warnings.Count);
			Assert.AreEqual("1 Warning", messagesList.TextOfWarningCount);
			Assert.AreEqual(2, messagesList.Errors.Count);
			Assert.AreEqual("2 Errors", messagesList.TextOfErrorCount);
		}

		[Test]
		public void OnlyShowingErrorFilter()
		{
			BuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = false;
			messagesList.IsShowingInfosAllowed = false;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		private static BuildMessagesListViewModel GetViewModelWithOneMessageForEachType()
		{
			var messagesList = new BuildMessagesListViewModel();
			messagesList.AddMessage("Test info".AsBuildTestInfoMessage());
			messagesList.AddMessage("Test warning".AsBuildTestWarning());
			messagesList.AddMessage("Test error".AsBuildTestError());
			return messagesList;
		}

		[Test]
		public void OnlyShowingWarningFilter()
		{
			BuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = false;
			messagesList.IsShowingWarningsAllowed = true;
			messagesList.IsShowingInfosAllowed = false;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void OnlyShowingInfoFilter()
		{
			BuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = false;
			messagesList.IsShowingWarningsAllowed = false;
			messagesList.IsShowingInfosAllowed = true;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void ShowingAllKindsOfMessages()
		{
			BuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;
			messagesList.IsShowingInfosAllowed = true;
			Assert.AreEqual(3, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void CheckMessagesMatchingCurrentFilterOrder()
		{
			BuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;
			messagesList.IsShowingInfosAllowed = true;

			IList<BuildMessageViewModel> messages = messagesList.MessagesMatchingCurrentFilter;
			DateTime timeStampOfFirstElement = messages[0].MessageData.TimeStamp;
			for (int i = 1; i < messages.Count; i++)
				Assert.IsTrue(messages[i].MessageData.TimeStamp >= timeStampOfFirstElement);
		}
	}
}