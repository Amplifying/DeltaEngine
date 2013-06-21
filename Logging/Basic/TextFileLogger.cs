namespace DeltaEngine.Logging.Basic
{
	public class TextFileLogger : Logger
	{
		public TextFileLogger()
			: base(new TextFileLogProvider()) {}
	}
}