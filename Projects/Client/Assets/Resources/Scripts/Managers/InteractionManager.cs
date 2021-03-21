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

    [Header("Raycast Settings")]
    public float raycastDistance;
    public float raycastSphereRadius;
    public LayerMask interactableLayer;

    [Header("UI Components")]
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

    [HideInInspector]
    public Camera interactCamera;

    bool buttonFaded;
    GameObject currentG;

    void Start()
    {
        interactButton.gameObject.SetActive(false);
        interactCamera = FindObjectOfType<Camera>();

        interactText.text = "";
    }

    void FixedUpdate()
    {
        if (interactCamera != null)
        {
            Ray ray = new Ray(interactCamera.transform.position, interactCamera.transform.forward);
            RaycastHit hitInfo;

            bool hitInteractableObject = Physics.SphereCast(ray, raycastSphereRadius, out hitInfo, raycastDistance, interactableLayer);
            if (hitInteractableObject)
            {
                InteractableItem interactable = hitInfo.transform.GetComponent<InteractableItem>();

                if (interactable.toolType == ToolsManager.instance.currentToolType && interactable.isInteractable || interactable.toolType == ToolsManager.ToolType.None && interactable.isInteractable)
                {
                    interactText.text = interactable.interactTxt;
                    interactName.text = interactable.name.Replace("-", " ");

                    if (interactButton.onClicked) { interactable.OnInteract(); }
                    if (!buttonFaded) { TweeningLibrary.FadeIn(interactButton.gameObject, 0.1f); buttonFaded = true; }

                    HighlightManager.Highlight(interactable.gameObject);
                    currentG = interactable.gameObject;
                }
            }
            else
            {
                HighlightManager.Restore(currentG); currentG = null;
                if (interactText != null) { interactText.text = ""; interactName.text = ""; }
                if (buttonFaded) { TweeningLibrary.FadeOut(interactButton.gameObject, 0.1f); buttonFaded = false; }
            }
        }
    }
}
