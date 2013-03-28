using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;

namespace GameOfDeath.Tests
{
	internal class ItemHandlerTests : TestStarter
	{
		[VisualTest]
		public void ShowMallet(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItemIfSufficientFunds(0, 100));
		}

		[VisualTest]
		public void ShowFire(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItemIfSufficientFunds(1, 100));
		}

		[VisualTest]
		public void ShowToxic(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItemIfSufficientFunds(2, 100));
		}

		[VisualTest]
		public void ShowAtomic(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItemIfSufficientFunds(3, 100));
		}

		[VisualTest]
		public void AllowToCastAnyItemInGame(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) => score.CurrentMoney = 5000);
		}

		[VisualTest]
		public void SimulateClicksInField(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (testResolver != null)
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
			}, () =>
			{
				if (testResolver != null)
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateClicksOnIcons(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (testResolver == null)
					return;

				testResolver.SetMousePosition(new Point(0.5f, 0.2f));
				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				score.CurrentMoney = 0;
				testResolver.SetMousePosition(Point.Half);
				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateMalletClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (testResolver == null)
					return;

				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				testResolver.SetMousePosition(Point.One);
				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateFireClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(1, 100);
				if (testResolver == null)
					return;

				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.25f);
			});
		}

		[VisualTest]
		public void SimulateToxicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(2, 100);
				if (testResolver == null)
					return;

				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.1f);
			});
		}

		[VisualTest]
		public void SimulateAtomicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(3, 100);
				if (testResolver == null)
					return;

				testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing);
				testResolver.AdvanceTimeAndExecuteRunners(0.5f);
			});
		}
	}
}