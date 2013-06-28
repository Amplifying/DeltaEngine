using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// 2D entities are the basis of all 2D renderables like lines, sprites etc.
	/// </summary>
	public class Entity2D : Entity
	{
		private Entity2D()
			: this(Rectangle.Zero) {}

		public Entity2D(Rectangle drawArea)
		{
			Add(drawArea);
			Start<SortAndRender>();
		}

		public virtual Rectangle DrawArea
		{
			get { return Get<Rectangle>(); }
			set { Set(value); }
		}

		public Point TopLeft
		{
			get { return Get<Rectangle>().TopLeft; }
			set { Set(new Rectangle(value, Get<Rectangle>().Size)); }
		}

		public virtual Point Center
		{
			get { return Get<Rectangle>().Center; }
			set { Set(Rectangle.FromCenter(value, Get<Rectangle>().Size)); }
		}

		public Size Size
		{
			get { return Get<Rectangle>().Size; }
			set { Set(Rectangle.FromCenter(Get<Rectangle>().Center, value)); }
		}

		public Visibility Visibility
		{
			get { return GetWithDefault(Visibility.Show); }
			set { Set(value); }
		}

		public Color Color
		{
			get { return GetWithDefault(Color.White); }
			set { Set(value); }
		}

		public float AlphaValue
		{
			get { return Color.AlphaValue; }
			set
			{
				var color = Color;
				Set(new Color(color.R, color.G, color.B, value));
			}
		}

		public virtual float Rotation
		{
			get { return GetWithDefault(0.0f); }
			set { Set(value); }
		}

		public int RenderLayer
		{
			get { return GetWithDefault(DefaultRenderLayer); }
			set { Set(value); }
		}

		public const int DefaultRenderLayer = 0;

		public bool RotatedDrawAreaContains(Point point)
		{
			var center = Contains<RotationCenter>() ? Get<RotationCenter>().Value : DrawArea.Center;
			return DrawArea.Contains(point.RotateAround(center, -Rotation));
		}

		/// <summary>
		/// Sorts all Entities into RenderLayer order; Then, for each, messages any listeners attached 
		/// that it's time to render it.
		/// </summary>
		public class SortAndRender : BatchedBehavior2D
		{
			public SortAndRender()
			{
				Filter = entity => ((Entity2D)entity).Visibility == Visibility.Show;
				Order = entity => GetSortKey((Entity2D)entity);
			}

			private static long GetSortKey(Entity2D entity)
			{
				long renderLayer = entity.RenderLayer;
				long hashCode = entity.GetType().GetHashCode();
				return renderLayer << 32 | hashCode;
			}

			public override void Handle(IEnumerable<Entity2D> entity2Ds)
			{
				isHandlingStarted = false;
				foreach (Entity2D entity in entity2Ds)
					ProcessEntity(entity);

				if (!isHandlingStarted)
					return;

				EntitySystem.Current.MessageAllListeners(new RenderBatch());
			}

			private bool isHandlingStarted;

			private void ProcessEntity(Entity2D entity)
			{
				long sortKey = GetSortKey(entity);
				if (isHandlingStarted && sortKey != lastSortKey)
					EntitySystem.Current.MessageAllListeners(new RenderBatch());

				entity.MessageAllListeners(new AddToBatch());
				lastSortKey = sortKey;
				isHandlingStarted = true;
			}

			private long lastSortKey;

			public class AddToBatch { }

			public class RenderBatch { }

			public override Priority Priority
			{
				get { return Priority.High; }
			}
		}
	}
}