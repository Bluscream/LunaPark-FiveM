using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public abstract class TimerBarBase
	{
		public string Label { get; set; }

		public TimerBarBase(string label)
		{
			Label = label;
		}

		public virtual void Draw(int interval)
		{
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			PointF safe = ScreenTools.SafezoneBounds;
			((Text)new UIResText(Label, new PointF((float)(int)res.Width - safe.X - 180f, (float)(int)res.Height - safe.Y - (float)(30 + 4 * interval)), 0.3f, Colors.White, (Font)0, (Alignment)2)).Draw();
			new Sprite("timerbars", "all_black_bg", new PointF((float)(int)res.Width - safe.X - 298f, (float)(int)res.Height - safe.Y - (float)(40 + 4 * interval)), new SizeF(300f, 37f), 0f, Color.FromArgb(180, 255, 255, 255)).Draw();
			Hud.HideComponentThisFrame((HudComponent)7);
			Hud.HideComponentThisFrame((HudComponent)9);
			Hud.HideComponentThisFrame((HudComponent)6);
		}
	}
}
