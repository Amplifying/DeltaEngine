using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Shapes;

namespace VehiclePhysicsGame
{
	public class Terrain
	{
		public Terrain(Physics physics)
		{
			this.physics = physics;
		}

		private readonly Physics physics;
		internal readonly List<Polygon> terrainParts = new List<Polygon>();

		public void CreateBlock(Rectangle rectangle, float rotation)
		{
			var blockPolygon = new Rect(rectangle, Color.Gray);
			blockPolygon.Rotation = rotation;
			PhysicsBody blockPhysicsBody = physics.CreateRectangle(rectangle.Size);
			blockPhysicsBody.Position = rectangle.Center;
			blockPhysicsBody.Rotation = rotation;
			blockPhysicsBody.IsStatic = true;
			blockPhysicsBody.Restitution = 0.3f;
			blockPhysicsBody.Friction = 0.99f;
			blockPolygon.Add(blockPhysicsBody);
			terrainParts.Add(blockPolygon);
		}

		public void CreateDefaultTerrain()
		{
			CreateBlock(Rectangle.FromCenter(Point.Half, new Size(0.7f, 0.2f)), -10.0f);
			CreateBlock(Rectangle.FromCenter(new Point(0.1f, 0.5f), new Size(0.29f, 0.1f)), -130.0f);
		}

		internal void clearTerrainParts()
		{
			terrainParts.Clear();
		}
	}
}