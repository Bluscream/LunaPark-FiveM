using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIProgressBar
	{
		public UIResRectangle Background;

		public UIResText Text;

		public UIResRectangle ProgressBar;

		public PointF Position;

		public int Percentage;

		public UIProgressBar(string text)
			: this(text, new PointF(800f, 1030f), Color.FromArgb(100, 255, 255, 255))
		{
			Text = new UIResText((text != "") ? text : "N/A", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)0);
		}

		public UIProgressBar(string text, PointF position, Color color)
		{
			Text = new UIResText((text != "") ? text : "N/A", new PointF(0f, 0f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)0);
			Background = new UIResRectangle(new PointF(0f, 0f), new SizeF(350f, 40f), Color.FromArgb(100, 0, 0, 0));
			ProgressBar = new UIResRectangle(new PointF(0f, 0f), new SizeF(0f, 30f), color);
			Position = position;
		}

		public async void Draw()
		{
			((Rectangle)Background).set_Position(Position);
			((Text)Text).set_Position(new PointF(Position.X + 170f, Position.Y + 5f));
			((Rectangle)ProgressBar).set_Position(new PointF(Position.X + 5f, Position.Y + 5f));
			((Rectangle)Background).Draw();
			((Text)Text).Draw();
			((Rectangle)ProgressBar).Draw();
		}
	}
}
