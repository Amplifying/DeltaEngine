using System;
using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	/// <summary>
	/// Tests and Visual Tests for the Tutorial-Dialogue
	/// </summary>
	internal class TutorialDialogueTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void DisplayFullDialogueAndAdvanceByClick(Type resolverType)
		{
			Start(resolverType, (ContentLoader loader, ScreenSpace screen, InputCommands commands) =>
			{
				InitDialogue(loader, screen);
				commands.Add(MouseButton.Left, State.Releasing, mouse => dialogue.AdvanceToNextMessage());
				dialogue.DialogueEnded += () => screen.Window.Dispose();
			});
		}

		[Test]
		public void AdvancingGivesAnEvent()
		{
			Start(typeof(MockResolver), (ContentLoader loader, ScreenSpace screen) =>
			{
				InitDialogue(loader, screen);
				bool advanced = false;
				dialogue.DialogueAdvanced += () => { advanced = true; };
				dialogue.AdvanceToNextMessage();
				Assert.IsTrue(advanced);
			});
		}

		[Test]
		public void AdvanceDialogueToEnd()
		{
			Start(typeof(MockResolver), (ContentLoader loader, ScreenSpace screen) =>
			{
				InitDialogue(loader, screen);
				bool ended = false;
				dialogue.DialogueEnded += () => { ended = true; };
				for (int i = 0; i < 9; i++)
					dialogue.AdvanceToNextMessage();
				Assert.IsTrue(ended);
			});
		}

		[Test]
		public void SkipTutorial()
		{
			Start(typeof(MockResolver), (ContentLoader loader, ScreenSpace screen) =>
			{
				InitDialogue(loader, screen);
				bool ended = false;
				dialogue.DialogueEnded += () => { ended = true; };
				dialogue.Skip();
				Assert.IsTrue(ended);
			});
		}

		private void InitDialogue(ContentLoader loader, ScreenSpace screen)
		{
			dialogue = new TutorialDialogue(loader, screen);
		}

		private TutorialDialogue dialogue;
	}
}