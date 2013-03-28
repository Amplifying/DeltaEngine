using DeltaEngine.Input;

namespace GameOfDeath
{
	/// <summary>
	/// Knits the lower level objects together by feeding their events to each other
	/// </summary>
	public class GameCoordinator
	{
		public GameCoordinator(RabbitGrid rabbits, Scoreboard scoreboard, ItemHandler items)
		{
			this.rabbits = rabbits;
			this.items = items;
			this.scoreboard = scoreboard;
			rabbits.MoneyEarned += money => scoreboard.CurrentMoney += money;
			rabbits.RabbitKilled += () => scoreboard.CurrentKills++;
			items.DidDamage += rabbits.DoDamage;
			items.MoneySpent += money => scoreboard.CurrentMoney -= money;
		}

		private readonly RabbitGrid rabbits;
		private readonly ItemHandler items;
		private readonly Scoreboard scoreboard;

		internal void RespondToInput(InputCommands input)
		{
			input.AddMouseMovement(mouse => items.CurrentItem.UpdatePosition(mouse.Position));
			input.Add(MouseButton.Left, State.Pressing, MouseButtonPress);
			input.Add(TouchPress);
		}

		private void MouseButtonPress(Mouse mouse)
		{
			if (!rabbits.IsGameOver())
				items.SelectIconOrHandleItemInGame(mouse.Position, scoreboard.CurrentMoney);
		}

		private void TouchPress(Touch touch)
		{
			if (!rabbits.IsGameOver())
				items.SelectIconOrHandleItemInGame(touch.GetPosition(0), scoreboard.CurrentMoney);
		}
	}
}