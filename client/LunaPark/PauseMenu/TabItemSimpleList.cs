using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabItemSimpleList : TabItem
	{
		public Dictionary<string, string> Dictionary { get; set; }

		public TabItemSimpleList(string title, Dictionary<string, string> dict)
			: base(title)
		{
			Dictionary = dict;
			DrawBg = false;
		}

		public override void Draw()
		{
			base.Draw();
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			int alpha = ((Focused || !base.CanBeFocused) ? 180 : 60);
			int blackAlpha = ((Focused || !base.CanBeFocused) ? 200 : 90);
			int fullAlpha = ((Focused || !base.CanBeFocused) ? 255 : 150);
			int rectSize = (int)(base.BottomRight.X - base.TopLeft.X);
			for (int i = 0; i < Dictionary.Count; i++)
			{
				((Rectangle)new UIResRectangle(new PointF(base.TopLeft.X, base.TopLeft.Y + (float)(40 * i)), new SizeF(rectSize, 40f), (i % 2 == 0) ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0))).Draw();
				KeyValuePair<string, string> item = Dictionary.ElementAt(i);
				((Text)new UIResText(item.Key, new PointF(base.TopLeft.X + 6f, base.TopLeft.Y + 5f + (float)(40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White))).Draw();
				((Text)new UIResText(item.Value, new PointF(base.BottomRight.X - 6f, base.TopLeft.Y + 5f + (float)(40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White), (Font)0, (Alignment)2)).Draw();
			}
		}
	}
}
