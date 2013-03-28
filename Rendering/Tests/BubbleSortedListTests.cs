using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class BubbleSortedListTests
	{
		[Test]
		public void AddElements()
		{
			var bubbleList = CreateBubbleList();
			Assert.AreEqual(ellipse3, bubbleList[0]);
			Assert.AreEqual(ellipse4, bubbleList[1]);
			Assert.AreEqual(ellipse6, bubbleList[2]);
			Assert.AreEqual(ellipse5, bubbleList[3]);
		}

		private BubbleSortedList CreateBubbleList()
		{
			var bubbleList = new BubbleSortedList();
			bubbleList.Add(ellipse5);
			bubbleList.Add(ellipse3);
			bubbleList.Add(ellipse4);
			bubbleList.Add(ellipse6);
			bubbleList.Add(ellipse5);
			Assert.AreEqual(4, bubbleList.Count); 
			return bubbleList;
		}

		private readonly Ellipse ellipse3 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = -2 };
		private readonly Ellipse ellipse4 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = -1 };
		private readonly Ellipse ellipse5 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = 3 };
		private readonly Ellipse ellipse6 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = 0 };

		[Test]
		public void CopyConstructor()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2, ellipse3, ellipse4 };
			var otherList = new BubbleSortedList(bubbleList);
			Assert.AreEqual(ellipse1, otherList[0]);
			Assert.AreEqual(ellipse2, otherList[1]);
			Assert.AreEqual(ellipse3, otherList[2]);
			Assert.AreEqual(ellipse4, otherList[3]);
		}

		private readonly Ellipse ellipse1 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = -4 };
		private readonly Ellipse ellipse2 = new Ellipse(Point.Zero, 0.5f, 0.7f) { RenderLayer = -3 };

		[Test]
		public void GetData()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2, ellipse3, ellipse4 };
			Assert.AreEqual(ellipse1, bubbleList.GetData()[0]);
			Assert.AreEqual(ellipse2, bubbleList.GetData()[1]);
			Assert.AreEqual(ellipse3, bubbleList.GetData()[2]);
			Assert.AreEqual(ellipse4, bubbleList.GetData()[3]);
		}

		[Test]
		public void Resort()
		{
			var bubbleList = CreateBubbleList();
			ellipse5.RenderLayer = -5;
			bubbleList.Resort();
			Assert.AreEqual(ellipse5, bubbleList[0]);
			Assert.AreEqual(ellipse3, bubbleList[1]);
			Assert.AreEqual(ellipse4, bubbleList[2]);
			Assert.AreEqual(ellipse6, bubbleList[3]);
		}

		[Test]
		public void Clear()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2, ellipse3, ellipse4 };
			Assert.AreEqual(4, bubbleList.Count);
			bubbleList.Clear();
			Assert.AreEqual(0, bubbleList.Count);
		}

		[Test]
		public void CopyToArray()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2, ellipse3, ellipse4 };
			var listRenderable = new Renderable[4];
			bubbleList.CopyTo(listRenderable, 0);
			Assert.AreEqual(ellipse1, listRenderable[0]);
			Assert.AreEqual(ellipse2, listRenderable[1]);
			Assert.AreEqual(ellipse3, listRenderable[2]);
			Assert.AreEqual(ellipse4, listRenderable[3]);
		}

		[Test]
		public void Remove()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2, ellipse3, ellipse4 };
			Assert.AreEqual(4, bubbleList.Count);
			bubbleList.Remove(ellipse1);
			bubbleList.Remove(ellipse1);
			bubbleList.Remove(ellipse2);
			bubbleList.RemoveAt(0);
			Assert.AreEqual(1, bubbleList.Count);
			Assert.AreEqual(0, bubbleList.IndexOf(ellipse4));
		}

		[Test]
		public void IndexOfElement()
		{
			var bubbleList = new BubbleSortedList { ellipse1, ellipse2 };
			Assert.AreEqual(0, bubbleList.IndexOf(ellipse1));
			Assert.AreEqual(1, bubbleList.IndexOf(ellipse2));
		}

		[Test]
		public void InsertItem()
		{
			var bubbleList = new BubbleSortedList();
			bubbleList.Insert(0, ellipse1);
			bubbleList.Insert(1, ellipse2);
			bubbleList.Insert(0, ellipse2);
			Assert.AreEqual(0, bubbleList.IndexOf(ellipse1));
			Assert.AreEqual(1, bubbleList.IndexOf(ellipse2));
		}

		[Test]
		public void Indexer()
		{
			var bubbleList = new BubbleSortedList();
			bubbleList[0] = ellipse1;
		}
	}
}