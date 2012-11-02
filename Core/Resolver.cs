using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Base resolver method definitions and functionally to run runners and presenters.
	/// </summary>
	public abstract class Resolver : IDisposable
	{
		protected void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var runner = instance as Runner;
			if (runner != null)
				runners.Add(runner);

			var presenter = instance as Presenter;
			if (presenter != null)
				presenters.Add(presenter);
		}

		protected readonly List<Runner> runners = new List<Runner>();
		protected readonly List<Presenter> presenters = new List<Presenter>();

		public void RunAllRunners()
		{
			foreach (Runner runner in runners)
				runner.Run();
		}

		public void RunAllPresenters()
		{
			foreach (Presenter presenter in presenters)
				presenter.Present();
		}

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