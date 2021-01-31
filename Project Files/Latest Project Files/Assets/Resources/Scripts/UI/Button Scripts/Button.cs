using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class Button : MonoBehaviour, IPointerDownHandler
{
    [Header("Colors used for the buttons, children will use opposite colors")]
    public Color tabIdle;
    public Color tabActive; 

    [Header("The actions to be triggered when clicked")]
    public UnityEvent onClickedEvents;

    [Header("Settings if you want to use an alert")]
    public bool useAlert;
    public GameObject alertUI;
    public string alertID;

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
        if (useAlert && SavingManager.SaveFile != null)
        {
            alertUI.SetActive(false);

            if (!SavingManager.SaveFile.clickedAlerts.Contains(alertID))
                SavingManager.SaveFile.clickedAlerts.Add(alertID);
        }

        if (onClickedEvents != null)
            try { onClickedEvents.Invoke(); } catch { }

        if (InputManager.instance != null) { InputManager.instance.Click(this, true); } else { MultiplayerInputManager.instance.Click(this, true); }
    }

    void Start()
    {
        button = gameObject.GetComponent<Image>();

        if (useAlert && SavingManager.SaveFile != null)
        {
            if (SavingManager.SaveFile.clickedAlerts.Contains(alertID) || SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer) { alertUI.SetActive(false); } else { alertUI.SetActive(true); }
        }
    }
}
