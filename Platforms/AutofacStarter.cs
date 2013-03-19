using System;
using DeltaEngine.Logging;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Starts an application on demand by registering, resolving and running it
	/// </summary>
	public class AutofacStarter : AutofacResolver
	{
		public void Start<AppEntryRunner>(int instancesToCreate = 1)
		{
			RegisterEntryRunner<AppEntryRunner>(instancesToCreate);
			Initialize<AppEntryRunner>(instancesToCreate);
			Run();
		}

		private void RegisterEntryRunner<AppEntryRunner>(int instancesToCreate)
		{
			if (typeof(AppEntryRunner).IsInterface || instancesToCreate > 1)
				Register<AppEntryRunner>();
			else
				RegisterSingleton<AppEntryRunner>();
		}

		private void Initialize<AppEntryRunner>(int instancesToCreate)
		{
			Initialized += () =>
			{
				for (int num = 0; num < instancesToCreate; num++)
					Resolve<AppEntryRunner>();
			};
		}

		protected event Action Initialized;

		protected virtual void RaiseInitializedEvent()
		{
			if (Initialized != null)
				Initialized();

			Initialized = null;
		}

		public virtual void Run(Action runCode = null)
		{
			RaiseInitializedEvent();
			var window = Resolve<Window>();
			do
				TryRunAllRunnersAndPresenters(runCode); 
			while (!window.IsClosing);
		}

		private void TryRunAllRunnersAndPresenters(Action runCode)
		{
			try
			{
				RunAllRunners();
				if (runCode != null)
					runCode();

				RunAllPresenters();
			}
			catch (Exception ex)
			{
				var logger = Resolve<Logger>();
				if(logger != null)
					logger.Error(ex);
			}
		}

		public void Start<FirstClass>(Action<FirstClass> initCode, Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			Initialized += () => initCode(Resolve<FirstClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass>(Action<FirstClass, SecondClass> initCode,
			Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			Initialized += () => initCode(Resolve<FirstClass>(), Resolve<SecondClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass, ThirdClass>(
			Action<FirstClass, SecondClass, ThirdClass> initCode, Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			RegisterSingleton<ThirdClass>();
			Initialized +=
				() => initCode(Resolve<FirstClass>(), Resolve<SecondClass>(), Resolve<ThirdClass>());
			Run(runCode);
		}

		public void Close()
		{
			Resolve<Window>().Dispose();
		}
	}
}