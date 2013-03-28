using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Holds the individual bricks making up a block and handles rotating them
	/// </summary>
	public class Block : Renderable
	{
		public Block(BlocksContent content, Randomizer random, Point topLeft)
		{
			this.random = random;
			CreateBricks(content);
			Left = topLeft.X;
			Top = topLeft.Y;
			RenderLayer = (int)Blocks.RenderLayer.Grid;
		}

		private readonly Randomizer random;

		private void CreateBricks(BlocksContent content)
		{
			int numberOfBricks = content.AreFiveBrickBlocksAllowed
				? GetNumberOfBricks() : NormalNumberOfBricks;
			var image = content.Load<Image>("Block" + random.Get(1, 8));
			Bricks = new List<Brick> { new Brick(image, Point.Zero) };
			for (int i = 1; i < numberOfBricks; i++)
				AddBrick(Bricks[i - 1], image);

			ShiftToTopLeft();
		}

		private int GetNumberOfBricks()
		{
			return random.Get() < 0.9f ? NormalNumberOfBricks : NormalNumberOfBricks + 1;
		}

		private const int NormalNumberOfBricks = 4;
		public List<Brick> Bricks { get; private set; }

		private void AddBrick(Brick lastBrick, Image image)
		{
			Brick newBrick;
			do
				newBrick = new Brick(image, lastBrick.Offset + GetRandomOffset()); 
			while (Bricks.Any(brick => brick.Offset == newBrick.Offset));

			Bricks.Add(newBrick);
		}

		private Point GetRandomOffset()
		{
			return random.Get(0, 2) == 0
				? new Point(random.Get(0, 2) * 2 - 1, 0) : new Point(0, random.Get(0, 2) * 2 - 1);
		}

		private void ShiftToTopLeft()
		{
			var left = (int)Bricks.Min(brick => brick.Offset.X);
			var top = (int)Bricks.Min(brick => brick.Offset.Y);
			foreach (Brick brick in Bricks)
				brick.Offset = new Point(brick.Offset.X - left, brick.Offset.Y - top);

			UpdateCenter();
		}

		private void UpdateCenter()
		{
			float minX = Bricks.Min(brick => brick.Offset.X);
			float maxX = Bricks.Max(brick => brick.Offset.X);
			float minY = Bricks.Min(brick => brick.Offset.Y);
			float maxY = Bricks.Max(brick => brick.Offset.Y);
			center = new Point((minX + maxX + 1) / 2, (minY + maxY + 1) / 2);
		}

		private Point center;

		public Point Center
		{
			get { return center; }
		}

		public void RotateClockwise()
		{
			Point oldCenter = center;
			foreach (Brick brick in Bricks)
				brick.Offset = new Point(-brick.Offset.Y, brick.Offset.X);

			ShiftToTopLeft();
			Left += (int)oldCenter.X - (int)center.X;
		}

		public void RotateAntiClockwise()
		{
			Point oldCenter = center;
			foreach (Brick brick in Bricks)
				brick.Offset = new Point(brick.Offset.Y, -brick.Offset.X);

			ShiftToTopLeft();
			Left += (int)oldCenter.X - (int)center.X;
		}

		public float Left
		{
			get { return Bricks[0].TopLeft.X; }
			set
			{
				foreach (Brick brick in Bricks)
					brick.TopLeft.X = value;
			}
		}

		public float Top
		{
			get { return Bricks[0].TopLeft.Y; }
			set
			{
				foreach (Brick brick in Bricks)
					brick.TopLeft.Y = value;
			}
		}

		public void Run(Time time, float fallSpeed)
		{
			Top += MathExtensions.Min(fallSpeed * time.CurrentDelta, 1.0f);
		}

		protected override void Render(Renderer renderer, Time time)
		{
			foreach (Brick brick in Bricks)
				brick.Render(renderer);
		}

		public override string ToString()
		{
			string result = "";
			for (int y = 0; y < Bricks.Count; y++)
				result += LineToString(y);

			return result;
		}

		private string LineToString(int y)
		{
			string line = y > 0 ? "/" : "";
			for (int x = 0; x < Bricks.Count; x++)
				line += Bricks.Any(brick => brick.Offset == new Point(x, y)) ? 'O' : '.';

			return line;
		}
	}
}