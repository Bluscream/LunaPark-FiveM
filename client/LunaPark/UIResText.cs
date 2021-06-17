using System;
using System.Drawing;
using System.Text;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UIResText : Text
	{
		public Alignment TextAlignment { get; set; }

		public float Wrap { get; set; } = 0f;


		[Obsolete("Use UIResText.Wrap instead.", true)]
		public SizeF WordWrap
		{
			get
			{
				return new SizeF(Wrap, 0f);
			}
			set
			{
				Wrap = value.Width;
			}
		}

		public UIResText(string caption, PointF position, float scale)
			: this(caption, position, scale)
		{
			TextAlignment = (Alignment)1;
		}

		public UIResText(string caption, PointF position, float scale, Color color)
			: this(caption, position, scale, color)
		{
			TextAlignment = (Alignment)1;
		}

		public UIResText(string caption, PointF position, float scale, Color color, Font font, Alignment justify)
			: this(caption, position, scale, color, font, (Alignment)1)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			TextAlignment = justify;
		}

		public static void AddLongString(string str)
		{
			int utf8ByteCount = Encoding.UTF8.GetByteCount(str);
			if (utf8ByteCount == str.Length)
			{
				AddLongStringForAscii(str);
			}
			else
			{
				AddLongStringForUtf8(str);
			}
		}

		private static void AddLongStringForAscii(string input)
		{
			for (int i = 0; i < input.Length; i += 99)
			{
				string substr = input.Substring(i, Math.Min(99, input.Length - i));
				API.AddTextComponentString(substr);
			}
		}

		internal static void AddLongStringForUtf8(string input)
		{
			bool flag = false;
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			Encoding enc = Encoding.UTF8;
			int utf8ByteCount = enc.GetByteCount(input);
			if (utf8ByteCount < 99)
			{
				API.AddTextComponentString(input);
				return;
			}
			int startIndex = 0;
			for (int i = 0; i < input.Length; i++)
			{
				int length = i - startIndex;
				if (enc.GetByteCount(input.Substring(startIndex, length)) > 99)
				{
					string substr = input.Substring(startIndex, length - 1);
					API.AddTextComponentString(substr);
					i--;
					startIndex = startIndex + length - 1;
				}
			}
			API.AddTextComponentString(input.Substring(startIndex, input.Length - startIndex));
		}

		[Obsolete("Use ScreenTools.GetTextWidth instead.", true)]
		public static float MeasureStringWidth(string str, Font font, float scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ScreenTools.GetTextWidth(str, font, scale);
		}

		[Obsolete("Use ScreenTools.GetTextWidth instead.", true)]
		public static float MeasureStringWidthNoConvert(string str, Font font, float scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ScreenTools.GetTextWidth(str, font, scale);
		}

		public override void Draw(SizeF offset)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected I4, but got Unknown
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Invalid comparison between Unknown and I4
			int screenw = Screen.get_Resolution().Width;
			int screenh = Screen.get_Resolution().Height;
			float ratio = (float)screenw / (float)screenh;
			float width = 1080f * ratio;
			float x = ((Text)this).get_Position().X / width;
			float y = ((Text)this).get_Position().Y / 1080f;
			API.SetTextFont((int)((Text)this).get_Font());
			API.SetTextScale(1f, ((Text)this).get_Scale());
			API.SetTextColour((int)((Text)this).get_Color().R, (int)((Text)this).get_Color().G, (int)((Text)this).get_Color().B, (int)((Text)this).get_Color().A);
			if (((Text)this).get_Shadow())
			{
				API.SetTextDropShadow();
			}
			if (((Text)this).get_Outline())
			{
				API.SetTextOutline();
			}
			Alignment textAlignment = TextAlignment;
			Alignment val = textAlignment;
			if ((int)val != 0)
			{
				if ((int)val == 2)
				{
					API.SetTextRightJustify(true);
					API.SetTextWrap(0f, x);
				}
			}
			else
			{
				API.SetTextCentre(true);
			}
			if (Wrap != 0f)
			{
				float xsize = (((Text)this).get_Position().X + Wrap) / width;
				API.SetTextWrap(x, xsize);
			}
			API.SetTextEntry("jamyfafi");
			AddLongString(((Text)this).get_Caption());
			API.DrawText(x, y);
		}
	}
}
