using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class Sprite
	{
		public PointF Position;

		public SizeF Size;

		public Color Color;

		public bool Visible;

		public float Heading;

		public string TextureName;

		private string _textureDict;

		public string TextureDict
		{
			get
			{
				return _textureDict;
			}
			set
			{
				_textureDict = value;
			}
		}

		public Sprite(string textureDict, string textureName, PointF position, SizeF size, float heading, Color color)
		{
			TextureDict = textureDict;
			TextureName = textureName;
			Position = position;
			Size = size;
			Heading = heading;
			Color = color;
			Visible = true;
		}

		public Sprite(string textureDict, string textureName, PointF position, SizeF size)
			: this(textureDict, textureName, position, size, 0f, Color.FromArgb(255, 255, 255, 255))
		{
		}

		public void Draw()
		{
			if (Visible)
			{
				if (!API.HasStreamedTextureDictLoaded(TextureDict))
				{
					API.RequestStreamedTextureDict(TextureDict, true);
				}
				int screenw = Screen.get_Resolution().Width;
				int screenh = Screen.get_Resolution().Height;
				float ratio = (float)screenw / (float)screenh;
				float width = 1080f * ratio;
				float w = Size.Width / width;
				float h = Size.Height / 1080f;
				float x = Position.X / width + w * 0.5f;
				float y = Position.Y / 1080f + h * 0.5f;
				API.DrawSprite(TextureDict, TextureName, x, y, w, h, Heading, (int)Color.R, (int)Color.G, (int)Color.B, (int)Color.A);
			}
		}

		public static void Draw(string dict, string name, int xpos, int ypos, int boxWidth, int boxHeight, float rotation, Color color)
		{
			if (!API.HasStreamedTextureDictLoaded(dict))
			{
				API.RequestStreamedTextureDict(dict, true);
			}
			int screenw = Screen.get_Resolution().Width;
			int screenh = Screen.get_Resolution().Height;
			float ratio = (float)screenw / (float)screenh;
			float width = 1080f * ratio;
			float w = (float)boxWidth / width;
			float h = (float)boxHeight / 1080f;
			float x = (float)xpos / width + w * 0.5f;
			float y = (float)ypos / 1080f + h * 0.5f;
			API.DrawSprite(dict, name, x, y, w, h, rotation, (int)color.R, (int)color.G, (int)color.B, (int)color.A);
		}

		public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName)
		{
			string tmpPath = Path.GetTempFileName();
			return WriteFileFromResources(yourAssembly, fullResourceName, tmpPath);
		}

		public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName, string savePath)
		{
			using (Stream stream = yourAssembly.GetManifestResourceStream(fullResourceName))
			{
				if (stream != null)
				{
					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
					using FileStream fileStream = File.Create(savePath);
					fileStream.Write(buffer, 0, Convert.ToInt32(stream.Length));
					fileStream.Close();
				}
			}
			return Path.GetFullPath(savePath);
		}
	}
}
