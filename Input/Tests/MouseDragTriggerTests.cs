using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseDragTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void DragMouseToCreateRectangles()
		{
			var rectangle = new FilledRect(Rectangle.Unused, Color.GetRandomColor());
			new Command(dragArea => rectangle.DrawArea = dragArea).Add(new MouseDragTrigger());
			new Command(() => rectangle = new FilledRect(Rectangle.Unused, Color.GetRandomColor())).Add(
				new MouseButtonTrigger(MouseButton.Left, State.Releasing));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			Assert.AreEqual(MouseButton.Left, new MouseDragTrigger().Button);
			Assert.AreEqual(MouseButton.Right, new MouseDragTrigger(MouseButton.Right).Button);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			Assert.AreEqual(MouseButton.Left, new MouseDragTrigger("").Button);
			Assert.AreEqual(MouseButton.Right, new MouseDragTrigger("Right").Button);
		}
	}
}