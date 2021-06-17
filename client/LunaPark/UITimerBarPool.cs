using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class UITimerBarPool
	{
		public List<UITimerBarItem> TimerBars;

		public UITimerBarPool()
		{
			TimerBars = new List<UITimerBarItem>();
		}

		public void Add(UITimerBarItem item)
		{
			TimerBars.Add(item);
		}

		public void Remove(int id)
		{
			TimerBars.RemoveAt(id);
		}

		public void Remove(UITimerBarItem item)
		{
			TimerBars.Remove(item);
		}

		public async Task Draw()
		{
			for (int i = 0; i < TimerBars.Count; i++)
			{
				UITimerBarItem item = TimerBars[i];
				item.Draw(38 + 38 * i + (LoadingPrompt.get_IsActive() ? 38 : 0));
			}
		}
	}
}
