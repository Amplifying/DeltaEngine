using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Triggers
{
	internal class CollisionTriggerData
	{
		public CollisionTriggerData(List<string> searchTags, Color triggeredColor, Color defaultColor)
		{
			SearchTags = searchTags;
			TriggeredColor = triggeredColor;
			DefaultColor = defaultColor;
		}

		public CollisionTriggerData(Color triggeredColor, Color defaultColor) :
			this(new List<string>(), triggeredColor, defaultColor){}

		public List<string> SearchTags { get; private set; }
		public Color TriggeredColor { get; private set; }
		public Color DefaultColor { get; private set; }
	}
}