using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] 
	Transform target;

	[SerializeField] Vector3 offset;

	// Camera speeds
	[Range(0, 10)]
	[SerializeField] float lerpPositionMultiplier = 1f;
	[Range(0, 10)]
	[SerializeField] float lerpRotationMultiplier = 1f;

	Camera cam;
	WheelVehicle controller;
	Rigidbody rb;
	bool oldHandbrake = true;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		controller = transform.parent.GetComponent<WheelVehicle>();
		cam = GetComponent<Camera>();

		cam.enabled = false;
	}

	void FixedUpdate()
	{
		if (oldHandbrake != controller.Handbrake)
        {
			if (controller.Handbrake)
            {
				cam.enabled = false;
            }
            else
            {
				cam.enabled = true;
            }
        }

		this.rb.velocity.Normalize();

		Quaternion curRot = transform.rotation;
		Vector3 tPos = target.position + target.TransformDirection(offset);

		transform.LookAt(target);

		if (tPos.y < target.position.y)
		{
			tPos.y = target.position.y;
		}

		transform.position = Vector3.Lerp(transform.position, tPos, Time.fixedDeltaTime * lerpPositionMultiplier);
		transform.rotation = Quaternion.Lerp(curRot, transform.rotation, Time.fixedDeltaTime * lerpRotationMultiplier);

		if (transform.position.y < 0.5f)
		{
			transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
		}
	}
}
