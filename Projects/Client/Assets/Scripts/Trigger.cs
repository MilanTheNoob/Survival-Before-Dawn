using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Trigger : MonoBehaviour
{
    public UnityEvent onCollide;

    // Call when something collides with us
    void OnTriggerEnter(Collider other)
    {
        // Check if it is a player
        if (other.gameObject.CompareTag("Player"))
            onCollide.Invoke();
    }
}
