using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;

namespace LunaParkClient
{
	static class FerrisWheel
	{
		static WheelPan Wheel = new WheelPan(null, 0, "IDLE");
		public static bool RideEnd = true;
		static bool Scaleform = false;
		static Scaleform Buttons = new Scaleform("instructional_buttons");
		static Camera Cam1 = new Camera(0);
		static FerrisWheelCamKeyboard Cam2Keyvoard = new FerrisWheelCamKeyboard();
		static FerrisWheelCamGamePad Cam2GamePad = new FerrisWheelCamGamePad();
		static int iLocal_355 = 0;
		static CabinPan ActualCabin;
		static CabinPan[] Cabins = new CabinPan[16]
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
			new CabinPan(15),
	};
		public static void Init()
		{
			Client.Instance.AddEventHandler("onResourceStop", new Action<string>(OnStop));
			Client.Instance.AddEventHandler("FerrisWheel:forceState", new Action<string>(ForceState));
			Client.Instance.AddEventHandler("FerrisWheel:UpdateCabins", new Action<int, int>(UpdateCabins));
			Client.Instance.AddEventHandler("FerrisWheel:StopWheel", new Action<bool>(WheelState));
			Client.Instance.AddEventHandler("FerrisWheel:playerGetOn", new Action<int, int>(PlayerGetOn));
			Client.Instance.AddEventHandler("FerrisWheel:playerGetOff", new Action<int, int>(PlayerGetOff));
			Client.Instance.AddEventHandler("FerrisWheel:UpdateGradient", new Action<int>(UpdateGradient));
			Blip Ferris = new Blip(AddBlipForCoord(-1663.97f, -1126.7f, 30.7f))
			{

				Sprite = BlipSprite.Fairground,
				IsShortRange = true,
				Name = "Ferris Wheel"
			};
			SetBlipDisplay(Ferris.Handle, 4);
			CaricaTutto();
		}

		private static async void UpdateGradient(int gradient)
		{
			Wheel.Gradient = gradient;
		}

