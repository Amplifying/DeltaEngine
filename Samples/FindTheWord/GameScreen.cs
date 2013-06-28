using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;

namespace FindTheWord
{
	public class GameScreen : Sprite, IDisposable
	{
		public GameScreen(InputCommands input)
			: base(
				ContentLoader.Load<Image>("GameBackground"), DefaultDrawArea)
		{
			Input = input;
			currentLevelIndex = -1;
			randomizer = new PseudoRandom();
			font = new Font("Tahoma30");
			currentLevelFontText = new FontText(font, "", new Point(0.5f, DrawArea.Top + 0.079f));
			nextLevel = new NextLevelScreen(input);
			CreateImageContainersForRiddle();
			CreateDisplayCharacterButtons();
			CreateLevels();
		}

		private static readonly Rectangle DefaultDrawArea = new Rectangle(0, 0.1875f, 1, 0.625f);
		private InputCommands Input { get; set; }
		private int currentLevelIndex;
		private readonly PseudoRandom randomizer;
		private readonly Font font;
		private readonly NextLevelScreen nextLevel;

		private void CreateImageContainersForRiddle()
		{
			image1 = CreateSprite(0.1075f, 0.3268f);
			image2 = CreateSprite(0.396f, 0.3246f);
			image3 = CreateSprite(0.691f, 0.3239f);
		}

		private Sprite image1;
		private Sprite image2;
		private Sprite image3;

		private Sprite CreateSprite(float xPosition, float yPosition)
		{
			const float Dimension = 245.0f / 1280.0f;
			var newSprite = new Sprite(Image, new Rectangle(xPosition, yPosition, Dimension, Dimension));
			newSprite.RenderLayer = 2;
			return newSprite;
		}

		private void CreateDisplayCharacterButtons()
		{
			displayCharacters = new List<CharacterButton>();
			float xCenter = DisplayCharactersCenterStartX;
			float yCenter = 0.6935f;
			for (int i = 0; i < 12; i++)
			{
				if (i != 0 && i % 6 == 0)
				{
					xCenter = DisplayCharactersCenterStartX;
					yCenter += FullCharacterGap;
				}
				AddNewDisplayCharToList(xCenter, yCenter, i);
				xCenter += FullCharacterGap;
			}
		}

		private void AddNewDisplayCharToList(float xCenter, float yCenter, int index)
		{
			var newCharButton = CreateEmptyCharacter(xCenter, yCenter);
			newCharButton.Clicked += OnDisplayCharButtonClicked;
			newCharButton.Index = index;
			displayCharacters.Add(newCharButton);
		}

		private List<CharacterButton> displayCharacters;
		private const float DisplayCharactersCenterStartX = 0.378f;
		private const float FullCharacterGap = CharacterButton.Dimension + 0.00025f;

		private CharacterButton CreateEmptyCharacter(float xCenter, float yCenter)
		{
			return new CharacterButton(Input, xCenter, yCenter);
		}

		private void OnDisplayCharButtonClicked(Button sender)
		{
			var displayCharButton = (CharacterButton)sender;
			if (!displayCharButton.IsClickable())
				return;
			var freeButton = FindNextFreeSolutionButton();
			if (freeButton == null)
				return;
			freeButton.Letter = displayCharButton.Letter;
			freeButton.Index = displayCharButton.Index;
			displayCharButton.RemoveLetter();
			if (IsWordCorrect())
				CompleteLevel();
		}

		private CharacterButton FindNextFreeSolutionButton()
		{
			foreach (CharacterButton button in solutionCharacters)
				if (button.Letter == CharacterButton.NoCharacter)
					return button;
			return null;
		}

		public void Hide()
		{
			Visibility = Visibility.Hide;
			image1.Visibility = Visibility.Hide;
			image2.Visibility = Visibility.Hide;
			image3.Visibility = Visibility.Hide;
			foreach (CharacterButton character in displayCharacters)
				character.Visibility = Visibility.Hide;
		}

		public void FadeIn()
		{
			StartNextLevel();
			Visibility = Visibility.Show;
			image1.Visibility = Visibility.Show;
			image2.Visibility = Visibility.Show;
			image3.Visibility = Visibility.Show;
			foreach (CharacterButton character in displayCharacters)
				character.Visibility = Visibility.Show;
		}

		public void CompleteLevel()
		{
			levelComplete = true;
			ClearLevel();
			nextLevel.ShowAndWaitForInput();
			nextLevel.StartNextLevel += () => AdvanceToNextLevel();
		}

		private bool levelComplete;

		private void AdvanceToNextLevel()
		{
			if (!levelComplete)
				return;
			StartNextLevel();
			UpdateLevelFontText();
		}

		public void StartNextLevel()
		{
			currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
			currentRiddle = levels[currentLevelIndex];
			SetImagesToCurrentLevel();
			SetDisplayCharactersToCurrentLevel();
			CreateSolutionCharacterButtons();
			UpdateLevelFontText();
			levelComplete = false;
		}

