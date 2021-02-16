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

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        try
        {
            PreviewInputManager.instance.ResetButton(this);
            onClickedEvents.Invoke();
        } catch { }
    }
    void Awake() { button = gameObject.GetComponent<Image>(); }
}
