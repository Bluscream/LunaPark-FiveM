namespace LunaPark.PauseMenu
{
	public class MissionLogo
	{
		internal bool IsGameTexture;

		internal string FileName { get; set; }

		internal string DictionaryName { get; set; }

		public MissionLogo(string filepath)
		{
			FileName = filepath;
			IsGameTexture = false;
		}

		public MissionLogo(string textureDict, string textureName)
		{
			FileName = textureName;
			DictionaryName = textureDict;
			IsGameTexture = true;
		}
	}
}
