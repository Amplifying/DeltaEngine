using System;
using System.Collections.Generic;
using Blobs.Creatures;
using DeltaEngine.Content;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs.Levels
{
	/// <summary>
	/// Base for all Blob levels
	/// </summary>
	public abstract class Level : IDisposable
	{
		protected Level(ScreenSpace screen, InputCommands input, ContentLoader content)
		{
			this.screen = screen;
			camera = (Camera2DControlledQuadraticScreenSpace)screen;
			this.input = input;
			vectorData = content.Load<XmlContent>("VectorText");
			Blobs = new List<Blob>();
		}

		private readonly ScreenSpace screen;
		protected readonly Camera2DControlledQuadraticScreenSpace camera;
		protected readonly InputCommands input;
		protected readonly XmlContent vectorData;
		private List<Blob> Blobs { get; set; }

		public virtual void Reset()
		{
			CreatePlayer();
			PositionPlayer();
			AddEnemies();
			AddPlatforms();
			AddText();
			SetupEvents();
			PositionCamera();
			elapsed = 0.0f;
		}

		protected float elapsed;

		private void CreatePlayer()
		{
			Player = new Player(screen, input) { Radius = 0.05f, Color = Color.Blue };
			Blobs.Add(Player);
		}

		protected Player Player { get; private set; }

		protected abstract void PositionPlayer();
		protected abstract void AddEnemies();
		protected abstract void AddPlatforms();
		protected abstract void AddText();
		protected abstract void SetupEvents();
		protected abstract void PositionCamera();

		protected void AddPlatform(Rectangle rectangle, float rotation, Color color)
		{
			var platform = new Platform(rectangle, rotation) { Color = color };
			platforms.Add(platform);
		}

		private readonly List<Platform> platforms = new List<Platform>();

		protected void AddEnemy(Point center, float radius)
		{
			var enemy = new Blob(screen, input) { Center = center, Radius = radius };
			enemy.UpdateColor(Player.Radius);
			Blobs.Add(enemy);
		}

		public virtual void Run()
		{
			for (int i = Blobs.Count - 1; i >= 0 && Blobs.Count > 0; i--)
				Blobs[i].Run();

			foreach (Platform platform in platforms)
				platform.Run();

			RemoveDeadBlobs();
			CheckForCollisionsWithPlatforms();
			CheckForCollisionsBetweenBlobs();
		}

		private void RemoveDeadBlobs()
		{
			for (int i = Blobs.Count - 1; i >= 0; i--)
				if (Blobs[i].HasDied)
					RemoveBlobAndCheckForVictory(Blobs[i]);

			if (Blobs.Count <= 1)
				PassLevel();
		}

		private void RemoveBlobAndCheckForVictory(Blob blob)
		{
			Blobs.Remove(blob);
		}

		protected void PassLevel()
		{
			Dispose();
			if (Passed != null)
				Passed();
		}

		public virtual void Dispose()
		{
			foreach (Platform platform in platforms)
				platform.Dispose();

			foreach (Blob blob in Blobs)
				blob.Dispose();

			platforms.Clear();
			Blobs.Clear();
		}

		public event Action Passed;

		private void CheckForCollisionsWithPlatforms()
		{
			foreach (Blob blob in Blobs)
				if (!blob.IsDying)
					foreach (Platform platform in platforms)
						platform.CheckForCollision(blob);
		}

		private void CheckForCollisionsBetweenBlobs()
		{
			bool isLevelFailed = false;
			for (int i = Blobs.Count - 1; i >= 0; i--)
				if (Blobs[i] != Player && !Blobs[i].IsDying && Player.CheckForCollision(Blobs[i]))
					if (Blobs[i].Radius <= Player.Radius)
						PlayerAbsorbsBlob(Blobs[i]);
					else
						isLevelFailed = true;

			if (isLevelFailed)
				FailLevel();
		}

		private void PlayerAbsorbsBlob(Blob blob)
		{
			Player.Grow(GrowthOnAbsorption);
			blob.Die();
			foreach (Blob b in Blobs)
				if (b != Player)
					b.UpdateColor(Player.Radius + GrowthOnAbsorption);
		}

		private const float GrowthOnAbsorption = 0.01f;

		private void FailLevel()
		{
			Dispose();
			if (Failed != null)
				Failed();
		}

		public event Action Failed;
	}
}