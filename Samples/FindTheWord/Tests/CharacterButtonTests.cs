using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	internal class CharacterButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void SettingCharactersSetsState()
		{
			var characterButton = new CharacterButton(Resolve<InputCommands>(), 0.5f, 0.5f);
			characterButton.Letter = 'S';
			characterButton.ShowLetter();
			Assert.IsTrue(characterButton.IsClickable());
			Assert.AreEqual("CharacterButton(0 - S)", characterButton.ToString());
		}

		[Test]
		public void NotClickableWhenLetterRemoved()
		{
			var characterButton = new CharacterButton(Resolve<InputCommands>(), 0.5f, 0.5f);
			characterButton.Letter = 'S';
			characterButton.RemoveLetter();
			Assert.IsFalse(characterButton.IsClickable());
		}
	}
}