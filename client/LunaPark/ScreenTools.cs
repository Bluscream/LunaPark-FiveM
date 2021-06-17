using System;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public static class ScreenTools
	{
		public static SizeF ResolutionMaintainRatio
		{
			get
			{
				int screenw = Screen.get_Resolution().Width;
				int screenh = Screen.get_Resolution().Height;
				float ratio = (float)screenw / (float)screenh;
				float width = 1080f * ratio;
				return new SizeF(width, 1080f);
			}
		}

		public static PointF SafezoneBounds
		{
			get
			{
				float t = API.GetSafeZoneSize();
				double g = Math.Round(Convert.ToDouble(t), 2);
				g = g * 100.0 - 90.0;
				g = 10.0 - g;
				int screenw = Screen.get_Resolution().Width;
				int screenh = Screen.get_Resolution().Height;
				float ratio = (float)screenw / (float)screenh;
				float wmp = ratio * 5.4f;
				return new PointF((float)g * wmp, (float)g * 5.4f);
			}
		}

		public static bool IsMouseInBounds(Point topLeft, Size boxSize)
		{
			Game.EnableControlThisFrame(0, (Control)239);
			Game.EnableControlThisFrame(0, (Control)240);
			SizeF res = ResolutionMaintainRatio;
			int mouseX = (int)Math.Round(API.GetDisabledControlNormal(0, 239) * res.Width);
			int mouseY = (int)Math.Round(API.GetDisabledControlNormal(0, 240) * res.Height);
			bool isX = mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width;
			bool isY = mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
			return isX && isY;
		}

		public static bool IsMouseInBounds(PointF topLeft, SizeF boxSize)
		{
			Game.EnableControlThisFrame(0, (Control)239);
			Game.EnableControlThisFrame(0, (Control)240);
			SizeF res = ResolutionMaintainRatio;
			float mouseX = API.GetDisabledControlNormal(0, 239) * res.Width;
			float mouseY = API.GetDisabledControlNormal(0, 240) * res.Height;
			bool isX = mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width;
			bool isY = mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
			return isX && isY;
		}

		public static bool IsMouseInBounds(Point topLeft, Size boxSize, Point DrawOffset)
		{
			Game.EnableControlThisFrame(0, (Control)239);
			Game.EnableControlThisFrame(0, (Control)240);
			SizeF res = ResolutionMaintainRatio;
			int mouseX = (int)Math.Round(API.GetDisabledControlNormal(0, 239) * res.Width);
			int mouseY = (int)Math.Round(API.GetDisabledControlNormal(0, 240) * res.Height);
			mouseX += DrawOffset.X;
			mouseY += DrawOffset.Y;
			return mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width && mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
		}

		public static bool IsMouseInBounds(PointF topLeft, SizeF boxSize, PointF DrawOffset)
		{
			Game.EnableControlThisFrame(0, (Control)239);
			Game.EnableControlThisFrame(0, (Control)240);
			SizeF res = ResolutionMaintainRatio;
			float mouseX = API.GetDisabledControlNormal(0, 239) * res.Width;
			float mouseY = API.GetDisabledControlNormal(0, 240) * res.Height;
			mouseX += DrawOffset.X;
			mouseY += DrawOffset.Y;
			return mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width && mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
		}

		public static float GetTextWidth(string text, Font font, float scale)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected I4, but got Unknown
			API.SetTextEntryForWidth("CELL_EMAIL_BCON");
			UIResText.AddLongString(text);
			API.SetTextFont((int)font);
			API.SetTextScale(1f, scale);
			float width = API.GetTextScreenWidth(true);
			return ResolutionMaintainRatio.Width * width;
		}

		public static int GetLineCount(string text, Point position, Font font, float scale, int wrap)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected I4, but got Unknown
			API.SetTextGxtEntry("CELL_EMAIL_BCON");
			UIResText.AddLongStringForUtf8(text);
			SizeF res = ResolutionMaintainRatio;
			float x = (float)position.X / res.Width;
			float y = (float)position.Y / res.Height;
			API.SetTextFont((int)font);
			API.SetTextScale(1f, scale);
			if (wrap > 0)
			{
				float start = (float)position.X / res.Width;
				float end = start + (float)wrap / res.Width;
				API.SetTextWrap(x, end);
			}
			return API.GetTextScreenLineCount(x, y);
		}

		public static int GetLineCount(string text, PointF position, Font font, float scale, float wrap)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected I4, but got Unknown
			API.SetTextGxtEntry("CELL_EMAIL_BCON");
			UIResText.AddLongStringForUtf8(text);
			SizeF res = ResolutionMaintainRatio;
			float x = position.X / res.Width;
			float y = position.Y / res.Height;
			API.SetTextFont((int)font);
			API.SetTextScale(1f, scale);
			if (wrap > 0f)
			{
				float start = position.X / res.Width;
				float end = start + wrap / res.Width;
				API.SetTextWrap(x, end);
			}
			return API.GetTextScreenLineCount(x, y);
		}
	}
}