		private static async Task SpawnaWheel()
		{
			Wheel.Entity = new Prop(CreateObject(GetHashKey("prop_ld_ferris_wheel"), 0f, 1f, 2f, false, false, false))
			{
				Position = new Vector3(-1663.97f, -1126.7f, 30.7f),
				Rotation = new Vector3(360, 0, 0),
				IsPositionFrozen = true,
				LodDistance = 1000,
				IsInvincible = true,
			};
			if (!IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			int i = 0;
			while (i < 16)
			{
				await BaseScript.Delay(0);
				Cabins[i].Entity = new Prop(CreateObject(GetHashKey("prop_ferris_car_01"), 0f, 1f, 2f, false, false, false))
				{
					IsInvincible = true,
					Position = func_147(i),
					LodDistance = 1000,
					IsPositionFrozen = true,
				};
				Cabins[i].Index = i;
				i++;
			}
			await Task.FromResult(0);
		}

		private static void UpdateCabins(int index, int players)
		{
			Cabins[index].NPlayer = players;
		}

		private static void WheelState(bool stato)
		{
			Wheel.Ferma = stato;
		}

		private static async void CaricaTutto()
		{
			RequestModel((uint)GetHashKey("prop_ld_ferris_wheel"));
			while (!HasModelLoaded((uint)GetHashKey("prop_ld_ferris_wheel"))) await BaseScript.Delay(100);
			RequestModel((uint)GetHashKey("prop_ferris_car_01"));
			while (!HasModelLoaded((uint)GetHashKey("prop_ferris_car_01"))) await BaseScript.Delay(100);
			RequestAnimDict("anim@mp_ferris_wheel");
			while (!HasAnimDictLoaded("anim@mp_ferris_wheel")) await BaseScript.Delay(100);
			RequestScriptAudioBank("SCRIPT\\FERRIS_WHALE_01", false);
			RequestScriptAudioBank("SCRIPT\\FERRIS_WHALE_02", false);
			RequestScriptAudioBank("THE_FERRIS_WHALE_SOUNDSET", false);
			await SpawnaWheel();
			Client.Instance.AddTick(MuoviWheel);
		}

		private static async Task MuoviWheel()
		{
			if (!Wheel.Ferma && Wheel.Entity != null)
			{
				float fVar2 = 0;
				if (iLocal_355 != 0)
					fVar2 = GetTimeDifference(GetNetworkTimeAccurate(), iLocal_355) / 800f;
				iLocal_355 = GetNetworkTimeAccurate();
				float speed = Wheel.Speed * fVar2;
				Wheel.Rotation += speed;

				if (Wheel.Rotation >= 360f)
					Wheel.Rotation -= 360f;

				if (IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				{
					Vector3 vVar1 = Game.PlayerPed.Position;
					SetAudioSceneVariable("FAIRGROUND_RIDES_FERRIS_WHALE", "HEIGHT", vVar1.Z - 13f);
				}

				foreach (var cab in Cabins)
				{
					if (Math.Abs(Wheel.Rotation - cab.Gradient) < 0.05f)
					{
						Wheel.Gradient = cab.Index + 1 > 15 ? 0 : cab.Index + 1;
						BaseScript.TriggerServerEvent("FerrisWheel:UpdateGradient", Wheel.Gradient);
						switch (Wheel.State)
						{
							case "FACCIO_SALIRE":
								ActualCabin = Cabins[Wheel.Gradient];
								BaseScript.TriggerServerEvent("FerrisWheel:playerGetOn",
									Game.PlayerPed.NetworkId, Wheel.Gradient);
								break;
							case "FACCIO_SCENDERE":
								BaseScript.TriggerServerEvent("FerrisWheel:playerGetOff",
									Game.PlayerPed.NetworkId, Wheel.Gradient);
								break;
						}
					}
				}

				Vector3 pitch = new Vector3(-Wheel.Rotation - 22.5f, 0, 0); // 22.5 --> 360 / 16
				Wheel.Entity.Rotation = pitch;

				Cabins.ToList().ForEach(o => func_145(Cabins.ToList().IndexOf(o)));
				SetAudioSceneVariable("FAIRGROUND_RIDES_FERRIS_WHALE", "HEIGHT", Game.PlayerPed.Position.Z - 13f);
			}
			await Task.FromResult(0);
		}


		// triggering an event is my attempt to make the other player Ped to seat in my ferris wheel (not being networked.. every client has its own ferris wheel) -- not working so far
		// same thing is done for the rollercoaster with same result.. nothing.. 
		private static async void PlayerGetOn(int player, int cab)
		{
			Ped Char = (Ped)Entity.FromNetworkId(player);
			CabinPan Cabin = Cabins[cab];
			if (IsEntityAtCoord(Char.Handle, -1661.95f, -1127.011f, 12.6973f, 1f, 1f, 1f, false, true, 0))
			{
				if (Char.NetworkId != Game.PlayerPed.NetworkId)
					if (!NetworkHasControlOfNetworkId(player))
						while (!NetworkRequestControlOfNetworkId(player)) await BaseScript.Delay(0);
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", true);
				Wheel.Ferma = true;
				await BaseScript.Delay(100);
				Vector3 coord = GetOffsetFromEntityInWorldCoords(Cabin.Entity.Handle, 0, 0, 0);
				int uLocal_376 = NetworkCreateSynchronisedScene(coord.X, coord.Y, coord.Z, 0f, 0f, 0f, 2, true, false, 1065353216, 0, 1065353216);
				NetworkAddPedToSynchronisedScene(Char.Handle, uLocal_376, "anim@mp_ferris_wheel", "enter_player_one", 8f, -8f, 131072, 0, 1148846080, 0);
				NetworkStartSynchronisedScene(uLocal_376);
				int iVar2 = NetworkConvertSynchronisedSceneToSynchronizedScene(uLocal_376);
				if (GetSynchronizedScenePhase(iVar2) > 0.99f)
				{
					uLocal_376 = NetworkCreateSynchronisedScene(coord.X, coord.Y, coord.Z, 0f, 0f, 0f, 2, true, false, 1065353216, 0, 1065353216);
					NetworkAddPedToSynchronisedScene(Char.Handle, uLocal_376, "anim@mp_ferris_wheel", "enter_player_one", 8f, -8f, 131072, 0, 1148846080, 0);
					NetworkStartSynchronisedScene(uLocal_376);
				}
				await BaseScript.Delay(7000);
				Vector3 attCoords = GetOffsetFromEntityGivenWorldCoords(Cabin.Entity.Handle, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
				AttachEntityToEntity(Char.Handle, Cabin.Entity.Handle, 0, attCoords.X, attCoords.Y, attCoords.Z, 0f, 0f, Game.PlayerPed.Heading, false, false, false, false, 2, true);
				BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", Cabin.Index, Cabin.NPlayer);
				if (Char.Handle == PlayerPedId())
					RideEnd = false;
				Wheel.State = "IDLE";
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", false);
				int iLocal_297 = GetSoundId();
				PlaySoundFromEntity(iLocal_297, "GENERATOR", Wheel.Entity.Handle, "THE_FERRIS_WHALE_SOUNDSET", false, 0);
				int iLocal_299 = GetSoundId();
				PlaySoundFromEntity(iLocal_299, "SLOW_SQUEAK", Wheel.Entity.Handle, "THE_FERRIS_WHALE_SOUNDSET", false, 0);
				int iLocal_300 = GetSoundId();
				PlaySoundFromEntity(iLocal_300, "SLOW_SQUEAK", Cabins[1].Entity.Handle, "THE_FERRIS_WHALE_SOUNDSET", false, 0);
				int iLocal_298 = GetSoundId();
				PlaySoundFromEntity(iLocal_298, "CARRIAGE", Cabins[1].Entity.Handle, "THE_FERRIS_WHALE_SOUNDSET", false, 0);
				if (Char.Handle == PlayerPedId())
					CreaCam();
			}
		}

		private static async void PlayerGetOff(int player, int cab)
		{
			Ped Char = (Ped)Entity.FromNetworkId(player);
			CabinPan Cabin = Cabins[cab];
			if (Char == Game.PlayerPed)
			{
				while (ActualCabin != Cabin) await BaseScript.Delay(0);
				RenderScriptCams(false, false, 1000, false, false);
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", true);
				Vector3 offset = GetOffsetFromEntityInWorldCoords(Cabin.Entity.Handle, 0f, 0f, 0f);
				Cam1.Delete();
				DestroyAllCams(false);
				int uLocal_377 = NetworkCreateSynchronisedScene(offset.X, offset.Y, offset.Z, 0f, 0f, 0f, 2, false, false, 1065353216, 0, 1065353216);
				NetworkAddPedToSynchronisedScene(Char.Handle, uLocal_377, "anim@mp_ferris_wheel", "exit_player_one", 8f, -8f, 131072, 0, 1148846080, 0);
				NetworkStartSynchronisedScene(uLocal_377);
				Char.Detach();
				await BaseScript.Delay(5000);
				Cabin.NPlayer = 0;
				BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", Cabin.Index, Cabin.NPlayer);
				if (IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
					StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
				if (IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW"))
					StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW");
				if (Char.Handle == PlayerPedId())
					RideEnd = true;
				BaseScript.TriggerServerEvent("FerrisWheel:StopWheel", false);
				Wheel.State = "IDLE";
				ActualCabin = null;
			}
			else
			{
				if (Char.NetworkId != Game.PlayerPed.NetworkId)
					if (!NetworkHasControlOfNetworkId(player))
						while (!NetworkRequestControlOfNetworkId(player)) await BaseScript.Delay(0);
				Vector3 offset = GetOffsetFromEntityInWorldCoords(Cabin.Entity.Handle, 0f, 0f, 0f);
				int uLocal_377 = NetworkCreateSynchronisedScene(offset.X, offset.Y, offset.Z, 0f, 0f, 0f, 2, false, false, 1065353216, 0, 1065353216);
				NetworkAddPedToSynchronisedScene(Char.Handle, uLocal_377, "anim@mp_ferris_wheel", "exit_player_one", 8f, -8f, 131072, 0, 1148846080, 0);
				NetworkStartSynchronisedScene(uLocal_377);
				Char.Detach();
				await BaseScript.Delay(5000);
				Cabin.NPlayer = 0;
				BaseScript.TriggerServerEvent("FerrisWheel:UpdateCabins", Cabin.Index, Cabin.NPlayer);
			}
		}

		private static async Task ControlloPlayer()
		{
			if (Game.PlayerPed.IsInRangeOf(new Vector3(-1661.95f, -1127.011f, 12.6973f), 20f))
			{
				if (!IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
					StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");

				if (Game.PlayerPed.IsInRangeOf(new Vector3(-1661.95f, -1127.011f, 12.6973f), 1.375f))
				{
					ShowNotification("Press ~INPUT_CONTEXT~ to get on the Wheel");
					if (Game.IsControlJustPressed(2, Control.Context))
					{
						ShowNotification("Wait.. The first free cabin is coming..");
						BaseScript.TriggerServerEvent("FerrisWheel:syncState", "FACCIO_SALIRE");
					}
				}
			}
			else
				StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			if (!RideEnd)
			{
				if (GetFollowPedCamViewMode() == 4) SetFollowPedCamViewMode(2);
				Game.DisableControlThisFrame(0, Control.NextCamera);
				UpdateTasti();
				if (Game.IsControlJustPressed(2, Control.FrontendY))
				{
					ShowNotification("The wheel will stop once your cabin reaches ground to let you get off");
					Wheel.State = "FACCIO_SCENDERE";
				}
				if (Game.IsControlJustPressed(2, Control.ScriptSelect))
					CambiaCam();
			}
			await Task.FromResult(0);
		}

		static private async void UpdateTasti()
		{
			if (!Scaleform)
			{
				Buttons = new Scaleform("instructional_buttons");
				while (!HasScaleformMovieLoaded(Buttons.Handle)) await BaseScript.Delay(0);

				Buttons.CallFunction("CLEAR_ALL");
				Buttons.CallFunction("TOGGLE_MOUSE_BUTTONS", false);


				Buttons.CallFunction("CLEAR_ALL");

				Buttons.CallFunction("SET_DATA_SLOT", 0, GetControlInstructionalButton(2, 236, 1), "Change Visual");
				Buttons.CallFunction("SET_DATA_SLOT", 1, GetControlInstructionalButton(2, 204, 1), "Get off the Wheel");

				Buttons.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
				Scaleform = true;
			}
			if (Scaleform)
				Buttons.Render2D();
		}

		private static async void func_145(int i)
		{
			Vector3 offset = func_147(i);
			SetEntityCoordsNoOffset(Cabins[i].Entity.Handle, offset.X, offset.Y, offset.Z, true, false, false);
		}



		private static void func_79()
		{
			if (IsAudioSceneActive("FAIRGROUND_RIDES_FERRIS_WHALE"))
				StopAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE");
			StartAudioScene("FAIRGROUND_RIDES_FERRIS_WHALE_ALTERNATIVE_VIEW");
		}

		private static async void CreaCam()
		{
			Cam1 = new Camera(CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", -1703.854f, -1082.222f, 42.006f, -8.3096f, 0f, -111.8213f, 50f, false, 0));
			Cam1.PointAt(Wheel.Entity);
			Cam1.IsActive = true;
			Screen.Fading.FadeOut(500);
			await BaseScript.Delay(800);
			RenderScriptCams(true, false, 1000, false, false);
			Screen.Fading.FadeIn(500);
			func_79();
			SetLocalPlayerInvisibleLocally(false);
		}

		static bool Cambia = true;
		static void CambiaCam()
		{
			Cambia = !Cambia;
			if (Cambia)
			{
				Cam1 = new Camera(CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", -1703.854f, -1082.222f, 42.006f, -8.3096f, 0f, -111.8213f, 50f, false, 0));
				Cam1.PointAt(Wheel.Entity);
				func_79();
				Cam1.IsActive = true;
				Client.Instance.RemoveTick(Cam2Controller);
				if (IsInputDisabled(2))
				{
					Cam2Keyvoard.CamEntity.Delete();
					Cam2Keyvoard.Value1 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value4 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value7 = 0;
					Cam2Keyvoard.Value20 = 0;
					Cam2Keyvoard.Value21 = 0;
					Cam2Keyvoard.Value22 = 0;
					Cam2Keyvoard.Value8 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value11 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value14 = new Vector3(0f, 0f, 0f);
					Cam2Keyvoard.Value17 = 0;
					Cam2Keyvoard.Value18 = 0;
					Cam2Keyvoard.Value23 = 0;
					Cam2Keyvoard.Value19 = 0;
					Cam2Keyvoard.Value24 = 0;
					Cam2Keyvoard.Value25 = 0;
					Cam2Keyvoard.Value29 = 0f;
					Cam2Keyvoard.Value30 = 0f;
					Cam2Keyvoard.Value26 = false;
					Cam2Keyvoard.Value28 = false;
					Cam2Keyvoard.Value27 = 0;
				}
			}
			else
			{
				if (IsInputDisabled(2))
				{
					Vector3 PedCoords = GetPedBoneCoords(PlayerPedId(), 31086, 0f, 0.2f, 0f);
					Vector3 Rot = GetEntityRotation(PlayerPedId(), 2);
					Cam2Keyvoard.CamEntity = new Camera(CreateCam("DEFAULT_SCRIPTED_CAMERA", false));
					SetCamParams(Cam2Keyvoard.CamEntity.Handle, PedCoords.X, PedCoords.Y, PedCoords.Z, Rot.X, Rot.Y, Rot.Z, 50f, 0, 1, 1, 2);
					Cam2Keyvoard.CamEntity.IsActive = true;
					Cam2Keyvoard.CamEntity.Shake(CameraShake.Hand, 0.19f);
					SetCamNearClip(Cam2Keyvoard.CamEntity.Handle, -1082130432f);
					AttachCamToPedBone(Cam2Keyvoard.CamEntity.Handle, PlayerPedId(), 31086, 0f, 0.2f, 0f, true);
					SetLocalPlayerInvisibleLocally(false);

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
					Cam2Keyvoard.Value27 = 0;
				}
				else
				{
					func_110(Cam2GamePad, true);
					func_109(Cam2GamePad, 0, 3000);
				}
				Client.Instance.AddTick(Cam2Controller);
				Cam1.Delete();
			}
			RenderScriptCams(true, false, 3000, true, false);
		}

		private static async Task Cam2Controller()
		{
			if (IsInputDisabled(2))
				func_101(Cam2Keyvoard, true, true, false, false, 0.1f, false, 1065353216f, false);
			else
				func_105(Cam2GamePad);
		}

		static async void func_105(FerrisWheelCamGamePad uParam0)
		{
			float uVar0 = 0;
			float uVar1 = 0;
			float fVar2;

			if (!uParam0.Value1) return;
			DisableInputGroup(2);
			if (uParam0.Value0)
			{
				if (Absf(GetControlNormal(2, 220)) > 0.1f)
				{
					uParam0.Value12 = uParam0.Value12 - (GetControlNormal(2, 220) * 60f * Timestep());
					if (uParam0.Value15)
					{
						if (uParam0.Value12 < -110)
							uParam0.Value12 = -110;
						if (uParam0.Value12 > 110)
							uParam0.Value12 = 110;
					}
					else uParam0.Value12 = func_102(uParam0.Value12, -80f, 80f);
				}
				if (Absf(GetControlNormal(2, 221)) > 0.1f)
				{
					fVar2 = ((GetControlNormal(2, 221) * 60f) * Timestep());
					if (IsLookInverted())
						fVar2 = (fVar2 * -1f);
					uParam0.Value11 -= fVar2;
					if (uParam0.Value14)
					{
						if (uParam0.Value11 < -30)
							uParam0.Value11 = -30;
						if (uParam0.Value11 > 30)
							uParam0.Value11 = 30;
					}
					else
						uParam0.Value11 = func_102(uParam0.Value11, -30f, 30f);
				}
				if (IsControlJustPressed(2, 231))
				{
					uParam0.Value11 = 0f;
					uParam0.Value12 = 0f;
				}
				if (Absf(GetControlNormal(2, 219)) > 0.1f)
				{
					fVar2 = GetControlNormal(2, 219) * (60f / 2f) * Timestep();
					uParam0.Value13 = (uParam0.Value13 + fVar2);
					uParam0.Value13 = func_102(uParam0.Value13, 20f, 50f);
				}
				if (DoesCamExist(uParam0.CamEntity.Handle))
				{
					SetCamFov(uParam0.CamEntity.Handle, uParam0.Value13);
					if (IsEntityDead(uParam0.Value8) && !IsEntityDead(PlayerPedId()))
						SetCamRot(uParam0.CamEntity.Handle, (Game.PlayerPed.Rotation + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).X, (Game.PlayerPed.Rotation + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Y, (Game.PlayerPed.Rotation + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Z, 2);
					else if (!IsEntityDead(uParam0.Value8) && !IsEntityDead(PlayerPedId()))
					{
						func_106(GetEntityCoords(uParam0.Value8, true), GetEntityCoords(uParam0.Value9, true), ref uVar0, ref uVar1, 1);
						SetCamRot(uParam0.CamEntity.Handle, (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).X, (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Y, (new Vector3(uVar1, 0f, uVar0) + new Vector3(uParam0.Value11, 0f, uParam0.Value12)).Z, 2);
					}
				}
			}
		}

		static void func_106(Vector3 vParam0, Vector3 vParam1, ref float uParam2, ref float uParam3, int iParam4)
		{
			Vector3 vVar0;
			vVar0 = vParam1 - vParam0;
			func_107(vVar0, ref uParam2, ref uParam3, iParam4);
		}

		static void func_107(Vector3 vParam0, ref float uParam1, ref float uParam2, int iParam3)
		{
			float fVar0;
			if (vParam0.Y != 0f)
				uParam2 = Atan2(vParam0.X, vParam0.Y);
			else if (vParam0.X < 0f)
				uParam2 = -90f;
			else
				uParam2 = 90f;
			if (iParam3 == 1)
			{
				uParam2 *= -1f;
				if (uParam2 < 0f) uParam2 += 360f;
			}
			fVar0 = Sqrt(((vParam0.X * vParam0.X) + (vParam0.Y * vParam0.Y)));
			if (fVar0 != 0f)
				uParam1 = Atan2(vParam0.Z, fVar0);
			else if (vParam0.Z < 0f)
				uParam1 = -90f;
			else
				uParam1 = 90f;
		}

		static int func_109(FerrisWheelCamGamePad uParam0, int iParam1, int iParam2)
		{
			if (!uParam0.Value1) return 0;
			uParam0.Value13 = 50f;
			if (!DoesCamExist(uParam0.CamEntity.Handle))
				uParam0.CamEntity = new Camera(CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", uParam0.Value2.X, uParam0.Value2.Y, uParam0.Value2.Z, uParam0.Value5.X, uParam0.Value5.Y, uParam0.Value5.Z, 50f, true, 2));
			if (uParam0.Value0)
			{
				AttachCamToPedBone(uParam0.CamEntity.Handle, PlayerPedId(), 31086, 0f, 0.2f, 0f, true);
				uParam0.Value11 = 0f;
				uParam0.Value12 = 0f;
			}
			if (DoesCamExist(uParam0.CamEntity.Handle))
				SetCamActive(uParam0.CamEntity.Handle, true);
			return 1;
		}

		static void func_110(FerrisWheelCamGamePad uParam0, bool bParam1)
		{
			uParam0.Value0 = true;
			uParam0.Value1 = true;
			uParam0.Value9 = PlayerPedId();
			uParam0.Value11 = 0f;
			uParam0.Value12 = 0f;
			if (bParam1)
				uParam0.Value15 = true;
		}


		static async void func_101(FerrisWheelCamKeyboard uParam0, bool bParam1, bool bParam2, bool bParam3, bool bParam4, float fParam5, bool bParam6, float fParam7, bool bParam8)
		{
			int[] iVar0 = new int[4];
			float fVar1;
			float fVar2;
			float fVar3;
			float fVar4;
			float fVar5;
			Vector3 vVar6;
			int iVar7;
			int iVar8;

			DisableInputGroup(2);
			func_104(ref iVar0[0], ref iVar0[1], ref iVar0[2], ref iVar0[3], false, false);
			if (IsLookInverted())
				iVar0[3] = (iVar0[3] * -1);
			if (IsInputDisabled(2))
			{
				fVar1 = GetControlUnboundNormal(2, 239);
				fVar2 = GetControlUnboundNormal(2, 240);
				fVar3 = (fVar1 - uParam0.Value29);
				fVar4 = (fVar2 - uParam0.Value30);
				uParam0.Value29 = fVar1;
				uParam0.Value30 = fVar2;
				if (bParam4)
				{
					iVar0[2] = -(int)Math.Round(((fVar3 * fParam5) * 127f));
					iVar0[3] = -(int)Math.Round(((fVar4 * fParam5) * 127f));
				}
				else
				{
					iVar0[2] = (int)Math.Round(((GetControlUnboundNormal(2, 290) * fParam5) * 127f));
					iVar0[3] = (int)Math.Round(((GetControlUnboundNormal(2, 291) * fParam5) * 127f));
				}
				iVar0[2] = func_103((iVar0[2] + uParam0.Value24), -127, 127);
				iVar0[3] = func_103((iVar0[3] + uParam0.Value25), -127, 127);
			}
			if (uParam0.Value24 == iVar0[2] && uParam0.Value25 == iVar0[3])
			{
				if (uParam0.Value27 < GetGameTimer())
				{
					uParam0.Value24 = 0;
					uParam0.Value25 = 0;
					if (IsInputDisabled(2))
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
				uParam0.Value27 = GetGameTimer() + 4000;
				uParam0.Value28 = false;
			}
			if (bParam2)
			{
				uParam0.Value8.Z = -(ToFloat(iVar0[2]) / 127f) * uParam0.Value20;
				uParam0.Value8.Y = -uParam0.Value8.Z * uParam0.Value22 / (uParam0.Value20);
				uParam0.Value8.X = -(ToFloat(iVar0[3]) / 127f) * uParam0.Value21;
			}
			else
			{
				uParam0.Value8 = new Vector3(0f, 0f, 0f);
				uParam0.Value24 = 0;
				uParam0.Value25 = 0;
			}
			fVar5 = 30f * Timestep();
			vVar6 = uParam0.Value8 + uParam0.Value11;
			if (IsInputDisabled(2) && bParam2 && !uParam0.Value28)
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
			if (IsInputDisabled(0) && bParam1)
			{
				if (uParam0.Value28)
					uParam0.Value17 = uParam0.Value7;
			}
			else
				uParam0.Value17 = uParam0.Value7;
			if (bParam1)
			{
				if (IsInputDisabled(0))
				{
					iVar7 = 40;
					iVar8 = 41;
					if (bParam6)
					{
						iVar7 = 241;
						iVar8 = 242;
					}
					if (IsDisabledControlJustPressed(0, iVar7))
					{
						uParam0.Value17 -= 5f;
						uParam0.Value27 = GetGameTimer() + 4000;
						uParam0.Value28 = false;
					}
					else if (IsDisabledControlJustPressed(0, iVar8))
					{
						uParam0.Value17 += 5f;
						uParam0.Value27 = GetGameTimer() + 4000;
						uParam0.Value28 = false;
					}
					if (bParam3)
						uParam0.Value17 = func_102(uParam0.Value17, (uParam0.Value7 - uParam0.Value19), uParam0.Value7);
					else
						uParam0.Value17 = func_102(uParam0.Value17, (uParam0.Value7 - uParam0.Value19), (uParam0.Value7 + uParam0.Value19));
				}
				else if (bParam8)
				{
					iVar0[1] = GetControlValue(2, 207);
					iVar0[3] = GetControlValue(2, 208);
					if (bParam3)
					{
						if (ToFloat(iVar0[3]) > 127f)
							uParam0.Value17 -= (int)Math.Round(iVar0[3] / 128f * (uParam0.Value19 / 2f));
					}
					else
					{
						uParam0.Value17 += (int)Math.Round(iVar0[1] / 128f * uParam0.Value19);
						uParam0.Value17 -= (int)Math.Round(iVar0[3] / 128f * uParam0.Value19);
					}
				}
				else if (bParam3)
				{
					if ((iVar0[1]) < 0f)
						uParam0.Value17 += (int)Math.Round(iVar0[1] / 128f * uParam0.Value19);
				}
				else
					uParam0.Value17 += (Round(((ToFloat(iVar0[1]) / 128f) * uParam0.Value19)));
			}
			uParam0.Value18 += ((((uParam0.Value17 - uParam0.Value18) * 0.06f) * fVar5));
			SetCamParams(uParam0.CamEntity.Handle, uParam0.Value1.X, uParam0.Value1.Y, uParam0.Value1.Z, (uParam0.Value4 + uParam0.Value14).X, (uParam0.Value4 + uParam0.Value14).Y, (uParam0.Value4 + uParam0.Value14).Z, uParam0.Value18, 0, 1, 1, 2);
			uParam0.CamEntity.Rotation = (Game.PlayerPed.Rotation + Cam2Keyvoard.Value14);
		}

		static float func_102(float fParam0, float fParam1, float fParam2)
		{
			return (fParam0 > fParam2) ? fParam2 : (fParam0 < fParam1) ? fParam1 : fParam0;
		}


		static int func_103(int iParam0, int iParam1, int iParam2)
		{
			return (iParam0 > iParam2) ? iParam2 : (iParam0 < iParam1) ? iParam1 : iParam0;
		}

		static void func_104(ref int uParam0, ref int uParam1, ref int uParam2, ref int uParam3, bool bParam4, bool bParam5)
		{
			uParam0 = Floor((GetControlUnboundNormal(2, 218) * 127f));
			uParam1 = Floor((GetControlUnboundNormal(2, 219) * 127f));
			uParam2 = Floor((GetControlUnboundNormal(2, 220) * 127f));
			uParam3 = Floor((GetControlUnboundNormal(2, 221) * 127f));
			if (bParam4)
			{
				if (!IsControlEnabled(2, 218))
					uParam0 = Floor((GetDisabledControlUnboundNormal(2, 218) * 127f));
				if (!IsControlEnabled(2, 219))
					uParam1 = Floor((GetDisabledControlUnboundNormal(2, 219) * 127f));
				if (!IsControlEnabled(2, 220))
					uParam2 = Floor((GetDisabledControlUnboundNormal(2, 220) * 127f));
				if (!IsControlEnabled(2, 221))
					uParam3 = Floor((GetDisabledControlUnboundNormal(2, 221) * 127f));
			}
			if (IsInputDisabled(2))
			{
				if (bParam5)
				{
					if (IsLookInverted())
						uParam3 *= -1;
					if (N_0xe1615ec03b3bb4fd())
						uParam3 *= -1;
				}
			}
		}

		private static void ForceState(string state) => Wheel.State = state;

		static Vector3 func_147(int iParam0)
		{
			float fVar0 = 6.28319f / 16 * iParam0;
			float y = Extensions.ConvertDegreesToRadians(15.3f) * Extensions.ConvertRadiansToDegrees((float)Math.Sin(fVar0));
			float z = Extensions.ConvertDegreesToRadians(-15.3f) * Extensions.ConvertRadiansToDegrees((float)Math.Cos(fVar0));
			return GetOffsetFromEntityInWorldCoords(Wheel.Entity.Handle, 0f, y, z);
		}

		private static void OnStop(string name)
		{
			if (name == GetCurrentResourceName())
			{
				if (Wheel.Entity != null)
				{
					Wheel.Entity.Delete();
					for (int i = 0; i < 16; i++)
						Cabins[i].Entity.Delete();
				}
			}
		}

		private static int GetCabIndex(Prop cab)
		{
			foreach (var _cab in Cabins)
			{
				if (cab == _cab.Entity)
					return _cab.Index;
			}

			return -1;
		}


	}

	internal class WheelPan
	{
		public Prop Entity;
		public int Gradient;
		public float Rotation;
		public string State = "IDLE";
		public float Speed = 5f; // 2f in gta scripts
		public bool Ferma = false;
		public WheelPan(Prop entity, int gradient, string state)
		{
			Entity = entity;
			Gradient = gradient;
			Rotation = 0f;
			State = state;
		}
	}

	internal class FerrisWheelCamKeyboard
	{
		public Camera CamEntity = new Camera(0);
		public float Value0 = 0;
		public Vector3 Value1;
		public float Value2 = 0;
		public float Value3 = 0;
		public Vector3 Value4;
		public float Value5 = 0;
		public float Value6 = 0;
		public float Value7 = 0;
		public Vector3 Value8;
		public float Value9 = 0;
		public float Value10 = 0;
		public Vector3 Value11;
		public float Value12 = 0;
		public float Value13 = 0;
		public Vector3 Value14;
		public float Value15 = 0;
		public float Value16 = 0;
		public float Value17 = 0;
		public float Value18 = 0;
		public int Value19 = 0;
		public int Value20 = 0;
		public int Value21 = 0;
		public int Value22 = 0;
		public int Value23 = 0;
		public int Value24 = 0;
		public int Value25 = 0;
		public bool Value26 = false;
		public float Value27 = 0;
		public bool Value28 = false;
		public float Value29 = 0;
		public float Value30 = 0;
	}

	internal class FerrisWheelCamGamePad
	{
		public Camera CamEntity = new Camera(0);
		public bool Value0 = false;
		public bool Value1 = false;
		public Vector3 Value2 = new Vector3(0);
		public float Value3 = 0;
		public float Value4;
		public Vector3 Value5 = new Vector3(0);
		public float Value6 = 0;
		public float Value7 = 0;
		public int Value8;
		public int Value9 = 0;
		public float Value10 = 0;
		public float Value11;
		public float Value12 = 0;
		public float Value13 = 0;
		public bool Value14 = true;
		public bool Value15 = true;
		public float Value16 = 0;
		public float Value17 = 0;
		public float Value18 = 0;
		public int Value19 = 0;
		public int Value20 = 0;
		public int Value21 = 0;
		public int Value22 = 0;
		public int Value23 = 0;
		public int Value24 = 0;
		public int Value25 = 0;
		public float Value26 = 0;
		public float Value27 = 0;
		public float Value28 = 0;
		public float Value29 = 0;
		public float Value30 = 0;
	}

	internal class CabinPan
	{
		public Prop Entity = new Prop(0);
		public int Index;
		public bool PlayerInside = false;
		public int NPlayer = 0;
		public float Gradient;
		public CabinPan(int index) 
		{
			Index = index;
			Gradient = (360f / 16) * Index;
		}
	}
}
