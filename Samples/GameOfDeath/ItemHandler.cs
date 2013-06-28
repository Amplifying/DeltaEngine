using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Each item costs a different amount of money. Also displays icons shown on top of the screen.
	/// </summary>
	public class ItemHandler
	{
		public ItemHandler(ScreenSpace screen)
		{
			this.screen = screen;
			CreateItems();
			CreateIcons();
			SelectItem(0);
		}

		private readonly ScreenSpace screen;

		private void CreateItems()
		{
			CreateMallet();
			CreateFire();
			CreateToxic();
			CreateAtomic();
		}

		private void CreateMallet()
		{
			var mallet = new Mallet();
			cachedItems.Add(mallet);
		}

		private void CreateFire()
		{
			var fire = new Fire();
			cachedItems.Add(fire);
		}

		private void CreateToxic()
		{
			var toxic = new Toxic();
			cachedItems.Add(toxic);
		}

		private void CreateAtomic()
		{
			var atomic = new Atomic();
			cachedItems.Add(atomic);
		}

		private readonly List<Item> cachedItems = new List<Item>();

		private void CreateIcons()
		{
			for (int index = 0; index < NumberOfItems; index++)
				icons[index] = new Icon(index, screen);
		}

		private const int NumberOfItems = 4;
		private readonly Sprite[] icons = new Sprite[NumberOfItems];

		private void SelectItem(int itemIndex)
		{
			CurrentItem.Visibility = Visibility.Hide;
			icons[currentIndex].Color = Color.Gray;
			currentIndex = itemIndex;
			CurrentItem.Visibility = Visibility.Show;
			icons[currentIndex].Color = Color.White;
		}

		private int currentIndex;

		public Item CurrentItem
		{
			get { return cachedItems[currentIndex]; }
		}

		public void SelectIconOrHandleItemInGame(Point position, int money)
		{
			if (IsMouseOverIcon(position, money))
				return;

			HandleInGameItem(position, money);
		}

		private bool IsMouseOverIcon(Point position, int money)
		{
			for (int index = 0; index < NumberOfItems; index++)
				if (icons[index].DrawArea.Contains(position))
				{
					SelectItemIfSufficientFunds(index, money);
					CurrentItem.UpdatePosition(position);
					return true;
				}

			return false;
		}

		internal void SelectItemIfSufficientFunds(int itemIndex, int money)
		{
			if (money >= cachedItems[itemIndex].Cost)
				SelectItem(itemIndex);
		}

		private void HandleInGameItem(Point position, int money)
		{
			if (money >= CurrentItem.Cost)
				CreateEffect(position);
			else
				SelectItem(0);
		}

		private void CreateEffect(Point position)
		{
			var effect = CurrentItem.CreateEffect(position);
			if (effect == null)
				return;

			if (DidDamage != null)
				CurrentItem.DoDamage = (point, impactSize, damage) => DidDamage(point, impactSize, damage);

			if (MoneySpent != null)
				MoneySpent(CurrentItem.Cost);
		}

		public event Action<Point, float, float> DidDamage;
		public event Action<int> MoneySpent;
	}
}