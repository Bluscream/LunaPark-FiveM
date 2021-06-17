using System;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class StringMeasurer
	{
		[Obsolete("Use Screen.GetTextWidth instead.", true)]
		public static int MeasureString(string input)
		{
			return (int)ScreenTools.GetTextWidth(input, (Font)0, 1f);
		}

		[Obsolete("Use Screen.GetTextWidth instead.", true)]
		public static float MeasureString(string input, Font font, float scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ScreenTools.GetTextWidth(input, font, scale);
		}
	}
}
