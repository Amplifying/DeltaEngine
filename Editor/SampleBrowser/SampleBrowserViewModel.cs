using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Used to display Sample Games, Tutorials and Tests of engine and user code.
	/// http://deltaengine.net/Start/SampleBrowser
	/// </summary>
	internal class SampleBrowserViewModel : INotifyPropertyChanged
	{
		public SampleBrowserViewModel()
		{
			AddSelectionFilters();
			allSampleGames = GetSamplesHardcoded();
			AddEverythingTogether();
			//ToolTip += " - " + allSampleGames.Count + " Games, " + allVisualTests.Count + " Visual Tests";

			OnComboBoxSelectionChanged = new RelayCommand(UpdateItems);
			OnSearchTextChanged = new RelayCommand(UpdateItems);
			OnSearchTextRemoved = new RelayCommand(ClearSearchFilter);
			OnHelpClicked = new RelayCommand(OpenHelpWebsite);
			OnViewButtonClicked = new RelayCommand<Sample>(ViewSourceCode);
			OnStartButtonClicked = new RelayCommand<Sample>(StartExecutable);
			OnLaunchButtonClicked = new RelayCommand<Sample>(LaunchOnDevice);
		}

		private void AddSelectionFilters()
		{
			AssembliesAvailable = new List<String> { "Everything", "All Sample Games" };
			SelectedAssembly = AssembliesAvailable[0];
		}

		public List<string> AssembliesAvailable { get; private set; }
		public string SelectedAssembly { get; set; }

		private readonly List<Sample> everything = new List<Sample>();
		private readonly List<Sample> allSampleGames = new List<Sample>();

		private static List<Sample> GetSamplesHardcoded()
		{
			return
				new List<Sample>(new[]
				{
					new Sample
					{
						Title = "Blobs",
						Description = "Blobs Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/Blobs.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\Blobs\\Blobs.csproj",
						ExecutableFilePath = "\\Samples\\Blobs\\bin\\Debug\\Blobs.exe"
					},
					new Sample
					{
						Title = "Blocks",
						Description = "Blocks Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/Blocks.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\Blocks\\Blocks.csproj",
						ExecutableFilePath = "\\Samples\\Blocks\\bin\\Debug\\Blocks.exe"
					},
					new Sample
					{
						Title = "Breakout",
						Description = "Breakout Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/Breakout.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\Breakout\\Breakout.csproj",
						ExecutableFilePath = "\\Samples\\Breakout\\bin\\Debug\\Breakout.exe"
					},
					new Sample
					{
						Title = "EmptyGame",
						Description = "EmptyGame Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/EmptyGame.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\EmptyGame\\EmptyGame.csproj",
						ExecutableFilePath = "\\Samples\\EmptyGame\\bin\\Debug\\EmptyGame.exe"
					},
					new Sample
					{
						Title = "GameOfDeath",
						Description = "GameOfDeath Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/GameOfDeath.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\GameOfDeath\\GameOfDeath.csproj",
						ExecutableFilePath = "\\Samples\\GameOfDeath\\bin\\Debug\\GameOfDeath.exe"
					},
					new Sample
					{
						Title = "LogoApp",
						Description = "LogoApp Sample",
						ImageFilePath = "http://DeltaEngine.net/Content/Icons/LogoApp.png",
						ProjectFilePath = "C:\\Code\\DeltaEngine\\Samples\\LogoApp\\LogoApp.csproj",
						ExecutableFilePath = "\\Samples\\LogoApp\\bin\\Debug\\LogoApp.exe"
					}
				});
		}

		private void AddEverythingTogether()
		{
			everything.AddRange(allSampleGames);
			UpdateItems();
		}

		public ICommand OnComboBoxSelectionChanged { get; set; }

		private void UpdateItems()
		{
			SelectItemsByAssemblyComboBoxSelection();
			FilterItemsBySearchBox();
			Samples = itemsToDisplay;
		}

		private void SelectItemsByAssemblyComboBoxSelection()
		{
			if (SelectedAssembly == AssembliesAvailable[0])
				itemsToDisplay = everything;
			else if (SelectedAssembly == AssembliesAvailable[1])
				itemsToDisplay = allSampleGames;
		}

		private List<Sample> itemsToDisplay = new List<Sample>();

		private void FilterItemsBySearchBox()
		{
			if (String.IsNullOrEmpty(SearchFilter))
				return;

			string filterText = SearchFilter;
			itemsToDisplay = itemsToDisplay.Where(item => item.ContainsFilterText(filterText)).ToList();
		}

		private List<Sample> samples;
		public List<Sample> Samples
		{
			get { return samples; }
			set
			{
				samples = value;
				OnPropertyChanged("Samples");
			}
		}

		public ICommand OnSearchTextChanged { get; set; }
		public ICommand OnSearchTextRemoved { get; set; }

		private void ClearSearchFilter()
		{
			SearchFilter = "";
		}

		private string searchFilter;
		public string SearchFilter
		{
			get { return searchFilter; }
			set
			{
				searchFilter = value;
				OnPropertyChanged("SearchFilter");
			}
		}

		public ICommand OnHelpClicked { get; set; }

		private void OpenHelpWebsite()
		{
			Process.Start("http://DeltaEngine.net/Start/SampleBrowser");
		}

		public ICommand OnViewButtonClicked { get; set; }
		private void ViewSourceCode(Sample button) {}

		public ICommand OnStartButtonClicked { get; set; }

		private void StartExecutable(Sample button)
		{
			string exePath = button.ExecutableFilePath;
			var directory = Directory.GetParent(Directory.GetCurrentDirectory());
			directory = Directory.GetParent(directory.FullName);
			directory = Directory.GetParent(directory.FullName);
			exePath = directory.FullName + exePath;
			int index = exePath.LastIndexOf("\\", StringComparison.Ordinal);
			string exeDirectory = exePath.Substring(0, index);
			var compiledOutputDirectory = new ProcessStartInfo(exePath)
			{
				WorkingDirectory = exeDirectory
			};
			Process.Start(compiledOutputDirectory);
		}

		public ICommand OnLaunchButtonClicked { get; set; }
		private void LaunchOnDevice(Sample button) {}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}