using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace LunaPark
{
	public static class Controls
	{
		private static readonly Control[] NecessaryControlsKeyboard;

		private static readonly Control[] NecessaryControlsGamePad;

		public static void Toggle(bool toggle)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected I4, but got Unknown
			if (toggle)
			{
				Game.EnableAllControlsThisFrame(2);
				return;
			}
			Game.DisableAllControlsThisFrame(2);
			Control[] list = (((int)Game.get_CurrentInputMode() == 1) ? NecessaryControlsGamePad : NecessaryControlsKeyboard);
			Control[] array = list;
			foreach (Control control in array)
			{
				API.EnableControlAction(0, (int)control, true);
			}
		}

		static Controls()
		{
			Control[] array = new Control[27];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			NecessaryControlsKeyboard = (Control[])(object)array;
			Control[] necessaryControlsKeyboard = NecessaryControlsKeyboard;
			Control[] array2 = new Control[4];
			RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			NecessaryControlsGamePad = necessaryControlsKeyboard.Concat((IEnumerable<Control>)(object)array2).ToArray();
		}
	}
}
