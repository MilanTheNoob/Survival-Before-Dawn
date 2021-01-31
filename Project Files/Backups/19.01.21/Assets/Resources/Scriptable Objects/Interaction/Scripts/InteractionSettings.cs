using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Interaction System/Interaction Settings")]
public class InteractionSettings : ScriptableObject
{
    private InteractableItem m_interactable;

    public InteractableItem Interactable
    {
        get => m_interactable;
        set => m_interactable = value;
    }

    public void Interact()
    {
        m_interactable.OnInteract();
    }

    public bool IsSameInteractable(InteractableItem _newInteractable) => m_interactable == _newInteractable;
    public bool IsEmpty() => m_interactable == null;
    public void ResetData() => m_interactable = null;
}
