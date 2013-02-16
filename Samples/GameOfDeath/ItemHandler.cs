using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Each item cost a different amount of money. Also displays icons shown on top of the screen.
	/// </summary>
	public class ItemHandler
	{
		public ItemHandler(Game game, Score score, Content content, InputCommands inputCommands,
			Renderer renderer)
		{
			this.game = game;
			this.score = score;
			this.renderer = renderer;
			CreateItems(content);
			CreateIcons(content);
			SelectItem(0);
			inputCommands.AddMouseMovement(mouse => CurrentItem.UpdatePosition(mouse.Position));
			inputCommands.Add(MouseButton.Left, State.Pressing,
				mouse => SelectIconOrHandleItemInGame(mouse.Position));
			inputCommands.Add(touch => SelectIconOrHandleItemInGame(touch.GetPosition(0)));
		}

		private readonly Game game;
		private readonly Score score;
		private readonly Renderer renderer;

		private void CreateItems(Content content)
		{
			cachedItems.Add(new Mallet(content, Point.Half));
			cachedItems.Add(new Fire(content, Point.Half));
			cachedItems.Add(new Toxic(content, Point.Half));
			cachedItems.Add(new Atomic(content, Point.Half));
		}

		private readonly List<Item> cachedItems = new List<Item>();

		private void CreateIcons(Content content)
		{
			for (int index = 0; index < NumberOfItems; index++)
				renderer.Add(icons[index] = new Icon(content, index, renderer.Screen));
		}

		private readonly Sprite[] icons = new Sprite[NumberOfItems];
		private const int NumberOfItems = 4;

		private void SelectIconOrHandleItemInGame(Point position)
		{
			if (game.IsGameOver() || IsMouseOverIcon(position))
				return;

			HandleInGameItem(position);
		}

		private bool IsMouseOverIcon(Point position)
		{
			for (int index = 0; index < NumberOfItems; index++)
				if (icons[index].DrawArea.Contains(position))
				{
					SelectItem(index);
					CurrentItem.UpdatePosition(position);
					return true;
				}

			return false;
		}

		private void HandleInGameItem(Point position)
		{
			if (score.CurrentMoney >= CurrentItem.Cost)
			{
				var effect = CurrentItem.CreateEffect(position, game);
				if (effect != null)
				{
					score.CurrentMoney -= CurrentItem.Cost;
					renderer.Add(effect);
				}
			}
			else
				SelectItem(0);
		}

		public void SelectItem(int itemIndex)
		{
			if (itemIndex > 0 && score.CurrentMoney < cachedItems[itemIndex].Cost)
				return;

			renderer.Remove(CurrentItem);
			icons[currentIndex].Color = Color.Gray;
			currentIndex = itemIndex;
			renderer.Add(CurrentItem);
			icons[currentIndex].Color = Color.White;
		}

		private int currentIndex;
		public Item CurrentItem
		{
			get { return cachedItems[currentIndex]; }
		}
	}
}