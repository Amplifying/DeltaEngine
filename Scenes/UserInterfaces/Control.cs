using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Base for all UI controls
	/// </summary>
	public abstract class Control : BinaryData, IDisposable
	{
		public virtual void SaveData(BinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(IsVisible);
			writer.Write(RenderLayer);
		}

		public string Name { get; set; }
		public abstract bool IsVisible { get; set; }
		public abstract int RenderLayer { get; set; }

		public virtual void LoadData(BinaryReader reader)
		{
			Name = reader.ReadString();
			IsVisible = reader.ReadBoolean();
			RenderLayer = reader.ReadInt32();
		}

		internal abstract void LoadContent(Content content);
		internal abstract void Show(Renderer renderer);
		internal abstract void Hide(Renderer renderer);
		public abstract void Dispose();
	}
}