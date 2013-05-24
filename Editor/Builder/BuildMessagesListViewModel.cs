using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.Builder
{
	public class BuildMessagesListViewModel : ViewModelBase
	{
		public BuildMessagesListViewModel()
		{
			Infos = new List<BuildMessage>();
			Warnings = new List<BuildMessage>();
			Errors = new List<BuildMessage>();
			IsShowingErrorsAllowed = true;
			IsShowingWarningsAllowed = true;
			IsShowingInfosAllowed = true;
		}

		public List<BuildMessage> Infos { get; set; }
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

		public bool IsShowingInfosAllowed
		{
			get { return isShowingInfosAllowed; }
			set
			{
				isShowingInfosAllowed = value;
				TriggerMatchingCurrentFilterChanged();
			}
		}

		private bool isShowingInfosAllowed;

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

		public string TextOfInfoCount
		{
			get { return GetCountAndWordInPluralIfNeeded("Message", Infos.Count); }
		}

		public void AddMessage(BuildMessage message)
		{
			if (message.Type == BuildMessageType.BuildWarning)
				AddMessageToWarnings(message);
			else if (message.Type == BuildMessageType.BuildError)
				AddMessageToErrors(message);
			else
				AddMessageToInfos(message);
			TriggerMatchingCurrentFilterChanged();
		}

		private void AddMessageToWarnings(BuildMessage message)
		{
			Warnings.Add(message);
			RaisePropertyChanged("TextOfWarningCount");
		}

		private void AddMessageToErrors(BuildMessage message)
		{
			Errors.Add(message);
			RaisePropertyChanged("TextOfErrorCount");
		}

		private void AddMessageToInfos(BuildMessage message)
		{
			Infos.Add(message);
			RaisePropertyChanged("TextOfInfoCount");
		}

		public List<BuildMessageViewModel> MessagesMatchingCurrentFilter
		{
			get
			{
				var messages = new List<BuildMessageViewModel>();
				if (IsShowingInfosAllowed)
					AddMessageToViewModelList(Infos, messages);
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