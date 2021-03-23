using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public Color tabIdle;
    public Color tabActive;

    public GameObject[] objectsToSwap;

    TabButton selectedTab;
    List<TabButton> tabButtons = new List<TabButton>();

    private void Start() { ResetTabs(); ResetTiles(); }
    public void Subscribe(TabButton button) { tabButtons.Add(button); }
    public void ResetTiles() { for (int i = 0; i < objectsToSwap.Length; i++) { objectsToSwap[i].SetActive(false); } }

    public void OnTabClicked(TabButton button)
    {
        AudioManager.PlayClick();
        TweeningLibrary.LerpColor(button.background, tabIdle, tabActive, 0.1f);

        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Length; i++)
        {
            if (i == index && !objectsToSwap[i].activeSelf)
            {
                TweeningLibrary.FadeIn(objectsToSwap[i], 0.1f);
            }
            else if (i != index)
            {
                TweeningLibrary.FadeOut(objectsToSwap[i], 0.1f);
            }
        }

        StartCoroutine(Reset(button));
    }

    IEnumerator Reset(TabButton button)
    {
        yield return new WaitForSeconds(0.2f);
        TweeningLibrary.LerpColor(button.background, tabActive, tabIdle, 0.1f);
    }

    public void ResetTabs()
    {
        for (int i = 0;  i < tabButtons.Count; i++)
        {
            if (selectedTab != null && tabButtons[i] == selectedTab) { continue; }
            TweeningLibrary.LerpColor(tabButtons[i].background, tabActive, tabIdle, 0.1f);
        }
    }
}
