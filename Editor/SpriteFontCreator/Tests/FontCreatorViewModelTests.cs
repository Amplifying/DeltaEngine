using NUnit.Framework;

namespace DeltaEngine.Editor.SpriteFontCreator.Tests
{
	internal class FontCreatorViewModelTests
	{
		[SetUp]
		public void SetupViewModel()
		{
			viewModel = new FontCreatorViewModel();
		}

		private FontCreatorViewModel viewModel;

		[Test]
		public void StartFontCreationFromFirstSystemFont()
		{
			viewModel.UpdateAvailableDefaultFonts();
			viewModel.FamilyFilename = viewModel.AvailableDefaultFontNames[0];
			viewModel.ContentName = "TestFont";
			viewModel.GenerateFontFromSettings();
		}

		[Test]
		public void GetSystemFontsForSelection()
		{
			viewModel.UpdateAvailableDefaultFonts();
			Assert.IsNotEmpty(viewModel.AvailableDefaultFontNames);
		}

		[Test]
		public void NoFontCreationWithoutContentName()
		{
			viewModel.ContentName = "";
			viewModel.GenerateFontFromSettings();
		}

		[Test]
		public void SettingFontStyles()
		{
			SetStyles();
			Assert.IsTrue(viewModel.Bold && !viewModel.Italic && viewModel.Underline && viewModel.AddShadow);
		}

		private void SetStyles()
		{
			viewModel.AddShadow = false;
			viewModel.Bold = false;
			viewModel.Underline = false;
			viewModel.Italic = true;
			viewModel.Bold = true;
			viewModel.Italic = false;
			viewModel.Underline = true;
			viewModel.AddShadow = true;
		}
	}
}