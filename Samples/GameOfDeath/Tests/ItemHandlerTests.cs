using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;

namespace GameOfDeath.Tests
{
	internal class ItemHandlerTests : TestWithAllFrameworks
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
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) => score.Money = 5000);
		}

		[VisualTest]
		public void SimulateClicksInField(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (mockResolver != null)
					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			}, () =>
			{
				if (mockResolver != null)
					mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateClicksOnIcons(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (mockResolver == null)
					return;

				mockResolver.input.SetMousePosition(new Point(0.5f, 0.2f));
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				score.Money = 0;
				mockResolver.input.SetMousePosition(Point.Half);
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateMalletClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Scoreboard score) =>
			{
				if (mockResolver == null)
					return;

				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				mockResolver.input.SetMousePosition(Point.One);
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
			});
		}

		[VisualTest]
		public void SimulateFireClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(1, 100);
				if (mockResolver == null)
					return;

				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.25f);
			});
		}

		[VisualTest]
		public void SimulateToxicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(2, 100);
				if (mockResolver == null)
					return;

				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
			});
		}

		[VisualTest]
		public void SimulateAtomicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler) =>
			{
				handler.SelectItemIfSufficientFunds(3, 100);
				if (mockResolver == null)
					return;

				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
			});
		}
	}
}