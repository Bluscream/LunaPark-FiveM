using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabView
	{
		internal static readonly string _browseTextLocalized = Game.GetGXTEntry("HUD_INPUT1C");

		public int Index;

		private bool _visible;

		private Scaleform _sc;

		public string Title { get; set; }

		public Sprite Photo { get; set; }

		public string Name { get; set; }

		public string Money { get; set; }

		public string MoneySubtitle { get; set; }

		public List<TabItem> Tabs { get; set; }

		public int FocusLevel { get; set; }

		public bool TemporarilyHidden { get; set; }

		public bool CanLeave { get; set; }

		public bool HideTabs { get; set; }

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				if (value)
				{
					Effects.Start((ScreenEffect)9, 0, false);
				}
				else
				{
					Effects.Stop((ScreenEffect)9);
				}
			}
		}

		public event EventHandler OnMenuClose;

		public TabView(string title)
		{
			Title = title;
			Tabs = new List<TabItem>();
			Index = 0;
			Name = Game.get_Player().get_Name();
			TemporarilyHidden = false;
			CanLeave = true;
		}

		public void AddTab(TabItem item)
		{
			Tabs.Add(item);
			item.Parent = this;
		}

		public void ShowInstructionalButtons()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (_sc == null)
			{
				_sc = new Scaleform("instructional_buttons");
			}
			_sc.CallFunction("CLEAR_ALL", new object[0]);
			_sc.CallFunction("TOGGLE_MOUSE_BUTTONS", new object[1] { 0 });
			_sc.CallFunction("CREATE_CONTAINER", new object[0]);
			_sc.CallFunction("SET_DATA_SLOT", new object[3]
			{
				0,
				API.GetControlInstructionalButton(2, 176, 0),
				UIMenu._selectTextLocalized
			});
			_sc.CallFunction("SET_DATA_SLOT", new object[3]
			{
				1,
				API.GetControlInstructionalButton(2, 177, 0),
				UIMenu._backTextLocalized
			});
			_sc.CallFunction("SET_DATA_SLOT", new object[3]
			{
				2,
				API.GetControlInstructionalButton(2, 206, 0),
				""
			});
			_sc.CallFunction("SET_DATA_SLOT", new object[3]
			{
				3,
				API.GetControlInstructionalButton(2, 205, 0),
				_browseTextLocalized
			});
		}

		public void DrawInstructionalButton(int slot, Control control, string text)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected I4, but got Unknown
			_sc.CallFunction("SET_DATA_SLOT", new object[3]
			{
				slot,
				API.GetControlInstructionalButton(2, (int)control, 0),
				text
			});
		}

		public void ProcessControls()
		{
			if (!Visible || TemporarilyHidden)
			{
				return;
			}
			API.DisableAllControlActions(0);
			if (Game.IsControlJustPressed(0, (Control)174) && FocusLevel == 0)
			{
				Tabs[Index].Active = false;
				Tabs[Index].Focused = false;
				Tabs[Index].Visible = false;
				Index = (1000 - 1000 % Tabs.Count + Index - 1) % Tabs.Count;
				Tabs[Index].Active = true;
				Tabs[Index].Focused = false;
				Tabs[Index].Visible = true;
				Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
			}
			else if (Game.IsControlJustPressed(0, (Control)175) && FocusLevel == 0)
			{
				Tabs[Index].Active = false;
				Tabs[Index].Focused = false;
				Tabs[Index].Visible = false;
				Index = (1000 - 1000 % Tabs.Count + Index + 1) % Tabs.Count;
				Tabs[Index].Active = true;
				Tabs[Index].Focused = false;
				Tabs[Index].Visible = true;
				Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
			}
			else if (Game.IsControlJustPressed(0, (Control)201) && FocusLevel == 0)
			{
				if (Tabs[Index].CanBeFocused)
				{
					Tabs[Index].Focused = true;
					Tabs[Index].JustOpened = true;
					FocusLevel = 1;
				}
				else
				{
					Tabs[Index].JustOpened = true;
					Tabs[Index].OnActivated();
				}
				Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
			}
			else if (Game.IsControlJustPressed(0, (Control)177) && FocusLevel == 1)
			{
				Tabs[Index].Focused = false;
				FocusLevel = 0;
				Game.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
			}
			else if (Game.IsControlJustPressed(0, (Control)177) && FocusLevel == 0 && CanLeave)
			{
				Visible = false;
				Game.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				this.OnMenuClose?.Invoke(this, EventArgs.Empty);
			}
			if (!HideTabs)
			{
				if (Game.IsControlJustPressed(0, (Control)205))
				{
					Tabs[Index].Active = false;
					Tabs[Index].Focused = false;
					Tabs[Index].Visible = false;
					Index = (1000 - 1000 % Tabs.Count + Index - 1) % Tabs.Count;
					Tabs[Index].Active = true;
					Tabs[Index].Focused = false;
					Tabs[Index].Visible = true;
					FocusLevel = 0;
					Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				}
				else if (Game.IsControlJustPressed(0, (Control)206))
				{
					Tabs[Index].Active = false;
					Tabs[Index].Focused = false;
					Tabs[Index].Visible = false;
					Index = (1000 - 1000 % Tabs.Count + Index + 1) % Tabs.Count;
					Tabs[Index].Active = true;
					Tabs[Index].Focused = false;
					Tabs[Index].Visible = true;
					FocusLevel = 0;
					Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				}
			}
			if (Tabs.Count > 0)
			{
				Tabs[Index].ProcessControls();
			}
		}

		public void RefreshIndex()
		{
			foreach (TabItem item in Tabs)
			{
				item.Focused = false;
				item.Active = false;
				item.Visible = false;
			}
			Index = (1000 - 1000 % Tabs.Count) % Tabs.Count;
			Tabs[Index].Active = true;
			Tabs[Index].Focused = false;
			Tabs[Index].Visible = true;
			FocusLevel = 0;
		}

		public void Update()
		{
			if (!Visible || TemporarilyHidden)
			{
				return;
			}
			ShowInstructionalButtons();
			API.HideHudAndRadarThisFrame();
			API.ShowCursorThisFrame();
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			PointF safe = new PointF(300f, 180f);
			if (!HideTabs)
			{
				UIResText uIResText = new UIResText(Title, new PointF(safe.X, safe.Y - 80f), 1f, Colors.White, (Font)4, (Alignment)1);
				((Text)uIResText).set_Shadow(true);
				((Text)uIResText).Draw();
				if (Photo == null)
				{
					new Sprite("char_multiplayer", "char_multiplayer", new PointF((float)(int)res.Width - safe.X - 64f, safe.Y - 80f), new SizeF(64f, 64f)).Draw();
				}
				else
				{
					Photo.Position = new PointF((float)(int)res.Width - safe.X - 100f, safe.Y - 80f);
					Photo.Size = new SizeF(64f, 64f);
					Photo.Draw();
				}
				UIResText uIResText2 = new UIResText(Name, new PointF((float)(int)res.Width - safe.X - 70f, safe.Y - 95f), 0.7f, Colors.White, (Font)4, (Alignment)2);
				((Text)uIResText2).set_Shadow(true);
				((Text)uIResText2).Draw();
				string t = Money;
				if (string.IsNullOrEmpty(Money))
				{
					t = DateTime.Now.ToString();
				}
				UIResText uIResText3 = new UIResText(t, new PointF((float)(int)res.Width - safe.X - 70f, safe.Y - 60f), 0.4f, Colors.White, (Font)4, (Alignment)2);
				((Text)uIResText3).set_Shadow(true);
				((Text)uIResText3).Draw();
				string subt = MoneySubtitle;
				if (string.IsNullOrEmpty(MoneySubtitle))
				{
					subt = "";
				}
				UIResText uIResText4 = new UIResText(subt, new PointF((float)(int)res.Width - safe.X - 70f, safe.Y - 40f), 0.4f, Colors.White, (Font)4, (Alignment)2);
				((Text)uIResText4).set_Shadow(true);
				((Text)uIResText4).Draw();
				for (int i = 0; i < Tabs.Count; i++)
				{
					float activeSize = res.Width - 2f * safe.X;
					activeSize -= 20f;
					int tabWidth = (int)activeSize / Tabs.Count;
					Game.EnableControlThisFrame(0, (Control)239);
					Game.EnableControlThisFrame(0, (Control)240);
					bool hovering = ScreenTools.IsMouseInBounds(safe.AddPoints(new PointF((tabWidth + 5) * i, 0f)), new SizeF(tabWidth, 40f));
					Color tabColor = (Tabs[i].Active ? Colors.White : (hovering ? Color.FromArgb(100, 50, 50, 50) : Colors.Black));
					((Rectangle)new UIResRectangle(safe.AddPoints(new PointF((tabWidth + 5) * i, 0f)), new SizeF(tabWidth, 40f), Color.FromArgb(Tabs[i].Active ? 255 : 200, tabColor))).Draw();
					((Text)new UIResText(Tabs[i].Title.ToUpper(), safe.AddPoints(new PointF(tabWidth / 2 + (tabWidth + 5) * i, 5f)), 0.35f, Tabs[i].Active ? Colors.Black : Colors.White, (Font)0, (Alignment)0)).Draw();
					if (Tabs[i].Active)
					{
						((Rectangle)new UIResRectangle(safe.SubtractPoints(new PointF(-((tabWidth + 5) * i), 10f)), new SizeF(tabWidth, 10f), Colors.DodgerBlue)).Draw();
					}
					if (hovering && Game.IsControlJustPressed(0, (Control)237) && !Tabs[i].Active)
					{
						Tabs[Index].Active = false;
						Tabs[Index].Focused = false;
						Tabs[Index].Visible = false;
						Index = (1000 - 1000 % Tabs.Count + i) % Tabs.Count;
						Tabs[Index].Active = true;
						Tabs[Index].Focused = true;
						Tabs[Index].Visible = true;
						Tabs[Index].JustOpened = true;
						if (Tabs[Index].CanBeFocused)
						{
							FocusLevel = 1;
						}
						else
						{
							FocusLevel = 0;
						}
						Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					}
				}
			}
			Tabs[Index].Draw();
			_sc.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", new object[1] { -1 });
			_sc.Render2D();
		}
	}
}
