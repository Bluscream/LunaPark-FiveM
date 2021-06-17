using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuItem
	{
		public enum BadgeStyle
		{
			None,
			BronzeMedal,
			GoldMedal,
			SilverMedal,
			Alert,
			Crown,
			Ammo,
			Armour,
			Barber,
			Clothes,
			Franklin,
			Bike,
			Car,
			Gun,
			Heart,
			Makeup,
			Mask,
			Michael,
			Star,
			Tatoo,
			Trevor,
			Lock,
			Tick,
			Sale,
			ArrowLeft,
			ArrowRight,
			Audio1,
			Audio2,
			Audio3,
			AudioInactive,
			AudioMute
		}

		internal UIResRectangle _rectangle;

		protected UIResText _text;

		protected Sprite _selectedSprite;

		protected Sprite _badgeLeft;

		protected Sprite _badgeRight;

		protected UIResText _labelText;

		private readonly Color _defaultColor = Color.FromArgb(20, 255, 255, 255);

		private readonly Color _disabledColor = Color.FromArgb(163, 159, 148);

		public Color MainColor { get; set; }

		public Color HighlightColor { get; set; }

		public Color TextColor { get; set; }

		public Color HighlightedTextColor { get; set; }

		public virtual bool Selected { get; set; }

		public virtual bool Hovered { get; set; }

		public virtual string Description { get; set; }

		public virtual bool Enabled { get; set; }

		public PointF Offset { get; set; }

		public string Text
		{
			get
			{
				return ((Text)_text).get_Caption();
			}
			set
			{
				((Text)_text).set_Caption(value);
			}
		}

		public virtual string RightLabel { get; private set; }

		public virtual BadgeStyle LeftBadge { get; private set; }

		public virtual BadgeStyle RightBadge { get; private set; }

		public UIMenu Parent { get; set; }

		public event ItemActivatedEvent Activated;

		public UIMenuItem(string text)
			: this(text, "", Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public UIMenuItem(string text, string description)
			: this(text, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public UIMenuItem(string text, string description, Color color, Color highlightColor)
		{
			Enabled = true;
			MainColor = color;
			HighlightColor = highlightColor;
			TextColor = Colors.White;
			HighlightedTextColor = Colors.Black;
			_rectangle = new UIResRectangle(new Point(0, 0), new Size(431, 38), _defaultColor);
			_text = new UIResText(text, new Point(8, 0), 0.33f, Colors.WhiteSmoke, (Font)0, (Alignment)1);
			Description = description;
			_selectedSprite = new Sprite("commonmenu", "gradient_nav", new Point(0, 0), new Size(431, 38));
			_badgeLeft = new Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
			_badgeRight = new Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
			_labelText = new UIResText("", new Point(0, 0), 0.35f)
			{
				TextAlignment = (Alignment)2
			};
		}

		internal virtual void ItemActivate(UIMenu sender)
		{
			this.Activated?.Invoke(sender, this);
		}

		public virtual void Position(int y)
		{
			((Rectangle)_rectangle).set_Position(new PointF(Offset.X, (float)(y + 144) + Offset.Y));
			_selectedSprite.Position = new PointF(0f + Offset.X, (float)(y + 144) + Offset.Y);
			((Text)_text).set_Position(new PointF(8f + Offset.X, (float)(y + 147) + Offset.Y));
			_badgeLeft.Position = new PointF(0f + Offset.X, (float)(y + 142) + Offset.Y);
			_badgeRight.Position = new PointF(385f + Offset.X, (float)(y + 142) + Offset.Y);
			((Text)_labelText).set_Position(new PointF(420f + Offset.X, (float)(y + 148) + Offset.Y));
		}

		public virtual async Task Draw()
		{
			if (Hovered && !Selected)
			{
				((Rectangle)_rectangle).Draw();
			}
			if (Selected)
			{
				_selectedSprite.Color = HighlightColor;
				_selectedSprite.Draw();
			}
			else
			{
				_selectedSprite.Color = MainColor;
				_selectedSprite.Draw();
			}
			((Text)_text).set_Color((!Enabled) ? _disabledColor : (Selected ? HighlightedTextColor : TextColor));
			if (LeftBadge == BadgeStyle.None)
			{
				((Text)_text).set_Position(new PointF(8f + Offset.X, ((Text)_text).get_Position().Y));
			}
			else
			{
				((Text)_text).set_Position(new PointF(35f + Offset.X, ((Text)_text).get_Position().Y));
				_badgeLeft.TextureDict = BadgeToSpriteLib(LeftBadge);
				_badgeLeft.TextureName = BadgeToSpriteName(LeftBadge, Selected);
				_badgeLeft.Color = BadgeToColor(LeftBadge, Selected);
				_badgeLeft.Draw();
			}
			if (RightBadge != 0)
			{
				_badgeRight.Position = new PointF(385f + Offset.X + (float)Parent.WidthOffset, _badgeRight.Position.Y);
				_badgeRight.TextureDict = BadgeToSpriteLib(RightBadge);
				_badgeRight.TextureName = BadgeToSpriteName(RightBadge, Selected);
				_badgeRight.Color = BadgeToColor(RightBadge, Selected);
				_badgeRight.Draw();
			}
			if (!string.IsNullOrWhiteSpace(RightLabel))
			{
				if (RightBadge == BadgeStyle.None)
				{
					((Text)_labelText).set_Position(new PointF(420f + Offset.X + (float)Parent.WidthOffset, ((Text)_labelText).get_Position().Y));
				}
				else
				{
					((Text)_labelText).set_Position(new PointF(390f + Offset.X + (float)Parent.WidthOffset, ((Text)_labelText).get_Position().Y));
				}
				((Text)_labelText).set_Caption(RightLabel);
				UIResText labelText = _labelText;
				Color color;
				((Text)_text).set_Color(color = ((!Enabled) ? _disabledColor : (Selected ? Colors.Black : Colors.WhiteSmoke)));
				((Text)labelText).set_Color(color);
				((Text)_labelText).Draw();
			}
			((Text)_text).Draw();
		}

		public virtual void SetLeftBadge(BadgeStyle badge)
		{
			LeftBadge = badge;
		}

		public virtual void SetRightBadge(BadgeStyle badge)
		{
			RightBadge = badge;
		}

		public virtual void SetRightLabel(string text)
		{
			RightLabel = text;
		}

		internal static string BadgeToSpriteLib(BadgeStyle badge)
		{
			switch (badge)
			{
			case BadgeStyle.Sale:
				return "mpshopsale";
			case BadgeStyle.Audio1:
			case BadgeStyle.Audio2:
			case BadgeStyle.Audio3:
			case BadgeStyle.AudioInactive:
			case BadgeStyle.AudioMute:
				return "mpleaderboard";
			default:
				return "commonmenu";
			}
		}

		internal static string BadgeToSpriteName(BadgeStyle badge, bool selected)
		{
			return badge switch
			{
				BadgeStyle.None => "", 
				BadgeStyle.BronzeMedal => "mp_medal_bronze", 
				BadgeStyle.GoldMedal => "mp_medal_gold", 
				BadgeStyle.SilverMedal => "medal_silver", 
				BadgeStyle.Alert => "mp_alerttriangle", 
				BadgeStyle.Crown => "mp_hostcrown", 
				BadgeStyle.Ammo => selected ? "shop_ammo_icon_b" : "shop_ammo_icon_a", 
				BadgeStyle.Armour => selected ? "shop_armour_icon_b" : "shop_armour_icon_a", 
				BadgeStyle.Barber => selected ? "shop_barber_icon_b" : "shop_barber_icon_a", 
				BadgeStyle.Clothes => selected ? "shop_clothing_icon_b" : "shop_clothing_icon_a", 
				BadgeStyle.Franklin => selected ? "shop_franklin_icon_b" : "shop_franklin_icon_a", 
				BadgeStyle.Bike => selected ? "shop_garage_bike_icon_b" : "shop_garage_bike_icon_a", 
				BadgeStyle.Car => selected ? "shop_garage_icon_b" : "shop_garage_icon_a", 
				BadgeStyle.Gun => selected ? "shop_gunclub_icon_b" : "shop_gunclub_icon_a", 
				BadgeStyle.Heart => selected ? "shop_health_icon_b" : "shop_health_icon_a", 
				BadgeStyle.Lock => "shop_lock", 
				BadgeStyle.Makeup => selected ? "shop_makeup_icon_b" : "shop_makeup_icon_a", 
				BadgeStyle.Mask => selected ? "shop_mask_icon_b" : "shop_mask_icon_a", 
				BadgeStyle.Michael => selected ? "shop_michael_icon_b" : "shop_michael_icon_a", 
				BadgeStyle.Star => "shop_new_star", 
				BadgeStyle.Tatoo => selected ? "shop_tattoos_icon_b" : "shop_tattoos_icon_", 
				BadgeStyle.Tick => "shop_tick_icon", 
				BadgeStyle.Trevor => selected ? "shop_trevor_icon_b" : "shop_trevor_icon_a", 
				BadgeStyle.Sale => "saleicon", 
				BadgeStyle.ArrowLeft => "arrowleft", 
				BadgeStyle.ArrowRight => "arrowright", 
				BadgeStyle.Audio1 => "leaderboard_audio_1", 
				BadgeStyle.Audio2 => "leaderboard_audio_2", 
				BadgeStyle.Audio3 => "leaderboard_audio_3", 
				BadgeStyle.AudioInactive => "leaderboard_audio_inactive", 
				BadgeStyle.AudioMute => "leaderboard_audio_mute", 
				_ => "", 
			};
		}

		internal static Color BadgeToColor(BadgeStyle badge, bool selected)
		{
			if (badge == BadgeStyle.Crown || (uint)(badge - 21) <= 1u)
			{
				return selected ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
			}
			return Color.FromArgb(255, 255, 255, 255);
		}
	}
}
