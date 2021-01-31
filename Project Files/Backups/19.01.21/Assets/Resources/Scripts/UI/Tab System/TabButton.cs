using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    [Header("The tab group this button is to subscribe to")]
    public TabGroup tabGroup;

    [Header("The Text & Image inside the button")]
    public Text text;
    public Image image;

    [HideInInspector]
    public Image background;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //Call OnTabSelected from the tabGroup when clicked
        tabGroup.OnTabClicked(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get the bg image component
        background = GetComponent<Image>();
        //Subscribe to the tab group we have been assigned
        tabGroup.Subscribe(this);
    }
}
