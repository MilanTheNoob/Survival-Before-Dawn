using UnityEngine;

public class WanderingAnimal : AIBase
{
    // Start is called before the first frame update
    void Start()
    {
        // Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);

        // Set the animator and controller
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        // Start getting random new headings
        StartCoroutine(NewHeading());
    }

    // Update is called every frame
    void Update()
    {
        // Calculate the gravity
        CalculateGravity();
        // Check the sensors
        SensorsUpdate();
    }
}
