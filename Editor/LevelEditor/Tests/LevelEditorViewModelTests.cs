using System.Windows;
using DeltaEngine.GameLogic;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	internal class LevelEditorViewModelTests : TestWithMocksOrVisually
	{
		[Test, Category("Slow")]
		public void BuildXmlData()
		{
			var viewModel = new LevelEditorViewModel(new MockService("Test", "Test"));
			viewModel.PlaceTile(LevelTileType.SpawnPoint, new Point(1, 2));
			viewModel.PlaceTile(LevelTileType.ExitPoint, new Point(7, 7));
			viewModel.AddWave(3, 1.5f, 0, "Cloth, Cloth, Cloth");
			var xml = viewModel.BuildXmlData();
			Assert.AreEqual("Level", xml.Name);
			Assert.AreEqual(2, xml.Attributes.Count);
			Assert.AreEqual(4, xml.Children.Count);
			Assert.AreEqual("SpawnPoint", xml.Children[0].Name);
			Assert.AreEqual("ExitPoint", xml.Children[1].Name);
			Assert.AreEqual("Map", xml.Children[2].Name);
			Assert.AreEqual("Wave", xml.Children[3].Name);
		}
	}
}