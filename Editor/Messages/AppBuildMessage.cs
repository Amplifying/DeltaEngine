using System;

namespace DeltaEngine.Editor.Messages
{
	public class AppBuildMessage
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected AppBuildMessage() {} // ncrunch: no coverage

		public AppBuildMessage(string text)
		{
			Text = text;
			TimeStamp = DateTime.Now;
			Type = AppBuildMessageType.BuildInfo;
			Filename = "";
			TextLine = "";
			TextColumn = "";
		}

		public string Text { get; private set; }
		public DateTime TimeStamp { get; private set; }
		public AppBuildMessageType Type { get; set; }
		public string Project { get; set; }
		public string Filename { get; set; }
		public string TextLine { get; set; }
		public string TextColumn { get; set; }

		// ncrunch: no coverage start
		public override string ToString()
		{
			return Type + ": " + Text;
		}
		// ncrunch: no coverage end
	}
}