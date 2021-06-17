using System.Drawing;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIResRectangle : Rectangle
	{
		public UIResRectangle()
			: this()
		{
		}

		public UIResRectangle(PointF pos, SizeF size)
			: this(pos, size)
		{
		}

		public UIResRectangle(PointF pos, SizeF size, Color color)
			: this(pos, size, color)
		{
		}

		public override void Draw(SizeF offset)
		{
			if (((Rectangle)this).get_Enabled())
			{
				int screenw = Screen.get_Resolution().Width;
				int screenh = Screen.get_Resolution().Height;
				float ratio = (float)screenw / (float)screenh;
				float width = 1080f * ratio;
				float w = ((Rectangle)this).get_Size().Width / width;
				float h = ((Rectangle)this).get_Size().Height / 1080f;
				float x = (((Rectangle)this).get_Position().X + offset.Width) / width + w * 0.5f;
				float y = (((Rectangle)this).get_Position().Y + offset.Height) / 1080f + h * 0.5f;
				API.DrawRect(x, y, w, h, (int)((Rectangle)this).get_Color().R, (int)((Rectangle)this).get_Color().G, (int)((Rectangle)this).get_Color().B, (int)((Rectangle)this).get_Color().A);
			}
		}

		public static void Draw(float xPos, float yPos, int boxWidth, int boxHeight, Color color)
		{
			int screenw = Screen.get_Resolution().Width;
			int screenh = Screen.get_Resolution().Height;
			float ratio = (float)screenw / (float)screenh;
			float width = 1080f * ratio;
			float w = (float)boxWidth / width;
			float h = (float)boxHeight / 1080f;
			float x = xPos / width + w * 0.5f;
			float y = yPos / 1080f + h * 0.5f;
			API.DrawRect(x, y, w, h, (int)color.R, (int)color.G, (int)color.B, (int)color.A);
		}
	}
}
