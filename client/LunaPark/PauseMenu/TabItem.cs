using System;
using System.Drawing;
using CitizenFX.Core.UI;

namespace LunaPark.PauseMenu
{
	public class TabItem
	{
		public bool DrawBg;

		protected Sprite RockstarTile;

		public virtual bool Visible { get; set; }

		public virtual bool Focused { get; set; }

		public string Title { get; set; }

		public bool Active { get; set; }

		public bool JustOpened { get; set; }

		public bool CanBeFocused { get; set; }

		public PointF TopLeft { get; set; }

		public PointF BottomRight { get; set; }

		public PointF SafeSize { get; set; }

		public bool UseDynamicPositionment { get; set; }

		public TabView Parent { get; set; }

		public bool FadeInWhenFocused { get; set; }

		public event EventHandler Activated;

		public event EventHandler DrawInstructionalButtons;

		public TabItem(string name)
		{
			RockstarTile = new Sprite("pause_menu_sp_content", "rockstartilebmp", default(PointF), new SizeF(64f, 64f), 0f, Color.FromArgb(40, 255, 255, 255));
			Title = name;
			DrawBg = true;
			UseDynamicPositionment = true;
		}

		public void OnActivated()
		{
			this.Activated?.Invoke(this, EventArgs.Empty);
		}

		public virtual void ProcessControls()
		{
		}

		public virtual void Draw()
		{
			if (!Visible)
			{
				return;
			}
			SizeF res = ScreenTools.ResolutionMaintainRatio;
			if (UseDynamicPositionment)
			{
				SafeSize = new PointF(300f, 240f);
				TopLeft = new PointF(SafeSize.X, SafeSize.Y);
				BottomRight = new PointF((float)(int)res.Width - SafeSize.X, (float)(int)res.Height - SafeSize.Y);
			}
			SizeF rectSize = new SizeF(BottomRight.SubtractPoints(TopLeft));
			this.DrawInstructionalButtons?.Invoke(this, EventArgs.Empty);
			if (DrawBg)
			{
				((Rectangle)new UIResRectangle(TopLeft, rectSize, Color.FromArgb((Focused || !FadeInWhenFocused) ? 200 : 120, 0, 0, 0))).Draw();
				int titleSize = 100;
				RockstarTile.Size = new SizeF(titleSize, titleSize);
				float cols = rectSize.Width / (float)titleSize;
				int fils = 4;
				for (int i = 0; (float)i < cols * (float)fils; i++)
				{
					RockstarTile.Position = TopLeft.AddPoints(new PointF((float)titleSize * ((float)i % cols), (float)titleSize * ((float)i / cols)));
					RockstarTile.Color = Color.FromArgb((int)MiscExtensions.LinearFloatLerp(40f, 0f, i / (int)cols, fils), 255, 255, 255);
					RockstarTile.Draw();
				}
			}
		}
	}
}
