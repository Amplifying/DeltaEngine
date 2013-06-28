using System;
using System.Runtime.Remoting.Messaging;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace TinyPlatformer
{
	/// <summary>
	/// Represents a moving rectangle, keeps its own data and updates itself.
	/// </summary>
	public class Actor
	{
		public Actor(BlockType[,] blocks)
		{
			this.blocks = blocks;
			colorSprite = new FilledRect(new Rectangle(
				0.0f, 0.0f, Constants.BlockSize, Constants.BlockSize), Color.White);
			accel = maxVelocityX/Constants.Accel;
			friction = maxVelocityX/Constants.Friction;
		}

		public string Type
		{
			get { return type; }
			set
			{
				type = value;
				if (value == "player")
					colorSprite.Color = Color.Green;
				else if (value == "monster")
					colorSprite.Color = Color.CornflowerBlue;
				else if (value == "treasure")
					colorSprite.Color = Color.White;
			}
		}

		public float maxVelocityX = Constants.Meter*Constants.MaxXSpeed;
		public float maxVelocityY = Constants.Meter*Constants.MaxYSpeed;
		public bool Left = false;
		public bool Right = false;
		public bool Jump = false;

		public void Update(float deltaTime)
		{
			bool isFalling = falling;
			var wasleft = velocity.X < 0;
			var wasright = velocity.X > 0;
			var localFriction = friction*(isFalling ? 0.5f : 1.0f);
			var localAccel = accel*(isFalling ? 0.5f : 1.0f);

			outOfTileOffsetX = (int) (position.X%Constants.TileSize);
			outOfTileOffsetY = (int) (position.Y%Constants.TileSize);

			acceleration = new Vector(0.0f, gravity, 0.0f);

			if (Left)
				acceleration.X -= localAccel;
			else if (wasleft)
				acceleration.X += localFriction;

			if (Right)
				acceleration.X += localAccel;
			else if (wasright)
				acceleration.X -= localFriction;

			if (Jump && !jumping && !isFalling)
			{
				acceleration.Y -= impulse;
				jumping = true;
			}
			CalculateActorPhysics(deltaTime);

			if ((wasleft && (velocity.X > 0)) || (wasright && (velocity.X < 0)))
				velocity.X = 0.0f;

			if (velocity.Y > 0)
			{
				if (HasCell_DownOrDiagonal_NoRight())
					Vertical_Stop();
			}
			else if (velocity.Y < 0)
			{
				if (HasCell_Right_NoDownOrDiag())
					Vertical_Adjust();
			}

			if (velocity.X > 0)
			{
				if (HasCell_RightOrDiag_NoDown())
					Horizontal_StopOnRightEdge();
			}
			else if (velocity.X < 0)
			{
				if (HasCell_Down_NoRightOrDiag())
					Horizontal_StopOnLeftEdge();
			}

			if (Type == "monster")
			{
				if (Left && (IsCell(Cell) || !IsCell(CellDown)))
				{
					Left = false;
					Right = true;
				}
				else if (Right && (IsCell(CellRight) || !IsCell(CellDiag)))
				{
					Right = false;
					Left = true;
				}
			}

			falling = !(IsCell(CellDown) ||
			            ((outOfTileOffsetX != 0) && IsCell(CellDiag)));
		}

		private bool HasCell_DownOrDiagonal_NoRight()
		{
			return (IsCell(CellDown) && !IsCell(Cell)) ||
			       (IsCell(CellDiag) && !IsCell(CellRight) && (outOfTileOffsetX != 0));
		}

		private void Vertical_Stop()
		{
			position.Y = TileY*Constants.TileSize;
			velocity.Y = 0.0f;
			falling = false;
			jumping = false;
			outOfTileOffsetY = 0;
		}

		private bool HasCell_Right_NoDownOrDiag()
		{
			return (IsCell(Cell) && !IsCell(CellDown)) ||
			       (IsCell(CellRight) && !IsCell(CellDiag) && (outOfTileOffsetX != 0));
		}

		private void Vertical_Adjust()
		{
			position.Y = (TileY + 1)*Constants.TileSize;
			velocity.Y = 0;
			Cell = CellDown;
			CellRight = CellDiag;
			outOfTileOffsetY = 0;
		}

		private bool HasCell_RightOrDiag_NoDown()
		{
			return (IsCell(CellRight) && !IsCell(Cell)) ||
			       (IsCell(CellDiag) && !IsCell(CellDown) && outOfTileOffsetY != 0);
		}

		private void Horizontal_StopOnRightEdge()
		{
			position.X = TileX*Constants.TileSize;
			velocity.X = 0;
		}

		private bool HasCell_Down_NoRightOrDiag()
		{
			return (IsCell(Cell) && !IsCell(CellRight)) ||
			       (IsCell(CellDown) && !IsCell(CellDiag) && outOfTileOffsetY != 0);
		}

		private void Horizontal_StopOnLeftEdge()
		{
			position.X = (TileX + 1)*Constants.TileSize;
			velocity.X = 0;
		}

		private void CalculateActorPhysics(float deltaTime)
		{
			position.X += deltaTime*velocity.X;
			position.Y += deltaTime*velocity.Y;
			velocity.X += deltaTime*acceleration.X;
			velocity.Y += deltaTime*acceleration.Y;
			velocity.X = (velocity.X < -maxVelocityX)
				? -maxVelocityX
				: (velocity.X > maxVelocityX) ? maxVelocityX : velocity.X;
			velocity.Y = (velocity.Y < -maxVelocityY)
				? -maxVelocityY
				: (velocity.Y > maxVelocityY) ? maxVelocityY : velocity.Y;
		}

		public void Render(float deltaTime)
		{
			var finalX = position.X + (velocity.X*deltaTime);
			var finalY = position.Y + (velocity.Y*deltaTime);
			colorSprite.TopLeft = new Point(
				Constants.ScreenGap.Width + finalX*XPixelAdjust,
				Constants.ScreenGap.Height + finalY*YPixelAdjust);
		}

		private static bool IsCell(BlockType cell)
		{
			return cell != BlockType.None;
		}

		private int TileX
		{
			get { return (int) Math.Floor(position.X/Constants.TileSize); }
		}

		private int TileY
		{
			get { return (int) Math.Floor(position.Y/Constants.TileSize); }
		}

		public BlockType Cell
		{
			get { return blocks[TileX, TileY]; }
			set { blocks[TileX, TileY] = value; }
		}

		public BlockType CellRight
		{
			get { return blocks[TileX + 1, TileY]; }
			set { blocks[TileX + 1, TileY] = value; }
		}

		public BlockType CellDown
		{
			get { return blocks[TileX, TileY + 1]; }
			set { blocks[TileX, TileY + 1] = value; }
		}

		public BlockType CellDiag
		{
			get { return blocks[TileX + 1, TileY + 1]; }
			set { blocks[TileX + 1, TileY + 1] = value; }
		}

		private string type;
		public Point position = new Point(0.0f, 0.0f);
		public Vector velocity = new Vector(0.0f, 0.0f, 0.0f);
		private float gravity = Constants.Meter * Constants.Gravity;
		private float accel;
		private float friction;
		private FilledRect colorSprite;
		private const float XPixelAdjust = 0.8f / (64.0f * Constants.TileSize);
		private const float YPixelAdjust = 0.6f / (48.0f * Constants.TileSize);
		private int outOfTileOffsetX;
		private int outOfTileOffsetY;
		private readonly BlockType[,] blocks;
		private Vector acceleration;
		private bool falling;
		private bool jumping;
		private float impulse = Constants.Meter*Constants.Impulse;
	}
}