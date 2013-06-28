using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaEngine.Datatypes
{
	public class BoundingBox
	{
		public BoundingBox(Vector min, Vector max)
		{
			Min = min;
			Max = max;
		}

		public Vector Min { get; set; }
		public Vector Max { get; set; }
	}
}
