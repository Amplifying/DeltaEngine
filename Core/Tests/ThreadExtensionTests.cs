using System.Threading;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class ThreadExtensionTests
	{
		[Test]
		public void Start1()
		{
			int num = 1;
			Assert.AreEqual(1, num);
			Thread t = ThreadExtensions.Start(() => IncrementNumber(ref num));
			t.Join();
			Assert.AreEqual(2, num);
		}

		private static void IncrementNumber(ref int someNumber)
		{
			Thread.Sleep(5);
			someNumber++;
		}

		[Test]
		public void Start2()
		{
			Thread t = ThreadExtensions.Start("thread name", () => {});
			Assert.AreEqual("thread name", t.Name);
		}

		[Test]
		public void Start3()
		{
			const string Param = "name from parameter";
			Thread t = ThreadExtensions.Start(UpdateNameViaParameter, Param);
			t.Join();
			Assert.AreEqual(Param, t.Name);
		}

		private void UpdateNameViaParameter(object name)
		{
			Thread.CurrentThread.Name = (string)name;
		}
	}
}