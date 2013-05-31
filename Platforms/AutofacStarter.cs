using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac.Core;
using DeltaEngine.Core;
using DeltaEngine.Entities;
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
			CreateEntitySystemAndAddAsRunner();
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

		internal void CreateEntitySystemAndAddAsRunner()
		{
			var entitySystem = new EntitySystem(new AutofacEntityResolver(this));
			RegisterInstanceAsRunnerOrPresenterIfPossible(entitySystem);
			EntitySystem.Use(entitySystem);
		}

		private class AutofacEntityResolver : EntityHandlerResolver
		{
			public AutofacEntityResolver(Resolver resolver)
			{
				this.resolver = resolver;
			}

			private readonly Resolver resolver;

			public EntityHandler Resolve(Type handlerType)
			{
				return resolver.Resolve(handlerType) as EntityHandler;
			}
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

		public virtual void Run(Action runCode = null)
		{
			RaiseInitializedEvent();

			var window = Resolve<Window>();
			do
				TryRunAllRunnersAndPresenters(runCode); 
			while (!window.IsClosing);
		}

		protected void RaiseInitializedEvent()
		{
			try
			{
				if (Initialized != null)
					Initialized();

				Initialized = null;
			}
			catch (DependencyResolutionException exception)
			{
				LogException(exception.InnerException ?? exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				DisplayMessageBoxAndCloseApp(exception.InnerException ?? exception,
					"Fatal Initialization Error");
			}
			catch (Exception exception)
			{
				LogException(exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				DisplayMessageBoxAndCloseApp(exception, "Fatal Initialization Error");
			}
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
			catch (DependencyResolutionException exception)
			{
				LogException(exception.InnerException ?? exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				DisplayMessageBoxAndCloseApp(exception.InnerException ?? exception, "Fatal Runtime Error");
			}
			catch (Exception exception)
			{
				LogException(exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				if (exception.IsWeak())
					return; //ncrunch: no coverage

				DisplayMessageBoxAndCloseApp(exception, "Fatal Runtime Error");
			}
		}

		private void LogException(Exception exception)
		{
			var logger = Resolve<Logger>();
			logger.Error(exception);
		}

		private void DisplayMessageBoxAndCloseApp(Exception exception, string title)
		{
			var window = Resolve<Window>();
			if (
				window.ShowMessageBox(title, "Unable to continue: " + exception, MessageBoxButton.Ignore) !=
					MessageBoxButton.Ignore)
				window.Dispose();
		}

		public void Start(Action initCode, Action runCode = null)
		{
			CreateEntitySystemAndAddAsRunner();
			Initialized += initCode;
			Run(runCode);
		}

		public void Start<FirstClass>(Action<FirstClass> initCode, Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			CreateEntitySystemAndAddAsRunner();
			Initialized += () => initCode(Resolve<FirstClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass>(Action<FirstClass, SecondClass> initCode,
			Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			CreateEntitySystemAndAddAsRunner();
			Initialized += () => initCode(Resolve<FirstClass>(), Resolve<SecondClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass, ThirdClass>(
			Action<FirstClass, SecondClass, ThirdClass> initCode, Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			RegisterSingleton<ThirdClass>();
			CreateEntitySystemAndAddAsRunner();
			Initialized +=
				() => initCode(Resolve<FirstClass>(), Resolve<SecondClass>(), Resolve<ThirdClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass, ThirdClass, FourthClass>(
			Action<FirstClass, SecondClass, ThirdClass, FourthClass> initCode, Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			RegisterSingleton<ThirdClass>();
			RegisterSingleton<FourthClass>();
			CreateEntitySystemAndAddAsRunner();
			Initialized +=
				() =>
					initCode(Resolve<FirstClass>(), Resolve<SecondClass>(), Resolve<ThirdClass>(),
						Resolve<FourthClass>());
			Run(runCode);
		}

		public void Start<FirstClass, SecondClass, ThirdClass, FourthClass, FifthClass>(
			Action<FirstClass, SecondClass, ThirdClass, FourthClass, FifthClass> initCode,
			Action runCode = null)
		{
			RegisterSingleton<FirstClass>();
			RegisterSingleton<SecondClass>();
			RegisterSingleton<ThirdClass>();
			RegisterSingleton<FourthClass>();
			RegisterSingleton<FifthClass>();
			CreateEntitySystemAndAddAsRunner();
			Initialized +=
				() =>
					initCode(Resolve<FirstClass>(), Resolve<SecondClass>(), Resolve<ThirdClass>(),
						Resolve<FourthClass>(), Resolve<FifthClass>());
			Run(runCode);
		}

		public void Start<AppEntryRunner, FirstClassToRegisterAndResolve>(int instancesToCreate = 1)
		{
			RegisterEntryRunner<AppEntryRunner>(instancesToCreate);
			RegisterSingleton<FirstClassToRegisterAndResolve>();
			CreateEntitySystemAndAddAsRunner();
			Resolve<FirstClassToRegisterAndResolve>();
			Initialize<AppEntryRunner>(instancesToCreate);
			Run();
		}

		public void Start
			<AppEntryRunner, FirstClassToRegisterAndResolve, SecondClassToRegisterAndResolve>(
			int instancesToCreate = 1)
		{
			RegisterEntryRunner<AppEntryRunner>(instancesToCreate);
			RegisterSingleton<FirstClassToRegisterAndResolve>();
			RegisterSingleton<SecondClassToRegisterAndResolve>();
			CreateEntitySystemAndAddAsRunner();
			Resolve<FirstClassToRegisterAndResolve>();
			Resolve<SecondClassToRegisterAndResolve>();
			Initialize<AppEntryRunner>(instancesToCreate);
			Run();
		}

		public void Start<AppEntryRunner>(List<Type> typesToRegisterAndResolve,
			int instancesToCreate = 1)
		{
			RegisterAllTypesToRegister<AppEntryRunner>(typesToRegisterAndResolve, instancesToCreate);
			CreateEntitySystemAndAddAsRunner();
			ResolveAllTypesToResolve(typesToRegisterAndResolve);
			Initialize<AppEntryRunner>(instancesToCreate);
			Run();
		}

		private void RegisterAllTypesToRegister<AppEntryRunner>(
			IEnumerable<Type> typesToRegisterAndResolve, int instancesToCreate)
		{
			RegisterEntryRunner<AppEntryRunner>(instancesToCreate);
			CreateEntitySystemAndAddAsRunner();
			foreach (Type type in typesToRegisterAndResolve)
				RegisterSingleton(type);
		}

		private void ResolveAllTypesToResolve(IEnumerable<Type> typesToRegisterAndResolve)
		{
			foreach (Type type in typesToRegisterAndResolve)
				Resolve(type);
		}
	}
}