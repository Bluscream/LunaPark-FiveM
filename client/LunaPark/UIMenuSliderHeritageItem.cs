using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuSliderHeritageItem : UIMenuSliderItem
	{
		public UIMenuSliderHeritageItem(string text, string description, bool divider)
			: base(text, description, divider)
		{
			_arrowLeft = new Sprite("mpleaderboard", "leaderboard_female_icon", new PointF(0f, 105f), new Size(40, 40), 0f, Color.FromArgb(255, 255, 255));
			_arrowRight = new Sprite("mpleaderboard", "leaderboard_male_icon", new PointF(0f, 105f), new Size(40, 40), 0f, Color.FromArgb(255, 255, 255));
		}

		public override void Position(int y)
		{
			base.Position(y);
			((Rectangle)_rectangleBackground).set_Position(new PointF(250f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 158.5f + base.Offset.Y));
			((Rectangle)_rectangleSlider).set_Position(new PointF(250f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 158.5f + base.Offset.Y));
			((Rectangle)_rectangleDivider).set_Position(new PointF(323.5f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(y + 153) + base.Offset.Y));
			_arrowLeft.Position = new PointF(217f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 143.5f + base.Offset.Y);
			_arrowRight.Position = new PointF(395f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 143.5f + base.Offset.Y);
		}

		public override async Task Draw()
		{
			await base.Draw();
			_arrowLeft.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Color.FromArgb(255, 102, 178) : Colors.WhiteSmoke));
			_arrowRight.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Color.FromArgb(51, 51, 255) : Colors.WhiteSmoke));
			_arrowLeft.Draw();
			_arrowRight.Draw();
			await Task.FromResult(0);
		}
	}
}
