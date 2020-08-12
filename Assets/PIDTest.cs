using System.Collections;
using UnityEngine;

public class PIDTest : MonoBehaviour
{
	//PID takes three floats for error correction.
	public PID pid;
	public float speed;
	public Transform actual;
	public Transform setpoint;

	public Vector3 actualPos;
	public Vector3 setpointPos;

	void Update()
	{
		actualPos = actual.transform.position;
		setpointPos = setpoint.transform.position;

		setpoint.Translate(Input.GetAxis("LeftStickHorizontal") * speed, 0, 0);
		setpoint.Translate(0, Input.GetAxis("LeftStickVertical") * speed, 0);
		actual.Translate(pid.Update(setpointPos.x, actualPos.x, Time.deltaTime), pid.Update(setpointPos.y, actualPos.y, Time.deltaTime), pid.Update(setpointPos.z, actualPos.z, Time.deltaTime));

	}
}