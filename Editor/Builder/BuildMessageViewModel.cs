using System;

namespace DeltaEngine.Editor.Builder
{
	public class BuildMessageViewModel
	{
		public BuildMessageViewModel(BuildMessage buildMessage)
		{
			MessageData = buildMessage;
		}

		internal readonly BuildMessage MessageData;

		public string ImagePath
		{
			get { return "/DeltaEngine.Editor.Builder;component/Images/" + GetIconFilename(); }
		}

		private string GetIconFilename()
		{
			return MessageData.Type + "Icon.png";
		}

		public string IsoTime
		{
			get
			{
				var time = MessageData.TimeStamp;
				return time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" +
					time.Second.ToString("00");
			}
		}

		public string Message
		{
			get { return MessageData.Text; }
		}

		public string Project
		{
			get { return MessageData.Project; }
		}

		public string FileWithLineAndColumn
		{
			get
			{
				if (String.IsNullOrEmpty(MessageData.Filename))
					return "";

				return MessageData.Filename + " (" + MessageData.TextLine + "," +
					MessageData.TextColumn + ")";
			}
		}
	}
}