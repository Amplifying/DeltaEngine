using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DeltaNinja.Entities;
using DeltaNinja.Pages;

namespace DeltaNinja
{
	class Match : Entity2D
	{
		public event EventHandler<GameOverEventArgs> GameEnded;

		public Match(ScreenSpace screen, InputCommands input, NumberFactory numberFactory, LogoFactory logoFactory) : base (Rectangle.Zero)
		{
			this.content = content;
			this.screen = screen;
			this.input = input;
			this.logoFactory = logoFactory;

			hud = new HudScene(screen, numberFactory);
			pause = new PausePage(screen, input);

			logoSet = new List<Logo>();
			Slice = new Slice();
			PointsTips = new List<PointsTip>();
			ErrorFlags = new List<ErrorFlag>();
			
			HideScore();
			
			screen.ViewportSizeChanged += () => RefreshSize();
			RefreshSize();
		}

		private readonly ContentLoader content;
		private readonly ScreenSpace screen;
		private readonly InputCommands input;
		private readonly LogoFactory logoFactory;

		private readonly HudScene hud;
		private readonly PausePage pause;

		private List<Logo> logoSet;
		public readonly Slice Slice;
		private Score score;
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

		public bool IsPaused { get; private set; }
		
		public void Start()
		{
			Slice.Reset();

			mouseMovement = input.AddMouseMovement(mouse => CheckMouse(mouse));
			mouseLeftClick = input.Add(MouseButton.Left, State.Pressing, mouse => StartSlice(mouse));

			score = new Score();

			hud.Reset();
			hud.Show();
			
			IsActive = true;
			Start<GameLogicsHandler>();

			IsPaused = false;
			pauseCommand = input.Add(Key.Space, x => SwitchPause());
			pauseMouseCommand = input.Add(MouseButton.Right, x => SwitchPause());
		}

		private Command pauseCommand;
		private Command pauseMouseCommand;

		private void Reset()
		{
			IsActive = false;

			input.Remove(pauseCommand);
			input.Remove(pauseMouseCommand);

			if (mouseMovement != null)
			{
				input.Remove(mouseMovement);
				mouseMovement = null;
			}

			if (mouseLeftClick != null)
			{
				input.Remove(mouseLeftClick);
				mouseLeftClick = null;
			}

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
		}

		private void EndGame(bool abort)
		{
			Reset();

			var handler = GameEnded;
			
			if (handler != null)
			{
				if(abort)
					handler(this, null);
				else
					handler(this, new GameOverEventArgs(score));
			}				
		}

		public void HideScore()
		{
			hud.Hide();
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
			if (IsPaused) return;
			Slice.Check(mouse.Position, logoSet);
		}

		private void StartSlice(Mouse mouse)
		{
			if (IsPaused) return;
			Slice.Switch(mouse.Position);
		}

		private void RefreshSize()
		{
			var view = screen.Viewport;

			//pointsNumber.Top = view.Top;
			//levelBox.Top = view.Top;
			//errorFlag.Top = view.Top;
		}

		public void AddOneMoreSlice()
		{
			score.Count += 1;
		}

		public bool AddError()
		{
			score.Errors += 1;
			hud.SetError(score.Errors);

			if (score.Errors < 3) return true;

			EndGame(false);
			return false;
		}

		public void AddPoints(int points)
		{
			score.Points += points;
			hud.SetPoints(score.Points);
		}

		public int CurrentLevel { get { return score.Level;  } }

		public void NextLevel()
		{
			score.Level += 1;
			hud.SetLevel(score.Level);
		}

		public void ShowError(float left, float width)
		{
			ErrorFlags.Add(new ErrorFlag(left, width, screen.Viewport.Bottom));			
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

		private void SwitchPause()
		{
			IsPaused = !IsPaused;

			foreach (var logo in logoSet) logo.SetPause(IsPaused);

			if (IsPaused)
			{
				Slice.Reset();
				pause.Show();
				pause.ButtonClicked += OnPauseButtonClicked;
			}
			else
			{
				pause.ButtonClicked -= OnPauseButtonClicked;
				pause.Hide();
			}				
		}

		protected void OnPauseButtonClicked(MenuButton code)
		{
			switch (code)
			{
				case (MenuButton.Resume):
					SwitchPause();
					break;

				case (MenuButton.NewGame):
					SwitchPause();
					Reset();
					Start();
					break;
					
				case (MenuButton.Abort):
					SwitchPause();
					EndGame(true);
					break;
			}
		}
	}
}
