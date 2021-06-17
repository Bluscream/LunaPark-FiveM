using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabInteractiveListItem : TabItem
	{
		protected const int MaxItemsPerView = 15;

		protected int _minItem;

		protected int _maxItem;

		public List<UIMenuItem> Items { get; set; }

		public int Index { get; set; }

		public bool IsInList { get; set; }

		public TabInteractiveListItem(string name, IEnumerable<UIMenuItem> items)
			: base(name)
		{
			DrawBg = false;
			base.CanBeFocused = true;
			Items = new List<UIMenuItem>(items);
			IsInList = true;
			_maxItem = 15;
			_minItem = 0;
		}

		public void MoveDown()
		{
			Index = (1000 - 1000 % Items.Count + Index + 1) % Items.Count;
			if (Items.Count > 15)
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

		public void MoveUp()
		{
			Index = (1000 - 1000 % Items.Count + Index - 1) % Items.Count;
			if (Items.Count > 15)
			{
				if (Index < _minItem)
				{
					_minItem--;
					_maxItem--;
				}
				if (Index == Items.Count - 1)
				{
					_minItem = Items.Count - 15;
					_maxItem = Items.Count;
				}
			}
		}

		public void RefreshIndex()
		{
			Index = 0;
			_maxItem = 15;
			_minItem = 0;
		}

		public override void ProcessControls()
		{
			if (!Visible)
			{
				return;
			}
			if (base.JustOpened)
			{
				base.JustOpened = false;
			}
			else if (Focused && Items.Count != 0)
			{
				if (Game.IsControlJustPressed(0, (Control)201) && Focused && Items[Index] is UIMenuCheckboxItem)
				{
					Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					((UIMenuCheckboxItem)Items[Index]).Checked = !((UIMenuCheckboxItem)Items[Index]).Checked;
					((UIMenuCheckboxItem)Items[Index]).CheckboxEventTrigger();
				}
				else if (Game.IsControlJustPressed(0, (Control)201) && Focused)
				{
					Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					Items[Index].ItemActivate(null);
				}
				if (Game.IsControlJustPressed(0, (Control)189) && Focused && Items[Index] is UIMenuListItem)
				{
					UIMenuListItem it = (UIMenuListItem)Items[Index];
					it.Index--;
					Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					it.ListChangedTrigger(it.Index);
				}
				if (Game.IsControlJustPressed(0, (Control)190) && Focused && Items[Index] is UIMenuListItem)
				{
					UIMenuListItem it2 = (UIMenuListItem)Items[Index];
					it2.Index++;
					Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					it2.ListChangedTrigger(it2.Index);
				}
				if (Game.IsControlJustPressed(0, (Control)188) || Game.IsControlJustPressed(0, (Control)32) || Game.IsControlJustPressed(0, (Control)241))
				{
					Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					MoveUp();
				}
				else if (Game.IsControlJustPressed(0, (Control)187) || Game.IsControlJustPressed(0, (Control)33) || Game.IsControlJustPressed(0, (Control)242))
				{
					Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					MoveDown();
				}
			}
		}

		public override void Draw()
		{
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			if (!Visible)
			{
				return;
			}
			base.Draw();
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			int alpha = (Focused ? 120 : 30);
			int blackAlpha = (Focused ? 200 : 100);
			int fullAlpha = (Focused ? 255 : 150);
			float submenuWidth = base.BottomRight.X - base.TopLeft.X;
			SizeF itemSize = new SizeF(submenuWidth, 40f);
			int i = 0;
			for (int c = _minItem; c < Math.Min(Items.Count, _maxItem); c++)
			{
				bool hovering = ScreenTools.IsMouseInBounds(base.SafeSize.AddPoints(new PointF(0f, (itemSize.Height + 3f) * (float)i)), itemSize);
				bool hasLeftBadge = Items[c].LeftBadge != UIMenuItem.BadgeStyle.None;
				bool hasRightBadge = Items[c].RightBadge != UIMenuItem.BadgeStyle.None;
				bool hasBothBadges = hasRightBadge && hasLeftBadge;
				bool hasAnyBadge = hasRightBadge || hasLeftBadge;
				((Rectangle)new UIResRectangle(base.SafeSize.AddPoints(new PointF(0f, (itemSize.Height + 3f) * (float)i)), itemSize, (Index == c && Focused) ? Color.FromArgb(fullAlpha, Colors.White) : ((Focused && hovering) ? Color.FromArgb(100, 50, 50, 50) : Color.FromArgb(blackAlpha, Colors.Black)))).Draw();
				((Text)new UIResText(Items[c].Text, base.SafeSize.AddPoints(new PointF(hasBothBadges ? 60 : (hasAnyBadge ? 30 : 6), 5f + (itemSize.Height + 3f) * (float)i)), 0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Colors.Black : Colors.White))).Draw();
				if (hasLeftBadge && !hasRightBadge)
				{
					new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge), UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, Index == c && Focused), base.SafeSize.AddPoints(new PointF(-2f, 1f + (itemSize.Height + 3f) * (float)i)), new SizeF(40f, 40f), 0f, UIMenuItem.BadgeToColor(Items[c].LeftBadge, Index == c && Focused)).Draw();
				}
				if (!hasLeftBadge && hasRightBadge)
				{
					new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge), UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, Index == c && Focused), base.SafeSize.AddPoints(new PointF(-2f, 1f + (itemSize.Height + 3f) * (float)i)), new SizeF(40f, 40f), 0f, UIMenuItem.BadgeToColor(Items[c].RightBadge, Index == c && Focused)).Draw();
				}
				if (hasLeftBadge && hasRightBadge)
				{
					new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge), UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, Index == c && Focused), base.SafeSize.AddPoints(new PointF(-2f, 1f + (itemSize.Height + 3f) * (float)i)), new SizeF(40f, 40f), 0f, UIMenuItem.BadgeToColor(Items[c].LeftBadge, Index == c && Focused)).Draw();
					new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge), UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, Index == c && Focused), base.SafeSize.AddPoints(new PointF(25f, 1f + (itemSize.Height + 3f) * (float)i)), new SizeF(40f, 40f), 0f, UIMenuItem.BadgeToColor(Items[c].RightBadge, Index == c && Focused)).Draw();
				}
				if (!string.IsNullOrEmpty(Items[c].RightLabel))
				{
					((Text)new UIResText(Items[c].RightLabel, base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 5f, 5f + (itemSize.Height + 3f) * (float)i)), 0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Colors.Black : Colors.White), (Font)0, (Alignment)2)).Draw();
				}
				if (Items[c] is UIMenuCheckboxItem)
				{
					string textureName = "";
					textureName = ((c != Index || !Focused) ? (((UIMenuCheckboxItem)Items[c]).Checked ? "shop_box_tick" : "shop_box_blank") : (((UIMenuCheckboxItem)Items[c]).Checked ? "shop_box_tickb" : "shop_box_blankb"));
					new Sprite("commonmenu", textureName, base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 60f, -5f + (itemSize.Height + 3f) * (float)i)), new SizeF(50f, 50f)).Draw();
				}
				else
				{
					UIMenuListItem convItem = Items[c] as UIMenuListItem;
					if (convItem != null)
					{
						int yoffset = 5;
						PointF basePos = base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 30f, (float)yoffset + (itemSize.Height + 3f) * (float)i));
						Sprite arrowLeft = new Sprite("commonmenu", "arrowleft", basePos, new SizeF(30f, 30f));
						Sprite arrowRight = new Sprite("commonmenu", "arrowright", basePos, new SizeF(30f, 30f));
						UIResText itemText = new UIResText("", basePos, 0.35f, Colors.White, (Font)0, (Alignment)1)
						{
							TextAlignment = (Alignment)2
						};
						string caption = convItem.Items[convItem.Index].ToString();
						float offset = ScreenTools.GetTextWidth(caption, ((Text)itemText).get_Font(), ((Text)itemText).get_Scale());
						bool selected = c == Index && Focused;
						((Text)itemText).set_Color((!convItem.Enabled) ? Color.FromArgb(163, 159, 148) : (selected ? Colors.Black : Colors.WhiteSmoke));
						((Text)itemText).set_Caption(caption);
						arrowLeft.Color = ((!convItem.Enabled) ? Color.FromArgb(163, 159, 148) : (selected ? Colors.Black : Colors.WhiteSmoke));
						arrowRight.Color = ((!convItem.Enabled) ? Color.FromArgb(163, 159, 148) : (selected ? Colors.Black : Colors.WhiteSmoke));
						arrowLeft.Position = base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 60f - (float)(int)offset, (float)yoffset + (itemSize.Height + 3f) * (float)i));
						if (selected)
						{
							arrowLeft.Draw();
							arrowRight.Draw();
							((Text)itemText).set_Position(base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 30f, (float)yoffset + (itemSize.Height + 3f) * (float)i)));
						}
						else
						{
							((Text)itemText).set_Position(base.SafeSize.AddPoints(new PointF(base.BottomRight.X - base.SafeSize.X - 5f, (float)yoffset + (itemSize.Height + 3f) * (float)i)));
						}
						((Text)itemText).Draw();
					}
				}
				if (Focused && hovering && Game.IsControlJustPressed(0, (Control)237))
				{
					bool open = Index == c;
					Index = (1000 - 1000 % Items.Count + c) % Items.Count;
					if (!open)
					{
						Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
					}
					else if (Items[Index] is UIMenuCheckboxItem)
					{
						Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
						((UIMenuCheckboxItem)Items[Index]).Checked = !((UIMenuCheckboxItem)Items[Index]).Checked;
						((UIMenuCheckboxItem)Items[Index]).CheckboxEventTrigger();
					}
					else
					{
						Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
						Items[Index].ItemActivate(null);
					}
				}
				i++;
			}
		}
	}
}
