using System;

namespace DeltaEngine.Datatypes
{
	public class UnknownBinaryDataTypeException : Exception
	{
		public UnknownBinaryDataTypeException(string message)
			: base(message) { }
	}
}