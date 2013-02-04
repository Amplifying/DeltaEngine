using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Windows;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseButtonStateHelperTests
	{
		[Test]
		public void TestIsPressed()
		{
			Assert.True(MouseButtonStateHelper.IsPressed(0x0201));
			Assert.False(MouseButtonStateHelper.IsPressed(0));
		}

		[Test]
		public void TestIsReleased()
		{
			Assert.True(MouseButtonStateHelper.IsReleased(0x00A2));
			Assert.False(MouseButtonStateHelper.IsReleased(0));
		}

		[Test]
		public void TestGetMessageButton()
		{
			Assert.AreEqual(MouseButton.Left, MouseButtonStateHelper.GetMessageButton(0x00A2, 0));
			Assert.AreEqual(MouseButton.Right, MouseButtonStateHelper.GetMessageButton(0x0205, 0));
			Assert.AreEqual(MouseButton.Middle, MouseButtonStateHelper.GetMessageButton(0x0209, 0));
			Assert.AreEqual(MouseButton.X1, MouseButtonStateHelper.GetMessageButton(0x020B, 65536));
			Assert.AreEqual(MouseButton.X2, MouseButtonStateHelper.GetMessageButton(0x020B, 0));
		}
	}
}