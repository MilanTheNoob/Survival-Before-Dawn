using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    float HoldDuration { get; }

    bool HoldInteract { get; }
    bool MultipleUse { get; }
    bool IsInteractable { get; }

    void OnInteract();

}
