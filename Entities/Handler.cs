namespace DeltaEngine.Entities
{
	/// <summary>
	/// The base class for Behaviors and EventListeners. Do not derive directly from this class.
	/// </summary>
	public abstract class Handler
	{
		public virtual Priority Priority
		{
			get { return Priority.Normal; }
		}
	}
}
