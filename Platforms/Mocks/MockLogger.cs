using System.Collections.Generic;
using DeltaEngine.Logging;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockLogger : Logger
	{
		public MockLogger()
			: base(new MockLogProvider()) { }

		public static readonly List<string> Lines = new List<string>();

		private class MockLogProvider : LogProvider
		{
			public void Log(Info info)
			{
				Lines.Add(info.Text);
			}

			public void Log(Warning warning)
			{
				Lines.Add(warning.Text);
			}

			public void Log(Error error)
			{
				Lines.Add(error.Text);
			}
		}
	}
}