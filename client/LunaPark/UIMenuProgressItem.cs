using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuProgressItem : UIMenuItem
	{
		protected UIResRectangle _background;

		protected int Items;

		protected bool Pressed;

		protected bool Counter;

		protected float _max = 407.5f;

		protected UIMenuGridAudio Audio;

		protected UIResRectangle _bar;

		protected int _index;

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				if (value > Items)
				{
					_index = Items;
				}
				else if (value < 0)
				{
					_index = 0;
				}
				else
				{
					_index = value;
				}
				if (Counter)
				{
					SetRightLabel(_index + "/" + Items);
				}
				else
				{
					SetRightLabel(_index.ToString() ?? "");
				}
				UpdateBar();
			}
		}

		public event OnProgressChanged OnProgressChanged;

		public event OnProgressSelected OnProgressSelected;

		protected void UpdateBar()
		{
			((Rectangle)_bar).set_Size(new SizeF((float)Index * (_max / (float)Items), ((Rectangle)_bar).get_Size().Height));
		}

		public UIMenuProgressItem(string text, string description, int maxItems, int index, bool counter)
			: base(text, description)
		{
			Items = maxItems;
			_index = index;
			Counter = counter;
			_max = 407.5f;
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
			_background = new UIResRectangle(new PointF(0f, 0f), new SizeF(415f, 14f), Color.FromArgb(255, 0, 0, 0));
			_bar = new UIResRectangle(new PointF(0f, 0f), new SizeF(407.5f, 7.5f));
			if (Counter)
			{
				SetRightLabel(_index + "/" + Items);
			}
			else
			{
				SetRightLabel(_index.ToString() ?? "");
			}
			UpdateBar();
		}

		public void ProgressChanged(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			this.OnProgressChanged?.Invoke(Menu, Item, index);
		}

		public void ProgressSelected(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			this.OnProgressSelected?.Invoke(Menu, Item, index);
		}

		public override void SetRightBadge(BadgeStyle badge)
		{
			throw new Exception("UIMenuProgressItem cannot have a right badge.");
		}

		public override void Position(int y)
		{
			((Rectangle)_rectangle).set_Position(new PointF(base.Offset.X, (float)(y + 144) + base.Offset.Y));
			_selectedSprite.Position = new PointF(0f + base.Offset.X, (float)(y + 144) + base.Offset.Y);
			((Text)_text).set_Position(new PointF(8f + base.Offset.X, (float)y + 141.5f + base.Offset.Y));
			((Text)_labelText).set_Position(new PointF(420f + base.Offset.X, (float)y + 141.5f + base.Offset.Y));
			_max = 407.5f + (float)base.Parent.WidthOffset;
			((Rectangle)_background).set_Size(new SizeF(415f + (float)base.Parent.WidthOffset, 14f));
			((Rectangle)_background).set_Position(new PointF(8f + base.Offset.X, 170f + (float)y + base.Offset.Y));
			((Rectangle)_bar).set_Position(new PointF(11.75f + base.Offset.X, 172.5f + (float)y + base.Offset.Y));
		}

		public void CalculateProgress(float CursorX)
		{
			float Progress = CursorX - ((Rectangle)_bar).get_Position().X;
			Index = (int)Math.Round((float)Items * ((Progress >= 0f && Progress <= _max) ? Progress : ((Progress < 0f) ? 0f : _max)) / _max);
		}

		public async void Functions()
		{
			if (ScreenTools.IsMouseInBounds(new PointF(((Rectangle)_bar).get_Position().X, ((Rectangle)_bar).get_Position().Y - 7.5f), new SizeF(_max, ((Rectangle)_bar).get_Size().Height + 19f)) && API.IsDisabledControlPressed(0, 24) && !Pressed)
			{
				Pressed = true;
				Audio.Id = API.GetSoundId();
				API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
				while (API.IsDisabledControlPressed(0, 24) && ScreenTools.IsMouseInBounds(new PointF(((Rectangle)_bar).get_Position().X, ((Rectangle)_bar).get_Position().Y - 7.5f), new SizeF(_max, ((Rectangle)_bar).get_Size().Height + 19f)))
				{
					await BaseScript.Delay(0);
					SizeF ress = ScreenTools.ResolutionMaintainRatio;
					float CursorX = API.GetDisabledControlNormal(0, 239) * ress.Width;
					CalculateProgress(CursorX);
					base.Parent.ProgressChange(this, _index);
					ProgressChanged(base.Parent, this, _index);
				}
				API.StopSound(Audio.Id);
				API.ReleaseSoundId(Audio.Id);
				Pressed = false;
			}
		}

		public override async Task Draw()
		{
			base.Draw();
			if (Selected)
			{
				((Rectangle)_background).set_Color(Colors.Black);
				((Rectangle)_bar).set_Color(Colors.White);
			}
			else
			{
				((Rectangle)_background).set_Color(Colors.White);
				((Rectangle)_bar).set_Color(Colors.Black);
			}
			Functions();
			((Rectangle)_background).Draw();
			((Rectangle)_bar).Draw();
		}
	}
}
