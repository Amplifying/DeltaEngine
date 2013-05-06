using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Used to display Sample Games, Tutorials and Tests of engine and user code.
	/// http://deltaengine.net/Start/SampleBrowser
	/// </summary>
	public class SampleBrowserViewModel : ViewModelBase
	{
		public SampleBrowserViewModel()
		{
			Samples = new List<Sample>();
			AddSelectionFilters();
			RegisterCommands();
		}

		public List<Sample> Samples
		{
			get { return samples; }
			set
			{
				samples = value;
				RaisePropertyChanged("Samples");
			}
		}
		private List<Sample> samples;

		private void AddSelectionFilters()
		{
			AssembliesAvailable = new List<String>
			{
				"All",
				"Sample Games",
				//"Tutorials",
				"Visual Tests"
			};
			SelectedAssembly = AssembliesAvailable[1];
		}

		public List<string> AssembliesAvailable { get; private set; }
		public string SelectedAssembly { get; private set; }

		private void RegisterCommands()
		{
			OnComboBoxSelectionChanged = new RelayCommand(UpdateItems);
			OnSearchTextChanged = new RelayCommand(UpdateItems);
			OnSearchTextRemoved = new RelayCommand(ClearSearchFilter);
			OnHelpClicked = new RelayCommand(OpenHelpWebsite);
			OnViewButtonClicked = new RelayCommand<Sample>(ViewSourceCode, CanViewSourceCode);
			OnStartButtonClicked = new RelayCommand<Sample>(StartExecutable, CanStartExecutable);
		}

		public ICommand OnComboBoxSelectionChanged { get; private set; }

		private void UpdateItems()
		{
			SelectItemsByAssemblyComboBoxSelection();
			FilterItemsBySearchBox();
			Samples = itemsToDisplay.OrderBy(o => o.ProjectFilePath).ToList();
		}

		private void SelectItemsByAssemblyComboBoxSelection()
		{
			if (SelectedAssembly == AssembliesAvailable[0])
				itemsToDisplay = everything;
			else if (SelectedAssembly == AssembliesAvailable[1])
				itemsToDisplay = allSampleGames;
			else if (SelectedAssembly == AssembliesAvailable[2])
				itemsToDisplay = allVisualTests;
		}

		private List<Sample> itemsToDisplay = new List<Sample>();
		private List<Sample> everything = new List<Sample>();
		private List<Sample> allSampleGames = new List<Sample>();
		private List<Sample> allVisualTests = new List<Sample>();

		private void FilterItemsBySearchBox()
		{
			if (String.IsNullOrEmpty(SearchFilter))
				return;

			string filterText = SearchFilter;
			itemsToDisplay = itemsToDisplay.Where(item => item.ContainsFilterText(filterText)).ToList();
		}

		public ICommand OnSearchTextChanged { get; private set; }
		public ICommand OnSearchTextRemoved { get; private set; }

		private void ClearSearchFilter()
		{
			SearchFilter = "";
		}

		public string SearchFilter
		{
			get { return searchFilter; }
			set
			{
				searchFilter = value;
				RaisePropertyChanged("SearchFilter");
			}
		}
		private string searchFilter;

		public ICommand OnHelpClicked { get; private set; }

		private static void OpenHelpWebsite()
		{
			Process.Start("http://DeltaEngine.net/Start/SampleBrowser");
		}

		public ICommand OnViewButtonClicked { get; private set; }

		private void ViewSourceCode(Sample sample)
		{
			sampleLauncher.OpenProject(sample);
		}

		private SampleLauncher sampleLauncher;

		private bool CanViewSourceCode(Sample sample)
		{
			return sampleLauncher.DoesProjectExist(sample);
		}

		public ICommand OnStartButtonClicked { get; private set; }

		private void StartExecutable(Sample sample)
		{
			sampleLauncher.StartExecutable(sample);
		}

		private bool CanStartExecutable(Sample sample)
		{
			return sampleLauncher.DoesAssemblyExist(sample);
		}

		public void GetSamples()
		{
			var sampleCreator = new SampleCreator();
			sampleCreator.CreateSamples();
			foreach (var sample in sampleCreator.Samples)
				if (sample.Category == SampleCategory.Game)
					allSampleGames.Add(sample);
				else
					allVisualTests.Add(sample);

			AddEverythingTogether();
			sampleLauncher = new SampleLauncher();
		}

		public void AddEverythingTogether()
		{
			everything.AddRange(allSampleGames);
			everything.AddRange(allVisualTests);
			UpdateItems();
		}

		public void SetAllSamples(List<Sample> list)
		{
			everything = list;
		}

		public void SetSampleGames(List<Sample> list)
		{
			allSampleGames = list;
		}

		public void SetVisualTests(List<Sample> list)
		{
			allVisualTests = list;
		}

		public List<Sample> GetItemsToDisplay()
		{
			return itemsToDisplay;
		}

		public void SetSelection(int index)
		{
			SelectedAssembly = AssembliesAvailable[index];
		}

		public void SetSearchText(string text)
		{
			SearchFilter = text;
		}
	}
}