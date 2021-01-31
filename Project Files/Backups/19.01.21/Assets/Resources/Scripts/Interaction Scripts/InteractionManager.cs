using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    #region Singleton

    // The singleton var
    public static InteractionManager instance;

    // Awake is called before Start
    void Awake()
    {
        // Set the singleton to us
        instance = this;
    }

    #endregion

    [Header("All the scriptable objects")]
    public InteractionInputSettings interactionInputSettings;
    public InteractionSettings interactionSettings;

    [Header("Raycast Settings")]
    public float raycastDistance;
    public float raycastSphereRadius;
    public LayerMask interactableLayer;

    [Header("UI Stuff")]
    public Button interactButton;
    public Text interactText;
    public Text interactName;

    public enum InteractableType
    {
        Object,
        Tree,
        Door,
        Trader
    }

    Camera interactCamera;
    bool buttonFaded;
    GameObject currentG;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the interact button
        interactButton.gameObject.SetActive(false);

        // Get the camera
        interactCamera = FindObjectOfType<Camera>();

        // Check if the interact text is null
        if (interactText == null)
            interactText = GameObject.Find("Interact Text").GetComponent<Text>();

        // Make sure there is no text in the interactTxt
        interactText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // Call all the check functions
        CheckForInteractable();
    }

    // Check for any interactable object
    void CheckForInteractable()
    {
        // Create a new Raycast from the player and get the hit info
        Ray ray = new Ray(interactCamera.transform.position, interactCamera.transform.forward);
        RaycastHit hitInfo;

        // Check if we hit an interactable object
        bool hitInteractableObject = Physics.SphereCast(ray, raycastSphereRadius, out hitInfo, raycastDistance, interactableLayer);
        // Do stuff depending on if we hit something
        if (hitInteractableObject)
        {
            // Get the interactable item
            InteractableItem interactable = hitInfo.transform.GetComponent<InteractableItem>();

            if (interactable.toolType == ToolsManager.instance.currentToolType && interactable.isInteractable || interactable.toolType == ToolsManager.ToolType.None && interactable.isInteractable)
            {
                // Set the Interaction Text appropiately
                interactText.text = interactable.interactTxt;
                interactName.text = interactable.name;

                // Do stuff with the UI
                if (interactButton.onClicked) { interactionSettings.Interact(); }
                if (!buttonFaded) { TweeningLibrary.FadeIn(interactButton.gameObject, 0.1f); buttonFaded = true; }

                // Highlight the interactable
                HighlightManager.Highlight(interactable.gameObject);
                currentG = interactable.gameObject;

                if (interactionSettings.IsEmpty())
                {
                    // Set the interactable item
                    interactionSettings.Interactable = interactable;
                } 
                else 
                {
                    // Check if the Interactable we have is the same in the InteractionSettings, 
                    // if not then set the Interaction object correctly
                    if (!interactionSettings.IsSameInteractable(interactable))
                    {
                        interactionSettings.Interactable = interactable;
                    }    
                }
            }
        } else
        {
            // Reset the Interaction Settings data and interact text
            interactionSettings.ResetData();

            // Restore the highlighted gameObject and fade out & reset the UI
            HighlightManager.Restore(currentG); currentG = null;
            if (interactText != null) { interactText.text = ""; interactName.text = ""; }
            if (buttonFaded) { TweeningLibrary.FadeOut(interactButton.gameObject, 0.1f); buttonFaded = false; }
        }

        // Draw a ray between the player and the interactable object
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, hitInteractableObject ? Color.green : Color.red);
    }
}
