using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class TextTimerBar : TimerBarBase
	{
		public string Text { get; set; }

		public TextTimerBar(string label, string text)
			: base(label)
		{
			Text = text;
		}

		public override void Draw(int interval)
		{
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			PointF safe = ScreenTools.SafezoneBounds;
			base.Draw(interval);
			((Text)new UIResText(Text, new PointF((float)(int)res.Width - safe.X - 10f, (float)(int)res.Height - safe.Y - (float)(42 + 4 * interval)), 0.5f, Colors.White, (Font)0, (Alignment)2)).Draw();
		}
	}
}
