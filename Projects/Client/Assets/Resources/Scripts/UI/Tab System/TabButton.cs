using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    TabGroup tabGroup;
    [HideInInspector]
    public Image background;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) { tabGroup.OnTabClicked(this); }
    void Start() { tabGroup = transform.parent.GetComponent<TabGroup>(); tabGroup.Subscribe(this); background = GetComponent<Image>(); }
}
