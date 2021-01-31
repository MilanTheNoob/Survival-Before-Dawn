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

    // Called at the beginning of the game
    private void Start()
    {
        // Get the bounds of our chunk
        chunkParentBounds = transform.parent.GetComponent<MeshFilter>().sharedMesh.bounds;
    }

    // Called to calculate gravity
    public virtual void CalculateGravity()
    {        
        //Check if we are colliding with a ground object
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, InputManager.instance.groundDistance);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        // Calculate the velocity of the NPC falling to the ground
        velocity.y += InputManager.instance.gravity * Time.deltaTime;

        // Apply the velocity to the character controller
        controller.Move(velocity * Time.deltaTime); 
    }

    // Called to get a new direction for random walking
    public IEnumerator NewHeading ()
	{
		while (true) 
        {
            // Get the max variation of the movement
			var floor = transform.eulerAngles.y - maxHeadingChange;
		    var ceil  = transform.eulerAngles.y + maxHeadingChange;
            // Get a random range in the middle
		    heading = Random.Range(floor, ceil);
            // Get the target rotation
		    targetRotation = new Vector3(0, heading, 0);

            // Wait for the direction change interval
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
    
    // Here to randomly walk
    public void RandomWalk()
    {
        // Set the rotation for the player
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
        // Walk forward
        Walk();
    }

    // Called to just move forward
    public void Walk()
    {
        // Check if we have reached the chunk border
        ReachedEdge(transform.position + transform.forward);

        // Get the position in front of us
        Vector3 move = transform.right * 0 + transform.forward * 1;
        // Move the controller
        controller.Move(move * walkSpeed * Time.deltaTime);
        // Set isWalking to true
        isWalking = true;

        // Use the walk animation
        anim.SetFloat("MovementValue", 0.5f);
    }

    // Used to move forward but rotate
    public void Walk(float horizontal)
    {
        // Set the rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + horizontal, 0);
        // Walk normally
		Walk();
    }

    // Called to walk in a certain direction and rotation
    public void Walk(float horizontal, float vertical)
    {
        // Set the rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + horizontal, 0);
        // Get the direction to move torwards
        var forward = transform.TransformDirection(Vector3.forward);
        // Move the controller
		controller.Move(((forward * walkSpeed) / vertical) * Time.deltaTime);
        // Set isWalking to true
        isWalking = true;

        // Use the walk animation
        anim.SetFloat("MovementValue", 0.5f / 2);
    }

    // Check if we reached edge and rotate the other way
    public bool ReachedEdge(Vector3 pos)
    {

        // See if we reached the edge
        if (!chunkParentBounds.Contains(pos))
        {
            // Look the other way
            transform.LookAt(new Vector3(transform.parent.GetComponent<Renderer>().bounds.center.x, transform.position.y, transform.parent.GetComponent<Renderer>().bounds.center.z));
            // Return true
            return true;
        } 
        else { return false; }
    }

    // Checks if we about to collide with something
    public void SensorsUpdate()
    {
        // Set up some variables
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        float avoidMultiplier = 0;
        isAvoiding = false;

        // Set the position of the front sensor
        sensorStartPos += transform.forward * frontSensorPosZ;
        sensorStartPos += transform.up * sensorPosY;
        // Send a raycast and check if we hit something
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            // Draw a line and set isAvoiding to true
            Debug.DrawLine(sensorStartPos, hit.point);   
            isAvoiding = true;
        }

        // Set the position of the side sensor
        sensorStartPos += transform.right * sideSensorPosZ;
        // Send out another raycast
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            // Draw a line, set isAvoiding to true and subtract to the avoidMultipler
            Debug.DrawLine(sensorStartPos, hit.point);   
            isAvoiding = true;
            avoidMultiplier -= 1f;
        }

        // Send out a raycast at an angle
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-angleSensors, transform.up) * transform.forward, out hit, sensorLength))
        {
            // Draw a line, set isAvoiding to true and subtract to the avoidMultipler
            Debug.DrawLine(sensorStartPos, hit.point);   
            isAvoiding = true;
            avoidMultiplier -= 0.5f;
        }

        // Set the position of the other side sensors
        sensorStartPos -= transform.right * sideSensorPosZ * 2;
        // Send out even more raycasts
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            // Draw a line, set isAvoiding to true and add to the avoidMultipler
            Debug.DrawLine(sensorStartPos, hit.point);   
            isAvoiding = true;
            avoidMultiplier += 1f;
        }

        // Send out another angled sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(angleSensors, transform.up) * transform.forward, out hit, sensorLength))
        {
            // Draw a line, set isAvoiding to true and add to the avoidMultipler
            Debug.DrawLine(sensorStartPos, hit.point);   
            isAvoiding = true;
            avoidMultiplier += 0.5f;
        }

        if (isAvoiding && avoidMultiplier == 0)
        {
            // Walk at an angle
            Walk(maxHeadingChange);
            // Return
            return;
        }

        // If we avoiding walk around the obstacle
        if (isAvoiding)
        {
            // Walk around the obstacle
            Walk(maxHeadingChange * avoidMultiplier);
        } else
        {
            // Walk randomly
            RandomWalk();
        }
    }
}
