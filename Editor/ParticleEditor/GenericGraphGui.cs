using System.Windows.Controls;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Base class of GraphGUIs for ranges of different datatypes.
	/// </summary>
	public class GenericGraphGui : Grid
	{
		//ncrunch: no coverage start
		protected void SetRows(int numberOfRows)
		{
			RowDefinitions.Clear();
			for (int i = 0; i < numberOfRows; i++)
				RowDefinitions.Add(new RowDefinition());
		}

		protected string propertyName;

		protected virtual void RefreshValuesFromRange() {}

		protected virtual void ClearGraphs() {}

		protected virtual void UpdateFromAdddedPointToRange(int insertedIndex) {}
	}
}