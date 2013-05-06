using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	/// <summary>
	/// Tests for the ViewModel of the SampleBrowser.
	/// </summary>
	public class SampleBrowserViewModelTests
	{
		[SetUp]
		public void Init()
		{
			viewModel = new SampleBrowserViewModel();
			ChangeComboBoxSelectionTo(0);
		}

		private SampleBrowserViewModel viewModel;

		[Test]
		public void AddSamples()
		{
			Assert.AreEqual(0, viewModel.Samples.Count);
			viewModel.Samples = GetSamplesMock();
			Assert.AreEqual(2, viewModel.Samples.Count);
		}

		private static List<Sample> GetSamplesMock()
		{
			return new List<Sample>
			{
				Sample.CreateTest("TestName", "TestName.csproj", "TestName.dll", "ClassName", "MethodName"),
				Sample.CreateGame("GameName", "GameName.csproj", "GameName.exe")
			};
		}

		[Test]
		public void CheckComboBoxSelections()
		{
			Assert.AreEqual(3, viewModel.AssembliesAvailable.Count);
			Assert.AreEqual("All", viewModel.SelectedAssembly);
		}

		[Test]
		public void ChangeComboBoxSelections()
		{
			viewModel.SetSampleGames(new List<Sample> { GetSamplesMock()[1] });
			viewModel.SetVisualTests(new List<Sample> { GetSamplesMock()[0] });
			viewModel.AddEverythingTogether();
			ChangeComboBoxSelectionTo(0);
			Assert.AreEqual(2, viewModel.GetItemsToDisplay().Count);
			ChangeComboBoxSelectionTo(1);
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("GameName", viewModel.GetItemsToDisplay()[0].Title);
			ChangeComboBoxSelectionTo(2);
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("TestName", viewModel.GetItemsToDisplay()[0].Title);
		}

		private void ChangeComboBoxSelectionTo(int index)
		{
			viewModel.SetSelection(index);
			viewModel.OnComboBoxSelectionChanged.Execute(null);
		}

		[Test]
		public void SortSamplesByProjectFilePath()
		{
			viewModel.SetAllSamples(GetSamplesMock());
			viewModel.AddEverythingTogether();
			Assert.AreEqual("GameName", viewModel.Samples[0].Title);
			Assert.AreEqual("TestName", viewModel.Samples[1].Title);
		}

		[Test]
		public void FilterSamplesWithSearchBox()
		{
			viewModel.SetAllSamples(GetSamplesMock());
			viewModel.AddEverythingTogether();
			ChangeSearchText("Game");
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("GameName", viewModel.GetItemsToDisplay()[0].Title);
			ChangeSearchText("Test");
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("TestName", viewModel.GetItemsToDisplay()[0].Title);
			ClearSearchText();
			Assert.AreEqual(2, viewModel.GetItemsToDisplay().Count);
		}

		private void ChangeSearchText(string text)
		{
			viewModel.SetSearchText(text);
			viewModel.OnSearchTextChanged.Execute(null);
		}

		private void ClearSearchText()
		{
			viewModel.OnSearchTextRemoved.Execute(null);
			viewModel.OnSearchTextChanged.Execute(null);
		}

		[Test, Ignore]
		public void ClickOnHelpShouldOpenWebsiteInWebbrowser()
		{
			viewModel.OnHelpClicked.Execute(null);
		}

		[Test]
		public void UninitializedViewClickShouldThrow()
		{
			Assert.Throws<TargetInvocationException>(() => viewModel.OnViewButtonClicked.Execute(null));
		}

		[Test]
		public void UninitializedStartClickShouldThrow()
		{
			Assert.Throws<TargetInvocationException>(() => viewModel.OnStartButtonClicked.Execute(null));
		}
	}
}