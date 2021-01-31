using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class PreviewButton : MonoBehaviour, IPointerDownHandler
{
    [Header("Colors used for the buttons, children will use opposite colors")]
    public Color tabIdle;
    public Color tabActive;

    [Header("The actions to be triggered when clicked")]
    public UnityEvent onClickedEvents;

    [HideInInspector]
    public bool isClicked;
    [HideInInspector]
    public bool onClicked;
    [HideInInspector]
    public bool onReleased;
    [HideInInspector]
    public Image button;

    // Called when the button is pressed by the player
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        // Call the OnPointer func
        OnPointerDown();
    }

    // Called by default to reset the button
    public virtual void OnPointerDown()
    {
        // Resets the button
        PreviewInputManager.instance.ResetButton(this, true);
        // Invoke the events
        try { onClickedEvents.Invoke(); } catch { }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Get the bg image component
        button = gameObject.GetComponent<Image>();
    }
}
