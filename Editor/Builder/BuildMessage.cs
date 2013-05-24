using System;
using DeltaEngine.Networking;

namespace DeltaEngine.Editor.Builder
{
	public class BuildMessage : TextMessage
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected BuildMessage() { }
		public BuildMessage(string text)
			: base(text)
		{
			TimeStamp = DateTime.Now;
			Type = BuildMessageType.BuildInfo;
		}

		public DateTime TimeStamp { get; private set; }
		public BuildMessageType Type { get; set; }
		public string Project { get; set; }
		public string Filename { get; set; }
		public int TextLine { get; set; }
		public int TextColumn { get; set; }

		public override string ToString()
		{
			return "Build" + Type + ": " + Text;
		}
	}
}