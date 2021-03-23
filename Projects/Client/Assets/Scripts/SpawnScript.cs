using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    void Start()
    {
        if (InputManager.instance != null && Random.Range(1, 3) == 1)
        {
            int r = Random.Range(0, InputManager.instance.smallRandomItems.Count);
            GameObject g = Instantiate(InputManager.instance.smallRandomItems[r].gameObject, transform.position, Quaternion.identity);
            g.transform.eulerAngles = new Vector3(0, Random.Range(0, 255), 0);
            g.transform.parent = transform.parent;
            g.transform.name = InputManager.instance.smallRandomItems[r].name;
        }
    }
}
