using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.Builder
{
	public class BuildMessagesListViewModel : ViewModelBase
	{
		public BuildMessagesListViewModel()
		{
			Warnings = new List<BuildMessage>();
			Errors = new List<BuildMessage>();
			IsShowingErrorsAllowed = true;
			IsShowingWarningsAllowed = true;
		}

		public List<BuildMessage> Warnings { get; set; }
		public List<BuildMessage> Errors { get; set; }

		public bool IsShowingErrorsAllowed
		{
			get { return isShowingErrorsAllowed; }
			set
			{
				isShowingErrorsAllowed = value;
				TriggerMatchingCurrentFilterChanged();
			}
		}

		private void TriggerMatchingCurrentFilterChanged()
		{
			RaisePropertyChanged("MessagesMatchingCurrentFilter");
		}

		private bool isShowingErrorsAllowed;

		public bool IsShowingWarningsAllowed
		{
			get { return isShowingWarningsAllowed; }
			set
			{
				isShowingWarningsAllowed = value;
				TriggerMatchingCurrentFilterChanged();
			}
		}

		private bool isShowingWarningsAllowed;

		public string TextOfErrorCount
		{
			get { return GetCountAndWordInPluralIfNeeded("Error", Errors.Count); }
		}

		private static string GetCountAndWordInPluralIfNeeded(string wordInSingular, int count)
		{
			return count + " " + wordInSingular + (count != 1 ? "s" : "");
		}

		public string TextOfWarningCount
		{
			get
			{
				return GetCountAndWordInPluralIfNeeded("Warning", Warnings.Count);
			}
		}

		public void AddMessage(BuildMessage message)
		{
			if (message.Type == BuildMessageType.BuildError)
				AddMessageToErrors(message);
			else
				AddMessageToWarnings(message);
			TriggerMatchingCurrentFilterChanged();
		}

		private void AddMessageToErrors(BuildMessage message)
		{
			Errors.Add(message);
			RaisePropertyChanged("TextOfErrorCount");
		}

		private void AddMessageToWarnings(BuildMessage message)
		{
			Warnings.Add(message);
			RaisePropertyChanged("TextOfWarningCount");
		}

		public List<BuildMessageViewModel> MessagesMatchingCurrentFilter
		{
			get
			{
				var messages = new List<BuildMessageViewModel>();
				if (IsShowingWarningsAllowed)
					AddMessageToViewModelList(Warnings, messages);
				if (IsShowingErrorsAllowed)
					AddMessageToViewModelList(Errors, messages);
				messages.Sort(SortByTimeStamp);
				return messages;
			}
		}

		private static void AddMessageToViewModelList(List<BuildMessage> messageList,
			List<BuildMessageViewModel> messageViewModelList)
		{
			foreach (BuildMessage message in messageList)
				messageViewModelList.Add(new BuildMessageViewModel(message));
		}

		private static int SortByTimeStamp(BuildMessageViewModel message, BuildMessageViewModel other)
		{
			return message.MessageData.TimeStamp.CompareTo(other.MessageData.TimeStamp);
		}
	}
}