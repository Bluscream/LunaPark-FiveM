using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace LunaPark
{
	internal class FerrisWheel : BaseScript
	{
		private WheelPan Wheel = new WheelPan(null, 0, "IDLE");

		public bool RideEnd = true;

		private bool Scaleform = false;

		private Scaleform Buttons = new Scaleform("instructional_buttons");

		private Camera Cam1 = new Camera(0);

		private FerrisWheelCamKeyboard Cam2Keyvoard = new FerrisWheelCamKeyboard();

		private FerrisWheelCamGamePad Cam2GamePad = new FerrisWheelCamGamePad();

		private int iLocal_355 = 0;

		private CabinPan ActualCabin;

		private CabinPan[] Cabins = new CabinPan[16]
		{
			new CabinPan(0),
			new CabinPan(1),
			new CabinPan(2),
			new CabinPan(3),
			new CabinPan(4),
			new CabinPan(5),
			new CabinPan(6),
			new CabinPan(7),
			new CabinPan(8),
			new CabinPan(9),
			new CabinPan(10),
			new CabinPan(11),
			new CabinPan(12),
			new CabinPan(13),
			new CabinPan(14),
			new CabinPan(15)
		};

		private bool Cambia = true;

		public FerrisWheel()
			: this()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Expected O, but got Unknown
			((BaseScript)this).get_EventHandlers().Add("onResourceStop", (Delegate)new Action<string>(OnStop));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:forceState", (Delegate)new Action<string>(ForceState));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:UpdateCabins", (Delegate)new Action<int, int>(UpdateCabins));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:StopWheel", (Delegate)new Action<bool>(WheelState));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:playerGetOn", (Delegate)new Action<int, int>(PlayerGetOn));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:playerGetOff", (Delegate)new Action<int, int>(PlayerGetOff));
			((BaseScript)this).get_EventHandlers().Add("FerrisWheel:UpdateGradient", (Delegate)new Action<int>(UpdateGradient));
			Blip val = new Blip(API.AddBlipForCoord(-1663.97f, -1126.7f, 30.7f));
			val.set_Sprite((BlipSprite)266);
			val.set_IsShortRange(true);
			val.set_Name("Ferris Wheel");
			Blip Ferris = val;
			API.SetBlipDisplay(((PoolObject)Ferris).get_Handle(), 4);
			CaricaTutto();
		}

		private async void UpdateGradient(int gradient)
		{
			Wheel.Gradient = gradient;
		}

		private async Task SpawnaWheel()
		{
			WheelPan wheel = Wheel;
			Prop val = new Prop(API.CreateObject(API.GetHashKey("prop_ld_ferris_wheel"), 0f, 1f, 2f, false, false, false));
			((Entity)val).set_Position(new Vector3(-1663.97f, -1126.7f, 30.7f));
			((Entity)val).set_Rotation(new Vector3(360f, 0f, 0f));
			((Entity)val).set_IsPositionFrozen(true);
			((Entity)val).set_LodDistance(1000);
			((Entity)val).set_IsInvincible(true);
			wheel.Entity = val;
			if (!API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
			{
				API.StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			}
			for (int i = 0; i < 16; i++)
			{
				await BaseScript.Delay(0);
				CabinPan obj = Cabins[i];
				Prop val2 = new Prop(API.CreateObject(API.GetHashKey("prop_ferris_car_01"), 0f, 1f, 2f, false, false, false));
				((Entity)val2).set_IsInvincible(true);
				((Entity)val2).set_Position(func_147(i));
				((Entity)val2).set_LodDistance(1000);
				((Entity)val2).set_IsPositionFrozen(true);
				obj.Entity = val2;
				Cabins[i].Index = i;
			}
			await Task.FromResult(0);
		}

		private void UpdateCabins(int index, int players)
		{
			Cabins[index].NPlayer = players;
		}

		private void WheelState(bool stato)
		{
			Wheel.Ferma = stato;
		}

		private async void CaricaTutto()
		{
			API.RequestModel((uint)API.GetHashKey("prop_ld_ferris_wheel"));
			while (!API.HasModelLoaded((uint)API.GetHashKey("prop_ld_ferris_wheel")))
			{
				await BaseScript.Delay(100);
			}
			API.RequestModel((uint)API.GetHashKey("prop_ferris_car_01"));
			while (!API.HasModelLoaded((uint)API.GetHashKey("prop_ferris_car_01")))
			{
				await BaseScript.Delay(100);
			}
			API.RequestAnimDict("anim@mp_ferris_wheel");
			while (!API.HasAnimDictLoaded("anim@mp_ferris_wheel"))
			{
				await BaseScript.Delay(100);
			}
			API.RequestScriptAudioBank("SCRIPT\\FERRIS_WHALE_01", false);
			API.RequestScriptAudioBank("SCRIPT\\FERRIS_WHALE_02", false);
			API.RequestScriptAudioBank("THE_FERRIS_WHALE_SOUNDSET", false);
			await SpawnaWheel();
			((BaseScript)this).add_Tick((Func<Task>)MuoviWheel);
			((BaseScript)this).add_Tick((Func<Task>)ControlloPlayer);
		}

		private async Task MuoviWheel()
		{
			if (!Wheel.Ferma && (Entity)(object)Wheel.Entity != (Entity)null)
			{
				float fVar2 = 0f;
				if (iLocal_355 != 0)
				{
					fVar2 = (float)API.GetTimeDifference(API.GetNetworkTimeAccurate(), iLocal_355) / 800f;
				}
				iLocal_355 = API.GetNetworkTimeAccurate();
				float speed = Wheel.Speed * fVar2;
				Wheel.Rotation += speed;
				if (Wheel.Rotation >= 360f)
				{
					Wheel.Rotation -= 360f;
				}
				if (API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				{
					API.SetAudioSceneVariable("FAIRGROUND_RIDES_FERRIS_WHALE", "HEIGHT", ((Entity)Game.get_PlayerPed()).get_Position().Z - 13f);
				}
				CabinPan[] cabins = Cabins;
				foreach (CabinPan cab in cabins)
				{
					if (!(Math.Abs(Wheel.Rotation - cab.Gradient) < 0.1f))
					{
						continue;
					}
					Wheel.Gradient = ((cab.Index + 1 <= 15) ? (cab.Index + 1) : 0);
					BaseScript.TriggerServerEvent("FerrisWheel:UpdateGradient", new object[1] { Wheel.Gradient });
					string state = Wheel.State;
					string text = state;
					if (text == null)
					{
						continue;
					}
					string text2 = text;
					if (!(text2 == "Let_IN"))
					{
						if (text2 == "Let_Out")
						{
							BaseScript.TriggerServerEvent("FerrisWheel:playerGetOff", new object[2]
							{
								((Entity)Game.get_PlayerPed()).get_NetworkId(),
								Wheel.Gradient
							});
						}
					}
					else
					{
						ActualCabin = Cabins[Wheel.Gradient];
						BaseScript.TriggerServerEvent("FerrisWheel:playerGetOn", new object[2]
						{
							((Entity)Game.get_PlayerPed()).get_NetworkId(),
							Wheel.Gradient
						});
					}
				}
				Vector3 pitch = new Vector3(0f - Wheel.Rotation - 22.5f, 0f, 0f);
				((Entity)Wheel.Entity).set_Rotation(pitch);
				Cabins.ToList().ForEach(delegate(CabinPan o)
				{
					func_145(Cabins.ToList().IndexOf(o));
				});
				API.SetAudioSceneVariable("FAIRGROUND_RIDES_FERRIS_WHALE", "HEIGHT", ((Entity)Game.get_PlayerPed()).get_Position().Z - 13f);
			}
			await Task.FromResult(0);
		}

		private async void PlayerGetOn(int player, int cab)
		{
			Ped Char = (Ped)Entity.FromNetworkId(player);
			CabinPan Cabin = Cabins[cab];
			if (!API.IsEntityAtCoord(((PoolObject)Char).get_Handle(), -1661.95f, -1127.011f, 12.6973f, 1f, 1f, 1f, false, true, 0))
			{
				return;
			}
			if (((Entity)Char).get_NetworkId() != ((Entity)Game.get_PlayerPed()).get_NetworkId() && !API.NetworkHasControlOfNetworkId(player))
			{
				while (!API.NetworkRequestControlOfNetworkId(player))
				{
					await BaseScript.Delay(0);
				}
			}
			BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", new object[1] { true });
			Wheel.Ferma = true;
			await BaseScript.Delay(100);
			Vector3 coord = API.GetOffsetFromEntityInWorldCoords(((PoolObject)Cabin.Entity).get_Handle(), 0f, 0f, 0f);
			int uLocal_377 = API.NetworkCreateSynchronisedScene(coord.X, coord.Y, coord.Z, 0f, 0f, 0f, 2, true, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
			API.NetworkAddPedToSynchronisedScene(((PoolObject)Char).get_Handle(), uLocal_377, "anim@mp_ferris_wheel", "enter_player_one", 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
			API.NetworkStartSynchronisedScene(uLocal_377);
			int iVar2 = API.NetworkConvertSynchronisedSceneToSynchronizedScene(uLocal_377);
			if (API.GetSynchronizedScenePhase(iVar2) > 0.99f)
			{
				uLocal_377 = API.NetworkCreateSynchronisedScene(coord.X, coord.Y, coord.Z, 0f, 0f, 0f, 2, true, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
				API.NetworkAddPedToSynchronisedScene(((PoolObject)Char).get_Handle(), uLocal_377, "anim@mp_ferris_wheel", "enter_player_one", 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
				API.NetworkStartSynchronisedScene(uLocal_377);
			}
			await BaseScript.Delay(7000);
			Vector3 attCoords = API.GetOffsetFromEntityGivenWorldCoords(((PoolObject)Cabin.Entity).get_Handle(), ((Entity)Game.get_PlayerPed()).get_Position().X, ((Entity)Game.get_PlayerPed()).get_Position().Y, ((Entity)Game.get_PlayerPed()).get_Position().Z);
			API.AttachEntityToEntity(((PoolObject)Char).get_Handle(), ((PoolObject)Cabin.Entity).get_Handle(), 0, attCoords.X, attCoords.Y, attCoords.Z, 0f, 0f, ((Entity)Game.get_PlayerPed()).get_Heading(), false, false, false, false, 2, true);
			BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", new object[2] { Cabin.Index, Cabin.NPlayer });
			if (((PoolObject)Char).get_Handle() == API.PlayerPedId())
			{
				RideEnd = false;
			}
			Wheel.State = "IDLE";
			BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", new object[1] { false });
			int iLocal_297 = API.GetSoundId();
			API.PlaySoundFromEntity(iLocal_297, "GENERATOR", ((PoolObject)Wheel.Entity).get_Handle(), "THE_FERRIS_WHALE_SOUNDSET", false, 0);
			int iLocal_299 = API.GetSoundId();
			API.PlaySoundFromEntity(iLocal_299, "SLOW_SQUEAK", ((PoolObject)Wheel.Entity).get_Handle(), "THE_FERRIS_WHALE_SOUNDSET", false, 0);
			int iLocal_300 = API.GetSoundId();
			API.PlaySoundFromEntity(iLocal_300, "SLOW_SQUEAK", ((PoolObject)Cabins[1].Entity).get_Handle(), "THE_FERRIS_WHALE_SOUNDSET", false, 0);
			int iLocal_298 = API.GetSoundId();
			API.PlaySoundFromEntity(iLocal_298, "CARRIAGE", ((PoolObject)Cabins[1].Entity).get_Handle(), "THE_FERRIS_WHALE_SOUNDSET", false, 0);
			if (((PoolObject)Char).get_Handle() == API.PlayerPedId())
			{
				CreaCam();
			}
		}

		private async void PlayerGetOff(int player, int cab)
		{
			Ped Char = (Ped)Entity.FromNetworkId(player);
			CabinPan Cabin = Cabins[cab];
			if ((Entity)(object)Char == (Entity)(object)Game.get_PlayerPed())
			{
				while (ActualCabin != Cabin)
				{
					await BaseScript.Delay(0);
				}
				API.RenderScriptCams(false, false, 1000, false, false);
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", new object[1] { true });
				Vector3 offset2 = API.GetOffsetFromEntityInWorldCoords(((PoolObject)Cabin.Entity).get_Handle(), 0f, 0f, 0f);
				((PoolObject)Cam1).Delete();
				API.DestroyAllCams(false);
				int uLocal_378 = API.NetworkCreateSynchronisedScene(offset2.X, offset2.Y, offset2.Z, 0f, 0f, 0f, 2, false, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
				API.NetworkAddPedToSynchronisedScene(((PoolObject)Char).get_Handle(), uLocal_378, "anim@mp_ferris_wheel", "exit_player_one", 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
				API.NetworkStartSynchronisedScene(uLocal_378);
				((Entity)Char).Detach();
				await BaseScript.Delay(5000);
				Cabin.NPlayer = 0;
				BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", new object[2] { Cabin.Index, Cabin.NPlayer });
				if (API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				{
					API.StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
				}
				if (API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW"))
				{
					API.StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW");
				}
				if (((PoolObject)Char).get_Handle() == API.PlayerPedId())
				{
					RideEnd = true;
				}
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", new object[1] { false });
				Wheel.State = "IDLE";
				ActualCabin = null;
				return;
			}
			if (((Entity)Char).get_NetworkId() != ((Entity)Game.get_PlayerPed()).get_NetworkId() && !API.NetworkHasControlOfNetworkId(player))
			{
				while (!API.NetworkRequestControlOfNetworkId(player))
				{
					await BaseScript.Delay(0);
				}
			}
			Vector3 offset = API.GetOffsetFromEntityInWorldCoords(((PoolObject)Cabin.Entity).get_Handle(), 0f, 0f, 0f);
			int uLocal_377 = API.NetworkCreateSynchronisedScene(offset.X, offset.Y, offset.Z, 0f, 0f, 0f, 2, false, false, 1.06535322E+09f, 0f, 1.06535322E+09f);
			API.NetworkAddPedToSynchronisedScene(((PoolObject)Char).get_Handle(), uLocal_377, "anim@mp_ferris_wheel", "exit_player_one", 8f, -8f, 131072, 0, 1.14884608E+09f, 0);
			API.NetworkStartSynchronisedScene(uLocal_377);
			((Entity)Char).Detach();
			await BaseScript.Delay(5000);
			Cabin.NPlayer = 0;
			BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", new object[2] { Cabin.Index, Cabin.NPlayer });
		}

		private async Task ControlloPlayer()
		{
			if (((Entity)Game.get_PlayerPed()).IsInRangeOf(new Vector3(-1661.95f, -1127.011f, 12.6973f), 20f))
			{
				if (!API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				{
					API.StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
				}
				if (((Entity)Game.get_PlayerPed()).IsInRangeOf(new Vector3(-1661.95f, -1127.011f, 12.6973f), 1.375f))
				{
					Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to get on the Wheel");
					if (Game.IsControlJustPressed(0, (Control)51))
					{
						Screen.ShowNotification("Wait.. The first free cabin is coming..", false);
						BaseScript.TriggerServerEvent("FerrisWheel:syncState", new object[1] { "Let_IN" });
					}
				}
			}
			else
			{
				API.StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			}
			if (!RideEnd)
			{
				if (API.GetFollowPedCamViewMode() == 4)
				{
					API.SetFollowPedCamViewMode(2);
				}
				Game.DisableControlThisFrame(0, (Control)0);
				UpdateTasti();
				if (Game.IsControlJustPressed(0, (Control)204))
				{
					Screen.ShowNotification("The wheel will stop once your cabin reaches ground to let you get off", false);
					Wheel.State = "Let_Out";
				}
				if (Game.IsControlJustPressed(0, (Control)236))
				{
					CambiaCam();
				}
			}
			await Task.FromResult(0);
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
				Buttons.CallFunction("CLEAR_ALL", new object[0]);
				Buttons.CallFunction("SET_DATA_SLOT", new object[3]
				{
					0,
					API.GetControlInstructionalButton(2, 236, 1),
					"Change Visual"
				});
				Buttons.CallFunction("SET_DATA_SLOT", new object[3]
				{
					1,
					API.GetControlInstructionalButton(2, 204, 1),
					"Get off the Wheel"
				});
				Buttons.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", new object[1] { -1 });
				Scaleform = true;
			}
			if (Scaleform)
			{
				Buttons.Render2D();
			}
		}

		private async void func_145(int i)
		{
			Vector3 offset = func_147(i);
			API.SetEntityCoordsNoOffset(((PoolObject)Cabins[i].Entity).get_Handle(), offset.X, offset.Y, offset.Z, true, false, false);
		}

		private void func_79()
		{
			if (API.IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
			{
				API.StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			}
			API.StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW");
		}

		private async void CreaCam()
		{
			Cam1 = new Camera(API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", -1703.854f, -1082.222f, 42.006f, -8.3096f, 0f, -111.8213f, 50f, false, 0));
			Cam1.PointAt((Entity)(object)Wheel.Entity, default(Vector3));
			Cam1.set_IsActive(true);
			Fading.FadeOut(500);
			await BaseScript.Delay(800);
			API.RenderScriptCams(true, false, 1000, false, false);
			Fading.FadeIn(500);
			func_79();
			API.SetLocalPlayerInvisibleLocally(false);
		}

		private void CambiaCam()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Expected O, but got Unknown
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			Cambia = !Cambia;
			if (Cambia)
			{
				Cam1 = new Camera(API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", -1703.854f, -1082.222f, 42.006f, -8.3096f, 0f, -111.8213f, 50f, false, 0));
				Cam1.PointAt((Entity)(object)Wheel.Entity, default(Vector3));
				func_79();
				Cam1.set_IsActive(true);
				((BaseScript)this).remove_Tick((Func<Task>)Cam2Controller);
				if (API.IsInputDisabled(2))
				{
					((PoolObject)Cam2Keyvoard.CamEntity).Delete();
					Cam2Keyvoard.Value1 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value4 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value7 = 0f;
					Cam2Keyvoard.Value20 = 0;
					Cam2Keyvoard.Value21 = 0;
					Cam2Keyvoard.Value22 = 0;
					Cam2Keyvoard.Value8 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value11 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value14 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value17 = 0f;
					Cam2Keyvoard.Value18 = 0f;
					Cam2Keyvoard.Value23 = 0;
					Cam2Keyvoard.Value19 = 0;
					Cam2Keyvoard.Value24 = 0;
					Cam2Keyvoard.Value25 = 0;
					Cam2Keyvoard.Value29 = 0f;
					Cam2Keyvoard.Value30 = 0f;
					Cam2Keyvoard.Value26 = false;
					Cam2Keyvoard.Value28 = false;
					Cam2Keyvoard.Value27 = 0f;
				}
			}
			else
			{
				if (API.IsInputDisabled(2))
				{
					Vector3 PedCoords = API.GetPedBoneCoords(API.PlayerPedId(), 31086, 0f, 0.2f, 0f);
					Vector3 Rot = API.GetEntityRotation(API.PlayerPedId(), 2);
					Cam2Keyvoard.CamEntity = new Camera(API.CreateCam("DEFAULT_SCRIPTED_CAMERA", false));
					API.SetCamParams(((PoolObject)Cam2Keyvoard.CamEntity).get_Handle(), PedCoords.X, PedCoords.Y, PedCoords.Z, Rot.X, Rot.Y, Rot.Z, 50f, 0, 1, 1, 2);
					Cam2Keyvoard.CamEntity.set_IsActive(true);
					Cam2Keyvoard.CamEntity.Shake((CameraShake)0, 0.19f);
					API.SetCamNearClip(((PoolObject)Cam2Keyvoard.CamEntity).get_Handle(), -1.08213043E+09f);
					API.AttachCamToPedBone(((PoolObject)Cam2Keyvoard.CamEntity).get_Handle(), API.PlayerPedId(), 31086, 0f, 0.2f, 0f, true);
					API.SetLocalPlayerInvisibleLocally(false);
					Cam2Keyvoard.Value1 = PedCoords;
					Cam2Keyvoard.Value4 = Rot;
					Cam2Keyvoard.Value7 = 50f;
					Cam2Keyvoard.Value20 = 160;
					Cam2Keyvoard.Value21 = 20;
					Cam2Keyvoard.Value22 = 3;
					Cam2Keyvoard.Value8 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value11 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value14 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value17 = 50f;
					Cam2Keyvoard.Value18 = 50f;
					Cam2Keyvoard.Value23 = 0;
					Cam2Keyvoard.Value19 = 1101004800;
					Cam2Keyvoard.Value24 = 0;
					Cam2Keyvoard.Value25 = 0;
					Cam2Keyvoard.Value29 = 0f;
					Cam2Keyvoard.Value30 = 0f;
					Cam2Keyvoard.Value26 = false;
					Cam2Keyvoard.Value28 = false;
					Cam2Keyvoard.Value27 = 0f;
				}
				else
				{
					func_110(Cam2GamePad, bParam1: true);
					func_109(Cam2GamePad, 0, 3000);
				}
				((BaseScript)this).add_Tick((Func<Task>)Cam2Controller);
				((PoolObject)Cam1).Delete();
			}
			API.RenderScriptCams(true, false, 3000, true, false);
		}

		private async Task Cam2Controller()
		{
			if (API.IsInputDisabled(2))
			{
				func_101(Cam2Keyvoard, bParam1: true, bParam2: true, bParam3: false, bParam4: false, 0.1f, bParam6: false, 1.06535322E+09f, bParam8: false);
			}
			else
			{
				func_105(Cam2GamePad);
			}
		}

		private async void func_105(FerrisWheelCamGamePad uParam0)
		{
			float uVar0 = 0f;
			float uVar1 = 0f;
			if (!uParam0.Value1)
			{
				return;
			}
			API.DisableInputGroup(2);
			if (!uParam0.Value0)
			{
				return;
			}
			if (API.Absf(API.GetControlNormal(2, 220)) > 0.1f)
			{
				uParam0.Value12 -= API.GetControlNormal(2, 220) * 60f * API.Timestep();
				if (uParam0.Value15)
				{
					if (uParam0.Value12 < -110f)
					{
						uParam0.Value12 = -110f;
					}
					if (uParam0.Value12 > 110f)
					{
						uParam0.Value12 = 110f;
					}
				}
				else
				{
					uParam0.Value12 = func_102(uParam0.Value12, -80f, 80f);
				}
			}
			if (API.Absf(API.GetControlNormal(2, 221)) > 0.1f)
			{
				float fVar3 = API.GetControlNormal(2, 221) * 60f * API.Timestep();
				if (API.IsLookInverted())
				{
					fVar3 *= -1f;
				}
				uParam0.Value11 -= fVar3;
				if (uParam0.Value14)
				{
					if (uParam0.Value11 < -30f)
					{
						uParam0.Value11 = -30f;
					}
					if (uParam0.Value11 > 30f)
					{
						uParam0.Value11 = 30f;
					}
				}
				else
				{
					uParam0.Value11 = func_102(uParam0.Value11, -30f, 30f);
				}
			}
			if (API.IsControlJustPressed(2, 231))
			{
				uParam0.Value11 = 0f;
				uParam0.Value12 = 0f;
			}
			if (API.Absf(API.GetControlNormal(2, 219)) > 0.1f)
			{
				float fVar3 = API.GetControlNormal(2, 219) * 30f * API.Timestep();
				uParam0.Value13 += fVar3;
				uParam0.Value13 = func_102(uParam0.Value13, 20f, 50f);
			}
			if (API.DoesCamExist(((PoolObject)uParam0.CamEntity).get_Handle()))
			{
				API.SetCamFov(((PoolObject)uParam0.CamEntity).get_Handle(), uParam0.Value13);
				if (API.IsEntityDead(uParam0.Value8) && !API.IsEntityDead(API.PlayerPedId()))
				{
					API.SetCamRot(((PoolObject)uParam0.CamEntity).get_Handle(), (((Entity)Game.get_PlayerPed()).get_Rotation() + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).X, (((Entity)Game.get_PlayerPed()).get_Rotation() + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Y, (((Entity)Game.get_PlayerPed()).get_Rotation() + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Z, 2);
				}
				else if (!API.IsEntityDead(uParam0.Value8) && !API.IsEntityDead(API.PlayerPedId()))
				{
					func_106(API.GetEntityCoords(uParam0.Value8, true), API.GetEntityCoords(uParam0.Value9, true), ref uVar0, ref uVar1, 1);
					API.SetCamRot(((PoolObject)uParam0.CamEntity).get_Handle(), (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).X, (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Y, (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Z, 2);
				}
			}
		}

		private void func_106(Vector3 vParam0, Vector3 vParam1, ref float uParam2, ref float uParam3, int iParam4)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 vVar0 = vParam1 - vParam0;
			func_107(vVar0, ref uParam2, ref uParam3, iParam4);
		}

		private void func_107(Vector3 vParam0, ref float uParam1, ref float uParam2, int iParam3)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (vParam0.Y != 0f)
			{
				uParam2 = API.Atan2(vParam0.X, vParam0.Y);
			}
			else if (vParam0.X < 0f)
			{
				uParam2 = -90f;
			}
			else
			{
				uParam2 = 90f;
			}
			if (iParam3 == 1)
			{
				uParam2 *= -1f;
				if (uParam2 < 0f)
				{
					uParam2 += 360f;
				}
			}
			float fVar0 = API.Sqrt(vParam0.X * vParam0.X + vParam0.Y * vParam0.Y);
			if (fVar0 != 0f)
			{
				uParam1 = API.Atan2(vParam0.Z, fVar0);
			}
			else if (vParam0.Z < 0f)
			{
				uParam1 = -90f;
			}
			else
			{
				uParam1 = 90f;
			}
		}

		private int func_109(FerrisWheelCamGamePad uParam0, int iParam1, int iParam2)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			if (!uParam0.Value1)
			{
				return 0;
			}
			uParam0.Value13 = 50f;
			if (!API.DoesCamExist(((PoolObject)uParam0.CamEntity).get_Handle()))
			{
				uParam0.CamEntity = new Camera(API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", uParam0.Value2.X, uParam0.Value2.Y, uParam0.Value2.Z, uParam0.Value5.X, uParam0.Value5.Y, uParam0.Value5.Z, 50f, true, 2));
			}
			if (uParam0.Value0)
			{
				API.AttachCamToPedBone(((PoolObject)uParam0.CamEntity).get_Handle(), API.PlayerPedId(), 31086, 0f, 0.2f, 0f, true);
				uParam0.Value11 = 0f;
				uParam0.Value12 = 0f;
			}
			if (API.DoesCamExist(((PoolObject)uParam0.CamEntity).get_Handle()))
			{
				API.SetCamActive(((PoolObject)uParam0.CamEntity).get_Handle(), true);
			}
			return 1;
		}

		private void func_110(FerrisWheelCamGamePad uParam0, bool bParam1)
		{
			uParam0.Value0 = true;
			uParam0.Value1 = true;
			uParam0.Value9 = API.PlayerPedId();
			uParam0.Value11 = 0f;
			uParam0.Value12 = 0f;
			if (bParam1)
			{
				uParam0.Value15 = true;
			}
		}

		private async void func_101(FerrisWheelCamKeyboard uParam0, bool bParam1, bool bParam2, bool bParam3, bool bParam4, float fParam5, bool bParam6, float fParam7, bool bParam8)
		{
			int[] iVar0 = new int[4];
			API.DisableInputGroup(2);
			func_104(ref iVar0[0], ref iVar0[1], ref iVar0[2], ref iVar0[3], bParam4: false, bParam5: false);
			if (API.IsLookInverted())
			{
				iVar0[3] *= -1;
			}
			if (API.IsInputDisabled(2))
			{
				float fVar1 = API.GetControlUnboundNormal(2, 239);
				float fVar2 = API.GetControlUnboundNormal(2, 240);
				float fVar3 = fVar1 - uParam0.Value29;
				float fVar4 = fVar2 - uParam0.Value30;
				uParam0.Value29 = fVar1;
				uParam0.Value30 = fVar2;
				if (bParam4)
				{
					iVar0[2] = -(int)Math.Round(fVar3 * fParam5 * 127f);
					iVar0[3] = -(int)Math.Round(fVar4 * fParam5 * 127f);
				}
				else
				{
					iVar0[2] = (int)Math.Round(API.GetControlUnboundNormal(2, 290) * fParam5 * 127f);
					iVar0[3] = (int)Math.Round(API.GetControlUnboundNormal(2, 291) * fParam5 * 127f);
				}
				iVar0[2] = func_103(iVar0[2] + uParam0.Value24, -127, 127);
				iVar0[3] = func_103(iVar0[3] + uParam0.Value25, -127, 127);
			}
			if (uParam0.Value24 == iVar0[2] && uParam0.Value25 == iVar0[3])
			{
				if (uParam0.Value27 < (float)API.GetGameTimer())
				{
					uParam0.Value24 = 0;
					uParam0.Value25 = 0;
					if (API.IsInputDisabled(2))
					{
						iVar0[2] = 0;
						iVar0[3] = 0;
						uParam0.Value28 = true;
					}
				}
			}
			else
			{
				uParam0.Value24 = iVar0[2];
				uParam0.Value25 = iVar0[3];
				uParam0.Value27 = API.GetGameTimer() + 4000;
				uParam0.Value28 = false;
			}
			if (bParam2)
			{
				uParam0.Value8.Z = (0f - API.ToFloat(iVar0[2]) / 127f) * (float)uParam0.Value20;
				uParam0.Value8.Y = (0f - uParam0.Value8.Z) * (float)uParam0.Value22 / (float)uParam0.Value20;
				uParam0.Value8.X = (0f - API.ToFloat(iVar0[3]) / 127f) * (float)uParam0.Value21;
			}
			else
			{
				uParam0.Value8 = new Vector3(0f, 0f, 0f);
				uParam0.Value24 = 0;
				uParam0.Value25 = 0;
			}
			float fVar5 = 30f * API.Timestep();
			Vector3 vVar6 = uParam0.Value8 + uParam0.Value11;
			if (API.IsInputDisabled(2) && bParam2 && !uParam0.Value28)
			{
				uParam0.Value14.X = vVar6.X;
				uParam0.Value14.Y = vVar6.Y;
				uParam0.Value14.Z = vVar6.Z;
			}
			else
			{
				uParam0.Value14.X += func_102((vVar6.X - uParam0.Value14.X) * 0.05f * fVar5 * fParam7, -3f, 3f);
				uParam0.Value14.Y += func_102((vVar6.Y - uParam0.Value14.Y) * 0.05f * fVar5 * fParam7, -3f, 3f);
				uParam0.Value14.Z += func_102((vVar6.Z - uParam0.Value14.Z) * 0.05f * fVar5 * fParam7, -3f, 3f);
			}
			if (uParam0.Value26)
			{
				uParam0.Value14.X = func_102(uParam0.Value14.X, -uParam0.Value21, uParam0.Value21);
				uParam0.Value14.Y = func_102(uParam0.Value14.Y, -uParam0.Value22, uParam0.Value22);
				uParam0.Value14.Z = func_102(uParam0.Value14.Z, -uParam0.Value20, uParam0.Value20);
			}
			if (API.IsInputDisabled(0) && bParam1)
			{
				if (uParam0.Value28)
				{
					uParam0.Value17 = uParam0.Value7;
				}
			}
			else
			{
				uParam0.Value17 = uParam0.Value7;
			}
			if (bParam1)
			{
				if (API.IsInputDisabled(0))
				{
					int iVar = 40;
					int iVar2 = 41;
					if (bParam6)
					{
						iVar = 241;
						iVar2 = 242;
					}
					if (API.IsDisabledControlJustPressed(0, iVar))
					{
						uParam0.Value17 -= 5f;
						uParam0.Value27 = API.GetGameTimer() + 4000;
						uParam0.Value28 = false;
					}
					else if (API.IsDisabledControlJustPressed(0, iVar2))
					{
						uParam0.Value17 += 5f;
						uParam0.Value27 = API.GetGameTimer() + 4000;
						uParam0.Value28 = false;
					}
					if (bParam3)
					{
						uParam0.Value17 = func_102(uParam0.Value17, uParam0.Value7 - (float)uParam0.Value19, uParam0.Value7);
					}
					else
					{
						uParam0.Value17 = func_102(uParam0.Value17, uParam0.Value7 - (float)uParam0.Value19, uParam0.Value7 + (float)uParam0.Value19);
					}
				}
				else if (bParam8)
				{
					iVar0[1] = API.GetControlValue(2, 207);
					iVar0[3] = API.GetControlValue(2, 208);
					if (bParam3)
					{
						if (API.ToFloat(iVar0[3]) > 127f)
						{
							uParam0.Value17 -= (int)Math.Round((float)iVar0[3] / 128f * ((float)uParam0.Value19 / 2f));
						}
					}
					else
					{
						uParam0.Value17 += (int)Math.Round((float)iVar0[1] / 128f * (float)uParam0.Value19);
						uParam0.Value17 -= (int)Math.Round((float)iVar0[3] / 128f * (float)uParam0.Value19);
					}
				}
				else if (bParam3)
				{
					if ((float)iVar0[1] < 0f)
					{
						uParam0.Value17 += (int)Math.Round((float)iVar0[1] / 128f * (float)uParam0.Value19);
					}
				}
				else
				{
					uParam0.Value17 += API.Round(API.ToFloat(iVar0[1]) / 128f * (float)uParam0.Value19);
				}
			}
			uParam0.Value18 += (uParam0.Value17 - uParam0.Value18) * 0.06f * fVar5;
			API.SetCamParams(((PoolObject)uParam0.CamEntity).get_Handle(), uParam0.Value1.X, uParam0.Value1.Y, uParam0.Value1.Z, (uParam0.Value4 + uParam0.Value14).X, (uParam0.Value4 + uParam0.Value14).Y, (uParam0.Value4 + uParam0.Value14).Z, uParam0.Value18, 0, 1, 1, 2);
			uParam0.CamEntity.set_Rotation(((Entity)Game.get_PlayerPed()).get_Rotation() + Cam2Keyvoard.Value14);
		}

		private float func_102(float fParam0, float fParam1, float fParam2)
		{
			return (fParam0 > fParam2) ? fParam2 : ((fParam0 < fParam1) ? fParam1 : fParam0);
		}

		private int func_103(int iParam0, int iParam1, int iParam2)
		{
			return (iParam0 > iParam2) ? iParam2 : ((iParam0 < iParam1) ? iParam1 : iParam0);
		}

		private void func_104(ref int uParam0, ref int uParam1, ref int uParam2, ref int uParam3, bool bParam4, bool bParam5)
		{
			uParam0 = API.Floor(API.GetControlUnboundNormal(2, 218) * 127f);
			uParam1 = API.Floor(API.GetControlUnboundNormal(2, 219) * 127f);
			uParam2 = API.Floor(API.GetControlUnboundNormal(2, 220) * 127f);
			uParam3 = API.Floor(API.GetControlUnboundNormal(2, 221) * 127f);
			if (bParam4)
			{
				if (!API.IsControlEnabled(2, 218))
				{
					uParam0 = API.Floor(API.GetDisabledControlUnboundNormal(2, 218) * 127f);
				}
				if (!API.IsControlEnabled(2, 219))
				{
					uParam1 = API.Floor(API.GetDisabledControlUnboundNormal(2, 219) * 127f);
				}
				if (!API.IsControlEnabled(2, 220))
				{
					uParam2 = API.Floor(API.GetDisabledControlUnboundNormal(2, 220) * 127f);
				}
				if (!API.IsControlEnabled(2, 221))
				{
					uParam3 = API.Floor(API.GetDisabledControlUnboundNormal(2, 221) * 127f);
				}
			}
			if (API.IsInputDisabled(2) && bParam5)
			{
				if (API.IsLookInverted())
				{
					uParam3 *= -1;
				}
				if (API.N_0xe1615ec03b3bb4fd())
				{
					uParam3 *= -1;
				}
			}
		}

		private void ForceState(string state)
		{
			Wheel.State = state;
		}

		private Vector3 func_147(int iParam0)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			float fVar0 = 0.392699361f * (float)iParam0;
			return API.GetOffsetFromEntityInWorldCoords(((PoolObject)Wheel.Entity).get_Handle(), 0f, Deg2rad(15.3f) * Rad2deg((float)Math.Sin(fVar0)), Deg2rad(-15.3f) * Rad2deg((float)Math.Cos(fVar0)));
		}

		private void OnStop(string name)
		{
			if (name == API.GetCurrentResourceName() && (Entity)(object)Wheel.Entity != (Entity)null)
			{
				((PoolObject)Wheel.Entity).Delete();
				for (int i = 0; i < 16; i++)
				{
					((PoolObject)Cabins[i].Entity).Delete();
				}
			}
		}

		private int GetCabIndex(Prop cabin)
		{
			CabinPan[] cabins = Cabins;
			foreach (CabinPan cab in cabins)
			{
				if ((Entity)(object)cabin == (Entity)(object)cab.Entity)
				{
					return cab.Index;
				}
			}
			return -1;
		}

		public float Rad2deg(float rad)
		{
			return rad * (180f / (float)Math.PI);
		}

		public float Deg2rad(float deg)
		{
			return deg * ((float)Math.PI / 180f);
		}
	}
}
