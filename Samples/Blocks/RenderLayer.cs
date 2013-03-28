namespace Blocks
{
	/// <summary>
	/// The various rendering layers. Higher layers overdraw lower ones 
	/// </summary>
	public enum RenderLayer  
	{
		Background = 0,
		Foreground = 1,
		Grid = 2,
		FallingBrick = 3,
		ZoomingBrick = 4
	}
}