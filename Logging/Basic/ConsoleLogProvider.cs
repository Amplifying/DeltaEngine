using System;

namespace DeltaEngine.Logging.Basic
{
	public class ConsoleLogProvider : LogProvider
	{
		public void Log(Info info)
		{
			Console.WriteLine(info.Text);
		}

		public void Log(Warning warning)
		{
			Console.WriteLine(warning.Text);
		}

		public void Log(Error error)
		{
			Console.WriteLine(error.Text);
		}
	}
}