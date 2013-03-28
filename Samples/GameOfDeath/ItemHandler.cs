using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Each item costs a different amount of money. Also displays icons shown on top of the screen.
	/// </summary>
	public class ItemHandler
	{
		public ItemHandler(Content content, Renderer renderer)
		{
			this.renderer = renderer;
			CreateItems(content);
			CreateIcons(content);
			SelectItem(0);
		}

		private readonly Renderer renderer;

		private void CreateItems(Content content)
		{
			cachedItems.Add(new Mallet(content));
			cachedItems.Add(new Fire(content));
			cachedItems.Add(new Toxic(content));
			cachedItems.Add(new Atomic(content));
		}

		private readonly List<Item> cachedItems = new List<Item>();

		private void CreateIcons(Content content)
		{
			for (int index = 0; index < NumberOfItems; index++)
				renderer.Add(icons[index] = new Icon(content, index, renderer.Screen));
		}

		private const int NumberOfItems = 4;
		private readonly Sprite[] icons = new Sprite[NumberOfItems];

		private void SelectItem(int itemIndex)
		{
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

			renderer.Add(effect);

			if (DidDamage != null)
				CurrentItem.DidDamage = (point, impactSize, damage) => DidDamage(point, impactSize, damage);

			if (MoneySpent != null)
				MoneySpent(CurrentItem.Cost);
		}

		public event Action<Point, float, float> DidDamage;
		public event Action<int> MoneySpent;
	}
}