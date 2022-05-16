using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
	/// <summary>
	/// Stabilizes a two-wheeled vehicle by steering
	/// not tested as of now, may be unusable
	/// </summary>
    public class CounterSteerController: VehicleComponent,IVehicleAssist,IDebuggableComponent
    {
		Vector3 prevPos;
		float speedVal;
		float prevAngle;
		float prevOmega;
		float prevSteer;

		public float lowSpeed;
		public float highSpeed;

		public float steerSens;
		public float controlAngle;
		public float controlOmega;
        public void OnWheel(UWheelCollider collider)
        {
            
        }

        public void DrawDebugText()
        {
            GUILayout.Label($"CS prevangle:{prevAngle}, prevsteer: {prevSteer}");
        }

        public void OnUpdate()
        {
			var posNow = vehicle.transform.position;
			var speed = (posNow - prevPos) / Time.fixedDeltaTime;
			prevPos = posNow;

			speedVal = speed.magnitude;
			var moveForward = speed.normalized;

			var angle = Vector3.Dot(moveForward, Vector3.Cross(vehicle.transform.up, new Vector3(0, 1, 0)));
			var omega = (angle - prevAngle) / Time.fixedDeltaTime;
			prevAngle = angle;
			prevOmega = omega;

			float steerinput = vehicle.ReadInputParameter(VehicleInputParameter.Steer);
			float accelinput = vehicle.ReadInputParameter(VehicleInputParameter.Accelerator);

			if (speedVal < lowSpeed)
			{
				float t = speedVal / lowSpeed;
				steerinput *= t * t;
				omega *= t * t;
				angle = angle * (2 - t);
				accelinput += Mathf.Abs(angle) * 3 * (1 - t);
			}

			if (speedVal > highSpeed)
			{
				float t = speedVal / highSpeed;
				if (omega * angle < 0f)
				{
					omega *= t;
				}
			}

			steerinput *= (1 - 2.5f * angle * angle);

			steerinput = 1f / (speed.sqrMagnitude + 1f) * (steerinput * steerSens + angle * controlAngle + omega * controlOmega);
			float steerDelta = 10 * Time.fixedDeltaTime;
			steerinput = Mathf.Clamp(steerinput, prevSteer - steerDelta, prevSteer + steerDelta);
			prevSteer = steerinput;

			vehicle.WriteInputParameter(VehicleInputParameter.Steer,steerinput);
			vehicle.WriteInputParameter(VehicleInputParameter.Accelerator, accelinput);

		}
	}
}