using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Contains the information needed to render a 3D model.
	/// </summary>
	public class Model : Entity3D
	{
		public Model(Geometry geometry, Material material)
		{
			Add(geometry);
			Add(material);
			OnDraw<GeometryRender>();
		}

		private Geometry Geometry
		{
			get { return Get<Geometry>(); }
			set { Set(value); }
		}

		/// <summary>
		/// Responsible for rendering a geometry
		/// </summary>
		public class GeometryRender : DrawBehavior
		{
			public GeometryRender(Drawing drawing)
			{
				this.drawing = drawing;
			}

			private readonly Drawing drawing;

			public void Draw(IEnumerable<DrawableEntity> entities)
			{
				foreach (var entity in entities)
				{
					var modelTranform = Matrix.FromQuaternion(entity.Get<Quaternion>());
					modelTranform.Translation = entity.Get<Vector>();
					drawing.AddGeometry(entity.Get<Geometry>(), entity.Get<Material>());
				}
			}
		}
	}
}