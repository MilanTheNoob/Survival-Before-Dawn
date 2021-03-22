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
    InteractableItem interactableI;

    void Start()
    {
        interactButton.gameObject.SetActive(false);
        interactCamera = FindObjectOfType<Camera>();

        interactText.text = "";
    }

    void FixedUpdate()
    {
        if (interactableI != null && interactButton.onClicked) { interactableI.OnInteract(); }

        if (interactCamera != null)
        {
            Ray ray = new Ray(interactCamera.transform.position, interactCamera.transform.forward);
            RaycastHit hitInfo;

            bool hitInteractableObject = Physics.SphereCast(ray, raycastSphereRadius, out hitInfo, raycastDistance, interactableLayer);
            if (hitInteractableObject && interactableI == null)
            {
                interactableI = hitInfo.transform.GetComponent<InteractableItem>();

                if (interactableI.toolType == ToolsManager.instance.currentToolType && interactableI.isInteractable || interactableI.toolType == ToolsManager.ToolType.None && interactableI.isInteractable)
                {
                    interactText.text = interactableI.interactTxt;
                    interactName.text = interactableI.name.Replace("-", " ");

                    TweeningLibrary.FadeIn(interactButton.gameObject, 0.1f);

                    HighlightManager.Highlight(interactableI.gameObject);
                }
            }
            else if (!hitInteractableObject && interactableI != null)
            {
                HighlightManager.Restore(interactableI.gameObject);
                interactableI = null;

                interactText.text = "";
                interactName.text = "";

                TweeningLibrary.FadeOut(interactButton.gameObject, 0.1f);
            }
        }
    }
}
