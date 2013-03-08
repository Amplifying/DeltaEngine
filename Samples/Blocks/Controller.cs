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
	public class Controller : Runner<Time, Renderer>
	{
		public Controller(Grid grid, Soundbank soundbank, BlocksContent content)
		{
			this.grid = grid;
			this.soundbank = soundbank;
			this.content = content;
		}

		private readonly Grid grid;
		protected readonly Soundbank soundbank;
		private readonly BlocksContent content;

		public void Run(Time time, Renderer renderer)
		{
			if (FallingBlock == null || !FallingBlock.IsVisible)
				GetNewFallingBlock(renderer);

			MoveFallingBlock(time);
		}

		public Block FallingBlock { get; protected set; }

		private void MoveFallingBlock(Time time)
		{
			float top = FallingBlock.Top;
			FallingBlock.Run(time, FallSpeed);
			if (grid.IsValidPosition(FallingBlock))
				return;

			FallingBlock.Top = top;
			FallingBlock.Settle(time, FallSpeed);
		}

		public float FallSpeed
		{
			get { return IsFallingFast ? FastFallSpeed : SlowFallSpeed; }
		}

		public bool IsFallingFast { get; set; }

		public float SlowFallSpeed
		{
			get { return BaseSpeed + totalRowsRemoved * SpeedUpPerRowRemoved; }
		}

		internal const float BaseSpeed = 2.0f;
		private int totalRowsRemoved;
		private const float SpeedUpPerRowRemoved = 0.2f;
		private const float FastFallSpeed = 16.0f;

		private void GetNewFallingBlock(Renderer renderer)
		{
			if (UpcomingBlock == null)
				CreateUpcomingBlock(renderer);

			FallingBlock = UpcomingBlock;
			FallingBlock.Affix += AffixBlock;
			CreateUpcomingBlock(renderer);

			while (IsABrickOnTopRowOrIsNoRoomForNextBlock())
				GameLost();
		}

		private void CreateUpcomingBlock(Renderer renderer)
		{
			UpcomingBlock = new Block(content, grid.Random, Point.Zero);
			renderer.Add(UpcomingBlock);
			UpcomingBlock.Left = UpcomingBlockCenter.X - UpcomingBlock.Center.X;
			UpcomingBlock.Top = UpcomingBlockCenter.Y - UpcomingBlock.Center.Y;
		}

		public Block UpcomingBlock { get; protected set; }
		private static readonly Point UpcomingBlockCenter = new Point(9, -4);

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

		private void GameLost()
		{
			soundbank.GameLost.Play();
			grid.Clear();
			totalRowsRemoved = 0;
			if (Lose != null)
				Lose();
		}

		public event Action Lose;

		protected void AffixBlock()
		{
			int rowsRemoved = grid.AffixBlock(FallingBlock);
			FallingBlock.Dispose();
			totalRowsRemoved += rowsRemoved;
			PlayBlockAffixedSound(rowsRemoved);
			if (AddToScore != null)
				AddToScore(RowRemovedBonus * rowsRemoved * rowsRemoved + BlockPlacedBonus);
		}

		private void PlayBlockAffixedSound(int rowsRemoved)
		{
			if (rowsRemoved == 0)
				soundbank.BlockAffixed.Play();
			else if (rowsRemoved == 1)
				soundbank.RowRemoved.Play();
			else
				soundbank.MultipleRowsRemoved.Play();
		}

		public event Action<int> AddToScore;
		private const int RowRemovedBonus = 10;
		private const int BlockPlacedBonus = 1;

		public void MoveBlockLeftIfPossible()
		{
			FallingBlock.Left--;
			if (grid.IsValidPosition(FallingBlock))
			{
				soundbank.BlockMoved.Play();
				return;
			}

			FallingBlock.Left++;
			soundbank.BlockCouldntMove.Play();
		}

		public void MoveBlockRightIfPossible()
		{
			FallingBlock.Left++;
			if (grid.IsValidPosition(FallingBlock))
			{
				soundbank.BlockMoved.Play();
				return;
			}

			FallingBlock.Left--;
			soundbank.BlockCouldntMove.Play();
		}

		public void RotateBlockAntiClockwiseIfPossible()
		{
			FallingBlock.RotateAntiClockwise();
			if (grid.IsValidPosition(FallingBlock))
			{
				soundbank.BlockMoved.Play();
				return;
			}

			FallingBlock.RotateAntiClockwise();
			soundbank.BlockCouldntMove.Play();
		}
	}
}