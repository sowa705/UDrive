using UnityEngine;

public class MotorcycleController : MonoBehaviour
{

	public UWheelCollider WColForward;
	public UWheelCollider WColBack;

	public Transform wheelF;
	public Transform wheelB;

	public Transform CenterOfMass;

	public float maxSteerAngle = 45;
	public float maxMotorTorque = 200;
	public float maxForwardBrake = 400;
	public float maxBackBrake = 400;

	public float wheelRadius = 0.3f;
	public float wheelOffset = 0.1f;

	public float steerSensivity = 30;
	public float controlAngle = 25;
	public float controlOmega = 30;

	public float lowSpeed = 8;
	public float highSpeed = 25;

	private WheelData[] wheels;

	private Transform thisTransform;

	public class WheelData
	{

		public WheelData( UWheelCollider collider)
		{
			wheelCollider = collider;
		}

		public UWheelCollider wheelCollider;
		public float rotation = 0f;
	}

	public struct MotoInput
	{
		public float steer;
		public float acceleration;
		public float brakeForward;
		public float brakeBack;
	}

	void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition;

		wheels = new WheelData[2];
		wheels[0] = new WheelData(WColForward);
		wheels[1] = new WheelData( WColBack);

		thisTransform = GetComponent<Transform>();
	}

	void FixedUpdate()
	{
		var input = new MotoInput();

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.acceleration = 1;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.steer += 1;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.steer -= 1;

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			input.brakeBack = 0.3f;
			input.brakeForward = 0.8f;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			input.brakeBack = 1f;
		}

		motoMove(motoControl(input));

		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			pickUp();
		}

		if (speedVal < 0.01f && thisTransform.up.y < 0.2f && Mathf.Abs(prevOmega) < 0.001f)
		{
			pickUp();
		}
	}

	private void pickUp()
	{
		Transform t = GetComponent<Transform>();
		t.position = t.position + new Vector3(0, 0.2f, 0);
		t.rotation = new Quaternion(0, 0, 0, 1);
	}

	private Vector3 prevPos = new Vector3();
	private float prevAngle = 0;
	private float prevOmega = 0;
	private float speedVal = 0;
	private float prevSteer = 0f;

	private MotoInput motoControl(MotoInput input)
	{
		var posNow = thisTransform.position;
		var speed = (posNow - prevPos) / Time.fixedDeltaTime;
		prevPos = posNow;

		speedVal = speed.magnitude;
		var moveForward = speed.normalized;

		var angle = Vector3.Dot(moveForward, Vector3.Cross(thisTransform.up, new Vector3(0, 1, 0)));
		var omega = (angle - prevAngle) / Time.fixedDeltaTime;
		prevAngle = angle;
		prevOmega = omega;

		float lowSpeed = 8f;
		float highSpeed = 25f;

		if (speedVal < lowSpeed)
		{
			float t = speedVal / lowSpeed;
			input.steer *= t * t;
			omega *= t * t;
			angle = angle * (2 - t);
			input.acceleration += Mathf.Abs(angle) * 3 * (1 - t);
		}

		if (speedVal > highSpeed)
		{
			float t = speedVal / highSpeed;
			if (omega * angle < 0f)
			{
				omega *= t;
			}
		}

		input.steer *= (1 - 2.5f * angle * angle);

		input.steer = 1f / (speed.sqrMagnitude + 1f) * (input.steer * steerSensivity + angle * controlAngle + omega * controlOmega);
		float steerDelta = 10 * Time.fixedDeltaTime;
		input.steer = Mathf.Clamp(input.steer, prevSteer - steerDelta, prevSteer + steerDelta);
		prevSteer = input.steer;

		return input;
	}

	private void motoMove(MotoInput input)
	{
		WColForward.SteerAngle = Mathf.Clamp(input.steer, -1, 1) * maxSteerAngle;

		WColForward.BrakeTorque = maxForwardBrake * input.brakeForward;
		WColBack.BrakeTorque = maxBackBrake * input.brakeBack;

		WColBack.EngineTorque = maxMotorTorque * input.acceleration;
	}
}
