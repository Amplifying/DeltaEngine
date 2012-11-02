using System;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Windows Xna config (graphics, sound, input) for any Delta Engine application or test.
	/// </summary>
	public class XnaResolver : WindowsResolver
	{
		public XnaResolver()
		{
			RegisterSingleton<XnaGame>();
			RegisterSingleton<XnaWindow>();
			RegisterSingleton<XnaDevice>();
			RegisterSingleton<XnaDrawing>();
		}
		
		public override void Run(Action runCode = null)
		{
			Resolve<XnaGame>().Run(runCode);
		}
	}
}