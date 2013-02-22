using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;

namespace GameOfDeath.Tests
{
	class ItemHandlerTests : TestStarter
	{
		[VisualTest]
		public void ShowMallet(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItem(0));
		}

		[VisualTest]
		public void ShowFire(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItem(1));
		}

		[VisualTest]
		public void ShowToxic(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItem(2));
		}

		[VisualTest]
		public void ShowAtomic(Type resolver)
		{
			Start(resolver, (ItemHandler handler) => handler.SelectItem(3));
		}

		[VisualTest]
		public void AllowToCastAnyItemInGame(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) => score.CurrentMoney = 5000);
		}

		[VisualTest]
		public void SimulateClicksInField(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				if (testResolver != null)
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
			}, () =>
			{
				if (testResolver != null)
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
			});
		}

		[VisualTest]
		public void SimulateClicksOnIcons(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, new Point(0.5f, 0.2f));
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					score.CurrentMoney = 0;
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
				}
			});
		}

		[VisualTest]
		public void SimulateMalletClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.One);
				}
			});
		}

		[VisualTest]
		public void SimulateFireClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				handler.SelectItem(1);
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(0.25f);
				}
			});
		}

		[VisualTest]
		public void SimulateToxicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				score.CurrentMoney = 50;
				handler.SelectItem(2);
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				}
			});
		}

		[VisualTest]
		public void SimulateAtomicClick(Type resolver)
		{
			Start(resolver, (UI bg, ItemHandler handler, Score score) =>
			{
				score.CurrentMoney = 50;
				handler.SelectItem(3);
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Pressing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(0.5f);
				}
			});
		}
	}
}