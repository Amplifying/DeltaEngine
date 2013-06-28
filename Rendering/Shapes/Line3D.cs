using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Cameras;

namespace DeltaEngine.Rendering.Shapes
{
	public class Line3D : Entity3D
	{
		public Line3D(Vector start, Vector end, Color color)
		{
			Add(color);
			Add(new List<Vector> { start, end });
			Start<Render>();
		}

		public List<Vector> Points
		{
			get { return Get<List<Vector>>(); }
			set { Set(value); }
		}

		public Vector StartPoint
		{
			get { return Points[0]; }
			set { Points[0] = value; }
		}

		public Vector EndPoint
		{
			get { return Points[1]; }
			set { Points[1] = value; }
		}

		/// <summary>
		/// Responsible for rendering 3D lines
		/// </summary>
		class Render : EventListener3D
		{
			public Render(Device device, Drawing draw, LookAtCamera camera)
			{
				this.device = device;
				this.draw = draw;
				this.camera = camera;
			}

			private readonly Device device;
			private readonly Drawing draw;
			private readonly LookAtCamera camera;

			public override void ReceiveMessage(Entity3D entity, object message)
			{
				var color = entity.Get<Color>();
				var points = entity.Get<List<Vector>>();
				List<VertexPositionColor> vertices = new List<VertexPositionColor>();
				for (int num = 0; num < points.Count; num++)
					vertices.Add(new VertexPositionColor(points[num], color));

				device.SetProjectionMatrix(Matrix.CreatePerspective(camera.FieldOfView,
					360.0f / 640.0f, camera.NearPlane, camera.FarPlane));
				device.SetModelViewMatrix(camera.ViewMatrix);
				draw.DisableTexturing();
				draw.DrawVertices(VerticesMode.Lines, vertices.ToArray());
			}
		}
	}
}
