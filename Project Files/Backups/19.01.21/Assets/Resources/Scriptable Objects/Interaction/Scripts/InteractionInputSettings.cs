using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Interaction System/Interaction Input Settings")]
public class InteractionInputSettings : ScriptableObject
{
    private bool m_interactedClicked;
    private bool m_interactedRelease;

    // Set interactedClicked bool
    public bool InteractedClicked
    {
        get => m_interactedClicked;
        set => m_interactedClicked = value;
    }

    // Set the interactedRelease bool
    public bool InteractedRelease
    {
        get => m_interactedRelease;
        set => m_interactedRelease = value;
    }

    // Reset the bools
    public void ResetInput()
    {
        m_interactedClicked = false;
        m_interactedRelease = false;
    }
}
