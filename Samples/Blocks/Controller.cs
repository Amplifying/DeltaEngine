using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Handles the falling and upcoming blocks.
	/// </summary>
	public class Controller : Runner<ModdableContent, Renderer, Time>
	{
		public Controller(Grid grid, SoundManager soundManager)
		{
			this.grid = grid;
			this.soundManager = soundManager;
		}

		private readonly Grid grid;
		protected readonly SoundManager soundManager;

		public void Run(ModdableContent content, Renderer renderer, Time time)
		{
			if (FallingBlock == null)
				GetNewFallingBlock(content, renderer);
			else
				FallingBlock.Run(time, fallSpeed + LevelSpeed);

			if (!grid.IsValidPosition(FallingBlock))
				AffixBlock(content, renderer);
		}

		public Block FallingBlock { get; protected set; }
		private float fallSpeed = SlowFallSpeed;
		private const float SlowFallSpeed = 2.0f;
		private const float FastFallSpeed = 8.0f;

		public float LevelSpeed
		{
			get { return totalRowsRemoved * SpeedUpPerRowRemoved; }
		}

		private int totalRowsRemoved;
		private const float SpeedUpPerRowRemoved = 0.1f;

		private void GetNewFallingBlock(Content content, Renderer renderer)
		{
			if (UpcomingBlock == null)
				renderer.Add(UpcomingBlock = new Block(content, grid.Random, UpcomingBlockPosition));

			if (FallingBlock != null)
				renderer.Remove(FallingBlock);

			FallingBlock = UpcomingBlock;
			renderer.Add(UpcomingBlock = new Block(content, grid.Random, UpcomingBlockPosition));

			while (IsABrickOnTopRowOrIsNoRoomForNextBlock())
				Lose();
		}

		public Block UpcomingBlock { get; protected set; }
		public static readonly Point UpcomingBlockPosition = new Point(14, 4);

		private bool IsABrickOnTopRowOrIsNoRoomForNextBlock()
		{
			return grid.IsABrickOnFirstRow() || !TryToPlaceFallingBlockOnGrid();
		}

		private bool TryToPlaceFallingBlockOnGrid()
		{
			List<int> validStartingPositions = grid.GetValidStartingColumns(FallingBlock);
			if (validStartingPositions.Count == 0)
				return false;

			int column = grid.Random.Get(0, validStartingPositions.Count);
			FallingBlock.Left = validStartingPositions[column];
			return true;
		}

		private void Lose()
		{
			soundManager.GameLost.Play();
			grid.Clear();
			totalRowsRemoved = 0;
			if (Lost != null)
				Lost();
		}

		public event Action Lost;

		private void AffixBlock(Content content, Renderer renderer)
		{
			int rowsRemoved = grid.AffixBlock(FallingBlock);
			totalRowsRemoved += rowsRemoved;
			PlayBlockAffixedSound(rowsRemoved);
			if (ScorePoints != null)
				ScorePoints(RowRemovedBonus * rowsRemoved * rowsRemoved + BlockPlacedBonus);

			GetNewFallingBlock(content, renderer);
		}

		private void PlayBlockAffixedSound(int rowsRemoved)
		{
			if (rowsRemoved == 0)
				soundManager.BlockAffixed.Play();
			else if (rowsRemoved==1)
				soundManager.RowRemoved.Play();
			else
				soundManager.RowsRemoved.Play();
		}

		public event Action<int> ScorePoints;
		public const int RowRemovedBonus = 10;
		public const int BlockPlacedBonus = 1;

		public void TryToMoveBlockLeft()
		{
			FallingBlock.Left--;
			if (grid.IsValidPosition(FallingBlock))
			{
				soundManager.BlockMoved.Play();
				return;
			}

			FallingBlock.Left++;
			soundManager.BlockCouldntMove.Play();
		}

		public void TryToMoveBlockRight()
		{
			FallingBlock.Left++;
			if (grid.IsValidPosition(FallingBlock))
			{
				soundManager.BlockMoved.Play();
				return;
			}

			FallingBlock.Left--;
			soundManager.BlockCouldntMove.Play();
		}

		public void TryToRotateBlockClockwise()
		{
			FallingBlock.RotateClockwise();
			if (grid.IsValidPosition(FallingBlock))
			{
				soundManager.BlockMoved.Play();
				return;
			}

			FallingBlock.RotateAntiClockwise();
			soundManager.BlockCouldntMove.Play();
		}

		public void DropBlockFast()
		{
			fallSpeed = FastFallSpeed;
		}

		public void DropBlockSlow()
		{
			fallSpeed = SlowFallSpeed;
		}
	}
}