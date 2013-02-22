using System;
using System.Threading;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	/// <summary>
	/// AssemblyUpdater is only used for visual tests and normal applications. NCrunch testing won't
	/// work with AssemblyUpdating (always disabled if App is not visual). ReSharper won't show the
	/// console output from another app domain, so use Program.cs and launch this normally.
	/// </summary>
	public class AssemblyUpdaterTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void LoadAndRunSystemRunners()
		{
			var updater = new AssemblyUpdater(MakeSureWeCanUseTypeInDomain, delegate(Type type)
			{
				try
				{
					if (type == typeof(Time))
						return Activator.CreateInstance(type, Activator.CreateInstance<StopwatchTime>());
					if (type == typeof(RunnerTests.Device))
						return Activator.CreateInstance(type, Activator.CreateInstance<RunnerTests.Window>());
					return Activator.CreateInstance(type);
				}
				catch (Exception ex)
				{
					throw new TypeInitializationException(type.FullName, ex);
				}
			});
			// This runs for 10 seconds, go to ConsoleRunner and change it while this is running!
			for (float time = 0; time < 10.0f; time += 0.1f)
			{
				updater.Run();
				Thread.Sleep(100);
			}
		}

		private static void MakeSureWeCanUseTypeInDomain()
		{
			Assert.IsNotNull(typeof(StopwatchTime));
		}
	}
}