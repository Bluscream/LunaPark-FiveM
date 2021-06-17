using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIMenuHorizontalOneLineGridPanel : UIMenuPanel
	{
		private UIResText Left;

		private UIResText Right;

		private Sprite Grid;

		private Sprite Circle;

		private UIMenuGridAudio Audio;

		private PointF SetCirclePosition;

		protected bool CircleLocked;

		protected bool Pressed;

		public PointF CirclePosition
		{
			get
			{
				return new PointF((float)Math.Round((Circle.Position.X - (Grid.Position.X + 20f) + Circle.Size.Width / 2f) / (Grid.Size.Width - 40f), 2), (float)Math.Round((Circle.Position.Y - (Grid.Position.Y + 20f) + Circle.Size.Height / 2f) / (Grid.Size.Height - 40f), 2));
			}
			set
			{
				Circle.Position.X = Grid.Position.X + 20f + (Grid.Size.Width - 40f) * ((value.X >= 0f && value.X <= 1f) ? value.X : 0f) - Circle.Size.Width / 2f;
				Circle.Position.Y = Grid.Position.Y + 20f + (Grid.Size.Height - 40f) * ((value.Y >= 0f && value.Y <= 1f) ? value.Y : 0f) - Circle.Size.Height / 2f;
			}
		}

		public UIMenuHorizontalOneLineGridPanel(string LeftText, string RightText, float CirclePositionX = 0.5f)
		{
			Enabled = true;
			Background = new Sprite("commonmenu", "gradient_bgd", new Point(0, 0), new Size(431, 275));
			Grid = new Sprite("NativeUI", "horizontal_grid", new Point(0, 0), new Size(200, 200), 0f, Color.FromArgb(255, 255, 255));
			Circle = new Sprite("mpinventory", "in_world_circle", new Point(0, 0), new Size(20, 20), 0f, Color.FromArgb(255, 255, 255));
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
			Left = new UIResText(LeftText ?? "Left", new Point(0, 0), 0.35f, Color.FromArgb(255, 255, 255), (Font)0, (Alignment)0);
			Right = new UIResText(RightText ?? "Right", new Point(0, 0), 0.35f, Color.FromArgb(255, 255, 255), (Font)0, (Alignment)0);
			SetCirclePosition = new PointF(CirclePositionX, 0.5f);
		}

		internal override void Position(float y)
		{
			float ParentOffsetX = base.ParentItem.Offset.X;
			int ParentOffsetWidth = base.ParentItem.Parent.WidthOffset;
			Background.Position = new PointF(ParentOffsetX, y);
			Grid.Position = new PointF(ParentOffsetX + 115.5f + (float)(ParentOffsetWidth / 2), 37.5f + y);
			((Text)Left).set_Position(new PointF(ParentOffsetX + 57.75f + (float)(ParentOffsetWidth / 2), 120f + y));
			((Text)Right).set_Position(new PointF(ParentOffsetX + 373.25f + (float)(ParentOffsetWidth / 2), 120f + y));
			if (!CircleLocked)
			{
				CircleLocked = true;
				CirclePosition = SetCirclePosition;
			}
		}

		private void UpdateParent(float X)
		{
			base.ParentItem.Parent.ListChange(base.ParentItem, base.ParentItem.Index);
			base.ParentItem.ListChangedTrigger(base.ParentItem.Index);
		}

		private async void Functions()
		{
			PointF safezoneOffset = ScreenTools.SafezoneBounds;
			if (ScreenTools.IsMouseInBounds(new PointF(Grid.Position.X + 20f + safezoneOffset.X, Grid.Position.Y + 20f + safezoneOffset.Y), new SizeF(Grid.Size.Width - 40f, Grid.Size.Height - 40f)) && API.IsDisabledControlPressed(0, 24) && !Pressed)
			{
				Pressed = true;
				Audio.Id = API.GetSoundId();
				API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
				while (API.IsDisabledControlPressed(0, 24) && ScreenTools.IsMouseInBounds(new PointF(Grid.Position.X + 20f + safezoneOffset.X, Grid.Position.Y + 20f + safezoneOffset.Y), new SizeF(Grid.Size.Width - 40f, Grid.Size.Height - 40f)))
				{
					await BaseScript.Delay(0);
					SizeF res = ScreenTools.ResolutionMaintainRatio;
					float mouseX = API.GetDisabledControlNormal(0, 239) * res.Width;
					mouseX -= Circle.Size.Width / 2f + safezoneOffset.X;
					Circle.Position = new PointF((mouseX > Grid.Position.X + 10f + Grid.Size.Width - 40f) ? (Grid.Position.X + 10f + Grid.Size.Width - 40f) : ((mouseX < Grid.Position.X + 20f - Circle.Size.Width / 2f) ? (Grid.Position.X + 20f - Circle.Size.Width / 2f) : mouseX), Circle.Position.Y);
					float resultX = (float)Math.Round((Circle.Position.X - (Grid.Position.X + 20f) + (Circle.Size.Width + 20f)) / (Grid.Size.Width - 40f), 2) + safezoneOffset.X;
					UpdateParent(((resultX >= 0f && resultX <= 1f) ? resultX : (((resultX <= 0f) ? 0f : 1f) * 2f)) - 1f);
				}
				API.StopSound(Audio.Id);
				API.ReleaseSoundId(Audio.Id);
				Pressed = false;
			}
		}

		internal override async Task Draw()
		{
			if (Enabled)
			{
				Background.Size = new Size(431 + base.ParentItem.Parent.WidthOffset, 275);
				Background.Draw();
				Grid.Draw();
				Circle.Draw();
				((Text)Left).Draw();
				((Text)Right).Draw();
				Functions();
				await Task.FromResult(0);
			}
		}
	}
}
