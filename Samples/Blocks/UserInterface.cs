using System;

namespace Blocks
{
	/// <summary>
	/// Swaps from landscape to portrait mode as the window aspect changes
	/// </summary>
	public class UserInterface : IDisposable
	{
		public UserInterface(BlocksContent content)
		{
			userInterfaceLandscape = new UserInterfaceLandscape(content);
			userInterfacePortrait = new UserInterfacePortrait(content);
		}

		private readonly UserInterfaceLandscape userInterfaceLandscape;
		private readonly UserInterfacePortrait userInterfacePortrait;

		public void ShowUserInterfaceLandscape()
		{
			userInterfacePortrait.Hide();
			userInterfaceLandscape.ResizeInterface();
			userInterfaceLandscape.Show();
		}

		public void ShowUserInterfacePortrait()
		{
			userInterfaceLandscape.Hide();
			userInterfaceLandscape.ResizeInterface();
			userInterfacePortrait.Show();
		}

		public void AddToScore(int points)
		{
			Score += points;
		}

		public void Lose()
		{
			Score = 0;
		}

		public int Score { get; set; }

		public void Dispose()
		{
			userInterfaceLandscape.Hide();
			userInterfacePortrait.Hide();
		}
	}
}