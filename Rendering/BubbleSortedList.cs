using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Facilitates the sorting of the objects using a rendering layer logic
	/// </summary>
	public class BubbleSortedList : IList<Renderable>
	{
		public BubbleSortedList(IEnumerable<Renderable> bubbleList, bool isReadOnly = false)
		{
			IsReadOnly = isReadOnly;
			foreach (Renderable t in bubbleList)
				data.Add(t);
		}

		public bool IsReadOnly { get; private set; }

		private readonly List<Renderable> data = new List<Renderable>();

		public BubbleSortedList(bool isReadOnly = false)
		{
			IsReadOnly = isReadOnly;
		}

		public List<Renderable> GetData()
		{
			return data;
		}

		public void Add(Renderable item)
		{
			if (data.IndexOf(item) >= 0)
				return;

			if (data.Count == 0)
				data.Add(item);
			else
				AddAndResortTheList(item);
		}

		private void AddAndResortTheList(Renderable item)
		{
			item.HasRenderLayerChanged = false;
			var changeNearestIndex = IsNearestIndexToZeroChanging(item);
			int index = item.RenderLayer < 0
				? ItemIsInBackground(item, changeNearestIndex) : ItemIsInForeground(item);
			if (changeNearestIndex)
				nearestIndexToLayerZero = index;
		}

		private int nearestIndexToLayerZero;

		private bool IsNearestIndexToZeroChanging(Renderable item)
		{
			bool changeNearestIndex = true;
			if (data.Count > 0)
				changeNearestIndex = MathExtensions.Abs(item.RenderLayer) <
					MathExtensions.Abs(data[nearestIndexToLayerZero].RenderLayer);

			return changeNearestIndex;
		}

		private int ItemIsInBackground(Renderable item, bool changeNearestIndex)
		{
			int index = CalculateIndexForNewItem(item.RenderLayer, nearestIndexToLayerZero + 1);
			data.Add(item);
			if (!changeNearestIndex)
				nearestIndexToLayerZero++;

			InsertItemInRightIndex(item, index);
			return index;
		}

		private int ItemIsInForeground(Renderable item)
		{
			int index = CalculateIndexForNewItem(item.RenderLayer, data.Count);
			data.Add(item);
			InsertItemInRightIndex(item, index);
			return index;
		}

		private int CalculateIndexForNewItem(int layer, int topIndex)
		{
			int index = topIndex;
			for (int i = 0; i < topIndex; i++)
			{
				if (data[i].RenderLayer <= layer)
					continue;

				index = i;
				i = data.Count;
			}
			return index;
		}

		private void InsertItemInRightIndex(Renderable item, int index)
		{
			for (int i = data.Count - 1; i > index; i--)
				data[i] = data[i - 1];

			data[index] = item;
		}

		public void Resort()
		{
			var values = data.ToList();
			Clear();
			foreach (Renderable renderable in values)
				Add(renderable);
		}

		public void Clear()
		{
			data.Clear();
			nearestIndexToLayerZero = 0;
		}

		public bool Contains(Renderable item)
		{
			return data.Contains(item);
		}

		public void CopyTo(Renderable[] array, int arrayIndex)
		{
			data.CopyTo(array, arrayIndex);
		}

		public bool Remove(Renderable item)
		{
			return data.Remove(item);
		}

		public int Count
		{
			get { return data.Count; }
		}

		public IEnumerator<Renderable> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public int IndexOf(Renderable item)
		{
			return data.IndexOf(item);
		}

		public void Insert(int index, Renderable item)
		{
			if (data.IndexOf(item) >= 0)
				return;

			data.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			data.RemoveAt(index);
		}

		public Renderable this[int index]
		{
			get { return data[index]; }
			set { Add(value); }
		}
	}
}