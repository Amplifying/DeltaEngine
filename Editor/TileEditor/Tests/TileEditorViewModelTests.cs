using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.TileEditor.Tests
{
	internal class TileEditorViewModelTests
	{
		[Test]
		public void BuildXmlData()
		{
			var viewModel = new TileEditorViewModel();
			viewModel.PlaceTile(TileType.SpawnPoint, new Point(1, 2));
			viewModel.PlaceTile(TileType.GoalPoint, new Point(7, 7));
			viewModel.AddWave(3, 1.5f, "Cloth, Cloth, Cloth");
			var xml = viewModel.BuildXmlData();
			Assert.AreEqual("Level", xml.Name);
			Assert.AreEqual(2, xml.Attributes.Count);
			Assert.AreEqual(4, xml.Children.Count);
		}
	}
}