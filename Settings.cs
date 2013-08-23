﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine
{
	/// <summary>
	/// Keeps a bunch of settings like the resolution, which are used when the application starts up.
	/// </summary>
	public abstract class Settings : IDisposable
	{
		public abstract void Save();
		protected const string SettingsFilename = "Settings.xml";

		public void Dispose()
		{
			if (wasChanged)
				Save();
		}

		protected bool wasChanged;

		public Size Resolution
		{
			get { return GetValue("Resolution", DefaultResolution); }
			set { SetValue("Resolution", value); }
		}

		protected abstract T GetValue<T>(string key, T defaultValue);

		protected static Size DefaultResolution
		{
			get { return ExceptionExtensions.IsDebugMode ? new Size(640, 360) : new Size(1280, 720); }
		}

		protected abstract void SetValue(string key, object value);

		public bool StartInFullscreen
		{
			get { return GetValue("StartInFullscreen", false); }
			set { SetValue("StartInFullscreen", value); }
		}

		public string PlayerName
		{
			get { return GetValue("PlayerName", "Player"); }
			set { SetValue("PlayerName", value); }
		}

		public string TwoLetterLanguageName
		{
			get { return GetValue("Language", "en"); }
			set { SetValue("Language", value); }
		}

		public float SoundVolume
		{
			get { return GetValue("SoundVolume", 1.0f); }
			set { SetValue("SoundVolume", value); }
		}

		public float MusicVolume
		{
			get { return GetValue("MusicVolume", 0.75f); }
			set { SetValue("MusicVolume", value); }
		}

		public int DepthBufferBits
		{
			get { return GetValue("DepthBufferBits", 24); }
			set { SetValue("DepthBufferBits", value); }
		}

		public int ColorBufferBits
		{
			get { return GetValue("ColorBufferBits", 32); }
			set { SetValue("ColorBufferBits", value); }
		}

		public int AntiAliasingSamples
		{
			get { return GetValue("AntiAliasingSamples", 4); }
			set { SetValue("AntiAliasingSamples", value); }
		}

		public int LimitFramerate
		{
			get { return GetValue("LimitFramerate", 0); }
			set { SetValue("LimitFramerate", value); }
		}

		public int UpdatesPerSecond
		{
			get { return GetValue("UpdatesPerSecond", DefaultUpdatesPerSecond); }
			set { SetValue("UpdatesPerSecond", value); }
		}

		public const int DefaultUpdatesPerSecond = 20;

		public int RapidUpdatesPerSecond
		{
			get { return GetValue("RapidUpdatesPerSecond", 60); }
			set { SetValue("RapidUpdatesPerSecond", value); }
		}

		public ProfilingMode ProfilingModes
		{
			get { return GetValue("ProfilingModes", ProfilingMode.None); }
			set { SetValue("ProfilingModes", value); }
		}
	}
}