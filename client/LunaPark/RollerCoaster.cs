using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	public class RollerCoaster : BaseScript
	{
		private int iLocal_715 = 0;

		private float fLocal_716 = 0f;

		private int ValueUnknownIndex = 0;

		private readonly string RollerAnim = "anim@mp_rollarcoaster";

		private string Place;

		private RollerCoasterNew Roller = new RollerCoasterNew();

		private Vector3 Coord = new Vector3(0f);

		private bool Active = false;

		public bool ImSitting = false;

		private bool Scaleform = false;

		private Scaleform Buttons = new Scaleform("instructional_buttons");

		private UITimerBarItem RollerBar = new UITimerBarItem("RollerCoaster:")
		{
			Enabled = false
		};

		private UITimerBarPool TimerBarPool = new UITimerBarPool();

		private List<Vector3> GetIn = new List<Vector3>
		{
			new Vector3(-1644.316f, -1123.53f, 17.3447f),
			new Vector3(-1644.92f, -1124.281f, 17.3447f),
			new Vector3(-1645.845f, -1125.413f, 17.3447f),
			new Vector3(-1646.562f, -1126.302f, 17.3447f),
			new Vector3(-1647.498f, -1127.438f, 17.3447f),
			new Vector3(-1648.23f, -1128.184f, 17.3447f),
			new Vector3(-1649.233f, -1129.399f, 17.3447f),
			new Vector3(-1649.937f, -1130.203f, 17.3447f)
		};

		private List<Vector3> GetOff = new List<Vector3>
		{
			new Vector3(-1641.914f, -1125.268f, 17.3424f),
			new Vector3(-1642.606f, -1126.24f, 17.3424f),
			new Vector3(-1643.573f, -1127.39f, 17.3424f),
			new Vector3(-1644.271f, -1128.2f, 17.3424f),
			new Vector3(-1645.343f, -1129.313f, 17.3424f),
			new Vector3(-1645.966f, -1130.067f, 17.3424f),
			new Vector3(-1647.022f, -1131.291f, 17.3424f),
			new Vector3(-1647.645f, -1132.016f, 17.3424f)
		};

		private List<Vector3> Dont_know_what_theyre_needed_for = new List<Vector3>
		{
			new Vector3(-1644.153f, -1125.433f, 18.3447f),
			new Vector3(-1645.723f, -1127.408f, 18.3447f),
			new Vector3(-1647.315f, -1129.374f, 18.3447f),
			new Vector3(-1648.95f, -1131.299f, 18.3447f)
		};

		private List<ObjectHash> DaEliminare = new List<ObjectHash>
		{
			ObjectHash.prop_roller_car_01,
			ObjectHash.prop_roller_car_02
		};

		private Prop MRClosest = new Prop(0);

		private int tempo = 30;

		public RollerCoaster()
			: this()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Expected O, but got Unknown
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Expected O, but got Unknown
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:forceState", (Delegate)new Action<string>(ForceState));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:playerGetOn", (Delegate)new Action<int, int, int>(playerGetOn));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:playerGetOff", (Delegate)new Action<int>(playerGetOff));
			((BaseScript)this).get_EventHandlers().Add("RollerCoaster:SyncCars", (Delegate)new Action<int, int>(SyncCars));
			((BaseScript)this).get_EventHandlers().Add("onResourceStop", (Delegate)new Action<string>(OnStop));
			Blip val = new Blip(API.AddBlipForCoord(-1651.641f, -1134.325f, 21.90398f));
			val.set_Sprite((BlipSprite)266);
			val.set_IsShortRange(true);
			val.set_Name("RollerCoaster");
			Blip roller = val;
			API.SetBlipDisplay(((PoolObject)roller).get_Handle(), 4);
			CaricaTutto();
			((BaseScript)this).add_Tick((Func<Task>)DeleteYellows);
			((BaseScript)this).add_Tick((Func<Task>)TimerBarHandler);
			TimerBarPool.Add(RollerBar);
		}

		private async Task SpawnaMontagne()
		{
			func_191();
			await Task.FromResult(0);
		}

		private void OnStop(string name)
		{
			if (API.GetCurrentResourceName() == name)
			{
				RollerCar[] cars = Roller.Cars;
				foreach (RollerCar v in cars)
				{
					((PoolObject)v.Entity).Delete();
				}
			}
		}

		private async void CaricaTutto()
		{
			func_220();
			API.RequestModel((uint)API.GetHashKey("ind_prop_dlc_roller_car"));
			while (!API.HasModelLoaded((uint)API.GetHashKey("ind_prop_dlc_roller_car")))
			{
				await BaseScript.Delay(100);
			}
			API.RequestModel((uint)API.GetHashKey("ind_prop_dlc_roller_car_02"));
			while (!API.HasModelLoaded((uint)API.GetHashKey("ind_prop_dlc_roller_car_02")))
			{
				await BaseScript.Delay(100);
			}
			API.RequestAnimDict(RollerAnim);
			while (!API.HasAnimDictLoaded(RollerAnim))
			{
				await BaseScript.Delay(100);
			}
			API.LoadStream("LEVIATHON_RIDE_MASTER", "");
			await SpawnaMontagne();
			((BaseScript)this).add_Tick((Func<Task>)MoveRollerCoaster);
			((BaseScript)this).add_Tick((Func<Task>)ControlloMontagne);
		}

		private async Task TimerBarHandler()
		{
			if (TimerBarPool.TimerBars.Count > 0)
			{
				TimerBarPool.Draw();
			}
		}

		private async Task DeleteYellows()
		{
			try
			{
				MRClosest = (from o in ((IEnumerable<Prop>)World.GetAllProps()).Select((Func<Prop, Prop>)((Prop o) => new Prop(((PoolObject)o).get_Handle())))
					where DaEliminare.Contains((ObjectHash)((Entity)o).get_Model().get_Hash())
					select o).FirstOrDefault(delegate(Prop o)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					Vector3 position = ((Entity)o).get_Position();
					return (double)((Vector3)(ref position)).DistanceToSquared(((Entity)Game.get_PlayerPed()).get_Position()) < Math.Pow(600.0, 2.0);
				});
				if ((Entity)(object)MRClosest != (Entity)null && MRClosest.Exists())
				{
					((PoolObject)MRClosest).Delete();
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Debug.WriteLine($"ERROR\n{e}\n{e.StackTrace}");
			}
			await BaseScript.Delay(500);
		}

		private async Task MoveRollerCoaster()
		{
			try
			{
				string state = Roller.State;
				string text = state;
				if (text == null)
				{
					return;
				}
				switch (text)
				{
				case "WAITING":
					((Text)RollerBar.TextTimerBar).set_Caption(tempo + " sec.");
					while (tempo > 0)
					{
						await BaseScript.Delay(1000);
						tempo--;
						((Text)RollerBar.TextTimerBar).set_Caption(tempo + "sec.");
						if (Roller.State != "WAITING")
						{
							return;
						}
					}
					await BaseScript.Delay(6000);
					BaseScript.TriggerServerEvent("RollerCoaster:syncState", new object[1] { "DEPARTING" });
					break;
				case "DEPARTING":
					if (Active)
					{
						break;
					}
					Roller.Cars.ToList().ForEach(delegate(RollerCar o)
					{
						API.PlayEntityAnim(((PoolObject)o.Entity).get_Handle(), "safety_bar_enter_roller_car", RollerAnim, 8f, false, true, false, 0f, 0);
					});
					API.PlaySoundFromEntity(-1, "Bar_Lower_And_Lock", ((PoolObject)Roller.Cars[1].Entity).get_Handle(), "DLC_IND_ROLLERCOASTER_SOUNDS", false, 0);
					if (!ImSitting)
					{
						await BaseScript.Delay(5000);
					}
					else
					{
						API.TaskPlayAnim(API.PlayerPedId(), RollerAnim, "safety_bar_enter_player_" + Place, 8f, -8f, -1, 2, 0f, false, false, false);
						while (API.GetEntityAnimCurrentTime(API.PlayerPedId(), RollerAnim, "safety_bar_enter_player_" + Place) < 0.2f)
						{
							await BaseScript.Delay(0);
						}
					}
					BaseScript.TriggerServerEvent("RollerCoaster:syncState", new object[1] { "TRIP" });
					Active = true;
					break;
				case "TRIP":
					if (ValueUnknownIndex != 0)
					{
						func_46(bParam0: true);
						if (ImSitting)
						{
							UpdateTasti();
							API.StartAudioScene("FAIRGROUND_RIDES_LEVIATHAN");
							API.PlayStreamFromPed(API.PlayerPedId());
							if (!API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "safety_bar_grip_move_a_player_" + Place, 3) && !API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_idle_a_player_" + Place, 3) && !API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_idle_a_player_" + Place, 3) && !API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_exit_player_" + Place, 3))
							{
								Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "safety_bar_grip_move_a_player_" + Place, 8f, -1, (AnimationFlags)1);
							}
							if (!Game.IsControlJustPressed(0, (Control)203))
							{
								break;
							}
							if (!API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_idle_a_player_" + Place, 3))
							{
								Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "hands_up_enter_player_" + Place, 8f, -1, (AnimationFlags)2);
								while (API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_enter_player_" + Place, 3))
								{
									await BaseScript.Delay(0);
								}
								Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "hands_up_idle_a_player_" + Place, 8f, -1, (AnimationFlags)1);
							}
							else if (API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_idle_a_player_" + Place, 3))
							{
								Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "hands_up_exit_player_" + Place, 8f, -1, (AnimationFlags)2);
								while (API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_exit_player_" + Place, 3))
								{
									await BaseScript.Delay(0);
								}
								Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "safety_bar_grip_move_a_player_" + Place, 8f, -1, (AnimationFlags)1);
							}
						}
						else
						{
							API.PlayStreamFromObject(((PoolObject)Roller.Cars[2].Entity).get_Handle());
						}
					}
					else
					{
						BaseScript.TriggerServerEvent("RollerCoaster:syncState", new object[1] { "ARRIVAL" });
					}
					break;
				case "ARRIVAL":
					API.PlaySoundFromEntity(-1, "Ride_Stop", ((PoolObject)Roller.Cars[1].Entity).get_Handle(), "DLC_IND_ROLLERCOASTER_SOUNDS", false, 0);
					if (API.IsEntityPlayingAnim(API.PlayerPedId(), RollerAnim, "hands_up_idle_a_player_" + Place, 3))
					{
						Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "hands_up_exit_player_" + Place, 8f, -1, (AnimationFlags)2);
						while (API.GetEntityAnimCurrentTime(API.PlayerPedId(), RollerAnim, "hands_up_exit_player_" + Place) < 0.99f)
						{
							await BaseScript.Delay(0);
						}
						Game.get_PlayerPed().get_Task().PlayAnimation(RollerAnim, "safety_bar_grip_move_a_player_" + Place, 8f, -1, (AnimationFlags)1);
					}
					BaseScript.TriggerServerEvent("RollerCoaster:syncState", new object[1] { "STOP" });
					break;
				case "STOP":
					if (Roller.VarSpeed > 1f)
					{
						func_46(bParam0: true);
						if (ImSitting)
						{
							API.PlayStreamFromPed(API.PlayerPedId());
						}
						else
						{
							API.PlayStreamFromObject(((PoolObject)Roller.Cars[2].Entity).get_Handle());
						}
					}
					else if (Active)
					{
						await BaseScript.Delay(1000);
						Roller.Cars.ToList().ForEach(delegate(RollerCar o)
						{
							API.PlayEntityAnim(((PoolObject)o.Entity).get_Handle(), "safety_bar_exit_roller_car", RollerAnim, 8f, false, true, false, 0f, 0);
						});
						API.PlaySoundFromEntity(-1, "Bar_Unlock_And_Raise", ((PoolObject)Roller.Cars[1].Entity).get_Handle(), "DLC_IND_ROLLERCOASTER_SOUNDS", false, 0);
						if (ImSitting)
						{
							BaseScript.TriggerServerEvent("RollerCoaster:playerGetOff", new object[1] { ((Entity)Game.get_PlayerPed()).get_NetworkId() });
						}
						Roller.Cars.ToList().ForEach(delegate(RollerCar o)
						{
							o.Occupied = 0;
						});
						Roller.Cars.ToList().ForEach(delegate(RollerCar o)
						{
							BaseScript.TriggerServerEvent("RollerCoaster:SyncCars", new object[2]
							{
								Roller.Cars.ToList().IndexOf(o),
								0
							});
						});
						await BaseScript.Delay(1000);
						iLocal_715 = 0;
						fLocal_716 = 0f;
						Buttons.Dispose();
						Scaleform = false;
						tempo = 30;
						BaseScript.TriggerServerEvent("RollerCoaster:syncState", new object[1] { "WAITING" });
						Active = false;
					}
					break;
				}
			}
			catch
			{
				Debug.WriteLine("ERROR MOVING ROLLERCOASTER");
			}
		}

		private async void SyncCars(int carrello, int Occupied)
		{
			Roller.Cars[carrello].Occupied = Occupied;
		}

		private async void playerGetOn(int playernetid, int index, int carrelloHash)
		{
			Ped personaggio = (Ped)Entity.FromNetworkId(playernetid);
			if (((Entity)personaggio).get_NetworkId() != ((Entity)Game.get_PlayerPed()).get_NetworkId() && !API.NetworkHasControlOfNetworkId(playernetid))
			{
				while (!API.NetworkRequestControlOfNetworkId(playernetid))
				{
					await BaseScript.Delay(0);
				}
			}
			Prop Carrello = Roller.Cars[index].Entity;
			switch (Roller.Cars[index].Occupied)
			{
			case 0:
				Place = "one";
				Roller.Cars[index].Occupied = 1;
				break;
			case 1:
				Place = "two";
				Roller.Cars[index].Occupied = 2;
				break;
			case 2:
				Screen.ShowNotification("This car is full!", false);
				return;
			}
			Debug.WriteLine("place = " + Place);
			API.TaskGoStraightToCoord(((PoolObject)personaggio).get_Handle(), Coord.X, Coord.Y, Coord.Z, 1f, -1, 229.3511f, 0.2f);
			await BaseScript.Delay(1000);
			int iLocal_1443 = API.NetworkCreateSynchronisedScene(Coord.X, Coord.Y, Coord.Z, 0f, 0f, 139.96f, 2, true, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
			API.NetworkAddPedToSynchronisedScene(((PoolObject)personaggio).get_Handle(), iLocal_1443, RollerAnim, "enter_player_" + Place, 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
			API.NetworkStartSynchronisedScene(iLocal_1443);
			int iVar1 = API.NetworkConvertSynchronisedSceneToSynchronizedScene(iLocal_1443);
			if (API.GetSynchronizedScenePhase(iVar1) > 0.99f)
			{
				iLocal_1443 = API.NetworkCreateSynchronisedScene(Coord.X, Coord.Y, Coord.Z, 0f, 0f, 139.96f, 2, false, true, 1.06535322E+09f, 0f, 1.06535322E+09f);
				API.NetworkAddPedToSynchronisedScene(((PoolObject)personaggio).get_Handle(), iLocal_1443, RollerAnim, "idle_a_player_" + Place, 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
				API.NetworkStartSynchronisedScene(iLocal_1443);
			}
			await BaseScript.Delay(5000);
			Vector3 vVar0 = API.GetOffsetFromEntityGivenWorldCoords(((PoolObject)Carrello).get_Handle(), ((Entity)personaggio).get_Position().X, ((Entity)personaggio).get_Position().Y, ((Entity)personaggio).get_Position().Z);
			API.AttachEntityToEntity(((PoolObject)personaggio).get_Handle(), ((PoolObject)Carrello).get_Handle(), 0, vVar0.X, vVar0.Y, vVar0.Z, 0f, 0f, ((Entity)personaggio).get_Heading() - 139.96f, false, false, false, false, 2, true);
			if (((Entity)personaggio).get_NetworkId() == ((Entity)Game.get_PlayerPed()).get_NetworkId())
			{
				ImSitting = true;
			}
			BaseScript.TriggerServerEvent("RollerCoaster:SyncCars", new object[2]
			{
				index,
				Roller.Cars[index].Occupied
			});
		}

		private async void playerGetOff(int playernetid)
		{
			Debug.WriteLine("playernetid = " + playernetid);
			Debug.WriteLine("game.playerped.networkdid = " + ((Entity)Game.get_PlayerPed()).get_NetworkId());
			Ped personaggio = (Ped)Entity.FromNetworkId(playernetid);
			if (!((Entity)(object)personaggio != (Entity)null))
			{
				return;
			}
			if (((Entity)personaggio).get_NetworkId() != ((Entity)Game.get_PlayerPed()).get_NetworkId() && !API.NetworkHasControlOfNetworkId(playernetid))
			{
				while (!API.NetworkRequestControlOfNetworkId(playernetid))
				{
					await BaseScript.Delay(0);
				}
			}
			if (((Entity)personaggio).IsAttached())
			{
				((Entity)personaggio).Detach();
			}
			if (((Entity)personaggio).get_NetworkId() == ((Entity)Game.get_PlayerPed()).get_NetworkId())
			{
				ImSitting = false;
			}
			int iLocal_1443 = API.NetworkCreateSynchronisedScene(Coord.X, Coord.Y, Coord.Z, 0f, 0f, 139.96f, 2, true, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
			API.NetworkAddPedToSynchronisedScene(((PoolObject)personaggio).get_Handle(), iLocal_1443, RollerAnim, "safety_bar_exit_player_" + Place, 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
			API.NetworkStartSynchronisedScene(iLocal_1443);
			await BaseScript.Delay(3000);
			int iLocal_1442 = API.NetworkCreateSynchronisedScene(Coord.X, Coord.Y, Coord.Z, 0f, 0f, 139.96f, 2, true, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
			API.NetworkAddPedToSynchronisedScene(((PoolObject)personaggio).get_Handle(), iLocal_1442, RollerAnim, "exit_player_" + Place, 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
			API.NetworkStartSynchronisedScene(iLocal_1442);
			await BaseScript.Delay(7000);
			personaggio.get_Task().ClearAll();
		}

		private async Task ControlloMontagne()
		{
			foreach (Vector3 v in GetIn)
			{
				if (((Entity)Game.get_PlayerPed()).IsInRangeOf(v, 1.3f) && Roller.State == "WAITING" && tempo > 0)
				{
					Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to get on the roller coaster");
					if (Game.IsControlJustPressed(0, (Control)51))
					{
						float fVar2 = ((GetIn.IndexOf(v) % 2 == 0) ? (-1.017f) : 0f);
						Coord = API.GetOffsetFromEntityInWorldCoords(((PoolObject)Roller.Cars[GetIn.IndexOf(v) / 2].Entity).get_Handle(), 0f, fVar2, 0f);
						BaseScript.TriggerServerEvent("RollerCoaster:playerGetOn", new object[3]
						{
							((Entity)Game.get_PlayerPed()).get_NetworkId(),
							GetIn.IndexOf(v) / 2,
							((Entity)Roller.Cars[GetIn.IndexOf(v) / 2].Entity).get_Model().get_Hash()
						});
					}
				}
			}
			if (ImSitting)
			{
				if (!API.LoadStreamWithStartOffset("Player_Ride", 0, "DLC_IND_ROLLERCOASTER_SOUNDS"))
				{
					API.LoadStreamWithStartOffset("Player_Ride", 0, "DLC_IND_ROLLERCOASTER_SOUNDS");
				}
			}
			else if (!API.LoadStreamWithStartOffset("Ambient_Ride", 1, "DLC_IND_ROLLERCOASTER_SOUNDS"))
			{
				API.LoadStreamWithStartOffset("Ambient_Ride", 1, "DLC_IND_ROLLERCOASTER_SOUNDS");
			}
			if (((Entity)Game.get_PlayerPed()).IsInRangeOf(new Vector3(-1646.863f, -1125.135f, 17.338f), 30f))
			{
				if (Roller.State == "WAITING" && tempo > 0)
				{
					RollerBar.Enabled = true;
				}
				else
				{
					RollerBar.Enabled = false;
				}
			}
			else
			{
				RollerBar.Enabled = false;
			}
		}

		private async void UpdateTasti()
		{
			if (!Scaleform)
			{
				Buttons = new Scaleform("instructional_buttons");
				while (!API.HasScaleformMovieLoaded(Buttons.get_Handle()))
				{
					await BaseScript.Delay(0);
				}
				Buttons.CallFunction("CLEAR_ALL", new object[0]);
				Buttons.CallFunction("TOGGLE_MOUSE_BUTTONS", new object[1] { false });
				Buttons.CallFunction("SET_DATA_SLOT", new object[3]
				{
					0,
					API.GetControlInstructionalButton(2, 203, 1),
					"Alza/Abbassa le braccia"
				});
				if (API.IsInputDisabled(2))
				{
					Buttons.CallFunction("SET_DATA_SLOT", new object[3]
					{
						2,
						API.GetControlInstructionalButton(2, 0, 1),
						"Cambia Visuale"
					});
				}
				else
				{
					Buttons.CallFunction("SET_DATA_SLOT", new object[3]
					{
						2,
						API.GetControlInstructionalButton(2, 217, 1),
						"Cambia Visuale"
					});
				}
				Buttons.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", new object[1] { -1 });
				Scaleform = true;
			}
			if (Scaleform)
			{
				Buttons.Render2D();
			}
		}

		private async void func_191()
		{
			for (int iVar0 = 0; iVar0 < Roller.Cars.Length; iVar0++)
			{
				int iVar1 = await func_138(Roller.Speed - 2.55f * (float)iVar0, ValueUnknownIndex);
				Vector3 vVar2 = func_50(Roller.Speed - 2.55f * (float)iVar0, iVar1);
				if (iVar0 == 0)
				{
					Roller.Cars[0].Entity = new Prop(API.CreateObject(API.GetHashKey("ind_prop_dlc_roller_car"), func_51(1).X, func_51(1).Y, func_51(1).Z, false, false, false));
				}
				else
				{
					Roller.Cars[iVar0].Entity = new Prop(API.CreateObject(API.GetHashKey("ind_prop_dlc_roller_car_02"), vVar2.X, vVar2.Y, vVar2.Z, false, false, false));
					func_134(iVar0, iVar1, Roller.Speed - 2.55f * (float)iVar0);
				}
				API.FreezeEntityPosition(((PoolObject)Roller.Cars[iVar0].Entity).get_Handle(), true);
				API.SetEntityLodDist(((PoolObject)Roller.Cars[iVar0].Entity).get_Handle(), 300);
				API.SetEntityInvincible(((PoolObject)Roller.Cars[iVar0].Entity).get_Handle(), true);
			}
			Roller.Speed = Roller.Value3[1];
			func_46(bParam0: false);
			Roller.Cars.ToList().ForEach(delegate(RollerCar o)
			{
				API.PlayEntityAnim(((PoolObject)o.Entity).get_Handle(), "idle_a_roller_car", RollerAnim, 8f, true, false, false, 0f, 0);
			});
			iLocal_715 = 0;
			func_133();
		}

		private async void func_133()
		{
			for (int iVar0 = 0; iVar0 < Roller.Cars.Length; iVar0++)
			{
				int iVar1 = await func_138(Roller.Speed - 2.55f * (float)iVar0, ValueUnknownIndex);
				Vector3 vVar2 = func_50(Roller.Speed - 2.55f * (float)iVar0, iVar1);
				API.SetEntityCoordsNoOffset(((PoolObject)Roller.Cars[iVar0].Entity).get_Handle(), vVar2.X, vVar2.Y, vVar2.Z, true, false, false);
				func_134(iVar0, iVar1, Roller.Speed - 2.55f * (float)iVar0);
			}
		}

		private async void func_46(bool bParam0)
		{
			if (bParam0)
			{
				if (iLocal_715 != 0)
				{
					fLocal_716 = (float)API.GetTimeDifference(API.GetNetworkTimeAccurate(), iLocal_715) / 1000f;
				}
				iLocal_715 = API.GetNetworkTimeAccurate();
			}
			float fVar0 = func_49();
			if (ValueUnknownIndex < 20)
			{
				if (Roller.VarSpeed < 3f)
				{
					Roller.VarSpeed += 0.3f;
				}
				else
				{
					Roller.VarSpeed -= 0.3f;
				}
				if (API.Absf(Roller.VarSpeed - 3f) < 0.3f)
				{
					Roller.VarSpeed = 3f;
				}
			}
			else
			{
				Roller.VarSpeed += fVar0 * fLocal_716;
				Roller.VarSpeed *= 1f;
			}
			if (Roller.Speed < Roller.Value3[1] && Roller.Speed + Roller.VarSpeed * fLocal_716 >= Roller.Value3[1])
			{
				Roller.Speed = Roller.Value3[1];
			}
			else
			{
				Roller.Speed += Roller.VarSpeed * fLocal_716;
			}
			bool bVar1 = false;
			if (Roller.VarSpeed >= 0f)
			{
				if (Roller.Speed >= Roller.Value3[Roller.Index - 1])
				{
					if (Roller.State != "ARRIVAL")
					{
						Roller.Speed -= Roller.Value3[Roller.Index - 1];
					}
					else
					{
						Roller.Speed = Roller.Value3[1];
					}
					ValueUnknownIndex = 0;
				}
				int iVar3 = func_48(ValueUnknownIndex);
				while (!bVar1)
				{
					await BaseScript.Delay(0);
					if (Roller.Speed < Roller.Value3[iVar3])
					{
						bVar1 = true;
						if (ValueUnknownIndex != iVar3 - 1 && Roller.Value4[iVar3 - 1] != Roller.VarSpeed)
						{
							Roller.VarSpeed = Roller.Value4[iVar3 - 1];
						}
						ValueUnknownIndex = iVar3 - 1;
					}
					iVar3 = func_48(iVar3);
				}
			}
			else
			{
				if (Roller.Speed < 0f)
				{
					Roller.Speed += Roller.Value3[Roller.Index - 1];
					ValueUnknownIndex = Roller.Index - 2;
				}
				int iVar3 = ValueUnknownIndex;
				while (!bVar1)
				{
					await BaseScript.Delay(0);
					if (Roller.Value3[iVar3] < Roller.Speed)
					{
						bVar1 = true;
						ValueUnknownIndex = iVar3;
					}
					iVar3 = func_47(iVar3);
				}
			}
			func_133();
		}

		private int func_47(int iParam0)
		{
			int iVar0 = iParam0 - 1;
			if (iVar0 < 0)
			{
				iVar0 = Roller.Index - 2;
			}
			return iVar0;
		}

		private int func_48(int iParam0)
		{
			int iVar0 = iParam0 + 1;
			if (iVar0 >= Roller.Index)
			{
				iVar0 = 1;
			}
			return iVar0;
		}

		private float func_49()
		{
			int iVar0 = func_48(ValueUnknownIndex);
			float fVar1 = Roller.Value0[ValueUnknownIndex].Z - Roller.Value0[iVar0].Z;
			float fVar2 = Roller.Value3[iVar0] - Roller.Value3[ValueUnknownIndex];
			if (fVar2 < 0f)
			{
				fVar2 += Roller.Value3[224];
			}
			float fVar3 = Rad2deg((float)Math.Asin(Deg2rad(fVar1 / fVar2)));
			return 10f * Rad2deg((float)Math.Sin(Deg2rad(fVar3)));
		}

		public float Rad2deg(float rad)
		{
			return rad * (180f / (float)Math.PI);
		}

		public float Deg2rad(float deg)
		{
			return deg * ((float)Math.PI / 180f);
		}

		private Vector3 func_50(float fParam0, int iParam1)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (fParam0 < 0f)
			{
				fParam0 += Roller.Value3[Roller.Index - 1];
			}
			int iVar1;
			int iVar2;
			if (Roller.VarSpeed >= 0f)
			{
				iVar1 = iParam1;
				iVar2 = func_48(iParam1);
			}
			else
			{
				iVar1 = func_48(iParam1);
				iVar2 = iParam1;
			}
			float fVar3 = API.Absf(Roller.Value3[iVar2] - Roller.Value3[iVar1]);
			float fVar4 = fParam0 - Roller.Value3[iVar1];
			float fVar5 = fVar4 / fVar3;
			Vector3 vVar6 = Vector3.Subtract(func_51(iVar2), func_51(iVar1));
			if (Roller.VarSpeed >= 0f)
			{
				return Vector3.Add(func_51(iVar1), Vector3.Multiply(vVar6, fVar5));
			}
			return Vector3.Subtract(func_51(iVar1), Vector3.Multiply(vVar6, fVar5));
		}

		private async void func_134(int iParam0, int iParam1, float fParam2)
		{
			float[] uVar5 = new float[4];
			float[] uVar6 = new float[4];
			float[] uVar7 = new float[4];
			int iVar0 = func_47(iParam1);
			int iVar = func_48(iParam1);
			int iVar2 = func_48(iVar);
			if (fParam2 < 0f)
			{
				fParam2 += Roller.Value3[Roller.Index - 1];
			}
			float fVar5 = (fParam2 - Roller.Value3[iParam1]) / (Roller.Value3[iVar] - Roller.Value3[iParam1]);
			float fVar4;
			if (fVar5 < 0.5f)
			{
				fVar4 = fVar5 + 0.5f;
				func_135(func_136(iVar0, iParam1), ref uVar5[0], ref uVar5[1], ref uVar5[2], ref uVar5[3]);
				func_135(func_136(iParam1, iVar), ref uVar6[0], ref uVar6[1], ref uVar6[2], ref uVar6[3]);
			}
			else
			{
				fVar4 = fVar5 - 0.5f;
				func_135(func_136(iParam1, iVar), ref uVar5[0], ref uVar5[1], ref uVar5[2], ref uVar5[3]);
				func_135(func_136(iVar, iVar2), ref uVar6[0], ref uVar6[1], ref uVar6[2], ref uVar6[3]);
			}
			API.SlerpNearQuaternion(fVar4, uVar5[0], uVar5[1], uVar5[2], uVar5[3], uVar6[0], uVar6[1], uVar6[2], uVar6[3], ref uVar7[0], ref uVar7[1], ref uVar7[2], ref uVar7[3]);
			API.SetEntityQuaternion(((PoolObject)Roller.Cars[iParam0].Entity).get_Handle(), uVar7[0], uVar7[1], uVar7[2], uVar7[3]);
			if (((Entity)Game.get_PlayerPed()).IsInRangeOf(((Entity)Roller.Cars[0].Entity).get_Position(), 50f) && iParam0 == 0 && iVar0 % 3 == 0)
			{
				API.SetPadShake(0, 32, 32);
			}
		}

		private void func_135(Vector3 Param0, ref float uParam1, ref float uParam2, ref float uParam3, ref float uParam4)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			float fVar0 = Param0.Y / 2f;
			float fVar1 = Param0.Z / 2f;
			float fVar2 = Param0.X / 2f;
			float fVar3 = Rad2deg((float)Math.Sin(Deg2rad(fVar0)));
			float fVar4 = Rad2deg((float)Math.Sin(Deg2rad(fVar1)));
			float fVar5 = Rad2deg((float)Math.Sin(Deg2rad(fVar2)));
			float fVar6 = Rad2deg((float)Math.Cos(Deg2rad(fVar0)));
			float fVar7 = Rad2deg((float)Math.Cos(Deg2rad(fVar1)));
			float fVar8 = Rad2deg((float)Math.Cos(Deg2rad(fVar2)));
			uParam1 = fVar5 * fVar6 * fVar7 - fVar8 * fVar3 * fVar4;
			uParam2 = fVar8 * fVar3 * fVar7 + fVar5 * fVar6 * fVar4;
			uParam3 = fVar8 * fVar6 * fVar4 - fVar5 * fVar3 * fVar7;
			uParam4 = fVar8 * fVar6 * fVar7 + fVar5 * fVar3 * fVar4;
		}

		private Vector3 func_136(int iParam0, int iParam1)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			Vector3 vVar0 = func_137(Roller.Value0[iParam1] - Roller.Value0[iParam0]);
			float fVar1 = API.Atan2(vVar0.X, vVar0.Y);
			float fVar2 = API.Atan2(vVar0.Z, API.Sqrt(vVar0.X * vVar0.X + vVar0.Y * vVar0.Y));
			return new Vector3(0f - fVar2, 0f, 0f - fVar1 - 180f);
		}

		private Vector3 func_137(Vector3 vParam0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			float fVar0 = API.Vmag(vParam0.X, vParam0.Y, vParam0.Z);
			if (fVar0 != 0f)
			{
				float fVar1 = 1f / fVar0;
				vParam0 *= fVar1;
			}
			else
			{
				vParam0.X = 0f;
				vParam0.Y = 0f;
				vParam0.Z = 0f;
			}
			return vParam0;
		}

		private async Task<int> func_138(float fParam0, int iParam1)
		{
			if (fParam0 <= 0f)
			{
				fParam0 += Roller.Value3[Roller.Index - 1];
				iParam1 = Roller.Index - 1;
			}
			for (int iVar0 = iParam1; iVar0 >= 0; iVar0 += -1)
			{
				await BaseScript.Delay(0);
				if (Roller.Value3[iVar0] < fParam0)
				{
					return iVar0;
				}
			}
			return 0;
		}

		private void func_220()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_0974: Unknown result type (might be due to invalid IL or missing references)
			//IL_098f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1019: Unknown result type (might be due to invalid IL or missing references)
			//IL_1034: Unknown result type (might be due to invalid IL or missing references)
			//IL_104f: Unknown result type (might be due to invalid IL or missing references)
			//IL_106a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_110c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1127: Unknown result type (might be due to invalid IL or missing references)
			//IL_1142: Unknown result type (might be due to invalid IL or missing references)
			//IL_115d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1178: Unknown result type (might be due to invalid IL or missing references)
			//IL_1193: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_121a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1235: Unknown result type (might be due to invalid IL or missing references)
			//IL_1250: Unknown result type (might be due to invalid IL or missing references)
			//IL_126b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1286: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_130d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1328: Unknown result type (might be due to invalid IL or missing references)
			//IL_1343: Unknown result type (might be due to invalid IL or missing references)
			//IL_135e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1379: Unknown result type (might be due to invalid IL or missing references)
			//IL_1394: Unknown result type (might be due to invalid IL or missing references)
			//IL_13af: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1400: Unknown result type (might be due to invalid IL or missing references)
			//IL_141b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1436: Unknown result type (might be due to invalid IL or missing references)
			//IL_1451: Unknown result type (might be due to invalid IL or missing references)
			//IL_146c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1487: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_150e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1529: Unknown result type (might be due to invalid IL or missing references)
			//IL_1544: Unknown result type (might be due to invalid IL or missing references)
			//IL_155f: Unknown result type (might be due to invalid IL or missing references)
			//IL_157a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1595: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1601: Unknown result type (might be due to invalid IL or missing references)
			//IL_161c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1637: Unknown result type (might be due to invalid IL or missing references)
			//IL_1652: Unknown result type (might be due to invalid IL or missing references)
			//IL_166d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1688: Unknown result type (might be due to invalid IL or missing references)
			//IL_16a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_16be: Unknown result type (might be due to invalid IL or missing references)
			//IL_16d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_170f: Unknown result type (might be due to invalid IL or missing references)
			//IL_172a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1745: Unknown result type (might be due to invalid IL or missing references)
			//IL_1760: Unknown result type (might be due to invalid IL or missing references)
			//IL_177b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1796: Unknown result type (might be due to invalid IL or missing references)
			//IL_17bb: Unknown result type (might be due to invalid IL or missing references)
			func_226(new Vector3(-1659.01f, -1143.129f, 17.4192f));
			func_226(new Vector3(-1643.524f, -1124.681f, 17.4326f));
			func_226(new Vector3(-1639.621f, -1120.021f, 17.6357f));
			func_226(new Vector3(-1638.199f, -1118.316f, 17.9966f));
			func_226(new Vector3(-1637.011f, -1116.896f, 18.5407f));
			func_226(new Vector3(-1635.772f, -1115.417f, 19.2558f));
			func_226(new Vector3(-1634.227f, -1113.569f, 20.1725f));
			func_226(new Vector3(-1632.692f, -1111.734f, 21.0835f));
			func_226(new Vector3(-1631.179f, -1109.922f, 21.9826f));
			func_226(new Vector3(-1629.692f, -1108.145f, 22.865f));
			func_226(new Vector3(-1628.243f, -1106.411f, 23.7252f));
			func_226(new Vector3(-1626.84f, -1104.733f, 24.558f));
			func_226(new Vector3(-1625.491f, -1103.12f, 25.3588f));
			func_226(new Vector3(-1624.206f, -1101.582f, 26.1218f));
			func_226(new Vector3(-1622.992f, -1100.13f, 26.8424f));
			func_226(new Vector3(-1620.721f, -1097.416f, 28.1892f));
			func_226(new Vector3(-1618.866f, -1095.196f, 29.2895f));
			func_226(new Vector3(-1617.533f, -1093.603f, 30.0795f));
			func_226(new Vector3(-1616.778f, -1092.699f, 30.5401f));
			func_226(new Vector3(-1615.677f, -1091.388f, 30.9156f));
			func_226(new Vector3(-1614.829f, -1090.377f, 31.0008f));
			func_226(new Vector3(-1614.011f, -1089.406f, 30.9417f));
			func_226(new Vector3(-1612.615f, -1087.747f, 30.3463f));
			func_226(new Vector3(-1610.992f, -1085.82f, 29.1724f));
			func_226(new Vector3(-1609.228f, -1083.725f, 27.949f));
			func_226(new Vector3(-1608.295f, -1082.615f, 27.4861f));
			func_226(new Vector3(-1606.937f, -1081.002f, 27.4328f));
			func_226(new Vector3(-1605.471f, -1079.258f, 27.5762f));
			func_226(new Vector3(-1604.159f, -1077.701f, 28.0216f));
			func_226(new Vector3(-1602.511f, -1075.749f, 28.5244f));
			func_226(new Vector3(-1600.932f, -1073.873f, 28.9813f));
			func_226(new Vector3(-1599.342f, -1071.983f, 29.1756f));
			func_226(new Vector3(-1597.851f, -1070.067f, 29.1552f));
			func_226(new Vector3(-1596.723f, -1067.995f, 29.0611f));
			func_226(new Vector3(-1596.123f, -1065.708f, 28.9503f));
			func_226(new Vector3(-1595.991f, -1063.354f, 28.8316f));
			func_226(new Vector3(-1596.365f, -1061.041f, 28.7074f));
			func_226(new Vector3(-1597.254f, -1058.857f, 28.577f));
			func_226(new Vector3(-1598.562f, -1056.894f, 28.4423f));
			func_226(new Vector3(-1600.27f, -1055.292f, 28.3045f));
			func_226(new Vector3(-1602.288f, -1054.077f, 28.163f));
			func_226(new Vector3(-1604.497f, -1053.295f, 28.019f));
			func_226(new Vector3(-1606.845f, -1053.063f, 27.8712f));
			func_226(new Vector3(-1609.193f, -1053.3f, 27.7214f));
			func_226(new Vector3(-1611.416f, -1054.029f, 27.5695f));
			func_226(new Vector3(-1613.432f, -1055.248f, 27.4148f));
			func_226(new Vector3(-1615.167f, -1056.844f, 27.2581f));
			func_226(new Vector3(-1616.486f, -1058.782f, 27.0998f));
			func_226(new Vector3(-1617.371f, -1060.964f, 26.9395f));
			func_226(new Vector3(-1617.803f, -1063.281f, 26.7771f));
			func_226(new Vector3(-1617.669f, -1065.625f, 26.6138f));
			func_226(new Vector3(-1617.071f, -1067.903f, 26.4484f));
			func_226(new Vector3(-1616.006f, -1069.994f, 26.2817f));
			func_226(new Vector3(-1614.489f, -1071.798f, 26.1132f));
			func_226(new Vector3(-1612.646f, -1073.265f, 25.9435f));
			func_226(new Vector3(-1610.523f, -1074.272f, 25.7722f));
			func_226(new Vector3(-1608.231f, -1074.807f, 25.5996f));
			func_226(new Vector3(-1605.875f, -1074.877f, 25.4258f));
			func_226(new Vector3(-1603.576f, -1074.385f, 25.251f));
			func_226(new Vector3(-5031f / (float)Math.PI, -1073.441f, 25.0748f));
			func_226(new Vector3(-1599.508f, -1072.067f, 24.8974f));
			func_226(new Vector3(-1597.961f, -1070.289f, 24.7188f));
			func_226(new Vector3(-1596.798f, -1068.241f, 24.5393f));
			func_226(new Vector3(-1596.121f, -1065.987f, 24.3586f));
			func_226(new Vector3(-1595.946f, -1063.637f, 24.177f));
			func_226(new Vector3(-1596.242f, -1061.301f, 23.9942f));
			func_226(new Vector3(-1597.079f, -1059.097f, 23.8103f));
			func_226(new Vector3(-1598.345f, -1057.109f, 23.6258f));
			func_226(new Vector3(-1599.996f, -1055.426f, 23.44f));
			func_226(new Vector3(-1601.991f, -1054.172f, 23.2534f));
			func_226(new Vector3(-1604.195f, -1053.339f, 23.0661f));
			func_226(new Vector3(-1606.533f, -1053.01f, 22.8773f));
			func_226(new Vector3(-1608.881f, -1053.199f, 22.6882f));
			func_226(new Vector3(-1611.144f, -1053.85f, 22.4984f));
			func_226(new Vector3(-1613.199f, -1055.015f, 22.3068f));
			func_226(new Vector3(-1614.982f, -1056.581f, 22.1126f));
			func_226(new Vector3(-1616.545f, -1058.398f, 21.7888f));
			func_226(new Vector3(-1618.098f, -1060.261f, 21.3373f));
			func_226(new Vector3(-1619.583f, -1062.043f, 20.7536f));
			func_226(new Vector3(-1621.058f, -1063.813f, 20.1778f));
			func_226(new Vector3(-1622.535f, -1065.582f, 19.6021f));
			func_226(new Vector3(-1624.009f, -1067.352f, 19.0262f));
			func_226(new Vector3(-1625.482f, -1069.119f, 18.4527f));
			func_226(new Vector3(-1626.88f, -1070.806f, 17.9515f));
			func_226(new Vector3(-1628.218f, -1072.426f, 17.7058f));
			func_226(new Vector3(-1629.509f, -1073.975f, 17.7076f));
			func_226(new Vector3(-1631.046f, -1075.816f, 17.7079f));
			func_226(new Vector3(-1632.36f, -1077.393f, 17.7079f));
			func_226(new Vector3(-1633.897f, -1079.234f, 17.7081f));
			func_226(new Vector3(-1635.397f, -1080.972f, 17.7074f));
			func_226(new Vector3(-1636.924f, -1082.801f, 17.7074f));
			func_226(new Vector3(-1638.383f, -1084.535f, 17.8395f));
			func_226(new Vector3(-1639.644f, -1086.005f, 18.3624f));
			func_226(new Vector3(-1640.985f, -1087.563f, 19.3675f));
			func_226(new Vector3(-1642.482f, -1089.276f, 20.5799f));
			func_226(new Vector3(-1644.108f, -1091.096f, 21.5914f));
			func_226(new Vector3(-1645.844f, -1092.97f, 21.9359f));
			func_226(new Vector3(-1647.561f, -1094.781f, 21.6225f));
			func_226(new Vector3(-1649.239f, -1096.506f, 20.9627f));
			func_226(new Vector3(-1650.894f, -1098.148f, 20.1969f));
			func_226(new Vector3(-1652.535f, -1099.704f, 19.525f));
			func_226(new Vector3(-1654.248f, -1101.247f, 18.9923f));
			func_226(new Vector3(-1656.05f, -1102.794f, 18.5631f));
			func_226(new Vector3(-1657.911f, -1104.315f, 18.2393f));
			func_226(new Vector3(-1659.798f, -1105.782f, 18.0219f));
			func_226(new Vector3(-1661.681f, -1107.168f, 17.911f));
			func_226(new Vector3(-1663.525f, -1108.445f, 17.9064f));
			func_226(new Vector3(-1665.293f, -1109.582f, 18.0057f));
			func_226(new Vector3(-1667.317f, -1110.773f, 18.2989f));
			func_226(new Vector3(-1669.263f, -1111.836f, 18.8213f));
			func_226(new Vector3(-1671.144f, -1112.787f, 19.5262f));
			func_226(new Vector3(-1673.022f, -1113.685f, 20.3691f));
			func_226(new Vector3(-1674.958f, -1114.582f, 21.2989f));
			func_226(new Vector3(-1676.995f, -1115.534f, 22.249f));
			func_226(new Vector3(-1679.084f, -1116.478f, 23.1368f));
			func_226(new Vector3(-1681.219f, -1117.389f, 23.9532f));
			func_226(new Vector3(-1683.374f, -1118.29f, 24.6845f));
			func_226(new Vector3(-1685.517f, -1119.208f, 25.3158f));
			func_226(new Vector3(-1687.62f, -1120.167f, 25.8329f));
			func_226(new Vector3(-1689.705f, -1121.188f, 26.2178f));
			func_226(new Vector3(-1691.772f, -1122.315f, 26.5427f));
			func_226(new Vector3(-1693.635f, -1123.75f, 26.845f));
			func_226(new Vector3(-1695.254f, -1125.474f, 27.1175f));
			func_226(new Vector3(-1696.581f, -1127.444f, 27.3373f));
			func_226(new Vector3(-1697.574f, -1129.602f, 27.5003f));
			func_226(new Vector3(-1698.207f, -1131.882f, 27.623f));
			func_226(new Vector3(-1698.465f, -1134.234f, 27.7231f));
			func_226(new Vector3(-1698.344f, -1136.602f, 27.7949f));
			func_226(new Vector3(-1697.841f, -1138.921f, 27.8328f));
			func_226(new Vector3(-1696.972f, -1141.131f, 27.8321f));
			func_226(new Vector3(-1695.759f, -1143.174f, 27.7892f));
			func_226(new Vector3(-1694.234f, -1144.995f, 27.7019f));
			func_226(new Vector3(-1692.435f, -1146.544f, 27.5687f));
			func_226(new Vector3(-1690.415f, -1147.784f, 27.39f));
			func_226(new Vector3(-1688.226f, -1148.682f, 27.1671f));
			func_226(new Vector3(-1685.926f, -1149.218f, 26.9025f));
			func_226(new Vector3(-1683.576f, -1149.376f, 26.5994f));
			func_226(new Vector3(-1681.237f, -1149.161f, 26.2631f));
			func_226(new Vector3(-1678.968f, -1148.578f, 25.8985f));
			func_226(new Vector3(-1676.825f, -1147.645f, 25.5118f));
			func_226(new Vector3(-1674.862f, -1146.388f, 25.1091f));
			func_226(new Vector3(-1673.124f, -1144.839f, 24.6968f));
			func_226(new Vector3(-1671.654f, -1143.039f, 24.2822f));
			func_226(new Vector3(-1670.486f, -1141.031f, 23.8716f));
			func_226(new Vector3(-1669.649f, -1138.865f, 23.472f));
			func_226(new Vector3(-1669.163f, -1136.595f, 23.0898f));
			func_226(new Vector3(-1669.04f, -1134.274f, 22.7307f));
			func_226(new Vector3(-1669.284f, -1131.962f, 22.4006f));
			func_226(new Vector3(-1669.888f, -1129.713f, 22.1045f));
			func_226(new Vector3(-1670.841f, -1127.585f, 21.8463f));
			func_226(new Vector3(-1672.12f, -1125.632f, 21.6294f));
			func_226(new Vector3(-1673.694f, -1123.904f, 21.4559f));
			func_226(new Vector3(-1675.523f, -1122.444f, 21.327f));
			func_226(new Vector3(-1677.563f, -1121.29f, 21.2429f));
			func_226(new Vector3(-1679.762f, -1120.476f, 21.2021f));
			func_226(new Vector3(-1682.064f, -1120.019f, 21.2025f));
			func_226(new Vector3(-1684.41f, -1119.933f, 21.2408f));
			func_226(new Vector3(-1686.742f, -1120.221f, 21.3125f));
			func_226(new Vector3(-1689f, -1120.877f, 21.4123f));
			func_226(new Vector3(-1691.128f, -1121.884f, 21.5343f));
			func_226(new Vector3(-1693.069f, -1123.218f, 21.672f));
			func_226(new Vector3(-1694.779f, -1124.849f, 21.8191f));
			func_226(new Vector3(-1695.93f, -1126.324f, 21.9029f));
			func_226(new Vector3(-1696.878f, -1127.99f, 21.9873f));
			func_226(new Vector3(-1697.674f, -1129.878f, 22.0778f));
			func_226(new Vector3(-1698.292f, -1131.96f, 22.1685f));
			func_226(new Vector3(-1698.699f, -1134.206f, 22.2533f));
			func_226(new Vector3(-1698.866f, -1136.587f, 22.3261f));
			func_226(new Vector3(-1698.764f, -1139.072f, 22.3807f));
			func_226(new Vector3(-1698.363f, -1141.633f, 22.4112f));
			func_226(new Vector3(-1697.633f, -1144.24f, 22.4113f));
			func_226(new Vector3(-1696.546f, -1146.863f, 22.3751f));
			func_226(new Vector3(-1695.061f, -1149.484f, 22.295f));
			func_226(new Vector3(-1693.239f, -1151.881f, 22.1555f));
			func_226(new Vector3(-1691.225f, -1153.872f, 21.965f));
			func_226(new Vector3(-1689.074f, -1155.483f, 21.737f));
			func_226(new Vector3(-1686.842f, -1156.74f, 21.485f));
			func_226(new Vector3(-1684.583f, -1157.674f, 21.2224f));
			func_226(new Vector3(-1682.351f, -1158.311f, 20.9625f));
			func_226(new Vector3(-1680.161f, -1158.67f, 20.7161f));
			func_226(new Vector3(-1678.176f, -1158.802f, 20.5023f));
			func_226(new Vector3(-1676.344f, -1158.712f, 20.3287f));
			func_226(new Vector3(-1674.755f, -1158.437f, 20.2097f));
			func_226(new Vector3(-1672.606f, -1157.658f, 20.0965f));
			func_226(new Vector3(-1670.584f, -1156.442f, 19.9803f));
			func_226(new Vector3(-1668.831f, -1154.866f, 19.8642f));
			func_226(new Vector3(-1667.418f, -1152.986f, 19.7482f));
			func_226(new Vector3(-1666.452f, -1150.833f, 19.6319f));
			func_226(new Vector3(-1665.911f, -1148.538f, 19.5158f));
			func_226(new Vector3(-1665.817f, -1146.181f, 19.3996f));
			func_226(new Vector3(-1666.207f, -1143.862f, 19.2836f));
			func_226(new Vector3(-1667.073f, -1141.668f, 19.1674f));
			func_226(new Vector3(-1668.339f, -1139.679f, 19.0512f));
			func_226(new Vector3(-1669.962f, -1137.965f, 18.935f));
			func_226(new Vector3(-1671.913f, -1136.65f, 18.8189f));
			func_226(new Vector3(-1674.087f, -1135.737f, 18.7028f));
			func_226(new Vector3(-1676.397f, -1135.256f, 18.5866f));
			func_226(new Vector3(-1678.751f, -1135.237f, 18.4705f));
			func_226(new Vector3(-1681.058f, -1135.73f, 18.3543f));
			func_226(new Vector3(-1683.23f, -1136.65f, 18.2382f));
			func_226(new Vector3(-1685.187f, -1137.968f, 18.1219f));
			func_226(new Vector3(-1686.824f, -1139.661f, 18.0059f));
			func_226(new Vector3(-1688.081f, -1141.657f, 17.8896f));
			func_226(new Vector3(-1688.938f, -1143.855f, 17.7735f));
			func_226(new Vector3(-1689.359f, -1146.177f, 17.6571f));
			func_226(new Vector3(-1689.26f, -1148.532f, 17.5411f));
			func_226(new Vector3(-1688.71f, -1150.826f, 17.425f));
			func_226(new Vector3(-1687.733f, -1152.976f, 17.3087f));
			func_226(new Vector3(-1686.342f, -1154.887f, 17.1759f));
			func_226(new Vector3(-1684.573f, -1156.462f, 17.0021f));
			func_226(new Vector3(-1682.54f, -1157.669f, 16.7987f));
			func_226(new Vector3(-1680.313f, -1158.466f, 16.5838f));
			func_226(new Vector3(-1677.973f, -1158.77f, 16.3415f));
			func_226(new Vector3(-1675.626f, -1158.601f, 16.0948f));
			func_226(new Vector3(-1673.361f, -1157.994f, 15.9205f));
			func_226(new Vector3(-1671.255f, -1156.966f, 15.8075f));
			func_226(new Vector3(-1669.435f, -1155.511f, 15.7719f));
			func_226(new Vector3(-1667.848f, -1153.66f, 15.766f));
			func_226(new Vector3(-1666.33f, -1151.852f, 15.7703f));
			func_226(new Vector3(-1664.875f, -1150.117f, 15.8984f));
			func_226(new Vector3(-1663.46f, -1148.431f, 16.198f));
			func_226(new Vector3(-1662.033f, -1146.731f, 16.6412f));
			func_226(new Vector3(-1660.556f, -1144.97f, 17.1643f));
			func_226(new Vector3(-1659.01f, -1143.129f, 17.4192f));
			func_225(Roller.Index - 1, Roller.Value0[0]);
			func_221();
			Roller.Value4[0] = 0f;
			Roller.Value4[1] = 0.3f;
			Roller.Value4[2] = 0.3f;
			Roller.Value4[3] = 3f;
			Roller.Value4[4] = 3f;
			Roller.Value4[5] = 3f;
			Roller.Value4[6] = 3f;
			Roller.Value4[7] = 3f;
			Roller.Value4[8] = 3f;
			Roller.Value4[9] = 3f;
			Roller.Value4[10] = 3f;
			Roller.Value4[11] = 3f;
			Roller.Value4[12] = 3f;
			Roller.Value4[13] = 3f;
			Roller.Value4[14] = 3f;
			Roller.Value4[15] = 3f;
			Roller.Value4[16] = 3f;
			Roller.Value4[17] = 3f;
			Roller.Value4[18] = 3f;
			Roller.Value4[19] = 3f;
			Roller.Value4[20] = 3f;
			Roller.Value4[21] = 3f;
			Roller.Value4[22] = 3.1794f;
			Roller.Value4[23] = 4.8025f;
			Roller.Value4[24] = 6.7585f;
			Roller.Value4[25] = 8.3448f;
			Roller.Value4[26] = 8.8436f;
			Roller.Value4[27] = 8.9045f;
			Roller.Value4[28] = 8.7073f;
			Roller.Value4[29] = 8.1965f;
			Roller.Value4[30] = 7.5921f;
			Roller.Value4[31] = 7.0097f;
			Roller.Value4[32] = 6.6959f;
			Roller.Value4[33] = 6.7221f;
			Roller.Value4[34] = 6.8771f;
			Roller.Value4[35] = 7.0232f;
			Roller.Value4[36] = 7.18f;
			Roller.Value4[37] = 7.3457f;
			Roller.Value4[38] = 7.5605f;
			Roller.Value4[39] = 7.6956f;
			Roller.Value4[40] = 7.8806f;
			Roller.Value4[41] = 8.0659f;
			Roller.Value4[42] = 8.2597f;
			Roller.Value4[43] = 8.4085f;
			Roller.Value4[44] = 8.6081f;
			Roller.Value4[45] = 8.7629f;
			Roller.Value4[46] = 8.9647f;
			Roller.Value4[47] = 9.1286f;
			Roller.Value4[48] = 9.2889f;
			Roller.Value4[49] = 9.4513f;
			Roller.Value4[50] = 9.6107f;
			Roller.Value4[51] = 9.8224f;
			Roller.Value4[52] = 9.9849f;
			Roller.Value4[53] = 10.1485f;
			Roller.Value4[54] = 10.3112f;
			Roller.Value4[55] = 10.4793f;
			Roller.Value4[56] = 10.648f;
			Roller.Value4[57] = 10.7657f;
			Roller.Value4[58] = 10.9378f;
			Roller.Value4[59] = 11.1113f;
			Roller.Value4[60] = 11.285f;
			Roller.Value4[61] = 11.4038f;
			Roller.Value4[62] = 11.5785f;
			Roller.Value4[63] = 11.7563f;
			Roller.Value4[64] = 11.8826f;
			Roller.Value4[65] = 12.0063f;
			Roller.Value4[66] = 12.1858f;
			Roller.Value4[67] = 12.311f;
			Roller.Value4[68] = 12.4905f;
			Roller.Value4[69] = 12.6186f;
			Roller.Value4[70] = 12.752f;
			Roller.Value4[71] = 12.9366f;
			Roller.Value4[72] = 13.069f;
			Roller.Value4[73] = 13.2002f;
			Roller.Value4[74] = 13.3271f;
			Roller.Value4[75] = 13.5131f;
			Roller.Value4[76] = 13.6428f;
			Roller.Value4[77] = 13.8557f;
			Roller.Value4[78] = 14.1467f;
			Roller.Value4[79] = 14.7078f;
			Roller.Value4[80] = 15.0933f;
			Roller.Value4[81] = 15.4812f;
			Roller.Value4[82] = 15.6897f;
			Roller.Value4[83] = 16.072f;
			Roller.Value4[84] = 16.4288f;
			Roller.Value4[85] = 16.6158f;
			Roller.Value4[86] = 16.615f;
			Roller.Value4[87] = 16.6148f;
			Roller.Value4[88] = 16.6148f;
			Roller.Value4[89] = 16.6147f;
			Roller.Value4[90] = 16.6151f;
			Roller.Value4[91] = 16.6151f;
			Roller.Value4[92] = 16.5645f;
			Roller.Value4[93] = 16.145f;
			Roller.Value4[94] = 15.4464f;
			Roller.Value4[95] = 14.6939f;
			Roller.Value4[96] = 14.0775f;
			Roller.Value4[97] = 13.7741f;
			Roller.Value4[98] = 13.9723f;
			Roller.Value4[99] = 14.39f;
			Roller.Value4[100] = 14.8988f;
			Roller.Value4[101] = 15.3516f;
			Roller.Value4[102] = 15.7118f;
			Roller.Value4[103] = 15.9945f;
			Roller.Value4[104] = 16.2069f;
			Roller.Value4[105] = 16.3518f;
			Roller.Value4[106] = 16.3944f;
			Roller.Value4[107] = 16.3977f;
			Roller.Value4[108] = 16.3236f;
			Roller.Value4[109] = 16.1241f;
			Roller.Value4[110] = 15.9292f;
			Roller.Value4[111] = 15.4375f;
			Roller.Value4[112] = 14.8409f;
			Roller.Value4[113] = 14.2057f;
			Roller.Value4[114] = 13.5752f;
			Roller.Value4[115] = 12.7336f;
			Roller.Value4[116] = 12.2095f;
			Roller.Value4[117] = 11.5221f;
			Roller.Value4[118] = 11.0986f;
			Roller.Value4[119] = 10.6031f;
			Roller.Value4[120] = 10.2269f;
			Roller.Value4[121] = 9.9111f;
			Roller.Value4[122] = 9.5337f;
			Roller.Value4[123] = 9.2694f;
			Roller.Value4[124] = 9.0583f;
			Roller.Value4[125] = 8.8516f;
			Roller.Value4[126] = 8.7288f;
			Roller.Value4[127] = 8.6021f;
			Roller.Value4[128] = 8.51f;
			Roller.Value4[129] = 8.4733f;
			Roller.Value4[130] = 8.4742f;
			Roller.Value4[131] = 8.5294f;
			Roller.Value4[132] = 8.6157f;
			Roller.Value4[133] = 8.7849f;
			Roller.Value4[134] = 8.9639f;
			Roller.Value4[135] = 9.2546f;
			Roller.Value4[136] = 9.514f;
			Roller.Value4[137] = 9.8293f;
			Roller.Value4[138] = 10.1114f;
			Roller.Value4[139] = 10.5312f;
			Roller.Value4[140] = 10.9067f;
			Roller.Value4[141] = 11.1773f;
			Roller.Value4[142] = 11.5836f;
			Roller.Value4[143] = 11.9962f;
			Roller.Value4[144] = 12.2748f;
			Roller.Value4[145] = 12.6632f;
			Roller.Value4[146] = 12.9328f;
			Roller.Value4[147] = 13.1711f;
			Roller.Value4[148] = 13.3959f;
			Roller.Value4[149] = 13.6873f;
			Roller.Value4[150] = 13.8612f;
			Roller.Value4[151] = 14.0083f;
			Roller.Value4[152] = 14.1297f;
			Roller.Value4[153] = 14.2155f;
			Roller.Value4[154] = 14.2732f;
			Roller.Value4[155] = 14.3011f;
			Roller.Value4[156] = 14.3008f;
			Roller.Value4[157] = 14.2746f;
			Roller.Value4[158] = 14.2251f;
			Roller.Value4[159] = 14.1577f;
			Roller.Value4[160] = 14.0733f;
			Roller.Value4[161] = 13.9385f;
			Roller.Value4[162] = 13.8385f;
			Roller.Value4[163] = 13.7996f;
			Roller.Value4[164] = 13.7301f;
			Roller.Value4[165] = 13.6586f;
			Roller.Value4[166] = 13.5902f;
			Roller.Value4[167] = 13.5308f;
			Roller.Value4[168] = 13.482f;
			Roller.Value4[169] = 13.4313f;
			Roller.Value4[170] = 13.4127f;
			Roller.Value4[171] = 13.4126f;
			Roller.Value4[172] = 13.433f;
			Roller.Value4[173] = 13.4949f;
			Roller.Value4[174] = 13.6046f;
			Roller.Value4[175] = 13.761f;
			Roller.Value4[176] = 13.8937f;
			Roller.Value4[177] = 14.1199f;
			Roller.Value4[178] = 14.2919f;
			Roller.Value4[179] = 14.4699f;
			Roller.Value4[180] = 14.6443f;
			Roller.Value4[181] = 14.7383f;
			Roller.Value4[182] = 14.889f;
			Roller.Value4[183] = 14.9538f;
			Roller.Value4[184] = 15.0334f;
			Roller.Value4[185] = 15.1141f;
			Roller.Value4[186] = 15.1923f;
			Roller.Value4[187] = 15.2712f;
			Roller.Value4[188] = 15.3499f;
			Roller.Value4[189] = 15.4286f;
			Roller.Value4[190] = 15.5073f;
			Roller.Value4[191] = 15.5512f;
			Roller.Value4[192] = 15.6299f;
			Roller.Value4[193] = 15.7082f;
			Roller.Value4[194] = 15.7859f;
			Roller.Value4[195] = 15.8647f;
			Roller.Value4[196] = 15.9434f;
			Roller.Value4[197] = 16.0226f;
			Roller.Value4[198] = 16.0645f;
			Roller.Value4[199] = 16.1422f;
			Roller.Value4[200] = 16.2213f;
			Roller.Value4[201] = 16.2996f;
			Roller.Value4[202] = 16.3783f;
			Roller.Value4[203] = 16.4226f;
			Roller.Value4[204] = 16.5003f;
			Roller.Value4[205] = 16.5796f;
			Roller.Value4[206] = 16.6622f;
			Roller.Value4[207] = 16.7055f;
			Roller.Value4[208] = 16.7837f;
			Roller.Value4[209] = 16.8729f;
			Roller.Value4[210] = 16.9907f;
			Roller.Value4[211] = 17.0661f;
			Roller.Value4[212] = 17.2127f;
			Roller.Value4[213] = 17.3761f;
			Roller.Value4[214] = 17.4689f;
			Roller.Value4[215] = 17.5883f;
			Roller.Value4[216] = 17.6648f;
			Roller.Value4[217] = 17.6783f;
			Roller.Value4[218] = 17.6822f;
			Roller.Value4[219] = 17.6806f;
			Roller.Value4[220] = 17.5908f;
			Roller.Value4[221] = 17.371f;
			Roller.Value4[222] = 17.1986f;
			Roller.Value4[223] = 16.8458f;
		}

		private void ForceState(string state)
		{
			Roller.State = state;
		}

		private Vector3 func_51(int iParam0)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return Roller.Value0[iParam0];
		}

		private async void func_221()
		{
			float fVar1 = 0f;
			for (int iVar0 = 0; iVar0 < Roller.Value0.Length && !func_222(iVar0); iVar0++)
			{
				Roller.Value3[iVar0] = fVar1;
				if (iVar0 < 224)
				{
					fVar1 += API.Vdist(func_51(iVar0).X, func_51(iVar0).Y, func_51(iVar0).Z, func_51(iVar0 + 1).X, func_51(iVar0 + 1).Y, func_51(iVar0 + 1).Z);
				}
			}
		}

		private bool func_222(int iParam0)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return func_223(Roller.Value0[iParam0]);
		}

		private bool func_223(Vector3 vParam0)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return func_224(vParam0, new Vector3(0f), bParam2: false);
		}

		private bool func_224(Vector3 vParam0, Vector3 vParam1, bool bParam2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (bParam2)
			{
				return vParam0.X == vParam1.X && vParam0.Y == vParam1.Y;
			}
			return vParam0.X == vParam1.X && vParam0.Y == vParam1.Y && vParam0.Z == vParam1.Z;
		}

		private void func_225(int iParam0, Vector3 vParam1)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (iParam0 >= 0 && iParam0 <= 225)
			{
				Roller.Value0[iParam0] = vParam1;
			}
		}

		private void func_226(Vector3 vParam0)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (Roller.Index < 225 && Roller.Index >= 0)
			{
				func_225(Roller.Index, vParam0);
				Roller.Index++;
			}
		}
	}
}
