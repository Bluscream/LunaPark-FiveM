using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuSliderItem : UIMenuItem
	{
		protected Sprite _arrowLeft;

		protected Sprite _arrowRight;

		protected UIResRectangle _rectangleBackground;

		protected UIResRectangle _rectangleSlider;

		protected UIResRectangle _rectangleDivider;

		protected int _value = 0;

		protected int _max = 100;

		protected int _multiplier = 5;

		public int Maximum
		{
			get
			{
				return _max;
			}
			set
			{
				_max = value;
				if (_value > value)
				{
					_value = value;
				}
			}
		}

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value > _max)
				{
					_value = _max;
				}
				else if (value < 0)
				{
					_value = 0;
				}
				else
				{
					_value = value;
				}
				SliderChanged();
			}
		}

		public int Multiplier
		{
			get
			{
				return _multiplier;
			}
			set
			{
				_multiplier = value;
			}
		}

		public event ItemSliderEvent OnSliderChanged;

		public UIMenuSliderItem(string text)
			: this(text, "", divider: false)
		{
		}

		public UIMenuSliderItem(string text, string description)
			: this(text, description, divider: false)
		{
		}

		public UIMenuSliderItem(string text, string description, bool divider)
			: base(text, description)
		{
			_arrowLeft = new Sprite("commonmenutu", "arrowleft", new Point(0, 105), new Size(15, 15));
			_arrowRight = new Sprite("commonmenutu", "arrowright", new Point(0, 105), new Size(15, 15));
			_rectangleBackground = new UIResRectangle(new Point(0, 0), new Size(150, 9), Color.FromArgb(255, 4, 32, 57));
			_rectangleSlider = new UIResRectangle(new Point(0, 0), new Size(75, 9), Color.FromArgb(255, 57, 116, 200));
			if (divider)
			{
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Colors.WhiteSmoke);
			}
			else
			{
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.Transparent);
			}
		}

		public override void Position(int y)
		{
			((Rectangle)_rectangleBackground).set_Position(new PointF(250f + base.Offset.X, (float)(y + 158) + base.Offset.Y));
			((Rectangle)_rectangleSlider).set_Position(new PointF(250f + base.Offset.X, (float)(y + 158) + base.Offset.Y));
			((Rectangle)_rectangleDivider).set_Position(new PointF(323f + base.Offset.X, (float)(y + 153) + base.Offset.Y));
			_arrowLeft.Position = new PointF(235f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(155 + y) + base.Offset.Y);
			_arrowRight.Position = new PointF(400f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(155 + y) + base.Offset.Y);
			base.Position(y);
		}

		public override async Task Draw()
		{
			base.Draw();
			_arrowLeft.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			_arrowRight.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			float offset = 176f + base.Offset.X + ((Rectangle)_rectangleBackground).get_Size().Width - ((Rectangle)_rectangleSlider).get_Size().Width;
			((Rectangle)_rectangleSlider).set_Position(new PointF((int)(offset + (float)_value / (float)_max * 73f), ((Rectangle)_rectangleSlider).get_Position().Y));
			_arrowLeft.Draw();
			_arrowRight.Draw();
			((Rectangle)_rectangleBackground).Draw();
			((Rectangle)_rectangleSlider).Draw();
			((Rectangle)_rectangleDivider).Draw();
		}

		internal virtual void SliderChanged()
		{
			this.OnSliderChanged?.Invoke(this, Value);
		}
	}
}
