using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// 2D entities are the basis of all 2D renderables like lines, sprites etc.
	/// </summary>
	public class Entity2D : Entity
	{
		public Entity2D()
			: this(Rectangle.Zero) { }

		public Entity2D(Rectangle drawArea)
			: this(drawArea, Color.White) {}

		public Entity2D(Rectangle drawArea, Color color, float rotation = 0)
		{
			Add(drawArea);
			Add(color);
			Add(rotation);
			Add<SortAndRender>();
			Add(Visibility.Show);
		}

		public Visibility Visibility
		{
			get { return Contains<Visibility>() ? Get<Visibility>() : Visibility.Show; }
			set
			{
				if (Contains<Visibility>())
					Set(value);
				else
					Add(value);
			}
		}

		public Rectangle DrawArea
		{
			get { return Get<Rectangle>(); }
			set { Set(value); }
		}

		public Point TopLeft
		{
			get { return Get<Rectangle>().TopLeft; }
			set { Set(new Rectangle(value, Get<Rectangle>().Size)); }
		}

		public Point Center
		{
			get { return Get<Rectangle>().Center; }
			set { Set(Rectangle.FromCenter(value, Get<Rectangle>().Size)); }
		}

		public Size Size
		{
			get { return Get<Rectangle>().Size; }
			set { Set(Rectangle.FromCenter(Get<Rectangle>().Center, value)); }
		}

		public Color Color
		{
			get { return Get<Color>(); }
			set { Set(value); }
		}

		public float AlphaValue
		{
			get { return Get<Color>().AlphaValue; }
			set
			{
				var color = Color;
				Set(new Color(color.R, color.G, color.B, value));
			}
		}

		public float Rotation
		{
			get { return Get<float>(); }
			set { Set(value); }
		}

		public int RenderLayer
		{
			get { return Contains<RenderLayer>() ? Get<RenderLayer>().Value : DefaultRenderLayer; }
			set
			{
				if (Contains<RenderLayer>())
					Set(new RenderLayer(value));
				else
					Add(new RenderLayer(value));
			}
		}

		public const int DefaultRenderLayer = 0;

		public bool RotatedDrawAreaContains(Point point)
		{
			var center = Contains<RotationCenter>() ? Get<RotationCenter>().Value : DrawArea.Center;
			point.RotateAround(center, -Rotation);
			return DrawArea.Contains(point);
		}

		/// <summary>
		/// Sorts all Entities into RenderLayer order; Then, for each, messages any listeners attached 
		/// that it's time to render it.
		/// </summary>
		public class SortAndRender : EntityHandler
		{
			public SortAndRender()
			{
				Filter = entity => ((Entity2D)entity).Visibility == Visibility.Show;
				Order = entity => ((Entity2D)entity).RenderLayer;
			}

			public override void Handle(Entity entity)
			{
				entity.MessageAllListeners(new TimeToRender());
			}

			public class TimeToRender {}

			public int NumberOfActiveRenderableObjects { get; private set; }

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.High; }
			}
		}
	}
}