using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeltaEngine.Datatypes;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for GenericGraphGui.xaml
	/// </summary>
	public partial class GenericGraphGui
	{
		public GenericGraphGui()
		{
			InitializeComponent();
			GraphContainer.Children.Add(new GraphControl());
		}

		public void SynchronizeToVectorGraph(TimeRangeGraph<Vector3D> vectorGraph)
		{
			GraphContainer.Children.Clear();
			var graphAxisX = new GraphControl();
			var graphAxisY = new GraphControl();
			var graphAxisZ = new GraphControl();
			GraphContainer.Children.Add(graphAxisX);
			GraphContainer.Children.Add(graphAxisY);
			GraphContainer.Children.Add(graphAxisZ);
		}

		public void SynchronizeToSizeGraph(TimeRangeGraph<Size> sizeGraph)
		{
			GraphContainer.Children.Clear();
			var graphAxisWidth = new GraphControl();
			var graphAxisHeight = new GraphControl();
			GraphContainer.Children.Add(graphAxisWidth);
			GraphContainer.Children.Add(graphAxisHeight);
		}
	}
}
