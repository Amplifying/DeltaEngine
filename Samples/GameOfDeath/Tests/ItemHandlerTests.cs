using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class ItemHandlerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowMallet()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(0, 100);
		}

		[Test]
		public void ShowFire()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(1, 100);
		}

		[Test]
		public void ShowToxic()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(2, 100);
		}

		[Test]
		public void ShowAtomic()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(3, 100);
		}

		[Test]
		public void AllowToCastAnyItemInGame()
		{
			Resolve<Scoreboard>().Money = 5000;
		}

		[Test]
		public void SimulateClicksInField()
		{
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			RunCode = () => { Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing); };
		}

		[Test]
		public void SimulateClicksOnIcons()
		{
			Resolve<MockMouse>().SetPosition(new Point(0.5f, 0.2f));
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Resolve<Scoreboard>().Money = 0;
			Resolve<MockMouse>().SetPosition(Point.Half);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
		}

		[Test]
		public void SimulateMalletClick()
		{
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Resolve<MockMouse>().SetPosition(Point.One);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
		}

		[Test]
		public void SimulateFireClick()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(1, 100);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.25f);
		}

		[Test]
		public void SimulateToxicClick()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(2, 100);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
		}

		[Test]
		public void SimulateAtomicClick()
		{
			Resolve<ItemHandler>().SelectItemIfSufficientFunds(3, 100);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
		}
	}
}