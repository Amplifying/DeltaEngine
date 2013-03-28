using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Represents a grid of bricks: the blocks that have come to rest and not yet been removed
	/// </summary>
	public class Grid
	{
		public Grid(Renderer renderer, BlocksContent content, Randomizer random)
		{
			this.renderer = renderer;
			this.content = content;
			Random = random;
		}

		private readonly Renderer renderer;
		private readonly BlocksContent content;
		public Randomizer Random { get; private set; }

		public int AffixBlock(Block block)
		{
			foreach (Brick brick in block.Bricks)
				if (!IsOccupied(brick))
					AffixBrick(brick);

			RemoveFilledRows();
			return removedRows;
		}

		protected readonly Brick[,] bricks = new Brick[Width,Height];
		internal const int Width = 12;
		internal const int Height = 19;

		private void AffixBrick(Brick brick)
		{
			brick.TopLeft = new Point((int)brick.TopLeft.X, (int)brick.TopLeft.Y + 1);
			bricks[(int)brick.Position.X, (int)brick.Position.Y - 1] = brick;
			renderer.Add(brick);
			renderer.Add(new ZoomingEffect(brick.Image, brick.DrawArea,
				Rectangle.FromCenter(brick.DrawArea.Center, brick.DrawArea.Size * 2), 0.2f)
			{
				Color = brick.Color,
				RenderLayer = (int)RenderLayer.ZoomingBrick
			});
		}

		private void RemoveFilledRows()
		{
			removedRows = 0;

			for (int y = 0; y < Height; y++)
				if (IsRowFilled(y))
					RemoveRow(y);
		}

		private int removedRows;

		private bool IsRowFilled(int y)
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, y] == null)
					return false;

			return true;
		}

		private void RemoveRow(int row)
		{
			for (int x = 0; x < Width; x++)
				RemoveBrick(x, row);

			for (int x = 0; x < Width; x++)
				for (int y = row; y > 0; y--)
					MoveBrickDown(x, y);

			removedRows++;
		}

		private void RemoveBrick(int x, int y)
		{
			var brick = bricks[x, y];
			renderer.Remove(brick);
			bricks[x, y] = null;
			if (content.DoBricksSplitInHalfWhenRowFull)
				AddPairOfFallingBricks(brick);
			else
				AddFallingBrick(brick);
		}

		private void AddPairOfFallingBricks(Brick brick)
		{
			AddTopFallingBrick(brick);
			AddBottomFallingBrick(brick);
		}

		private void AddTopFallingBrick(Sprite brick)
		{
			var filename = content.GetFilenameWithoutPrefix(brick.Image.Filename);
			var image = content.Load<Image>(filename + "_Top");
			renderer.Add(new FallingEffect(image, brick.DrawArea, 5.0f)
			{
				Color = brick.Color,
				Velocity = new Point(Random.Get(-0.5f, 0.5f), Random.Get(-1.0f, 0.0f)),
				RotationSpeed = Random.Get(-360, 360),
				RenderLayer = (int)RenderLayer.FallingBrick
			});
		}

		private void AddBottomFallingBrick(Sprite brick)
		{
			var filename = content.GetFilenameWithoutPrefix(brick.Image.Filename);
			var image = content.Load<Image>(filename + "_Bottom");
			renderer.Add(new FallingEffect(image, brick.DrawArea, 5.0f)
			{
				Color = brick.Color,
				Velocity = new Point(Random.Get(-0.5f, 0.5f), Random.Get(-1.0f, 0.0f)),
				RotationSpeed = Random.Get(-360, 360),
				RenderLayer = (int)RenderLayer.FallingBrick
			});
		}

		private void AddFallingBrick(Sprite brick)
		{
			renderer.Add(new FallingEffect(brick.Image, brick.DrawArea, 5.0f)
			{
				Color = brick.Color,
				Velocity = new Point(Random.Get(-0.5f, 0.5f), Random.Get(-1.0f, 0.0f)),
				RotationSpeed = Random.Get(-360, 360),
				RenderLayer = (int)RenderLayer.FallingBrick
			});
		}

		private void MoveBrickDown(int x, int y)
		{
			bricks[x, y] = bricks[x, y - 1];
			if (bricks[x, y] != null)
				bricks[x, y].TopLeft.Y++;
		}

		public bool IsValidPosition(Block block)
		{
			foreach (Brick brick in block.Bricks)
				if (IsOutsideTheGrid(brick) || IsOccupied(brick))
					return false;

			return true;
		}

		private static bool IsOutsideTheGrid(Brick brick)
		{
			return brick.Position.X < 0 || brick.Position.X >= Width || brick.Position.Y < 1 ||
				brick.Position.Y >= Height;
		}

		private bool IsOccupied(Brick brick)
		{
			return bricks[(int)brick.Position.X, (int)brick.Position.Y] != null;
		}

		public List<int> GetValidStartingColumns(Block block)
		{
			block.Top = 1;
			List<int> validStartingColumns = content.DoBlocksStartInARandomColumn
				? GetAllValidStartingColumns(block) : GetMiddleColumnIfValid(block);

			return validStartingColumns;
		}

		private List<int> GetAllValidStartingColumns(Block block)
		{
			var validStartingColumns = new List<int>();
			for (int x = 0; x < Width; x++)
				if (IsAValidStartingColumn(block, x))
					validStartingColumns.Add(x);

			return validStartingColumns;
		}

		private bool IsAValidStartingColumn(Block block, int column)
		{
			block.Left = column;
			return IsValidPosition(block);
		}

		private List<int> GetMiddleColumnIfValid(Block block)
		{
			var validStartingColumns = new List<int>();
			if (IsAValidStartingColumn(block, Middle))
				validStartingColumns.Add(Middle - (int)block.Center.X);

			return validStartingColumns;
		}

		private const int Middle = Width / 2;

		public bool IsABrickOnFirstRow()
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, 0] != null)
					return true;

			return false;
		}

		public void Clear()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if (bricks[x, y] != null)
						RemoveBrick(x, y);
		}
	}
}