		private void SetImagesToCurrentLevel()
		{
			image1.Visibility = Visibility.Show;
			image2.Visibility = Visibility.Show;
			image3.Visibility = Visibility.Show;
			image1.Image = ContentLoader.Load<Image>(currentRiddle.Image1);
			image2.Image = ContentLoader.Load<Image>(currentRiddle.Image2);
			image3.Image = ContentLoader.Load<Image>(currentRiddle.Image3);
		}

		private void SetDisplayCharactersToCurrentLevel()
		{
			List<char> wordPlusFillCharacters = GetWordPlusFillCharacters();
			for (int i = 0; i < displayCharacters.Count; i++)
				displayCharacters[i].Letter = wordPlusFillCharacters[i];
		}

		private LevelData currentRiddle;

		private List<char> GetWordPlusFillCharacters()
		{
			var list = new List<char>(currentRiddle.SearchedWord.ToUpper().ToCharArray());
			while (list.Count < displayCharacters.Count)
				list.Add(GetRandomUpperCaseLetter());
			list = RandomizeList(list);
			return list;
		}

		private char GetRandomUpperCaseLetter()
		{
			int randomCharIndex = randomizer.Get('A', 'Z');
			return (char)randomCharIndex;
		}

		private List<char> RandomizeList(List<char> list)
		{
			var randomizedList = new List<char>();
			while (list.Count > 0)
			{
				int randomSelectionIndex = randomizer.Get(0, list.Count);
				randomizedList.Add(list[randomSelectionIndex]);
				list.RemoveAt(randomSelectionIndex);
			}
			return randomizedList;
		}

		private void CreateSolutionCharacterButtons()
		{
			if (solutionCharacters != null)
				foreach (CharacterButton character in solutionCharacters)
					character.Clicked -= OnSolutionCharButtonClicked;
			solutionCharacters = new List<CharacterButton>();
			int numberOfCharacters = currentRiddle.SearchedWord.Length;
			float xCenter = GetSolutionCharacterCenterStartX(numberOfCharacters);
			const float YCenter = 0.6f;
			const float ButtonGap = CharacterButton.Dimension + 0.00025f;
			for (int i = 0; i < numberOfCharacters; i++)
			{
				AddNewSolutionButtonToList(xCenter, YCenter, i);
				xCenter += ButtonGap;
			}
		}

		private void AddNewSolutionButtonToList(float xCenter, float yCenter, int index)
		{
			CharacterButton newCharButton = CreateEmptyCharacter(xCenter, yCenter);
			newCharButton.Clicked += OnSolutionCharButtonClicked;
			newCharButton.Index = index;
			solutionCharacters.Add(newCharButton);
		}

		private List<CharacterButton> solutionCharacters;

		private void OnSolutionCharButtonClicked(Button sender)
		{
			var solutionCharButton = (CharacterButton)sender;
			var displayCharButton = displayCharacters[solutionCharButton.Index];
			solutionCharButton.Letter = CharacterButton.NoCharacter;
			displayCharButton.ShowLetter();
		}

		private static float GetSolutionCharacterCenterStartX(int numberOfCharacters)
		{
			const float FourCharStartPos = DisplayCharactersCenterStartX + FullCharacterGap;
			return numberOfCharacters == 4 ? FourCharStartPos : FourCharStartPos - FullCharacterGap / 2;
		}

		private void UpdateLevelFontText()
		{
			if (levelComplete)
			{
				currentLevelFontText.Visibility = Visibility.Hide;
				return;
			}
			currentLevelFontText.Visibility = Visibility.Show;
			currentLevelFontText.Text = "Level " + (currentLevelIndex + 1);
		}

		private readonly FontText currentLevelFontText;

		private void CreateLevels()
		{
			levels = new List<LevelData>();
			levels.Add(CreateLevelData("Wurm", "WORM"));
			levels.Add(CreateLevelData("Mouse", "MOUSE"));
			levels.Add(CreateLevelData("Sea", "WATER"));
		}

		private List<LevelData> levels;

		private static LevelData CreateLevelData(string imageBaseName, string searchedWord)
		{
			return new LevelData
			{
				Image1 = imageBaseName + 1,
				Image2 = imageBaseName + 2,
				Image3 = imageBaseName + 3,
				SearchedWord = searchedWord,
			};
		}

		public void Dispose()
		{
			ClearLevel();
		}

		private void ClearSolutionChars()
		{
			if (solutionCharacters == null)
				return;
			foreach (var solutionCharacter in solutionCharacters)
				solutionCharacter.Dispose();
			solutionCharacters.Clear();
		}

		private bool IsWordCorrect()
		{
			for (int i = 0; i < solutionCharacters.Count; i++)
				if (!solutionCharacters[i].Letter.Equals(levels[currentLevelIndex].SearchedWord[i]))
					return false;

			return true;
		}

		private void ClearLevel()
		{
			ClearSolutionChars();
			image1.Visibility = Visibility.Hide;
			image2.Visibility = Visibility.Hide;
			image3.Visibility = Visibility.Hide;
		}
	}
}