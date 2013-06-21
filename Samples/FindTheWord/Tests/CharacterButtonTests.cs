using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	class CharacterButtonTests : TestWithAllFrameworks
	{
		[Test]
		public void SettingCharactersSetsState()
		{
			Start(typeof(MockResolver), (ContentLoader content, InputCommands input) =>
			{
				var characterButton = new CharacterButton(input, content, 0.5f, 0.5f);
				characterButton.Letter = 'S';
				characterButton.ShowLetter();
				Assert.IsTrue(characterButton.IsClickable());
				Assert.AreEqual("CharacterButton(0 - S)", characterButton.ToString());
			});
		}

		[Test]
		public void NotClickableWhenLetterRemoved()
		{
			Start(typeof(MockResolver), (ContentLoader content, InputCommands input) =>
			{
				var characterButton = new CharacterButton(input, content, 0.5f, 0.5f);
				characterButton.Letter = 'S';
				characterButton.RemoveLetter();
				Assert.IsFalse(characterButton.IsClickable());
			});
		}
	}
}
