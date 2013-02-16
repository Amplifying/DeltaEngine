using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Input.Windows
{
	internal static class MouseButtonStateHelper
	{
		private static readonly int[] LeftButtonIds =
		{
			0x0201, 0x0202, 0x0203, 0x00A1, 0x00A2, 0x00A3
		};

		private static readonly int[] RightButtonIds =
		{
			0x0204, 0x0205, 0x0206, 0x00A4, 0x00A5, 0x00A6
		};

		private static readonly int[] MiddleButtonIds =
		{
			0x0207, 0x0208, 0x0209, 0x00A7, 0x00A8, 0x00A9
		};

		private static readonly int[] DownButtonIds =
		{
			0x0201, 0x0203, 0x0204, 0x0206, 0x0207, 0x0209, 
			0x020B, 0x020D, 0x00A1, 0x00A3, 0x00A4, 0x00A6, 
			0x00A7, 0x00A9, 0x00AB, 0x00AD
		};

		private static readonly int[] UpButtonIds =
		{
			0x0202, 0x0205, 0x0208, 0x020C, 0x00A2, 0x00A5, 
			0x00A8, 0x00AC
		};

		internal static bool IsPressed(int wParam)
		{
			return IsAnyId(wParam, DownButtonIds);
		}

		internal static bool IsReleased(int wParam)
		{
			return IsAnyId(wParam, UpButtonIds);
		}

		internal static MouseButton GetMessageButton(int intParam, int mouseData)
		{
			if (IsAnyId(intParam, LeftButtonIds))
				return MouseButton.Left;
			if (IsAnyId(intParam, RightButtonIds))
				return MouseButton.Right;
			if (IsAnyId(intParam, MiddleButtonIds))
				return MouseButton.Middle;

			return mouseData == 65536 ? MouseButton.X1 : MouseButton.X2;
		}

		private static bool IsAnyId(int value, IEnumerable<int> ids)
		{
			return ids.Any(id => id == value);
		}
	}
}