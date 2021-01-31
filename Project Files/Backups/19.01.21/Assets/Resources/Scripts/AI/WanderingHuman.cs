using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class WanderingHuman : AIBase
{
    [Header("More Basic GameObjects")]
    public GameObject[] hairPrefabs;
    public GameObject[] randomColorObj;

    // Start is called before the first frame update
    void Start()
    {
        // Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);

        // Set the Animator & Controller
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        // Start getting random new headings
        StartCoroutine(NewHeading());
    }

    // Update is called every frame
    void Update()
    {
        // Call the calculate gravity func
        CalculateGravity();
        // Call the Sensors Update Func
        SensorsUpdate(); 
    }
}
