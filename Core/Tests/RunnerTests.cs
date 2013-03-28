using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	/// <summary>
	/// Tests the Runner and Presenter classes manually without DeltaEngine.Configuration.
	/// </summary>
	public class RunnerTests : Runner
	{
		[Test]
		public void Run()
		{
			Output.Clear();
			var window = new Window();
			var device = new Device(window);
			device.Run();
			Output.Add("RunnerTests.Run");
			device.Present();

			const string ExpectedOutput = "Window.Run, Device.Run, RunnerTests.Run, Device.Present";
			Assert.AreEqual(ExpectedOutput, Output.ToText());
		}

		private static readonly List<string> Output = new List<string>();

		internal class Device : Presenter
		{
			public Device(Window window)
			{
				this.window = window;
			}

			private readonly Window window;

			public void Run()
			{
				window.Run();
				Output.Add("Device.Run");
			}

			public void Present()
			{
				Output.Add("Device.Present");
			}
		}

		internal class Window : Runner
		{
			public void Run()
			{
				Output.Add("Window.Run");
			}
		}
	}
}