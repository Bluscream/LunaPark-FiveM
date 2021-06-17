using System;
using System.Drawing;
using System.Threading.Tasks;

namespace LunaPark
{
	public class UIMenuCheckboxItem : UIMenuItem
	{
		protected Sprite _checkedSprite;

		public UIMenuCheckboxStyle Style { get; set; }

		public bool Checked { get; set; }

		public event ItemCheckboxEvent CheckboxEvent;

		public UIMenuCheckboxItem(string text, bool check)
			: this(text, check, "")
		{
		}

		public UIMenuCheckboxItem(string text, bool check, string description)
			: this(text, UIMenuCheckboxStyle.Tick, check, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public UIMenuCheckboxItem(string text, UIMenuCheckboxStyle style, bool check, string description)
			: this(text, style, check, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public UIMenuCheckboxItem(string text, UIMenuCheckboxStyle style, bool check, string description, Color mainColor, Color highlightColor)
			: base(text, description, mainColor, highlightColor)
		{
			Style = style;
			_checkedSprite = new Sprite("commonmenu", "shop_box_blank", new PointF(410f, 95f), new SizeF(50f, 50f));
			Checked = check;
		}

		public override void Position(int y)
		{
			base.Position(y);
			_checkedSprite.Position = new PointF(380f + base.Offset.X + (float)base.Parent.WidthOffset, (float)(y + 138) + base.Offset.Y);
		}

		public override async Task Draw()
		{
			base.Draw();
			_checkedSprite.Position = new PointF(380f + base.Offset.X + (float)base.Parent.WidthOffset, _checkedSprite.Position.Y);
			_checkedSprite.TextureName = ((!Selected) ? ((!Checked) ? "shop_box_blank" : ((Style == UIMenuCheckboxStyle.Tick) ? "shop_box_tick" : "shop_box_cross")) : ((!Checked) ? "shop_box_blankb" : ((Style == UIMenuCheckboxStyle.Tick) ? "shop_box_tickb" : "shop_box_crossb")));
			_checkedSprite.Draw();
		}

		public void CheckboxEventTrigger()
		{
			this.CheckboxEvent?.Invoke(this, Checked);
		}

		public override void SetRightBadge(BadgeStyle badge)
		{
			throw new Exception("UIMenuCheckboxItem cannot have a right badge.");
		}

		public override void SetRightLabel(string text)
		{
			throw new Exception("UIMenuListItem cannot have a right label.");
		}
	}
}
