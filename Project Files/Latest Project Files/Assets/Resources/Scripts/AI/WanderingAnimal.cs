﻿using UnityEngine;

public class WanderingAnimal : AIBase
{
    void Start()
    {
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);

        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        StartCoroutine(NewHeading());
    }

    void Update() { CalculateGravity(); SensorsUpdate(); }
}