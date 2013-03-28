using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Allows to use all the registered log provider for logging purposes.
	/// </summary>
	public abstract class Logger
	{
		protected Logger(params LogProvider[] providers)
		{
			this.providers = providers;
		}

		protected readonly LogProvider[] providers;
		public Info LastMessage { get; private set; }

		public void Info(string message)
		{
			LastMessage = new Info(message);
			foreach (var logProvider in providers)
				logProvider.Log(LastMessage);
		}

		public void Warning(string message)
		{
			LogWarningByProviders(new Warning(message));
		}

		private void LogWarningByProviders(Warning warning)
		{
			foreach (var logProvider in providers)
				logProvider.Log(warning);
			LastMessage = warning;
		}

		public void Warning(Exception exception)
		{
			LogWarningByProviders(new Warning(exception));
		}

		public void Error(Exception exception)
		{
			var errorLog = new Error(exception);
			foreach (var logProvider in providers)
				logProvider.Log(errorLog);
			LastMessage = errorLog;
		}
	}
}