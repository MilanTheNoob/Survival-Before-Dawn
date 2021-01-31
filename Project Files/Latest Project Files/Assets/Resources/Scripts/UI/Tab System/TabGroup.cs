using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [Header("Colors used for different states of the buttons")]
    public Color tabIdle;
    public Color tabActive;

    [Header("The list of objects to swap depending on input from the buttons")]
    public GameObject[] objectsToSwap;

    TabButton selectedTab;
    List<TabButton> tabButtons = new List<TabButton>();

    private void Start()
    {
        //Reset tabs & tiles
        ResetTabs();
        ResetTiles();
    }

    public void Subscribe(TabButton button)
    {
        //Add the button
        tabButtons.Add(button);
    }

    public void OnTabClicked(TabButton button)
    {
        // Play click noise
        AudioManager.PlayClick();

        // Set the color of the button & txt
        TweeningLibrary.LerpColor(button.background, tabIdle, tabActive, 0.1f);

        TweeningLibrary.LerpColor(button.text, tabActive, tabIdle, 0.1f);
        TweeningLibrary.LerpColor(button.image, tabActive, tabIdle, 0.1f);

        // Get the siblings index of the button
        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Length; i++)
        {
            if (i == index && !objectsToSwap[i].activeSelf)
            {
                // Enable the menu object
                TweeningLibrary.FadeIn(objectsToSwap[i], 0.1f);
            }
            else if (i != index)
            {
                //Disable the rest
                TweeningLibrary.FadeOut(objectsToSwap[i], 0.1f);
            }
        }

        StartCoroutine(Reset(button));
    }

    IEnumerator Reset(TabButton button)
    {
        yield return new WaitForSeconds(0.2f);

        // Set the color of the button & txt
        TweeningLibrary.LerpColor(button.background, tabActive, tabIdle, 0.1f);

        TweeningLibrary.LerpColor(button.text, tabIdle, tabActive, 0.1f);
        TweeningLibrary.LerpColor(button.image, tabIdle, tabActive, 0.1f);
    }

    public void ResetTabs()
    {
        for (int i = 0;  i < tabButtons.Count; i++)
        {
            //Make sure the selectedTab isn't affected by the line underneath
            if (selectedTab != null && tabButtons[i] == selectedTab) { continue; }

            //Set the color of all buttons & txt to the idle color
            TweeningLibrary.LerpColor(tabButtons[i].background, tabActive, tabIdle, 0.1f);

            TweeningLibrary.LerpColor(tabButtons[i].text, tabIdle, tabActive, 0.1f);
            TweeningLibrary.LerpColor(tabButtons[i].image, tabIdle, tabActive, 0.1f);
        }
    }

    public void ResetTiles()
    {
        for (int i = 0; i < objectsToSwap.Length; i++)
        {
            //Disable all the tiles
            objectsToSwap[i].SetActive(false);
        }
    }
}
