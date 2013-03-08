namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// Loads Xml data via the Content system
	/// </summary>
	public abstract class XmlContent : ContentData
	{
		protected XmlContent(string contentFilename)
			: base(contentFilename) {}

		public virtual XmlData XmlData { get; set; }

		public override void Dispose() {}
	}
}