using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// 2D entities are the basis of all 2D renderables like lines, sprites etc.
	/// </summary>
	public class Entity2D : Entity
	{
		public Entity2D(Rectangle drawArea)
			: this(drawArea, Color.White) {}

		public Entity2D(Rectangle drawArea, Color color, float rotation = 0)
		{
			Add(drawArea);
			Add(color);
			Add(new Rotation(rotation));
			Add<SortAndRenderEntity2D>();
		}

		private Entity2D()
			: this(Rectangle.Zero, Color.White) {}

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
			get { return Get<Rotation>().Value; }
			set { Set(new Rotation(value)); }
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

		/*
			//Add(new Transition());//TODO: remove here
			//Add<PerformTransition>();//TODO: remove here
		public Color TransitionColor
		{
			set
			{
				if (Contains<TransitionColor>())
					Get<TransitionColor>().End = value;
				else
					Add(new TransitionColor { End = value });
			}
		}

		public Color TransitionOutlineColor
		{
			set
			{
				if (Contains<TransitionOutlineColor>())
					Get<TransitionOutlineColor>().End = value;
				else
					Add(new TransitionOutlineColor { End = value });
			}
		}

		public Point TransitionPosition
		{
			set
			{
				if (Contains<TransitionPosition>())
					Get<TransitionPosition>().End = value;
				else
					Add(new TransitionPosition { End = value });
			}
		}

		public Size TransitionSize
		{
			set
			{
				if (Contains<TransitionSize>())
					Get<TransitionSize>().End = value;
				else
					Add(new TransitionSize { End = value });
			}
		}

		public float TransitionRotation
		{
			set
			{
				if (Contains<TransitionRotation>())
					Get<TransitionRotation>().End = value;
				else
					Add(new TransitionRotation { End = value });
			}
		}

		public float TransitionDuration
		{
			get { return Get<Transition>().Duration; }
			set
			{
				var transition = Get<Transition>();
				transition.Duration = value;
				transition.Elapsed = value;
			}
		}

		public void BeginTransitionThenRemove()
		{
			BeginTransition(true);
		}

		private void BeginTransition(bool isEntityToBeRemovedWhenFinished)
		{
			SetStartColorsIfRequired();
			SetStartPositionAndSizeIfRequired();
			SetStartRotationIfRequired();
			Get<Transition>().Elapsed = 0;
			Get<Transition>().IsEntityToBeRemovedWhenFinished = isEntityToBeRemovedWhenFinished;
		}

		private void SetStartColorsIfRequired()
		{
			if (Contains<TransitionColor>())
				Get<TransitionColor>().Start = Color;

			if (Contains<OutlineColor>() && Contains<TransitionOutlineColor>())
				Get<TransitionOutlineColor>().Start = Get<OutlineColor>().Value;
		}

		private void SetStartPositionAndSizeIfRequired()
		{
			if (Contains<TransitionPosition>())
				Get<TransitionPosition>().Start = TopLeft;

			if (Contains<TransitionSize>())
				Get<TransitionSize>().Start = Size;
		}

		private void SetStartRotationIfRequired()
		{
			if (Contains<TransitionRotation>())
				Get<TransitionRotation>().Start = Rotation;
		}

		public void BeginTransition()
		{
			BeginTransition(false);
		}*/
	}
}