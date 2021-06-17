using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabTextItem : TabItem
	{
		public string TextTitle { get; set; }

		public string Text { get; set; }

		public int WordWrap { get; set; }

		public TabTextItem(string name, string title)
			: base(name)
		{
			TextTitle = title;
		}

		public TabTextItem(string name, string title, string text)
			: base(name)
		{
			TextTitle = title;
			Text = text;
		}

		public override void Draw()
		{
			base.Draw();
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			int alpha = ((Focused || !base.CanBeFocused) ? 255 : 200);
			if (!string.IsNullOrEmpty(TextTitle))
			{
				((Text)new UIResText(TextTitle, base.SafeSize.AddPoints(new PointF(40f, 20f)), 1.5f, Color.FromArgb(alpha, Colors.White))).Draw();
			}
			if (!string.IsNullOrEmpty(Text))
			{
				float ww = ((WordWrap == 0) ? (base.BottomRight.X - base.TopLeft.X - 40f) : ((float)WordWrap));
				((Text)new UIResText(Text, base.SafeSize.AddPoints(new PointF(40f, 150f)), 0.4f, Color.FromArgb(alpha, Colors.White))
				{
					Wrap = ww
				}).Draw();
			}
		}
	}
}
