using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class Button : MonoBehaviour, IPointerDownHandler
{
    [Header("Colors")]
    public Color tabIdle;
    public Color tabActive; 

    [Header("Actions triggered when clicked")]
    public UnityEvent onClickedEvents;

    [Header("Alert UI Settings")]
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
            if (!SavingManager.SaveData.clickedAlerts.Contains(alertID)) { SavingManager.SaveData.clickedAlerts.Add(alertID); }
        }

        try { onClickedEvents.Invoke(); } catch { }

        if (InputManager.instance != null) { InputManager.instance.Click(this); }
        if (MultiplayerInputManager.instance != null) { MultiplayerInputManager.instance.Click(this); }
        if (PreviewInputManager.instance != null) { PreviewInputManager.instance.Click(this); }
    }

    void Start()
    {
        button = gameObject.GetComponent<Image>();

        if (useAlert && SavingManager.SaveFile != null)
        {
            if (SavingManager.SaveData.clickedAlerts.Contains(alertID) || SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer) 
            {
                alertUI.SetActive(false); 
            } 
            else 
            { 
                alertUI.SetActive(true); 
            }
        }
    }
}
