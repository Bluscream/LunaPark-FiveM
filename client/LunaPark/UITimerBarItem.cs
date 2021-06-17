using System.Drawing;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UITimerBarItem
	{
		public UIResText Text;

		public UIResText TextTimerBar;

		public PointF Position;

		public Sprite Background;

		public bool Enabled;

		public UITimerBarItem(string text)
			: this(text, Color.FromArgb(200, 255, 255, 255))
		{
			Text = new UIResText((text != string.Empty) ? text : "N/A", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)2);
		}

		public UITimerBarItem(string text, Color color)
			: this(text, "timerbars", "all_black_bg", color)
		{
			Background = new Sprite("timerbars", "all_black_bg", new PointF(0f, 0f), new SizeF(350f, 35f), 0f, color);
			Text = new UIResText((text != string.Empty) ? text : "N/A", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)2);
		}

		public UITimerBarItem(string text, string txtDictionary, string txtName, Color color)
		{
			Background = new Sprite("timerbars", "all_black_bg", new PointF(0f, 0f), new SizeF(350f, 35f), 0f, color);
			Text = new UIResText((text != string.Empty) ? text : "N/A", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)2);
			TextTimerBar = new UIResText("", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)2);
			Position = new PointF(1580f, 1082f);
			Enabled = true;
		}

		public virtual async void Draw(float offset)
		{
			if (Enabled)
			{
				Background.Position = new PointF(Position.X, Position.Y - offset);
				Background.Draw();
				((Text)Text).set_Position(new PointF(Position.X + 170f, Position.Y - offset));
				((Text)Text).Draw();
				if (((Text)TextTimerBar).get_Caption() != "")
				{
					((Text)TextTimerBar).set_Position(new PointF(Position.X + 340f, Position.Y - offset));
					((Text)TextTimerBar).Draw();
				}
				for (int i = 6; i < 10; i++)
				{
					API.HideHudComponentThisFrame(i);
				}
			}
		}
	}
}
