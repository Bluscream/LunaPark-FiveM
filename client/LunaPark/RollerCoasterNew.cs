using CitizenFX.Core;

namespace LunaPark
{
	internal class RollerCoasterNew
	{
		public float Speed;

		public float VarSpeed = 0f;

		public int Index = 0;

		public string State = "WAITING";

		public RollerCar[] Cars = new RollerCar[4]
		{
			new RollerCar(),
			new RollerCar(),
			new RollerCar(),
			new RollerCar()
		};

		public Vector3[] Value0 = (Vector3[])(object)new Vector3[225];

		public float[] Value4 = new float[225];

		public float[] Value3 = new float[225];
	}
}
