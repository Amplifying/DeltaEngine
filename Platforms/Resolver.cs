using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Base resolver methods like Resolve and functionally to run runners and presenters.
	/// </summary>
	public abstract class Resolver : IDisposable
	{
		static Resolver()
		{
			typeof(Color).AddConvertTypeCreation(value => new Color(value));
			typeof(Size).AddConvertTypeCreation(value => new Size(value));
			typeof(Point).AddConvertTypeCreation(value => new Point(value));
			typeof(Range).AddConvertTypeCreation(value => new Range(value));
			typeof(Vector).AddConvertTypeCreation(value => new Vector(value));
			typeof(Rectangle).AddConvertTypeCreation(value => new Rectangle(value));
		}

		protected void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var runner = instance as Runner;
			if (runner != null)
			{
				if (priorityRunners.Contains(runner) || runners.Contains(runner))
					throw new RunnerWasAlreadyAdded(runner);

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
		}

		private class RunnerWasAlreadyAdded : Exception
		{
			public RunnerWasAlreadyAdded(Runner runner)
				: base(runner.ToString()) {}
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
			RunPriorityRunners();
			RunRunners();
			RunGenericRunners();
		}

		private void RunPriorityRunners()
		{
			var copyOfPriorityRunners = new List<Runner>(priorityRunners);
			foreach (Runner runner in copyOfPriorityRunners)
				runner.Run();
		}

		private void RunRunners()
		{
			var copyOfRunners = new List<Runner>(runners);
			foreach (Runner runner in copyOfRunners)
				runner.Run();
		}

		private void RunGenericRunners()
		{
			var copyOfGenericRunners = new List<RunnerArguments>(genericRunners);
			foreach (RunnerArguments runner in copyOfGenericRunners)
				runner.Invoke();
		}

		public void RunAllPresenters()
		{
			var copyOfPresenters = new List<Presenter>(presenters);
			foreach (Presenter presenter in copyOfPresenters)
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