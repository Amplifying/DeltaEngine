using System;
using System.Threading;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Keeps a bunch of settings like the resolution, which are used when the application starts up.
	/// </summary>
	public abstract class Settings : IDisposable
	{
		public void Dispose()
		{
			if (wasChanged)
				Save();
		}

		public abstract void Save();

		protected void SetFallbackSettings()
		{
			data = new XmlData("Settings");
			data.AddChild("Resolution", DefaultResolution);
			data.AddChild("StartInFullscreen", false);
			data.AddChild("PlayerName", Environment.UserName);
			data.AddChild("Language", Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
			data.AddChild("SoundVolume", 1.0f);
			data.AddChild("MusicVolume", 0.75f);
			data.AddChild("DepthBufferBits", 24);
			data.AddChild("ColorBufferBits", 32);
			data.AddChild("AntiAliasingSamples", 4);
			data.AddChild("LimitFramerate", 0);
			data.AddChild("UpdatesPerSecond", 5);
			data.AddChild("ContentServerIp", "deltaengine.net");
			data.AddChild("ContentServerPort", "800");
			data.AddChild("LogServerIp", "deltaengine.net");
			data.AddChild("LogServerPort", "777");
			data.AddChild("ProfilingModes", ProfilingMode.None);
			wasChanged = true;
		}

		protected const string SettingsFilename = "Settings.xml";
		protected XmlData data;
		private bool wasChanged;

		private static Size DefaultResolution
		{
			get { return ExceptionExtensions.IsDebugMode ? new Size(640, 360) : new Size(1920, 1080); }
		}

		public Size Resolution
		{
			get { return data.GetChildValue("Resolution", DefaultResolution); }
			set
			{
				SetValue("Resolution", value);
			}
		}

		private void SetValue(string key, object value)
		{
			data.GetChild(key).Value = StringExtensions.ToInvariantString(value);
			wasChanged = true;
		}

		public bool StartInFullscreen
		{
			get { return data.GetChildValue("StartInFullscreen", false); }
			set
			{
				SetValue("StartInFullscreen", value);
			}
		}

		public string PlayerName
		{
			get { return data.GetChildValue("PlayerName", "Player"); }
			set
			{
				SetValue("PlayerName", value);
			}
		}

		public string TwoLetterLanguageName
		{
			get { return data.GetChildValue("Language", "en"); }
			set
			{
				SetValue("Language", value);
			}
		}

		public float SoundVolume
		{
			get { return data.GetChildValue("SoundVolume", 1.0f); }
			set
			{
				SetValue("SoundVolume", value);
			}
		}

		public float MusicVolume
		{
			get { return data.GetChildValue("MusicVolume", 0.75f); }
			set
			{
				SetValue("MusicVolume", value);
			}
		}
		
		public int DepthBufferBits
		{
			get { return data.GetChildValue("DepthBufferBits", 24); }
			set
			{
				SetValue("DepthBufferBits", value);
			}
		}

		public int ColorBufferBits
		{
			get { return data.GetChildValue("ColorBufferBits", 32); }
			set
			{
				SetValue("ColorBufferBits", value);
			}
		}

		public int AntiAliasingSamples
		{
			get { return data.GetChildValue("AntiAliasingSamples", 4); }
			set
			{
				SetValue("AntiAliasingSamples", value);
			}
		}
		
		public int LimitFramerate
		{
			get { return data.GetChildValue("LimitFramerate", 0); }
			set
			{
				SetValue("LimitFramerate", value);
			}
		}

		public int UpdatesPerSecond
		{
			get { return data.GetChildValue("UpdatesPerSecond", 5); }
			set
			{
				SetValue("UpdatesPerSecond", value);
			}
		}

		public string ContentServerIp
		{
			get { return data.GetChildValue("ContentServerIp", "deltaengine.net"); }
			set
			{
				SetValue("ContentServerIp", value);
			}
		}

		public int ContentServerPort
		{
			get { return data.GetChildValue("ContentServerPort", 800); }
			set
			{
				SetValue("ContentServerPort", value);
			}
		}

		public string LogServerIp
		{
			get { return data.GetChildValue("LogServerIp", "deltaengine.net"); }
			set
			{
				SetValue("LogServerIp", value);
			}
		}

		public int LogServerPort
		{
			get { return data.GetChildValue("LogServerPort", 777); }
			set
			{
				SetValue("LogServerPort", value);
			}
		}

		public ProfilingMode ProfilingModes
		{
			get { return data.GetChildValue("ProfilingModes", ProfilingMode.None); }
		}
	}
}