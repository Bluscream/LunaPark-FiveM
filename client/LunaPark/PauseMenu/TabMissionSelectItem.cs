using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabMissionSelectItem : TabItem
	{
		protected const int MaxItemsPerView = 15;

		protected int _minItem;

		protected int _maxItem;

		public List<MissionInformation> Heists { get; set; }

		public int Index { get; set; }

		protected Sprite _noLogo { get; set; }

		public event OnItemSelect OnItemSelect;

		public TabMissionSelectItem(string name, IEnumerable<MissionInformation> list)
			: base(name)
		{
			base.FadeInWhenFocused = true;
			DrawBg = false;
			_noLogo = new Sprite("gtav_online", "rockstarlogo256", default(PointF), new SizeF(512f, 256f));
			_maxItem = 15;
			_minItem = 0;
			base.CanBeFocused = true;
			Heists = new List<MissionInformation>(list);
		}

		public override void ProcessControls()
		{
			if (!Focused || Heists.Count == 0)
			{
				return;
			}
			if (base.JustOpened)
			{
				base.JustOpened = false;
				return;
			}
			if (Game.IsControlJustPressed(0, (Control)176))
			{
				Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				this.OnItemSelect?.Invoke(Heists[Index]);
			}
			if (Game.IsControlJustPressed(0, (Control)188) || Game.IsControlJustPressed(0, (Control)32))
			{
				Index = (1000 - 1000 % Heists.Count + Index - 1) % Heists.Count;
				Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				if (Heists.Count > 15)
				{
					if (Index < _minItem)
					{
						_minItem--;
						_maxItem--;
					}
					if (Index == Heists.Count - 1)
					{
						_minItem = Heists.Count - 15;
						_maxItem = Heists.Count;
					}
				}
			}
			else
			{
				if (!Game.IsControlJustPressed(0, (Control)187) && !Game.IsControlJustPressed(0, (Control)33))
				{
					return;
				}
				Index = (1000 - 1000 % Heists.Count + Index + 1) % Heists.Count;
				Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				if (Heists.Count > 15)
				{
					if (Index >= _maxItem)
					{
						_maxItem++;
						_minItem++;
					}
					if (Index == 0)
					{
						_minItem = 0;
						_maxItem = 15;
					}
				}
			}
		}

		public override void Draw()
		{
			base.Draw();
			if (Heists.Count != 0)
			{
				SizeF res = ScreenTools.ResolutionMaintainRatio;
				float activeWidth = res.Width - base.SafeSize.X * 2f;
				SizeF itemSize = new SizeF((int)activeWidth - 515, 40f);
				int alpha = (Focused ? 120 : 30);
				int blackAlpha = (Focused ? 200 : 100);
				int fullAlpha = (Focused ? 255 : 150);
				int counter = 0;
				for (int i = _minItem; i < Math.Min(Heists.Count, _maxItem); i++)
				{
					((Rectangle)new UIResRectangle(base.SafeSize.AddPoints(new PointF(0f, (itemSize.Height + 3f) * (float)counter)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Colors.White) : Color.FromArgb(blackAlpha, Colors.Black))).Draw();
					((Text)new UIResText(Heists[i].Name, base.SafeSize.AddPoints(new PointF(6f, 5f + (itemSize.Height + 3f) * (float)counter)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Colors.Black : Colors.White))).Draw();
					counter++;
				}
				if (Heists[Index].Logo == null || string.IsNullOrEmpty(Heists[Index].Logo.FileName))
				{
					_noLogo.Position = new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y);
					_noLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
					_noLogo.Draw();
				}
				else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null && !Heists[Index].Logo.IsGameTexture)
				{
					string target = Heists[Index].Logo.FileName;
				}
				else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null && Heists[Index].Logo.IsGameTexture)
				{
					Sprite newLogo = new Sprite(Heists[Index].Logo.DictionaryName, Heists[Index].Logo.FileName, default(PointF), new SizeF(512f, 256f))
					{
						Position = new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y),
						Color = Color.FromArgb(blackAlpha, 0, 0, 0)
					};
					newLogo.Draw();
				}
				((Rectangle)new UIResRectangle(new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y + 256f), new SizeF(512f, 40f), Color.FromArgb(fullAlpha, Colors.Black))).Draw();
				((Text)new UIResText(Heists[Index].Name, new PointF((float)(int)res.Width - base.SafeSize.X - 4f, base.SafeSize.Y + 260f), 0.5f, Color.FromArgb(fullAlpha, Colors.White), (Font)1, (Alignment)2)).Draw();
				for (int j = 0; j < Heists[Index].ValueList.Count; j++)
				{
					((Rectangle)new UIResRectangle(new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y + 256f + 40f + (float)(40 * j)), new SizeF(512f, 40f), (j % 2 == 0) ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0))).Draw();
					string text = Heists[Index].ValueList[j].Item1;
					string label = Heists[Index].ValueList[j].Item2;
					((Text)new UIResText(text, new PointF((float)(int)res.Width - base.SafeSize.X - 506f, base.SafeSize.Y + 260f + 42f + (float)(40 * j)), 0.35f, Color.FromArgb(fullAlpha, Colors.White))).Draw();
					((Text)new UIResText(label, new PointF((float)(int)res.Width - base.SafeSize.X - 6f, base.SafeSize.Y + 260f + 42f + (float)(40 * j)), 0.35f, Color.FromArgb(fullAlpha, Colors.White), (Font)0, (Alignment)2)).Draw();
				}
				if (!string.IsNullOrEmpty(Heists[Index].Description))
				{
					int propLen = Heists[Index].ValueList.Count;
					((Rectangle)new UIResRectangle(new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y + 256f + 42f + (float)(40 * propLen)), new SizeF(512f, 2f), Color.FromArgb(fullAlpha, Colors.White))).Draw();
					((Text)new UIResText(Heists[Index].Description, new PointF((float)(int)res.Width - base.SafeSize.X - 508f, base.SafeSize.Y + 256f + 45f + (float)(40 * propLen) + 4f), 0.35f, Color.FromArgb(fullAlpha, Colors.White))
					{
						Wrap = 508f
					}).Draw();
					((Rectangle)new UIResRectangle(new PointF((float)(int)res.Width - base.SafeSize.X - 512f, base.SafeSize.Y + 256f + 44f + (float)(40 * propLen)), new SizeF(512f, 45 * (int)(ScreenTools.GetTextWidth(Heists[Index].Description, (Font)0, 0.35f) / 500f)), Color.FromArgb(blackAlpha, 0, 0, 0))).Draw();
				}
			}
		}
	}
}
