using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenu
	{
		public enum MenuControls
		{
			Up,
			Down,
			Left,
			Right,
			Select,
			Back
		}

		private readonly Container _mainMenu;

		private readonly Sprite _background;

		private readonly UIResRectangle _descriptionBar;

		private readonly Sprite _descriptionRectangle;

		private readonly UIResText _descriptionText;

		private readonly UIResText _counterText;

		private int _activeItem = 1000;

		private bool _visible;

		private bool _buttonsEnabled = true;

		private bool _justOpened = true;

		private bool _itemsDirty = false;

		private const int MaxItemsOnScreen = 9;

		private int _minItem;

		private int _maxItem = 9;

		private readonly Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<Control, int>>>> _keyDictionary = new Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<Control, int>>>>();

		private readonly List<InstructionalButton> _instructionalButtons = new List<InstructionalButton>();

		private readonly Sprite _upAndDownSprite;

		private readonly UIResRectangle _extraRectangleUp;

		private readonly UIResRectangle _extraRectangleDown;

		private readonly Scaleform _instructionalButtonsScaleform;

		private readonly Scaleform _glareScaleform;

		private readonly int _extraYOffset;

		private static readonly MenuControls[] _menuControls = Enum.GetValues(typeof(MenuControls)).Cast<MenuControls>().ToArray();

		private float PanelOffset = 0f;

		private SizeF GlareSize;

		private PointF GlarePosition;

		private bool ReDraw = true;

		internal static readonly string _selectTextLocalized = Game.GetGXTEntry("HUD_INPUT2");

		internal static readonly string _backTextLocalized = Game.GetGXTEntry("HUD_INPUT3");

		public string AUDIO_LIBRARY = "HUD_FRONTEND_DEFAULT_SOUNDSET";

		public string AUDIO_UPDOWN = "NAV_UP_DOWN";

		public string AUDIO_LEFTRIGHT = "NAV_LEFT_RIGHT";

		public string AUDIO_SELECT = "SELECT";

		public string AUDIO_BACK = "BACK";

		public string AUDIO_ERROR = "ERROR";

		public List<UIMenuItem> MenuItems = new List<UIMenuItem>();

		public bool MouseEdgeEnabled = false;

		public bool ControlDisablingEnabled = true;

		public bool ResetCursorOnOpen = false;

		[Obsolete("The description is now formated automatically by the game.")]
		public bool FormatDescriptions = true;

		public bool MouseControlsEnabled = true;

		public bool ScaleWithSafezone = true;

		public List<UIMenuHeritageWindow> Windows = new List<UIMenuHeritageWindow>();

		private int _controlCounter;

		private PointF Safe { get; set; }

		private SizeF BackgroundSize { get; set; }

		private SizeF DrawWidth { get; set; }

		public PointF Offset { get; }

		public Sprite BannerSprite { get; private set; }

		public UIResRectangle BannerRectangle { get; private set; }

		public string BannerTexture { get; private set; }

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if (value)
				{
					MenuOpenEv();
				}
				else
				{
					MenuCloseEv();
				}
				_visible = value;
				_justOpened = value;
				_itemsDirty = value;
				UpdateScaleform();
				if (ParentMenu == null && value && ResetCursorOnOpen)
				{
					API.SetCursorLocation(0.5f, 0.5f);
					Hud.set_CursorSprite((CursorSprite)1);
				}
			}
		}

		public int CurrentSelection
		{
			get
			{
				return (MenuItems.Count != 0) ? (_activeItem % MenuItems.Count) : 0;
			}
			set
			{
				if (MenuItems.Count == 0)
				{
					_activeItem = 0;
				}
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem = 1000000 - 1000000 % MenuItems.Count + value;
				if (CurrentSelection > _maxItem)
				{
					_maxItem = CurrentSelection;
					_minItem = CurrentSelection - 9;
				}
				else if (CurrentSelection < _minItem)
				{
					_maxItem = 9 + CurrentSelection;
					_minItem = CurrentSelection;
				}
			}
		}

		public static bool IsUsingController => !API.IsInputDisabled(2);

		public int Size => MenuItems.Count;

		public UIResText Title { get; }

		public UIResText Subtitle { get; }

		public string CounterPretext { get; set; }

		public UIMenu ParentMenu { get; set; }

		public UIMenuItem ParentItem { get; set; }

		public Dictionary<UIMenuItem, UIMenu> Children { get; }

		public int WidthOffset { get; private set; }

		public event IndexChangedEvent OnIndexChange;

		public event ListChangedEvent OnListChange;

		public event ListSelectedEvent OnListSelect;

		public event SliderChangedEvent OnSliderChange;

		public event ProgressSliderChangedEvent OnProgressSliderChange;

		public event OnProgressChanged OnProgressChange;

		public event OnProgressSelected OnProgressSelect;

		public event CheckboxChangeEvent OnCheckboxChange;

		public event ItemSelectEvent OnItemSelect;

		public event MenuOpenEvent OnMenuOpen;

		public event MenuCloseEvent OnMenuClose;

		public event MenuChangeEvent OnMenuChange;

		public UIMenu(string title, string subtitle)
			: this(title, subtitle, new PointF(0f, 0f), "commonmenu", "interaction_bgd")
		{
		}

		public UIMenu(string title, string subtitle, PointF offset)
			: this(title, subtitle, offset, "commonmenu", "interaction_bgd")
		{
		}

		public UIMenu(string title, string subtitle, PointF offset, string customBanner)
			: this(title, subtitle, offset, "commonmenu", "interaction_bgd")
		{
			BannerTexture = customBanner;
		}

		public UIMenu(string title, string subtitle, PointF offset, string spriteLibrary, string spriteName)
		{
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Expected O, but got Unknown
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Expected O, but got Unknown
			Offset = offset;
			Children = new Dictionary<UIMenuItem, UIMenu>();
			WidthOffset = 0;
			_instructionalButtonsScaleform = new Scaleform("instructional_buttons");
			_glareScaleform = new Scaleform("MP_MENU_GLARE");
			UpdateScaleform();
			_mainMenu = new Container(new PointF(0f, 0f), new SizeF(700f, 500f), Color.FromArgb(0, 0, 0, 0));
			BannerSprite = new Sprite(spriteLibrary, spriteName, new PointF(0f + Offset.X, 0f + Offset.Y), new SizeF(431f, 100f));
			_mainMenu.get_Items().Add((IElement)(object)(Title = new UIResText(title, new PointF(215f + Offset.X, 13f + Offset.Y), 1.05f, Colors.White, (Font)1, (Alignment)0)));
			if (!string.IsNullOrWhiteSpace(subtitle))
			{
				_mainMenu.get_Items().Add((IElement)(object)new UIResRectangle(new PointF(0f + offset.X, 100f + Offset.Y), new SizeF(431f, 37f), Colors.Black));
				_mainMenu.get_Items().Add((IElement)(object)(Subtitle = new UIResText(subtitle, new PointF(8f + Offset.X, 103f + Offset.Y), 0.35f, Colors.WhiteSmoke, (Font)0, (Alignment)1)));
				if (subtitle.StartsWith("~"))
				{
					CounterPretext = subtitle.Substring(0, 3);
				}
				_counterText = new UIResText("", new PointF(425f + Offset.X, 103f + Offset.Y), 0.35f, Colors.WhiteSmoke, (Font)0, (Alignment)2);
				_extraYOffset = 30;
			}
			_upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new PointF(190f + Offset.X, 517f + Offset.Y - 37f + (float)_extraYOffset), new SizeF(50f, 50f));
			_extraRectangleUp = new UIResRectangle(new PointF(0f + Offset.X, 524f + Offset.Y - 37f + (float)_extraYOffset), new SizeF(431f, 18f), Color.FromArgb(200, 0, 0, 0));
			_extraRectangleDown = new UIResRectangle(new PointF(0f + Offset.X, 542f + Offset.Y - 37f + (float)_extraYOffset), new SizeF(431f, 18f), Color.FromArgb(200, 0, 0, 0));
			_descriptionBar = new UIResRectangle(new PointF(Offset.X, 123f), new SizeF(431f, 4f), Colors.Black);
			_descriptionRectangle = new Sprite("commonmenu", "gradient_bgd", new PointF(Offset.X, 127f), new SizeF(431f, 30f));
			_descriptionText = new UIResText("Description", new PointF(Offset.X + 5f, 125f), 0.35f, Color.FromArgb(255, 255, 255, 255), (Font)0, (Alignment)1);
			_background = new Sprite("commonmenu", "gradient_bgd", new PointF(Offset.X, 144f + Offset.Y - 37f + (float)_extraYOffset), new SizeF(290f, 25f));
			SetKey(MenuControls.Up, (Control)172);
			SetKey(MenuControls.Up, (Control)241);
			SetKey(MenuControls.Down, (Control)173);
			SetKey(MenuControls.Down, (Control)242);
			SetKey(MenuControls.Left, (Control)174);
			SetKey(MenuControls.Right, (Control)175);
			SetKey(MenuControls.Select, (Control)201);
			SetKey(MenuControls.Back, (Control)177);
			SetKey(MenuControls.Back, (Control)199);
		}

		[Obsolete("Use Controls.Toggle instead.", true)]
		public static void DisEnableControls(bool toggle)
		{
			Controls.Toggle(toggle);
		}

		[Obsolete("Use ScreenTools.ResolutionMaintainRatio instead.", true)]
		public static SizeF GetScreenResolutionMaintainRatio()
		{
			return ScreenTools.ResolutionMaintainRatio;
		}

		[Obsolete("Use ScreenTools.ResolutionMaintainRatio instead.", true)]
		public static SizeF GetScreenResiolutionMantainRatio()
		{
			return ScreenTools.ResolutionMaintainRatio;
		}

		[Obsolete("Use ScreenTools.IsMouseInBounds instead.", true)]
		public static bool IsMouseInBounds(Point topLeft, Size boxSize)
		{
			return ScreenTools.IsMouseInBounds(topLeft, boxSize);
		}

		[Obsolete("Use ScreenTools.SafezoneBounds instead.", true)]
		public static PointF GetSafezoneBounds()
		{
			return ScreenTools.SafezoneBounds;
		}

		public void SetMenuWidthOffset(int widthOffset)
		{
			WidthOffset = widthOffset;
			BannerSprite.Size = new SizeF(431 + WidthOffset, 100f);
			_mainMenu.get_Items()[0].set_Position(new PointF(((float)WidthOffset + Offset.X + 431f) / 2f, 20f + Offset.Y));
			((Text)_counterText).set_Position(new PointF(425f + Offset.X + (float)widthOffset, 110f + Offset.Y));
			if (_mainMenu.get_Items().Count >= 1)
			{
				UIResRectangle tmp = (UIResRectangle)(object)_mainMenu.get_Items()[1];
				((Rectangle)tmp).set_Size(new SizeF(431 + WidthOffset, 37f));
			}
			if (BannerRectangle != null)
			{
				((Rectangle)BannerRectangle).set_Size(new SizeF(431 + WidthOffset, 100f));
			}
		}

		public void DisableInstructionalButtons(bool disable)
		{
			_buttonsEnabled = !disable;
		}

		public void SetBannerType(Sprite spriteBanner)
		{
			BannerSprite = spriteBanner;
			BannerSprite.Size = new SizeF(431 + WidthOffset, 100f);
			BannerSprite.Position = new PointF(Offset.X, Offset.Y);
		}

		public void SetBannerType(UIResRectangle rectangle)
		{
			BannerSprite = null;
			BannerRectangle = rectangle;
			((Rectangle)BannerRectangle).set_Position(new PointF(Offset.X, Offset.Y));
			((Rectangle)BannerRectangle).set_Size(new SizeF(431 + WidthOffset, 100f));
		}

		public void SetBannerType(string pathToCustomSprite)
		{
			BannerTexture = pathToCustomSprite;
		}

		public void AddItem(UIMenuItem item)
		{
			int selectedItem = CurrentSelection;
			item.Offset = Offset;
			item.Parent = this;
			item.Position(MenuItems.Count * 25 - 37 + _extraYOffset);
			MenuItems.Add(item);
			ReDraw = true;
			CurrentSelection = selectedItem;
		}

		public void AddWindow(UIMenuHeritageWindow window)
		{
			window.ParentMenu = this;
			window.Offset = Offset;
			Windows.Add(window);
			ReDraw = true;
		}

		public void RemoveWindowAt(int index)
		{
			Windows.RemoveAt(index);
			ReDraw = true;
		}

		public void UpdateDescription()
		{
			ReDraw = true;
		}

		public void RemoveItemAt(int index)
		{
			int selectedItem = CurrentSelection;
			if (Size > 9 && _maxItem == Size - 1)
			{
				_maxItem--;
				_minItem--;
			}
			MenuItems.RemoveAt(index);
			ReDraw = true;
			CurrentSelection = selectedItem;
		}

		public void RefreshIndex()
		{
			if (MenuItems.Count == 0)
			{
				_activeItem = 1000;
				_maxItem = 9;
				_minItem = 0;
			}
			else
			{
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem = 1000 - 1000 % MenuItems.Count;
				_maxItem = 9;
				_minItem = 0;
				ReDraw = true;
			}
		}

		public void Clear()
		{
			MenuItems.Clear();
			ReDraw = true;
		}

		public void Remove(Func<UIMenuItem, bool> predicate)
		{
			List<UIMenuItem> TempList = new List<UIMenuItem>(MenuItems);
			foreach (UIMenuItem item in TempList)
			{
				if (predicate(item))
				{
					MenuItems.Remove(item);
				}
			}
			ReDraw = true;
		}

		private float CalculateWindowHeight()
		{
			float height = 0f;
			if (Windows.Count > 0)
			{
				for (int i = 0; i < Windows.Count; i++)
				{
					height += Windows[i].Background.Size.Height;
				}
			}
			return height;
		}

		private float CalculateItemHeight()
		{
			float ItemOffset = 0f + _mainMenu.get_Items()[1].get_Position().Y - 37f;
			for (int i = 0; i < MenuItems.Count; i++)
			{
				ItemOffset += ((Rectangle)MenuItems[i]._rectangle).get_Size().Height;
			}
			return ItemOffset;
		}

		private float CalculatePanelsPosition(bool hasDescription)
		{
			float Height = CalculateWindowHeight() + 40f + ((Rectangle)_mainMenu).get_Position().Y + CalculateCinematicHeight();
			if (hasDescription)
			{
				Height += _descriptionRectangle.Size.Height + 5f;
			}
			return CalculateItemHeight() + Height;
		}

		private void DrawCalculations()
		{
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			float WindowHeight = CalculateWindowHeight();
			float CinematicHeight = CalculateCinematicHeight();
			DrawWidth = new SizeF(431 + WidthOffset, 100f);
			Safe = ScreenTools.SafezoneBounds;
			BackgroundSize = ((Size > 10) ? new SizeF(431 + WidthOffset, 38f * (10f + WindowHeight + CinematicHeight)) : new SizeF(431 + WidthOffset, (float)(38 * Size) + WindowHeight));
			((Rectangle)_extraRectangleUp).set_Size(new SizeF(431 + WidthOffset, 18f + WindowHeight + CinematicHeight));
			((Rectangle)_extraRectangleDown).set_Size(new SizeF(431 + WidthOffset, 18f + WindowHeight + CinematicHeight));
			_upAndDownSprite.Position = new PointF(190f + Offset.X + (float)((WidthOffset > 0) ? (WidthOffset / 2) : WidthOffset), 517f + Offset.Y - 37f + (float)_extraYOffset + WindowHeight + CinematicHeight);
			ReDraw = false;
			if (MenuItems.Count != 0 && !string.IsNullOrWhiteSpace(MenuItems[_activeItem % MenuItems.Count].Description))
			{
				RecalculateDescriptionPosition();
				string descCaption = MenuItems[_activeItem % MenuItems.Count].Description;
				((Text)_descriptionText).set_Caption(descCaption);
				_descriptionText.Wrap = 400f;
				int numLines = ScreenTools.GetLineCount(descCaption, ((Text)_descriptionText).get_Position(), ((Text)_descriptionText).get_Font(), ((Text)_descriptionText).get_Scale(), ((Text)_descriptionText).get_Position().X + 400f);
				_descriptionRectangle.Size = new SizeF(431 + WidthOffset, numLines * 25 + 15);
			}
		}

		public async Task GoUpOverflow()
		{
			if (Size <= 10)
			{
				return;
			}
			if (_activeItem % MenuItems.Count <= _minItem)
			{
				if (_activeItem % MenuItems.Count == 0)
				{
					_minItem = MenuItems.Count - 9 - 1;
					_maxItem = MenuItems.Count - 1;
					MenuItems[_activeItem % MenuItems.Count].Selected = false;
					_activeItem = 1000 - 1000 % MenuItems.Count;
					_activeItem += MenuItems.Count - 1;
					MenuItems[_activeItem % MenuItems.Count].Selected = true;
				}
				else
				{
					_minItem--;
					_maxItem--;
					MenuItems[_activeItem % MenuItems.Count].Selected = false;
					_activeItem--;
					MenuItems[_activeItem % MenuItems.Count].Selected = true;
				}
			}
			else
			{
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem--;
				MenuItems[_activeItem % MenuItems.Count].Selected = true;
			}
			Game.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
			IndexChange(CurrentSelection);
			await BaseScript.Delay(75);
		}

		public async Task GoUp()
		{
			if (Size <= 10)
			{
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem--;
				MenuItems[_activeItem % MenuItems.Count].Selected = true;
				Game.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
				IndexChange(CurrentSelection);
				await BaseScript.Delay(75);
			}
		}

		public async Task GoDownOverflow()
		{
			if (Size <= 10)
			{
				return;
			}
			if (_activeItem % MenuItems.Count >= _maxItem)
			{
				if (_activeItem % MenuItems.Count == MenuItems.Count - 1)
				{
					_minItem = 0;
					_maxItem = 9;
					MenuItems[_activeItem % MenuItems.Count].Selected = false;
					_activeItem = 1000 - 1000 % MenuItems.Count;
					MenuItems[_activeItem % MenuItems.Count].Selected = true;
				}
				else
				{
					_minItem++;
					_maxItem++;
					MenuItems[_activeItem % MenuItems.Count].Selected = false;
					_activeItem++;
					MenuItems[_activeItem % MenuItems.Count].Selected = true;
				}
			}
			else
			{
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem++;
				MenuItems[_activeItem % MenuItems.Count].Selected = true;
			}
			Game.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
			IndexChange(CurrentSelection);
			await BaseScript.Delay(75);
		}

		public async Task GoDown()
		{
			if (Size <= 10)
			{
				MenuItems[_activeItem % MenuItems.Count].Selected = false;
				_activeItem++;
				MenuItems[_activeItem % MenuItems.Count].Selected = true;
				Game.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
				IndexChange(CurrentSelection);
				await BaseScript.Delay(75);
			}
		}

		public async Task GoLeft()
		{
			if (MenuItems[CurrentSelection].Enabled && (MenuItems[CurrentSelection] is UIMenuListItem || MenuItems[CurrentSelection] is UIMenuSliderItem || MenuItems[CurrentSelection] is UIMenuDynamicListItem || MenuItems[CurrentSelection] is UIMenuSliderProgressItem || MenuItems[CurrentSelection] is UIMenuProgressItem))
			{
				if (MenuItems[CurrentSelection] is UIMenuListItem)
				{
					UIMenuListItem it = (UIMenuListItem)MenuItems[CurrentSelection];
					it.Index--;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					ListChange(it, it.Index);
					it.ListChangedTrigger(it.Index);
				}
				else if (MenuItems[CurrentSelection] is UIMenuDynamicListItem)
				{
					UIMenuDynamicListItem it3 = (UIMenuDynamicListItem)MenuItems[CurrentSelection];
					string newItem = (it3.CurrentListItem = it3.Callback(it3, UIMenuDynamicListItem.ChangeDirection.Left));
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
				}
				else if (MenuItems[CurrentSelection] is UIMenuSliderItem)
				{
					UIMenuSliderItem it5 = (UIMenuSliderItem)MenuItems[CurrentSelection];
					it5.Value -= it5.Multiplier;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					SliderChange(it5, it5.Value);
				}
				else if (MenuItems[CurrentSelection] is UIMenuSliderProgressItem)
				{
					UIMenuSliderProgressItem it4 = (UIMenuSliderProgressItem)MenuItems[CurrentSelection];
					it4.Value -= it4.Multiplier;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					SliderProgressChange(it4, it4.Value);
				}
				else if (MenuItems[CurrentSelection] is UIMenuProgressItem)
				{
					UIMenuProgressItem it2 = (UIMenuProgressItem)MenuItems[CurrentSelection];
					it2.Index--;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					ProgressChange(it2, it2.Index);
				}
				await BaseScript.Delay(75);
			}
		}

		public async Task GoRight()
		{
			if (MenuItems[CurrentSelection].Enabled && (MenuItems[CurrentSelection] is UIMenuListItem || MenuItems[CurrentSelection] is UIMenuSliderItem || MenuItems[CurrentSelection] is UIMenuDynamicListItem || MenuItems[CurrentSelection] is UIMenuSliderProgressItem || MenuItems[CurrentSelection] is UIMenuProgressItem))
			{
				if (MenuItems[CurrentSelection] is UIMenuListItem)
				{
					UIMenuListItem it = (UIMenuListItem)MenuItems[CurrentSelection];
					it.Index++;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					ListChange(it, it.Index);
					it.ListChangedTrigger(it.Index);
				}
				else if (MenuItems[CurrentSelection] is UIMenuDynamicListItem)
				{
					UIMenuDynamicListItem it3 = (UIMenuDynamicListItem)MenuItems[CurrentSelection];
					string newItem = (it3.CurrentListItem = it3.Callback(it3, UIMenuDynamicListItem.ChangeDirection.Right));
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
				}
				else if (MenuItems[CurrentSelection] is UIMenuSliderItem)
				{
					UIMenuSliderItem it5 = (UIMenuSliderItem)MenuItems[CurrentSelection];
					it5.Value += it5.Multiplier;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					SliderChange(it5, it5.Value);
				}
				else if (MenuItems[CurrentSelection] is UIMenuSliderProgressItem)
				{
					UIMenuSliderProgressItem it4 = (UIMenuSliderProgressItem)MenuItems[CurrentSelection];
					it4.Value += it4.Multiplier;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					SliderProgressChange(it4, it4.Value);
				}
				else if (MenuItems[CurrentSelection] is UIMenuProgressItem)
				{
					UIMenuProgressItem it2 = (UIMenuProgressItem)MenuItems[CurrentSelection];
					it2.Index++;
					Game.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
					ProgressChange(it2, it2.Index);
				}
				await BaseScript.Delay(75);
			}
		}

		public void SelectItem()
		{
			if (!MenuItems[CurrentSelection].Enabled)
			{
				Game.PlaySound(AUDIO_ERROR, AUDIO_LIBRARY);
				return;
			}
			if (MenuItems[CurrentSelection] is UIMenuCheckboxItem)
			{
				UIMenuCheckboxItem it2 = (UIMenuCheckboxItem)MenuItems[CurrentSelection];
				it2.Checked = !it2.Checked;
				Game.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
				CheckboxChange(it2, it2.Checked);
				it2.CheckboxEventTrigger();
				return;
			}
			if (MenuItems[CurrentSelection] is UIMenuListItem)
			{
				UIMenuListItem it = (UIMenuListItem)MenuItems[CurrentSelection];
				Game.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
				ListSelect(it, it.Index);
				it.ListSelectedTrigger(it.Index);
				return;
			}
			Game.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
			ItemSelect(MenuItems[CurrentSelection], CurrentSelection);
			MenuItems[CurrentSelection].ItemActivate(this);
			if (Children.ContainsKey(MenuItems[CurrentSelection]))
			{
				Visible = false;
				Children[MenuItems[CurrentSelection]].Visible = true;
				MenuChangeEv(Children[MenuItems[CurrentSelection]], forward: true);
			}
		}

		public void GoBack()
		{
			Game.PlaySound(AUDIO_BACK, AUDIO_LIBRARY);
			Visible = false;
			if (ParentMenu != null)
			{
				PointF tmp = new PointF(0.5f, 0.5f);
				ParentMenu.Visible = true;
				MenuChangeEv(ParentMenu, forward: false);
				if (ResetCursorOnOpen)
				{
					API.SetCursorLocation(tmp.X, tmp.Y);
				}
			}
		}

		public void BindMenuToItem(UIMenu menuToBind, UIMenuItem itemToBindTo)
		{
			if (!MenuItems.Contains(itemToBindTo))
			{
				AddItem(itemToBindTo);
			}
			menuToBind.ParentMenu = this;
			menuToBind.ParentItem = itemToBindTo;
			if (Children.ContainsKey(itemToBindTo))
			{
				Children[itemToBindTo] = menuToBind;
			}
			else
			{
				Children.Add(itemToBindTo, menuToBind);
			}
		}

		public bool ReleaseMenuFromItem(UIMenuItem releaseFrom)
		{
			if (!Children.ContainsKey(releaseFrom))
			{
				return false;
			}
			Children[releaseFrom].ParentItem = null;
			Children[releaseFrom].ParentMenu = null;
			Children.Remove(releaseFrom);
			return true;
		}

		public void SetKey(MenuControls control, Keys keyToSet)
		{
			if (_keyDictionary.ContainsKey(control))
			{
				_keyDictionary[control].Item1.Add(keyToSet);
				return;
			}
			_keyDictionary.Add(control, new Tuple<List<Keys>, List<Tuple<Control, int>>>(new List<Keys>(), new List<Tuple<Control, int>>()));
			_keyDictionary[control].Item1.Add(keyToSet);
		}

		public void SetKey(MenuControls control, Control gtaControl)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			SetKey(control, gtaControl, 0);
			SetKey(control, gtaControl, 1);
			SetKey(control, gtaControl, 2);
		}

		public void SetKey(MenuControls control, Control gtaControl, int controlIndex)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (_keyDictionary.ContainsKey(control))
			{
				_keyDictionary[control].Item2.Add(new Tuple<Control, int>(gtaControl, controlIndex));
				return;
			}
			_keyDictionary.Add(control, new Tuple<List<Keys>, List<Tuple<Control, int>>>(new List<Keys>(), new List<Tuple<Control, int>>()));
			_keyDictionary[control].Item2.Add(new Tuple<Control, int>(gtaControl, controlIndex));
		}

		public void ResetKey(MenuControls control)
		{
			_keyDictionary[control].Item1.Clear();
			_keyDictionary[control].Item2.Clear();
		}

		public bool HasControlJustBeenPressed(MenuControls control, Keys key = Keys.None)
		{
			List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
			List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);
			if (key != 0)
			{
			}
			if (tmpControls.Any((Tuple<Control, int> tuple) => Game.IsControlJustPressed(tuple.Item2, tuple.Item1)))
			{
				return true;
			}
			return false;
		}

		public bool HasControlJustBeenReleaseed(MenuControls control, Keys key = Keys.None)
		{
			List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
			List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);
			if (key != 0)
			{
			}
			if (tmpControls.Any((Tuple<Control, int> tuple) => Game.IsControlJustReleased(tuple.Item2, tuple.Item1)))
			{
				return true;
			}
			return false;
		}

		public bool IsControlBeingPressed(MenuControls control, Keys key = Keys.None)
		{
			List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
			List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);
			if (HasControlJustBeenReleaseed(control, key))
			{
				_controlCounter = 0;
			}
			if (_controlCounter > 0)
			{
				_controlCounter++;
				if (_controlCounter > 30)
				{
					_controlCounter = 0;
				}
				return false;
			}
			if (key != 0)
			{
			}
			if (tmpControls.Any((Tuple<Control, int> tuple) => Game.IsControlPressed(tuple.Item2, tuple.Item1)))
			{
				_controlCounter = 1;
				return true;
			}
			return false;
		}

		public void AddInstructionalButton(InstructionalButton button)
		{
			_instructionalButtons.Add(button);
		}

		public void RemoveInstructionalButton(InstructionalButton button)
		{
			_instructionalButtons.Remove(button);
		}

		private void RecalculateDescriptionPosition()
		{
			float WindowHeight = CalculateWindowHeight();
			float Cinematic = CalculateCinematicHeight();
			((Rectangle)_descriptionBar).set_Position(new PointF(Offset.X, (float)(112 + _extraYOffset) + Offset.Y + WindowHeight + Cinematic));
			_descriptionRectangle.Position = new PointF(Offset.X, (float)(112 + _extraYOffset) + Offset.Y + WindowHeight + Cinematic);
			((Text)_descriptionText).set_Position(new PointF(Offset.X + 8f, (float)(118 + _extraYOffset) + Offset.Y + WindowHeight + Cinematic));
			((Rectangle)_descriptionBar).set_Size(new SizeF(431 + WidthOffset, 5f));
			_descriptionRectangle.Size = new SizeF(431 + WidthOffset, 30f);
			int count = Size;
			if (count > 10)
			{
				count = 11;
			}
			((Rectangle)_descriptionBar).set_Position(new PointF(Offset.X, (float)(38 * count) + ((Rectangle)_descriptionBar).get_Position().Y));
			_descriptionRectangle.Position = new PointF(Offset.X, (float)(38 * count) + _descriptionRectangle.Position.Y);
			((Text)_descriptionText).set_Position(new PointF(Offset.X + 8f, (float)(38 * count) + ((Text)_descriptionText).get_Position().Y));
		}

		private float CalculateCinematicHeight()
		{
			return 0f;
		}

		private int IsMouseInListItemArrows(UIMenuItem item, PointF topLeft, PointF safezone)
		{
			API.BeginTextCommandWidth("jamyfafi");
			UIResText.AddLongString(item.Text);
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			float screenw = res.Width;
			float screenh = res.Height;
			float ratio = screenw / screenh;
			float width = 1080f * ratio;
			int labelSize = (int)(API.EndTextCommandGetWidth(false) * width * 0.35f);
			int labelSizeX = 5 + labelSize + 10;
			int arrowSizeX = 431 - labelSizeX;
			return ScreenTools.IsMouseInBounds(topLeft, new SizeF(labelSizeX, 38f)) ? 1 : (ScreenTools.IsMouseInBounds(new PointF(topLeft.X + (float)labelSizeX, topLeft.Y), new SizeF(arrowSizeX, 38f)) ? 2 : 0);
		}

		public async Task Draw()
		{
			if (!Visible)
			{
				return;
			}
			if (ControlDisablingEnabled)
			{
				Controls.Toggle(toggle: false);
			}
			if (_buttonsEnabled)
			{
				API.DrawScaleformMovieFullscreen(_instructionalButtonsScaleform.get_Handle(), 255, 255, 255, 255, 0);
				Hud.HideComponentThisFrame((HudComponent)6);
				Hud.HideComponentThisFrame((HudComponent)7);
				Hud.HideComponentThisFrame((HudComponent)9);
			}
			float CinematicHeight = CalculateCinematicHeight();
			if (ScaleWithSafezone)
			{
				API.SetScriptGfxAlign(76, 84);
				API.SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
			}
			if (ReDraw)
			{
				DrawCalculations();
			}
			if (string.IsNullOrWhiteSpace(BannerTexture))
			{
				if (BannerSprite != null)
				{
					BannerSprite.Draw();
				}
				else
				{
					UIResRectangle bannerRectangle = BannerRectangle;
					if (bannerRectangle != null)
					{
						((Rectangle)bannerRectangle).Draw();
					}
				}
			}
			else if (!ScaleWithSafezone)
			{
				new PointF(0f, 0f);
			}
			else
			{
				_ = Safe;
			}
			BannerSprite.Position = new PointF(BannerSprite.Position.X, Offset.Y + CinematicHeight);
			_mainMenu.get_Items()[0].set_Position(new PointF(_mainMenu.get_Items()[0].get_Position().X, 13f + Offset.Y + CinematicHeight));
			_mainMenu.get_Items()[1].set_Position(new PointF(_mainMenu.get_Items()[1].get_Position().X, 100f + Offset.Y + CinematicHeight));
			_mainMenu.get_Items()[2].set_Position(new PointF(_mainMenu.get_Items()[2].get_Position().X, 103f + Offset.Y + CinematicHeight));
			_background.Position = new PointF(_background.Position.X, 144f + Offset.Y - 37f + (float)_extraYOffset + CinematicHeight);
			if (BannerSprite != null)
			{
				_glareScaleform.CallFunction("SET_DATA_SLOT", new object[1] { GameplayCamera.get_RelativeHeading() });
				SizeF res = ScreenTools.ResolutionMaintainRatio;
				SizeF _glareSize = new SizeF(1f, 1.054f);
				PointF gl = new PointF(Offset.X / res.Width + 0.4491f, Offset.Y / res.Height + 0.475f);
				API.DrawScaleformMovie(_glareScaleform.get_Handle(), gl.X, gl.Y, _glareSize.Width, _glareSize.Height, 255, 255, 255, 255, 0);
			}
			ReDraw = true;
			((Rectangle)_mainMenu).Draw();
			if (MenuItems.Count == 0 && Windows.Count == 0)
			{
				API.ResetScriptGfxAlign();
				return;
			}
			_background.Size = BackgroundSize;
			_background.Draw();
			MenuItems[_activeItem % MenuItems.Count].Selected = true;
			if (!string.IsNullOrWhiteSpace(MenuItems[_activeItem % MenuItems.Count].Description))
			{
				((Rectangle)_descriptionBar).Draw();
				_descriptionRectangle.Draw();
				((Text)_descriptionText).Draw();
			}
			float WindowHeight = CalculateWindowHeight();
			if (MenuItems.Count <= 10)
			{
				int count2 = 0;
				foreach (UIMenuItem item2 in MenuItems)
				{
					item2.Position(count2 * 38 - 37 + _extraYOffset + (int)Math.Round(WindowHeight) + (int)Math.Round(CinematicHeight));
					item2.Draw();
					count2++;
				}
			}
			else
			{
				int count = 0;
				for (int index2 = _minItem; index2 <= _maxItem; index2++)
				{
					UIMenuItem item = MenuItems[index2];
					item.Position(count * 38 - 37 + _extraYOffset + (int)Math.Round(WindowHeight) + (int)Math.Round(CinematicHeight));
					item.Draw();
					count++;
				}
				((Rectangle)_extraRectangleUp).Draw();
				((Rectangle)_extraRectangleDown).Draw();
				_upAndDownSprite.Draw();
				if (_counterText != null)
				{
					string cap = CurrentSelection + 1 + " / " + Size;
					((Text)_counterText).set_Caption(CounterPretext + cap);
					((Text)_counterText).Draw();
				}
			}
			if (Windows.Count > 0)
			{
				float WindowOffset = 0f;
				for (int index = 0; index < Windows.Count; index++)
				{
					if (index > 0)
					{
						WindowOffset += Windows[index].Background.Size.Height;
					}
					Windows[index].Position(WindowOffset + (float)_extraYOffset + 37f);
					Windows[index].Draw();
				}
			}
			if (MenuItems[CurrentSelection] is UIMenuListItem && (MenuItems[CurrentSelection] as UIMenuListItem).Panels.Count > 0)
			{
				PanelOffset = CalculatePanelsPosition(!string.IsNullOrWhiteSpace(MenuItems[CurrentSelection].Description));
				for (int i = 0; i < (MenuItems[CurrentSelection] as UIMenuListItem).Panels.Count; i++)
				{
					if (i > 0)
					{
						PanelOffset = PanelOffset + (MenuItems[CurrentSelection] as UIMenuListItem).Panels[i - 1].Background.Size.Height + 5;
					}
					(MenuItems[CurrentSelection] as UIMenuListItem).Panels[i].Position(PanelOffset);
					(MenuItems[CurrentSelection] as UIMenuListItem).Panels[i].Draw();
				}
			}
			if (ScaleWithSafezone)
			{
				API.ResetScriptGfxAlign();
			}
		}

		public void ProcessMouse()
		{
			float WindowHeight = CalculateWindowHeight();
			float cinematic = CalculateCinematicHeight();
			if (!Visible || _justOpened || MenuItems.Count == 0 || IsUsingController || !MouseControlsEnabled)
			{
				API.EnableControlAction(2, 2, true);
				API.EnableControlAction(2, 1, true);
				API.EnableControlAction(2, 25, true);
				API.EnableControlAction(2, 24, true);
				if (_itemsDirty)
				{
					MenuItems.Where((UIMenuItem i) => i.Hovered).ToList().ForEach(delegate(UIMenuItem i)
					{
						i.Hovered = false;
					});
					_itemsDirty = false;
				}
				return;
			}
			PointF safezoneOffset = ScreenTools.SafezoneBounds;
			API.ShowCursorThisFrame();
			int limit = MenuItems.Count - 1;
			int counter = 0;
			if (MenuItems.Count > 10)
			{
				limit = _maxItem;
			}
			if (ScreenTools.IsMouseInBounds(new PointF(0f, 0f), new SizeF(30f, 1080f)) && MouseEdgeEnabled)
			{
				GameplayCamera.set_RelativeHeading(GameplayCamera.get_RelativeHeading() + 5f);
				API.SetCursorSprite(6);
			}
			else if (ScreenTools.IsMouseInBounds(new PointF(Convert.ToInt32(ScreenTools.ResolutionMaintainRatio.Width - 30f), 0f), new SizeF(30f, 1080f)) && MouseEdgeEnabled)
			{
				GameplayCamera.set_RelativeHeading(GameplayCamera.get_RelativeHeading() - 5f);
				API.SetCursorSprite(7);
			}
			else if (MouseEdgeEnabled)
			{
				API.SetCursorSprite(1);
			}
			for (int j = _minItem; j <= limit; j++)
			{
				float xpos = Offset.X + safezoneOffset.X;
				float ypos = Offset.Y + 144f - 37f + (float)_extraYOffset + (float)(counter * 38) + safezoneOffset.Y + WindowHeight + cinematic;
				float yposSelected = Offset.Y + 144f - 37f + (float)_extraYOffset + safezoneOffset.Y + (float)(CurrentSelection * 38) + WindowHeight + cinematic;
				int xsize = 431 + WidthOffset;
				UIMenuItem uiMenuItem = MenuItems[j];
				if (ScreenTools.IsMouseInBounds(new PointF(xpos, ypos), new SizeF(xsize, 38f)))
				{
					uiMenuItem.Hovered = true;
					int res = IsMouseInListItemArrows(MenuItems[j], new PointF(xpos, yposSelected), safezoneOffset);
					if (uiMenuItem.Hovered && res == 1 && MenuItems[j] is IListItem)
					{
						API.SetMouseCursorSprite(5);
					}
					if (Game.IsControlJustPressed(0, (Control)24))
					{
						if (uiMenuItem.Selected && uiMenuItem.Enabled)
						{
							if (MenuItems[j] is IListItem && IsMouseInListItemArrows(MenuItems[j], new PointF(xpos, ypos), safezoneOffset) > 0)
							{
								switch (res)
								{
								case 1:
									SelectItem();
									break;
								case 2:
									GoRight();
									break;
								}
							}
							else
							{
								SelectItem();
							}
						}
						else if (!uiMenuItem.Selected)
						{
							CurrentSelection = j;
							Game.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
							IndexChange(CurrentSelection);
							UpdateScaleform();
						}
						else if (!uiMenuItem.Enabled && uiMenuItem.Selected)
						{
							Game.PlaySound(AUDIO_ERROR, AUDIO_LIBRARY);
						}
					}
				}
				else
				{
					uiMenuItem.Hovered = false;
				}
				counter++;
			}
			float extraY = 524f + Offset.Y - 37f + (float)_extraYOffset + safezoneOffset.Y + WindowHeight + cinematic;
			float extraX = safezoneOffset.X + Offset.X;
			if (Size <= 10)
			{
				return;
			}
			if (ScreenTools.IsMouseInBounds(new PointF(extraX, extraY), new SizeF(431 + WidthOffset, 18f)))
			{
				((Rectangle)_extraRectangleUp).set_Color(Color.FromArgb(255, 30, 30, 30));
				if (Game.IsControlJustPressed(0, (Control)24))
				{
					if (Size > 10)
					{
						GoUpOverflow();
					}
					else
					{
						GoUp();
					}
				}
			}
			else
			{
				((Rectangle)_extraRectangleUp).set_Color(Color.FromArgb(200, 0, 0, 0));
			}
			if (ScreenTools.IsMouseInBounds(new PointF(extraX, extraY + 18f), new SizeF(431 + WidthOffset, 18f)))
			{
				((Rectangle)_extraRectangleDown).set_Color(Color.FromArgb(255, 30, 30, 30));
				if (Game.IsControlJustPressed(0, (Control)24))
				{
					if (Size > 10)
					{
						GoDownOverflow();
					}
					else
					{
						GoDown();
					}
				}
			}
			else
			{
				((Rectangle)_extraRectangleDown).set_Color(Color.FromArgb(200, 0, 0, 0));
			}
		}

		public async Task ProcessControl(Keys key = Keys.None)
		{
			if (!Visible)
			{
				return;
			}
			if (_justOpened)
			{
				_justOpened = false;
				return;
			}
			if (HasControlJustBeenReleaseed(MenuControls.Back, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				GoBack();
			}
			if (MenuItems.Count == 0)
			{
				return;
			}
			if (IsControlBeingPressed(MenuControls.Up, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				if (Size <= 10)
				{
					await GoUp();
				}
				else
				{
					await GoUpOverflow();
				}
			}
			else if (IsControlBeingPressed(MenuControls.Down, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				if (Size <= 10)
				{
					await GoDown();
				}
				else
				{
					await GoDownOverflow();
				}
			}
			else if (IsControlBeingPressed(MenuControls.Left, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				await GoLeft();
			}
			else if (IsControlBeingPressed(MenuControls.Right, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				await GoRight();
			}
			else if (HasControlJustBeenPressed(MenuControls.Select, key) && API.UpdateOnscreenKeyboard() != 0 && !API.IsWarningMessageActive())
			{
				SelectItem();
			}
			UpdateScaleform();
		}

		public void ProcessKey(Keys key)
		{
			if ((from MenuControls menuControl in _menuControls
				select new List<Keys>(_keyDictionary[menuControl].Item1)).Any((List<Keys> tmpKeys) => tmpKeys.Any((Keys k) => k == key)))
			{
				ProcessControl(key);
			}
		}

		public void UpdateScaleform()
		{
			if (!Visible || !_buttonsEnabled)
			{
				return;
			}
			_instructionalButtonsScaleform.CallFunction("CLEAR_ALL", new object[0]);
			_instructionalButtonsScaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", new object[1] { 0 });
			_instructionalButtonsScaleform.CallFunction("CREATE_CONTAINER", new object[0]);
			_instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				0,
				API.GetControlInstructionalButton(2, 176, 0),
				_selectTextLocalized
			});
			_instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", new object[3]
			{
				1,
				API.GetControlInstructionalButton(2, 177, 0),
				_backTextLocalized
			});
			int count = 2;
			foreach (InstructionalButton button2 in _instructionalButtons.Where((InstructionalButton button) => button.ItemBind == null || MenuItems[CurrentSelection] == button.ItemBind))
			{
				_instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", new object[3]
				{
					count,
					button2.GetButtonId(),
					button2.Text
				});
				count++;
			}
			_instructionalButtonsScaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", new object[1] { -1 });
		}

		protected virtual void IndexChange(int newindex)
		{
			ReDraw = true;
			this.OnIndexChange?.Invoke(this, newindex);
		}

		internal virtual void ListChange(UIMenuListItem sender, int newindex)
		{
			this.OnListChange?.Invoke(this, sender, newindex);
		}

		internal virtual void ProgressChange(UIMenuProgressItem sender, int newindex)
		{
			this.OnProgressChange?.Invoke(this, sender, newindex);
		}

		protected virtual void ListSelect(UIMenuListItem sender, int newindex)
		{
			this.OnListSelect?.Invoke(this, sender, newindex);
		}

		protected virtual void SliderChange(UIMenuSliderItem sender, int newindex)
		{
			this.OnSliderChange?.Invoke(this, sender, newindex);
		}

		internal virtual void SliderProgressChange(UIMenuSliderProgressItem sender, int newindex)
		{
			this.OnProgressSliderChange?.Invoke(this, sender, newindex);
		}

		protected virtual void ItemSelect(UIMenuItem selecteditem, int index)
		{
			this.OnItemSelect?.Invoke(this, selecteditem, index);
		}

		protected virtual void CheckboxChange(UIMenuCheckboxItem sender, bool Checked)
		{
			this.OnCheckboxChange?.Invoke(this, sender, Checked);
		}

		protected virtual void MenuOpenEv()
		{
			this.OnMenuOpen?.Invoke(this);
		}

		protected virtual void MenuCloseEv()
		{
			this.OnMenuClose?.Invoke(this);
		}

		protected virtual void MenuChangeEv(UIMenu newmenu, bool forward)
		{
			this.OnMenuChange?.Invoke(this, newmenu, forward);
		}
	}
}
