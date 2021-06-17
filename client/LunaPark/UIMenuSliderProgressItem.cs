using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuSliderProgressItem : UIMenuItem
	{
		protected Sprite _arrowLeft;

		protected Sprite _arrowRight;

		protected bool Pressed;

		protected UIMenuGridAudio Audio;

		protected UIResRectangle _rectangleBackground;

		protected UIResRectangle _rectangleSlider;

		protected UIResRectangle _rectangleDivider;

		protected int _value = 0;

		protected int _max;

		protected int _multiplier = 5;

		public int Value
		{
			get
			{
				float offset = ((Rectangle)_rectangleBackground).get_Size().Width / (float)_max * (float)_value;
				((Rectangle)_rectangleSlider).set_Size(new SizeF(offset, ((Rectangle)_rectangleSlider).get_Size().Height));
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
				SliderProgressChanged();
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

		public event ItemSliderProgressEvent OnSliderChanged;

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, bool divider = false)
			: this(text, maxCount, startIndex, "", divider)
		{
			_max = maxCount;
			_value = startIndex;
		}

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, string description, bool divider = false)
			: this(text, maxCount, startIndex, description, Color.FromArgb(255, 57, 119, 200), Color.FromArgb(255, 4, 32, 57), divider)
		{
			_max = maxCount;
			_value = startIndex;
		}

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, string description, Color sliderColor, Color backgroundSliderColor, bool divider = false)
			: base(text, description)
		{
			_max = maxCount;
			_value = startIndex;
			_arrowLeft = new Sprite("commonmenu", "arrowleft", new PointF(0f, 105f), new SizeF(25f, 25f));
			_arrowRight = new Sprite("commonmenu", "arrowright", new PointF(0f, 105f), new SizeF(25f, 25f));
			_rectangleBackground = new UIResRectangle(new PointF(0f, 0f), new SizeF(150f, 10f), backgroundSliderColor);
			_rectangleSlider = new UIResRectangle(new PointF(0f, 0f), new SizeF(75f, 10f), sliderColor);
			if (divider)
			{
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Colors.WhiteSmoke);
			}
			else
			{
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.Transparent);
			}
			float offset = ((Rectangle)_rectangleBackground).get_Size().Width / (float)_max * (float)_value;
			((Rectangle)_rectangleSlider).set_Size(new SizeF(offset, ((Rectangle)_rectangleSlider).get_Size().Height));
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
		}

		public override void Position(int y)
		{
			base.Position(y);
			((Rectangle)_rectangleBackground).set_Position(new PointF(250f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 158.5f + base.Offset.Y));
			((Rectangle)_rectangleSlider).set_Position(new PointF(250f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 158.5f + base.Offset.Y));
			((Rectangle)_rectangleDivider).set_Position(new PointF(323.5f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(y + 153) + base.Offset.Y));
			_arrowLeft.Position = new PointF(225f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 150.5f + base.Offset.Y);
			_arrowRight.Position = new PointF(400f + base.Offset.X + (float)base.Parent.WidthOffset, (float)y + 150.5f + base.Offset.Y);
		}

		internal virtual void SliderProgressChanged()
		{
			this.OnSliderChanged?.Invoke(this, Value);
			base.Parent.SliderProgressChange(this, Value);
		}

		public async void Functions()
		{
			if (ScreenTools.IsMouseInBounds(new PointF(((Rectangle)_rectangleBackground).get_Position().X, ((Rectangle)_rectangleBackground).get_Position().Y), new SizeF(150f, ((Rectangle)_rectangleBackground).get_Size().Height)))
			{
				if (API.IsDisabledControlPressed(0, 24) && !Pressed)
				{
					Pressed = true;
					Audio.Id = API.GetSoundId();
					API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
					while (API.IsDisabledControlPressed(0, 24) && ScreenTools.IsMouseInBounds(new PointF(((Rectangle)_rectangleBackground).get_Position().X, ((Rectangle)_rectangleBackground).get_Position().Y), new SizeF(150f, ((Rectangle)_rectangleBackground).get_Size().Height)))
					{
						await BaseScript.Delay(0);
						SizeF ress = ScreenTools.ResolutionMaintainRatio;
						float CursorX = API.GetDisabledControlNormal(0, 239) * ress.Width;
						float Progress = CursorX - ((Rectangle)_rectangleSlider).get_Position().X;
						Value = (int)Math.Round((float)_max * ((Progress >= 0f && Progress <= 150f) ? Progress : ((Progress < 0f) ? 0f : 150f)) / 150f);
						SliderProgressChanged();
					}
					API.StopSound(Audio.Id);
					API.ReleaseSoundId(Audio.Id);
					Pressed = false;
				}
			}
			else if (ScreenTools.IsMouseInBounds(_arrowLeft.Position, _arrowLeft.Size))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					Value -= Multiplier;
					SliderProgressChanged();
				}
			}
			else if (ScreenTools.IsMouseInBounds(_arrowRight.Position, _arrowRight.Size) && API.IsDisabledControlPressed(0, 24))
			{
				Value += Multiplier;
				SliderProgressChanged();
			}
		}

		public override async Task Draw()
		{
			base.Draw();
			_arrowLeft.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			_arrowRight.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			_arrowLeft.Draw();
			_arrowRight.Draw();
			((Rectangle)_rectangleBackground).Draw();
			((Rectangle)_rectangleSlider).Draw();
			((Rectangle)_rectangleDivider).Draw();
			Functions();
		}
	}
}
