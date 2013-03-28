using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	internal class ScreenSpaceRenderingTests : TestStarter
	{
		[VisualTest]
		public void ScreenSpaceShouldBeAChoiceOfTheUser(Type resolver)
		{
			Start(resolver, (Renderer renderer, Window window, InputCommands input) =>
			{
				input.Add(MouseButton.Left, mouse => renderer.Screen = new QuadraticScreenSpace(window));
				input.Add(MouseButton.Middle, mouse => renderer.Screen = new RelativeScreenSpace(window));
				input.Add(MouseButton.Right, mouse => renderer.Screen = new PixelScreenSpace(window));
			}, (Renderer renderer) => renderer.DrawLine(Point.Zero, Point.One, Color.LightBlue));
		}

		[VisualTest]
		public void CustomUserInvertedScreenSpace(Type resolver)
		{
			Start(resolver, (Renderer renderer, Window window, InputCommands input) =>
			{
				input.Add(MouseButton.Left, mouse => renderer.Screen = new QuadraticScreenSpace(window));
				input.Add(MouseButton.Right, mouse => renderer.Screen = new InvertedScreenSpace(window));
			}, (Renderer renderer) => renderer.DrawLine(Point.Zero, Point.One, Color.LightBlue));
		}

		[VisualTest]
		public void UpdateResolutionInInvertedScreenSpace(Type resolver)
		{
			Start(resolver, (Renderer renderer, Window window, InputCommands input) =>
			{
				input.Add(MouseButton.Left, mouse => renderer.Screen = new InvertedScreenSpace(window));
				input.Add(MouseButton.Right, mouse => window.TotalPixelSize = new Size(400, 600));
				input.Add(MouseButton.Middle, mouse => window.TotalPixelSize = new Size(100, 400));
			}, (Renderer renderer) => renderer.DrawLine(Point.Zero, Point.One, Color.LightBlue));
		}

		[VisualTest]
		public void UpdateResolutionInRelativeScreenSpace(Type resolver)
		{
			Start(resolver, (Renderer renderer, Window window, InputCommands input) =>
			{
				input.Add(MouseButton.Left, mouse => renderer.Screen = new RelativeScreenSpace(window));
				input.Add(MouseButton.Right, mouse => window.TotalPixelSize = new Size(400, 600));
				input.Add(MouseButton.Middle, mouse => window.TotalPixelSize = new Size(100, 400));
			}, (Renderer renderer) => renderer.DrawLine(Point.Zero, Point.One, Color.LightBlue));
		}

		[VisualTest]
		public void UpdateResolutionInPixelScreenSpace(Type resolver)
		{
			Start(resolver, (Renderer renderer, Window window, InputCommands input) =>
			{
				input.Add(MouseButton.Left, mouse => renderer.Screen = new PixelScreenSpace(window));
				input.Add(MouseButton.Right, mouse => window.TotalPixelSize = new Size(400, 600));
				input.Add(MouseButton.Middle, mouse => window.TotalPixelSize = new Size(100, 400));
			}, (Renderer renderer) => renderer.DrawLine(Point.Zero, Point.One, Color.LightBlue));
		}
	}
}


