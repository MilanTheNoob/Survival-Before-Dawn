using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    Vector3 oldPos;
    Animator anim;

    void Start()
    {
        oldPos = transform.position;
        anim = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
        {
            if (transform.position == oldPos) { anim.SetFloat("MovementValue", 0f); } else { anim.SetFloat("MovementValue", 1f); }
            oldPos = transform.position;
        }
    }
}
