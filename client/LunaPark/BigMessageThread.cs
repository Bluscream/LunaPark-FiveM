using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace LunaPark
{
	public class BigMessageThread : BaseScript
	{
		public static BigMessageHandler MessageInstance { get; set; }

		public BigMessageThread()
			: this()
		{
			MessageInstance = new BigMessageHandler();
			((BaseScript)this).add_Tick((Func<Task>)BigMessageThread_Tick);
		}

		private async Task BigMessageThread_Tick()
		{
			MessageInstance.Update();
		}
	}
}
