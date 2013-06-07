using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DeltaNinja
{
	class Match : Entity
	{
		public event EventHandler<GameOverEventArgs> GameEnded;

		public Match(ContentLoader content, ScreenSpace screen, InputCommands input, NumberFactory numberFactory, LogoFactory logoFactory)
		{
			this.content = content;
			this.screen = screen;
			this.input = input;
			this.logoFactory = logoFactory;

			var view = screen.Viewport;
			var center = view.Width / 2f;

			logoSet = new List<Logo>();
			Slice = new Slice();
			PointsTips = new List<PointsTip>();
			ErrorFlags = new List<ErrorFlag>();
			
			pointsNumber = numberFactory.CreateNumber(view.Left, view.Top, 0.05f, Alignment.Left, 0);
			levelBox = new LevelBox(content, numberFactory, center, view.Top);

			errorFlag = new ErrorsBox(content, view.Right, view.Top);

			HideScore();
			
			screen.ViewportSizeChanged += () => RefreshSize();
			RefreshSize();
		}

		private readonly ContentLoader content;
		private readonly ScreenSpace screen;
		private readonly InputCommands input;
		private readonly LogoFactory logoFactory;

		private List<Logo> logoSet;
		public readonly Slice Slice;
		private Score score;
		private Number pointsNumber;
		private LevelBox levelBox;
		private ErrorsBox errorFlag;
		public readonly List<PointsTip> PointsTips;
		public readonly List<ErrorFlag> ErrorFlags;

		private Command mouseMovement;
		private Command mouseLeftClick;

		public int LogoCount
		{
			get { return logoSet.Count(x => x.IsActive); }
		}

		public IEnumerable<Logo> LogoArray
		{
			get { return logoSet.ToArray(); }
		}

		//public bool IsActive { get; private set; }

		public void Start()
		{
			Slice.Reset();

			mouseMovement = input.AddMouseMovement(mouse => CheckMouse(mouse));
			mouseLeftClick = input.Add(MouseButton.Left, State.Pressing, mouse => StartSlice(mouse));

			score = new Score();

			pointsNumber.Show();
			levelBox.Show();
			errorFlag.Show();

			IsActive = true;
			Add<GameLogicsHandler>();
		}

		private void End()
		{
			IsActive = false;

			if (mouseMovement != null) input.Remove(mouseMovement);
			if (mouseLeftClick != null) input.Remove(mouseLeftClick);

			foreach (var logo in logoSet)
				logo.IsActive = false;
			logoSet.Clear();

			Slice.Reset();

			foreach (var logo in logoSet)
				logo.ResetSlicing();

			foreach (var tip in PointsTips)
				tip.Reset();

			foreach (var flag in ErrorFlags)
				flag.Reset();

			levelBox.Hide();

			var handler = GameEnded;
			
			if (handler != null)
			{
				handler(this, new GameOverEventArgs(score));
			}
		}

		public void HideScore()
		{
			pointsNumber.Hide();
			levelBox.Hide();
			errorFlag.Hide();
		}

		public void CreateLogos(int count)
		{
			for (int i = 0; i < count; i++)
				logoSet.Add(logoFactory.Create());
		}

		public void RemoveLogo(Logo logo)
		{
			logoSet.Remove(logo);
		}

		private void CheckMouse(Mouse mouse)
		{
			Slice.Check(mouse.Position, logoSet);
		}

		private void StartSlice(Mouse mouse)
		{
			Slice.Switch(mouse.Position);
		}

		private void RefreshSize()
		{
			var view = screen.Viewport;

			pointsNumber.Top = view.Top;
			levelBox.Top = view.Top;
			errorFlag.Top = view.Top;
		}

		public void AddOneMoreSlice()
		{
			score.Count += 1;
		}

		public bool AddError()
		{
			score.Errors += 1;
			errorFlag.SetError(score.Errors);

			if (score.Errors < 3) return true;

			End();
			return false;
		}

		public void AddPoints(int points)
		{
			score.Points += points;
			pointsNumber.SetValue(score.Points);
		}

		public int CurrentLevel { get { return score.Level;  } }

		public void NextLevel()
		{
			score.Level += 1;
			levelBox.Value = score.Level;
		}

		public void ShowError(float left, float width)
		{
			ErrorFlags.Add(new ErrorFlag(content, left, width, screen.Viewport.Bottom));			
		}

		public void ClearEntities()
		{			
			var time = Time.Current;

			foreach (var tip in PointsTips.ToArray())
			{
				if (tip.Time + 1500 < time.Milliseconds)
				{
					tip.Reset();
					PointsTips.Remove(tip);
				}
			}
			
			foreach (var flag in ErrorFlags.ToArray())
			{
				if (flag.Time + 1500 < time.Milliseconds)
				{
					flag.IsActive = false;
					ErrorFlags.Remove(flag);
				}
			}
		}
	}
}
