using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;

namespace NuovaGM.Server.Veicoli
{
	internal class ParkServer : BaseScript
	{
		public ParkServer()
			: this()
		{
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:syncState", (Delegate)new Action<Player, string, int>(SyncRuotaPan));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:StopWheel", (Delegate)new Action<bool>(FermaRuota));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:UpdateCabins", (Delegate)new Action<int, int>(UpdateCabins));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:playerGetOff", (Delegate)new Action<Player, int, int>(RuotaGetOff));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:playerGetOn", (Delegate)new Action<Player, int, int>(RuotaGetOn));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:updateGradient", (Delegate)new Action<Player, int>(UpdateGradient));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:playerGetOff", (Delegate)new Action<Player, int>(MontagneGetOff));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:playerGetOn", (Delegate)new Action<Player, int, int, int>(MontagneGetOn));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:syncState", (Delegate)new Action<Player, string>(SyncMontagne));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:SyncCars", (Delegate)new Action<int, int>(SyncCars));
		}

		private void UpdateGradient([FromSource] Player player, int gradient)
		{
			if ((from x in ((IEnumerable<Player>)((BaseScript)this).get_Players()).ToList()
				orderby x.get_Handle()
				select x).FirstOrDefault() == player)
			{
				BaseScript.TriggerClientEvent("FerrisWheel:UpdateGradient", new object[1] { gradient });
			}
		}

		public void SyncRuotaPan([FromSource] Player p, string state, int Player)
		{
			if ((from x in ((IEnumerable<Player>)((BaseScript)this).get_Players()).ToList()
				orderby x.get_Handle()
				select x).FirstOrDefault() == p)
			{
				BaseScript.TriggerClientEvent("FerrisWheel:forceState", new object[1] { state });
			}
		}

		public void SyncMontagne([FromSource] Player p, string state)
		{
			if ((from x in ((IEnumerable<Player>)((BaseScript)this).get_Players()).ToList()
				orderby x.get_Handle()
				select x).FirstOrDefault() == p)
			{
				BaseScript.TriggerClientEvent("RollerCoaster:forceState", new object[1] { state });
			}
		}

		public void UpdateCabins(int cabin, int player)
		{
			BaseScript.TriggerClientEvent("FerrisWheel:UpdateCabins", new object[2] { cabin, player });
		}

		public void FermaRuota(bool stato)
		{
			BaseScript.TriggerClientEvent("FerrisWheel:FermaRuota", new object[1] { stato });
		}

		public void RuotaGetOn([FromSource] Player p, int player, int cabin)
		{
			BaseScript.TriggerClientEvent("FerrisWheel:playerGetOn", new object[2] { player, cabin });
		}

		public void RuotaGetOff([FromSource] Player p, int player, int cabin)
		{
			BaseScript.TriggerClientEvent("FerrisWheel:playerGetOff", new object[2] { player, cabin });
		}

		public void MontagneGetOn([FromSource] Player p, int player, int index, int carrello)
		{
			BaseScript.TriggerClientEvent("RollerCoaster:playerGetOn", new object[3] { player, index, carrello });
		}

		public void MontagneGetOff([FromSource] Player p, int player)
		{
			BaseScript.TriggerClientEvent("RollerCoaster:playerGetOff", new object[1] { player });
		}

		public void SyncCars(int Carrello, int Occupato)
		{
			BaseScript.TriggerClientEvent("RollerCoaster:SyncCars", new object[2] { Carrello, Occupato });
		}
	}
}
