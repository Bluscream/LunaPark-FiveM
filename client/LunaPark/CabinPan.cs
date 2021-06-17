using CitizenFX.Core;

namespace LunaPark
{
	internal class CabinPan
	{
		public Prop Entity = new Prop(0);

		public int Index;

		public bool PlayerInside = false;

		public int NPlayer = 0;

		public float Gradient;

		public CabinPan(int index)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			Index = index;
			Gradient = 22.5f * (float)Index;
		}
	}
}
