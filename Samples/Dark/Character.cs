using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace Dark
{
	public enum CharacterState
	{
		Stopped,
		Moving
	}

	public abstract class Character : Sprite
	{
		protected Character(Image image, Vector position)
			: base(image, Rectangle.Zero)
		{
			Position = position;
			Facing = upVector;
			Speed = 0.0f;
			State = CharacterState.Stopped;
			Center = Point.Zero;
			DrawArea = Rectangle.FromCenter(position.X, position.Y, CharacterSize, CharacterSize);
			RenderLayer = 64;
		}

		protected const float CharacterSize = 0.125f;
		protected const float TimePerImage = 0.15f;

		public Vector Position { get; set; }
		public Vector Facing { get; set; }
		public float Speed { get; set; }

		public CharacterState State
		{
			get { return state; }
			set 
			{
				if (value != state)
				{
					state = value;
					animationStep = 0;
				}
			}
		}

		protected CharacterState state;
		protected float timeSinceLastImageUpdate;
		protected int animationStep;
		protected Vector upVector = new Vector(0.0f, 1.0f, 0.0f);
		protected List<Image> images = new List<Image>();
		
		public virtual void Update()
		{
			Position += Facing * Time.Current.Delta * Speed;
			DrawArea = Rectangle.FromCenter(Position.X, Position.Y, Size.Width, Size.Height);
			Rotation = Vector2DMath.GetAngle(upVector, Facing) * RadiansToDegrees + 180.0f;
			timeSinceLastImageUpdate += Time.Current.Delta;
			SelectImage();
		}

		private const float RadiansToDegrees = 57.2957795f;

		protected abstract void SelectImage();
	}
}