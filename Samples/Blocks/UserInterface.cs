using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Swaps from landscape to portrait mode as the window aspect changes
	/// </summary>
	public class UserInterface : IDisposable
	{
		public UserInterface(Renderer renderer, BlocksContent content, InputCommands input)
		{
			this.renderer = renderer;
			this.content = content;
			this.input = input;
			userInterfaceLandscape = new UserInterfaceLandscape(renderer, content);
			userInterfacePortrait = new UserInterfacePortrait(renderer, content);
			renderer.Screen.Window.ViewportSizeChanged += ShowCorrectSceneForAspect;
			ShowCorrectSceneForAspect(renderer.Screen.Window.ViewportPixelSize);
		}

		private readonly Renderer renderer;
		private readonly BlocksContent content;
		private readonly InputCommands input;
		private readonly UserInterfaceLandscape userInterfaceLandscape;
		private readonly UserInterfacePortrait userInterfacePortrait;

		private void ShowCorrectSceneForAspect(Size size)
		{
			if (size.AspectRatio >= 1.0f)
				ShowUserInterfaceLandscape();
			else
				ShowUserInterfacePortrait();
		}

		private void ShowUserInterfaceLandscape()
		{
			userInterfacePortrait.Hide();
			userInterfaceLandscape.Show(renderer, content, input);
		}

		private void ShowUserInterfacePortrait()
		{
			userInterfaceLandscape.Hide();
			userInterfacePortrait.Show(renderer, content, input);
		}

		public void AddToScore(int points)
		{
			userInterfaceLandscape.AddToScore(points);
			userInterfacePortrait.AddToScore(points);
		}

		public void Lose()
		{
			userInterfaceLandscape.Lose();
			userInterfacePortrait.Lose();
		}

		internal string Message
		{
			get { return userInterfaceLandscape.Message.Text; }
		}

		internal string Scoreboard
		{
			get { return userInterfaceLandscape.Scoreboard.Text; }
		}

		internal int Score
		{
			get { return userInterfaceLandscape.Score; }
		}

		public void Dispose()
		{
			userInterfaceLandscape.Dispose();
			userInterfacePortrait.Dispose();
		}
	}
}