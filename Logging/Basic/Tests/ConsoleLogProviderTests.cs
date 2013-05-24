using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class ConsoleLogProviderTests
	{
		[SetUp]
		public void InitializeConsoleAndProvider()
		{
			console = new StringWriter();
			Console.SetOut(console);
			provider = new ConsoleLogProvider();
		}

		private TextWriter console;
		private ConsoleLogProvider provider;

		[Test]
		public void LogInfoMessage()
		{
			Assert.AreEqual("", console.ToString());
			provider.Log(new Info("Hello"));
			Assert.AreEqual("Hello" + console.NewLine, console.ToString());
		}

		[Test]
		public void LogWarning()
		{
			Assert.AreEqual("", console.ToString());
			provider.Log(new Warning("Ohoh"));
			Assert.AreEqual("Ohoh" + console.NewLine, console.ToString());

			provider.Log(new Warning(new NullReferenceException()));
			Assert.IsTrue(console.ToString().Contains("NullReferenceException"));
		}

		[Test]
		public void LogError()
		{
			Assert.AreEqual("", console.ToString());
			provider.Log(new Error(new ExternalException()));
			Assert.IsTrue(console.ToString().Contains("ExternalException"));
		}
	}
}