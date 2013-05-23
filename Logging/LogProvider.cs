namespace DeltaEngine.Logging
{
	/// <summary>
	/// Derived classes specify how logging is implemented.
	/// </summary>
	public interface LogProvider
	{
		void Log(Info info);
		void Log(Warning warning);
		void Log(Error error);
	}
}