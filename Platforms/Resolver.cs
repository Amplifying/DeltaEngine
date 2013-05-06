using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace DeltaEngine.Platforms
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
			{
				if (runner is PriorityRunner)
					priorityRunners.Add(runner);
				else
					runners.Add(runner);
			}
			else
				AddGenericRunnerIfPossible(instance);

			var presenter = instance as Presenter;
			if (presenter != null)
				presenters.Add(presenter);

			var entity = instance as Entity;
			if (entity != null)
				Resolve<EntitySystem>().Add(entity);
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

		private readonly List<Runner> priorityRunners = new List<Runner>();
		private readonly List<Runner> runners = new List<Runner>();
		private readonly List<RunnerArguments> genericRunners = new List<RunnerArguments>();
		private readonly List<Presenter> presenters = new List<Presenter>();

		private struct RunnerArguments
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
			foreach (Runner runner in priorityRunners)
				runner.Run();

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

		internal abstract object Resolve(Type baseType, object customParameter = null);
		internal abstract BaseType Resolve<BaseType>();

		public virtual void Dispose()
		{
			if (isAlreadyDisposed)
				return;

			foreach (var disposableRunner in priorityRunners.OfType<IDisposable>())
				disposableRunner.Dispose();
			foreach (var disposableRunner in runners.OfType<IDisposable>())
				disposableRunner.Dispose();

			isAlreadyDisposed = true;
		}

		private bool isAlreadyDisposed;
	}
}