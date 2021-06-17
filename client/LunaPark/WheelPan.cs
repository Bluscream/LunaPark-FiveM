using CitizenFX.Core;

namespace LunaPark
{
	internal class WheelPan
	{
		public Prop Entity;

		public int Gradient;

		public float Rotation;

		public string State = "IDLE";

		public float Speed = 5f;

		public bool Ferma = false;

		public WheelPan(Prop entity, int gradient, string state)
		{
			Entity = entity;
			Gradient = gradient;
			Rotation = 0f;
			State = state;
		}
	}
}
