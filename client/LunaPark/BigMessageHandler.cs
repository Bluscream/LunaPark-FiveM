using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace LunaPark
{
	public class BigMessageHandler
	{
		private Scaleform _sc;

		private int _start;

		private int _timer;

		public async Task Load()
		{
			if (_sc == null)
			{
				_sc = new Scaleform("MP_BIG_MESSAGE_FREEMODE");
				int timeout = 1000;
				DateTime start = DateTime.Now;
				while (!API.HasScaleformMovieLoaded(_sc.get_Handle()) && DateTime.Now.Subtract(start).TotalMilliseconds < (double)timeout)
				{
					await BaseScript.Delay(0);
				}
			}
		}

		public void Dispose()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			Function.Call((Hash)2095068147598518289L, (InputArgument[])(object)new InputArgument[1] { (InputArgument)new OutputArgument((object)_sc.get_Handle()) });
			_sc = null;
		}

		public async void ShowMissionPassedMessage(string msg, string sub, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", new object[6] { msg, sub, 100, true, 0, true });
			_timer = time;
		}

		public async void ShowColoredShard(string msg, string desc, HudColor textColor, HudColor bgColor, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_SHARD_CENTERED_MP_MESSAGE", new object[4]
			{
				msg,
				desc,
				(int)bgColor,
				(int)textColor
			});
			_timer = time;
		}

		public async void ShowOldMessage(string msg, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", new object[1] { msg });
			_timer = time;
		}

		public async void ShowSimpleShard(string title, string subtitle, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_SHARD_CREW_RANKUP_MP_MESSAGE", new object[2] { title, subtitle });
			_timer = time;
		}

		public async void ShowRankupMessage(string msg, string subtitle, int rank, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_BIG_MP_MESSAGE", new object[5] { msg, subtitle, rank, "", "" });
			_timer = time;
		}

		public async void ShowWeaponPurchasedMessage(string bigMessage, string weaponName, WeaponHash weapon, int time = 5000)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_WEAPON_PURCHASED", new object[5]
			{
				bigMessage,
				weaponName,
				(int)weapon,
				"",
				100
			});
			_timer = time;
		}

		public async void ShowMpMessageLarge(string msg, string sub, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_CENTERED_MP_MESSAGE_LARGE", new object[5] { msg, sub, 100, true, 100 });
			_sc.CallFunction("TRANSITION_IN", new object[0]);
			_timer = time;
		}

		public async void ShowMpWastedMessage(string msg, string sub, int time = 5000)
		{
			await Load();
			_start = Game.get_GameTime();
			_sc.CallFunction("SHOW_SHARD_WASTED_MP_MESSAGE", new object[2] { msg, sub });
			_timer = time;
		}

		public async void ShowCustomShard(string funcName, params object[] paremeters)
		{
			await Load();
			_sc.CallFunction(funcName, paremeters);
		}

		internal void Update()
		{
			if (_sc != null)
			{
				_sc.Render2D();
				if (_start != 0 && Game.get_GameTime() - _start > _timer)
				{
					_sc.CallFunction("TRANSITION_OUT", new object[0]);
					_start = 0;
					Dispose();
				}
			}
		}
	}
}
