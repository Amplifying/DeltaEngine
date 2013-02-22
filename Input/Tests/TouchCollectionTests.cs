using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Windows;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchCollectionTests
	{
		public TouchCollectionTests()
		{
			resolver = new TestResolver();
		}

		private readonly TestResolver resolver;

		[Test]
		public void FindIndexByIdOrGetFreeIndex()
		{
			var touchCollection = new TouchCollection(null);
			Assert.AreEqual(0, touchCollection.FindIndexByIdOrGetFreeIndex(478));
		}

		[Test]
		public void FindIndexByIdWithExistingId()
		{
			var touchCollection = new TouchCollection(null);
			touchCollection.ids[5] = 5893;
			Assert.AreEqual(5, touchCollection.FindIndexByIdOrGetFreeIndex(5893));
		}

		[Test]
		public void FindFreeIndex()
		{
			var touchCollection = new TouchCollection(null);
			Assert.AreEqual(0, touchCollection.FindIndexByIdOrGetFreeIndex(456));
		}

		[Test]
		public void FindFreeIndexWithoutAnyFreeIndices()
		{
			var touchCollection = new TouchCollection(null);
			for (int index = 0; index < touchCollection.ids.Length; index++)
				touchCollection.ids[index] = 1;
			Assert.AreEqual(-1, touchCollection.FindIndexByIdOrGetFreeIndex(546));
		}

		[Test]
		public void IsTouchDown()
		{
			Assert.True(TouchCollection.IsTouchDown(0x0001));
			Assert.True(TouchCollection.IsTouchDown(0x0001 | 0x0002));
			Assert.True(TouchCollection.IsTouchDown(0x0002));
			Assert.False(TouchCollection.IsTouchDown(0x0008));
		}

		[Test]
		public void UpdateTouchState()
		{
			var touchCollection = new TouchCollection(null);
			touchCollection.UpdateTouchState(0, 0x0001);
			Assert.AreEqual(State.Pressing, touchCollection.states[0]);
			touchCollection.UpdateTouchState(0, 0);
			Assert.AreEqual(State.Releasing, touchCollection.states[0]);
		}

		[Test]
		public void CalculateQuadraticPosition()
		{
			TouchCollection touchCollection = CreateCollection();
			Point quadPosition = touchCollection.CalculateQuadraticPosition(40000, 30000);
			Assert.AreEqual(new Point(0.5f, 0.5f), quadPosition);
		}

		[Test]
		public void ProcessNewTouches()
		{
			TouchCollection touchCollection = CreateCollection();
			var newTouches = new List<NativeTouchInput> { new NativeTouchInput { Flags = 0x0001, Id = 15, X = 40000, Y = 30000 } };
			touchCollection.ProcessNewTouches(newTouches);

			Assert.AreEqual(15, touchCollection.ids[0]);
			Assert.AreEqual(new Point(0.5f, 0.5f), touchCollection.locations[0]);
			Assert.AreEqual(State.Pressing, touchCollection.states[0]);
		}

		[Test]
		public void UpdateTouchStateWithoutNewData()
		{
			TouchCollection touchCollection = CreateCollection();
			touchCollection.ids[0] = 15;

			touchCollection.states[0] = State.Releasing;
			touchCollection.UpdateTouchStateWithoutNewData(0);
			Assert.AreEqual(State.Released, touchCollection.states[0]);
			Assert.AreEqual(15, touchCollection.ids[0]);

			touchCollection.states[0] = State.Released;
			touchCollection.UpdateTouchStateWithoutNewData(0);
			Assert.AreEqual(State.Released, touchCollection.states[0]);
			Assert.AreEqual(-1, touchCollection.ids[0]);
		}

		[Test]
		public void UpdateAllTouches()
		{
			TouchCollection touchCollection = CreateCollection();
			var newTouches = new List<NativeTouchInput>
			{ new NativeTouchInput { Flags = 0x0001, Id = 15, X = 40000, Y = 30000 } };

			touchCollection.ids[0] = 15;
			touchCollection.states[0] = State.Pressing;
			touchCollection.UpdateAllTouches(newTouches);

			Assert.AreEqual(0, newTouches.Count);
			Assert.AreEqual(15, touchCollection.ids[0]);
			Assert.AreEqual(new Point(0.5f, 0.5f), touchCollection.locations[0]);
			Assert.AreEqual(State.Pressed, touchCollection.states[0]);
		}

		[Test]
		public void UpdateTouchWithUpdatedActiveTouch()
		{
			TouchCollection touchCollection = CreateCollection();
			var newTouches = new List<NativeTouchInput> { new NativeTouchInput { Flags = 0x0001, Id = 15, X = 40000, Y = 30000 } };

			touchCollection.ids[0] = 15;
			touchCollection.states[0] = State.Pressing;
			touchCollection.UpdatePreviouslyActiveTouches(newTouches);

			Assert.AreEqual(0, newTouches.Count);
			Assert.AreEqual(15, touchCollection.ids[0]);
			Assert.AreEqual(new Point(0.5f, 0.5f), touchCollection.locations[0]);
			Assert.AreEqual(State.Pressed, touchCollection.states[0]);
		}

		[Test]
		public void UpdateTouchWithoutAnyActiveTouch()
		{
			TouchCollection touchCollection = CreateCollection();
			var newTouches = new List<NativeTouchInput>();
			touchCollection.ids[0] = 15;
			touchCollection.states[0] = State.Releasing;

			touchCollection.UpdateTouchBy(0, newTouches);
			Assert.AreEqual(15, touchCollection.ids[0]);
			Assert.AreEqual(State.Released, touchCollection.states[0]);

			touchCollection.UpdateTouchBy(0, newTouches);
			Assert.AreEqual(-1, touchCollection.ids[0]);
			Assert.AreEqual(State.Released, touchCollection.states[0]);
		}

		[Test]
		public void UpdateTouchIfPreviouslyPresentWithMultipleNewTouches()
		{
			TouchCollection touchCollection = CreateCollection();
			var newTouches = new List<NativeTouchInput> { new NativeTouchInput { Id = 3 }, new NativeTouchInput { Id = 15 } };
			touchCollection.ids[0] = 15;
			touchCollection.states[0] = State.Releasing;

			touchCollection.UpdateTouchBy(0, newTouches);
			Assert.AreEqual(15, touchCollection.ids[0]);
			Assert.AreEqual(State.Released, touchCollection.states[0]);
		}

		private TouchCollection CreateCollection()
		{
			var window = resolver.Resolve<Window>();
			var screen = new QuadraticScreenSpace(window);
			var positionTranslator = new CursorPositionTranslater(window, screen);
			return new TouchCollection(positionTranslator);
		}
	}
}