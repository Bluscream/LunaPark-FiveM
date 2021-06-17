using System;
using System.Drawing;
using CitizenFX.Core;

namespace LunaPark
{
	public static class MiscExtensions
	{
		public static Random SharedRandom = new Random();

		public static Point AddPoints(this Point left, Point right)
		{
			return new Point(left.X + right.X, left.Y + right.Y);
		}

		public static Point SubtractPoints(this Point left, Point right)
		{
			return new Point(left.X - right.X, left.Y - right.Y);
		}

		public static PointF AddPoints(this PointF left, PointF right)
		{
			return new PointF(left.X + right.X, left.Y + right.Y);
		}

		public static PointF SubtractPoints(this PointF left, PointF right)
		{
			return new PointF(left.X - right.X, left.Y - right.Y);
		}

		public static float Clamp(this float val, float min, float max)
		{
			if (val > max)
			{
				return max;
			}
			if (val < min)
			{
				return min;
			}
			return val;
		}

		public static Vector3 LinearVectorLerp(Vector3 start, Vector3 end, int currentTime, int duration)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			result.X = LinearFloatLerp(start.X, end.X, currentTime, duration);
			result.Y = LinearFloatLerp(start.Y, end.Y, currentTime, duration);
			result.Z = LinearFloatLerp(start.Z, end.Z, currentTime, duration);
			return result;
		}

		public static Vector3 VectorLerp(Vector3 start, Vector3 end, int currentTime, int duration, Func<float, float, int, int, float> easingFunc)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			result.X = easingFunc(start.X, end.X, currentTime, duration);
			result.Y = easingFunc(start.Y, end.Y, currentTime, duration);
			result.Z = easingFunc(start.Z, end.Z, currentTime, duration);
			return result;
		}

		public static float LinearFloatLerp(float start, float end, int currentTime, int duration)
		{
			float change = end - start;
			return change * (float)currentTime / (float)duration + start;
		}

		public static float QuadraticEasingLerp(float start, float end, int currentTime, int duration)
		{
			float time = currentTime;
			float dur = duration;
			float change = end - start;
			time /= dur / 2f;
			if (time < 1f)
			{
				return change / 2f * time * time + start;
			}
			time -= 1f;
			return (0f - change) / 2f * (time * (time - 2f) - 1f) + start;
		}
	}
}
