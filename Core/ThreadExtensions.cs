using System;
using System.Threading;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Useful wrapper functions for threading
	/// </summary>
	public static class ThreadExtensions
	{
		public static Thread Start(this Action threadRunCode)
		{
			var newThread = new Thread(new ThreadStart(threadRunCode));
			newThread.Start();
			return newThread;
		}

		public static Thread Start(string threadName, Action threadRunCode)
		{
			var newThread = new Thread(new ThreadStart(threadRunCode)) { Name = threadName };
			newThread.Start();
			return newThread;
		}

		public static Thread Start(this Action<object> threadRunCode, object param)
		{
			var newThread = new Thread(new ParameterizedThreadStart(threadRunCode));
			newThread.Start(param);
			return newThread;
		}
	}
}