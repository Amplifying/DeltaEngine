using System.Linq;
using DeltaEngine.Datatypes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Convert between display and simulation units and between Farseer and Delta Engine datatypes 
	/// </summary>
	internal class UnitConverter
	{
		public UnitConverter(float displayUnitsPerSimUnit)
		{
			displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
			simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
		}

		private readonly float displayUnitsToSimUnitsRatio;
		private readonly float simUnitsToDisplayUnitsRatio;

		public Vector2 ToDisplayUnits(Vector2 simUnits)
		{
			return simUnits * displayUnitsToSimUnitsRatio;
		}

		public float ToSimUnits(float displayUnits)
		{
			return displayUnits * simUnitsToDisplayUnitsRatio;
		}

		public Vector2 ToSimUnits(Vector2 displayUnits)
		{
			return displayUnits * simUnitsToDisplayUnitsRatio;
		}

		public Point Convert(Vector2 value)
		{
			return new Point(value.X, value.Y);
		}

		public Vector2 Convert(Point value)
		{
			return new Vector2(value.X, value.Y);
		}

		public Vertices Convert(params Point[] vertices)
		{
			var fVertices = new Vertices(vertices.Length);
			fVertices.AddRange(vertices.Select(t => ToSimUnits(Convert(t))));
			return fVertices;
		}
	}
}