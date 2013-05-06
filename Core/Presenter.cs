namespace DeltaEngine.Core
{
	/// <summary>
	/// While Runners execute before app code is run, presenters are run after app code has been
	/// executed at the end of each frame. Presenters are quick and executed in the same thread.
	/// http://DeltaEngine.net/About/CodingStyle#Presenters
	/// </summary>
	public interface Presenter : Runner
	{
		void Present();
	}
}