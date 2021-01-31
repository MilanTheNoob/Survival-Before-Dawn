using System.Collections;
using UnityEngine;

public class AIBase : MonoBehaviour
{    
    [Header("Basic Movement Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 7.5f;
	public float directionChangeInterval = 1f;
	public float maxHeadingChange = 1f;

    [Header("All the Stats for the Sensor")]
    public float sensorLength = 5f;

    public float sensorPosY = 2f;
    public float frontSensorPosZ = 0f;
    public float sideSensorPosZ = 0.75f;
    public float angleSensors = 30f;

    [Header("Simple GameObjects")]
    public GameObject groundCheck;
    public GameObject bodyMesh;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CharacterController controller;

    [HideInInspector]
    public float heading;
    [HideInInspector]
    public float movementValue;

	[HideInInspector]
    public Vector3 targetRotation;
    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public bool isWalking;
    [HideInInspector]
    public bool isRunning;
    [HideInInspector]
    public bool isAvoiding;
    [HideInInspector]
    public bool isGrounded;

    Bounds chunkParentBounds;

    private void Start() { chunkParentBounds = transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds; }

    // Called to calculate gravity
    public virtual void CalculateGravity()
    {        
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, InputManager.instance.groundDistance);
        if (isGrounded && velocity.y < 0) { velocity.y = -2f; }
        velocity.y += InputManager.instance.gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime); 
    }

    public IEnumerator NewHeading ()
	{
		while (true) 
        {
			var floor = transform.eulerAngles.y - maxHeadingChange;
		    var ceil  = transform.eulerAngles.y + maxHeadingChange;
		    heading = Random.Range(floor, ceil);
		    targetRotation = new Vector3(0, heading, 0);

			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
    
    public void RandomWalk()
    {
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
        Walk();
    }

    public void Walk()
    {
        ReachedEdge(transform.position + transform.forward);

        Vector3 move = transform.right * 0 + transform.forward * 1;
        controller.Move(move * walkSpeed * Time.deltaTime);
        isWalking = true;

        anim.SetFloat("MovementValue", 0.5f);
    }

    public void Walk(float horizontal)
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + horizontal, 0);
		Walk();
    }

    public void Walk(float horizontal, float vertical)
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + horizontal, 0);
        var forward = transform.TransformDirection(Vector3.forward);
		controller.Move(((forward * walkSpeed) / vertical) * Time.deltaTime);
        isWalking = true;

        anim.SetFloat("MovementValue", 0.5f / 2);
    }

    public bool ReachedEdge(Vector3 pos)
    {
        if (!chunkParentBounds.Contains(pos))
        {
            transform.LookAt(new Vector3(transform.parent.GetComponent<Renderer>().bounds.center.x, transform.position.y, transform.parent.GetComponent<Renderer>().bounds.center.z));
            return true;
        }

        return false;
    }

    public void SensorsUpdate()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        float avoidMultiplier = 0;
        isAvoiding = false;

        sensorStartPos += transform.forward * frontSensorPosZ;
        sensorStartPos += transform.up * sensorPosY;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength)) { isAvoiding = true; }

        sensorStartPos += transform.right * sideSensorPosZ;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength)) { isAvoiding = true; avoidMultiplier -= 1f; }
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-angleSensors, transform.up) * transform.forward, out hit, sensorLength)) { isAvoiding = true; avoidMultiplier -= 0.5f; }

        sensorStartPos -= transform.right * sideSensorPosZ * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength)) { isAvoiding = true; avoidMultiplier += 1f; }
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angleSensors, transform.up) * transform.forward, out hit, sensorLength)) { isAvoiding = true; avoidMultiplier += 0.5f; }

        if (isAvoiding && avoidMultiplier == 0) { Walk(maxHeadingChange); return; }
        if (isAvoiding) { Walk(maxHeadingChange * avoidMultiplier); } else { RandomWalk(); }
    }
}
