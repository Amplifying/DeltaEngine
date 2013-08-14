using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Very simple logger just spaming out all log events into the console. Used by default.
	/// </summary>
	public class ConsoleLogger : Logger
	{
		public ConsoleLogger()
			: base(true) {}

		public override void Write(MessageType messageType, string message)
		{
			Console.WriteLine(CreateMessageTypePrefix(messageType) + message);
		}

		public override void Dispose()
		{
			base.Dispose();
			if (NumberOfRepeatedMessagesIgnored > 0)
				Console.WriteLine("NumberOfRepeatedMessagesIgnored=" + NumberOfRepeatedMessagesIgnored);
		}
	}
}