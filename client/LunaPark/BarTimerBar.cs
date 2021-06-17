using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class BarTimerBar : TimerBarBase
	{
		public float Percentage { get; set; }

		public Color BackgroundColor { get; set; }

		public Color ForegroundColor { get; set; }

		public BarTimerBar(string label)
			: base(label)
		{
			BackgroundColor = Colors.DarkRed;
			ForegroundColor = Colors.Red;
		}

		public override void Draw(int interval)
		{
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			PointF safe = ScreenTools.SafezoneBounds;
			base.Draw(interval);
			PointF start = new PointF((float)(int)res.Width - safe.X - 160f, (float)(int)res.Height - safe.Y - (float)(28 + 4 * interval));
			((Rectangle)new UIResRectangle(start, new SizeF(150f, 15f), BackgroundColor)).Draw();
			((Rectangle)new UIResRectangle(start, new SizeF((int)(150f * Percentage), 15f), ForegroundColor)).Draw();
		}
	}
}
