using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuListItem : UIMenuItem, IListItem
	{
		protected UIResText _itemText;

		protected Sprite _arrowLeft;

		protected Sprite _arrowRight;

		protected int _index;

		protected List<dynamic> _items;

		public List<UIMenuPanel> Panels = new List<UIMenuPanel>();

		public int Index
		{
			get
			{
				return _index % Items.Count;
			}
			set
			{
				_index = 100000000 - 100000000 % Items.Count + value;
			}
		}

		public List<object> Items
		{
			get
			{
				return _items;
			}
			set
			{
				Index = 0;
				_items = value;
			}
		}

		public event ItemListEvent OnListChanged;

		public event ItemListEvent OnListSelected;

		public UIMenuListItem(string text, List<dynamic> items, int index)
			: this(text, items, index, "")
		{
		}

		public UIMenuListItem(string text, List<dynamic> items, int index, string description)
			: this(text, items, index, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public UIMenuListItem(string text, List<dynamic> items, int index, string description, Color mainColor, Color higlightColor)
			: base(text, description, mainColor, higlightColor)
		{
			_items = items;
			_arrowLeft = new Sprite("commonmenu", "arrowleft", new PointF(110f, 105f), new SizeF(30f, 30f));
			_arrowRight = new Sprite("commonmenu", "arrowright", new PointF(280f, 105f), new SizeF(30f, 30f));
			_itemText = new UIResText("", new PointF(290f, 104f), 0.35f, Colors.White, (Font)0, (Alignment)1)
			{
				TextAlignment = (Alignment)2
			};
			Index = index;
		}

		public override void Position(int y)
		{
			_arrowLeft.Position = new PointF(300f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(147 + y) + base.Offset.Y);
			_arrowRight.Position = new PointF(400f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(147 + y) + base.Offset.Y);
			((Text)_itemText).set_Position(new PointF(300f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(y + 147) + base.Offset.Y));
			base.Position(y);
		}

		[Obsolete("Use UIMenuListItem.Items.FindIndex(p => ReferenceEquals(p, item)) instead.")]
		public virtual int ItemToIndex(dynamic item)
		{
			return _items.FindIndex((dynamic p) => ReferenceEquals(p, item));
		}

		[Obsolete("Use UIMenuListItem.Items[Index] instead.")]
		public virtual dynamic IndexToItem(int index)
		{
			return _items[index];
		}

		public override async Task Draw()
		{
			base.Draw();
			string caption = _items[Index].ToString();
			float offset = ScreenTools.GetTextWidth(caption, ((Text)_itemText).get_Font(), ((Text)_itemText).get_Scale());
			((Text)_itemText).set_Color((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			((Text)_itemText).set_Caption(caption);
			_arrowLeft.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			_arrowRight.Color = ((!Enabled) ? Color.FromArgb(163, 159, 148) : (Selected ? Colors.Black : Colors.WhiteSmoke));
			_arrowLeft.Position = new PointF((float)(375 - (int)offset) + base.Offset.X + (float)base.Parent.WidthOffset, _arrowLeft.Position.Y);
			if (Selected)
			{
				_arrowLeft.Draw();
				_arrowRight.Draw();
				((Text)_itemText).set_Position(new PointF(403f + base.Offset.X + (float)base.Parent.WidthOffset, ((Text)_itemText).get_Position().Y));
			}
			else
			{
				((Text)_itemText).set_Position(new PointF(418f + base.Offset.X + (float)base.Parent.WidthOffset, ((Text)_itemText).get_Position().Y));
			}
			((Text)_itemText).Draw();
		}

		internal virtual void ListChangedTrigger(int newindex)
		{
			this.OnListChanged?.Invoke(this, newindex);
		}

		internal virtual void ListSelectedTrigger(int newindex)
		{
			this.OnListSelected?.Invoke(this, newindex);
		}

		public override void SetRightBadge(BadgeStyle badge)
		{
			throw new Exception("UIMenuListItem cannot have a right badge.");
		}

		public override void SetRightLabel(string text)
		{
			throw new Exception("UIMenuListItem cannot have a right label.");
		}

		public virtual void AddPanel(UIMenuPanel panel)
		{
			Panels.Add(panel);
			panel.SetParentItem(this);
		}

		public virtual void RemovePanelAt(int Index)
		{
			Panels.RemoveAt(Index);
		}

		[Obsolete("Use UIMenuListItem.Items[Index].ToString() instead.")]
		public string CurrentItem()
		{
			return _items[Index].ToString();
		}
	}
}
