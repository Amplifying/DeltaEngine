using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Base resolver methods like Resolve and functionally to run runners and presenters.
	/// </summary>
	public abstract class Resolver : IDisposable
	{
		protected virtual void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var runner = instance as Runner;
			if (runner != null)
				runners.Add(runner);
			else
				AddGenericRunnerIfPossible(instance);

			var presenter = instance as Presenter;
			if (presenter != null)
				presenters.Add(presenter);
		}

		protected void AddGenericRunnerIfPossible(object instance)
		{
			foreach (var interfaceType in instance.GetType().GetInterfaces())
				if (interfaceType.Name.StartsWith("Runner`"))
					AddGenericRunnerFromInterface(instance, interfaceType);
		}

		private void AddGenericRunnerFromInterface(object instance, Type interfaceType)
		{
			var arguments = interfaceType.GetGenericArguments();
			if (arguments.Length > 0)
				genericRunners.Add(new RunnerArguments(instance, arguments, this));
		}

		protected readonly List<Runner> runners = new List<Runner>();
		protected readonly List<RunnerArguments> genericRunners = new List<RunnerArguments>();
		protected readonly List<Presenter> presenters = new List<Presenter>();

		protected class RunnerArguments
		{
			public RunnerArguments(object instance, Type[] arguments, Resolver resolver)
			{
				this.instance = instance;
				resolvedInstances = new object[arguments.Length];
				for (int num = 0; num < arguments.Length; num++)
					resolvedInstances[num] = resolver.Resolve(arguments[num]);
				genericMethod = instance.GetType().GetMethod("Run");
			}

			private readonly object instance;
			private readonly object[] resolvedInstances;
			private readonly MethodInfo genericMethod;

			public void Invoke()
			{
				genericMethod.Invoke(instance, resolvedInstances);
			}
		}

		public void RunAllRunners()
		{
			foreach (Runner runner in runners)
				runner.Run();

			foreach (RunnerArguments runner in genericRunners)
				runner.Invoke();
		}

		public void RunAllPresenters()
		{
			foreach (Presenter presenter in presenters)
				presenter.Present();
		}

		protected abstract object Resolve(Type baseType);
		public abstract BaseType Resolve<BaseType>(object customParameter = null);

		public virtual void Dispose()
		{
			if (isAlreadyDisposed)
				return;

			foreach (var disposableRunner in runners.OfType<IDisposable>())
				disposableRunner.Dispose();

			isAlreadyDisposed = true;
		}

		private bool isAlreadyDisposed;
	}
}