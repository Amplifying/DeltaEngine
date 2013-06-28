using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Base entity for 3D objects.
	/// </summary>
	public class Entity3D : Entity
	{
		public Entity3D()
			: this(new Transform()) { }

		public Entity3D(Transform transform)
		{
			Add(transform);
			Add(Visibility.Show);
			Start<SortAndRender>();
		}

		public Vector Position
		{
			get { return Get<Transform>().Position; }
			set { Get<Transform>().Position = value; }
		}

		public Vector EulerAngles
		{
			get { return Get<Transform>().Angles; }
			set { Get<Transform>().Angles = value; }
		}

		public Visibility Visibility
		{
			get { return Get<Visibility>(); }
			set { Set(value); }
		}

		/// <summary>
		/// Notifies attached listeners to render.
		/// </summary>
		class SortAndRender : Behavior3D
		{
			public SortAndRender()
			{
				Filter = entity => ((Entity3D)entity).Visibility == Visibility.Show;
				Order = entity => ((Entity3D)entity).Position.Z;
			}

			public override void Handle(Entity3D entity)
			{
				entity.MessageAllListeners(new TimeToRender());
			}

			private class TimeToRender { }

			public override Priority Priority
			{
				get { return Priority.High; }
			}
		}
	}
}
