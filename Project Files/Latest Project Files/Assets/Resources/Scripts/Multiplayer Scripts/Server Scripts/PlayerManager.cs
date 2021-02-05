using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    Vector3 oldPos;
    public Animator anim;

    int animationTick;

    void Start()
    {
        oldPos = transform.position;
        anim = gameObject.GetComponent<Animator>();

        anim.SetFloat("MovementValue", 0f);
    }

    void FixedUpdate()
    {
        if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
        {
            if (transform.position == oldPos) { animationTick += 1; } else { animationTick = 0; }
            if (animationTick >= 5) { anim.SetFloat("MovementValue", 0f); } else { anim.SetFloat("MovementValue", 1f); }
            oldPos = transform.position;
        }
    }
}
