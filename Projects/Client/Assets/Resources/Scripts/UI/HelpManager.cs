using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HelpManager : MonoBehaviour
{
    [Header("The sections for the FAQ")]
    public List<SectionStruct> sections;

    [Header("The different UI comps")]
    public GameObject sectionsList;
    public GameObject sectionsListView;

    [Space]

    public GameObject section;
    public GameObject itemList;
    public Button sectionBackButton;
    public Text sectionDescription;

    [Space]

    public GameObject item;
    public Button itemBackButton;
    public Image itemSprite;
    public Text itemName;
    public Text itemDescription;

    [Header("Misc")]
    public Font font;

    public enum SectionType
    {
        ItemSettings,
        CraftingSettings,
        Standard
    }

    List<Button> itemButtons = new List<Button>();
    List<Button> sectionButtons = new List<Button>();

    SectionStruct currentSection;

    void Start()
    {
        sectionsList.SetActive(true);
        section.SetActive(false);
        item.SetActive(false);

        for (int i = 0; i < sections.Count; i++)
        {
            GameObject sectionButtonG = new GameObject("Button");
            sectionButtonG.transform.parent = sectionsListView.transform;
            sectionButtonG.AddComponent<HorizontalLayoutGroup>();
            sectionButtonG.transform.localScale = new Vector3(1, 1, 1);

            Image sectionButtonI = sectionButtonG.AddComponent<Image>();
            sectionButtonI.color = new Color32(36, 36, 36, 255);
            sectionButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            sectionButtonI.type = Image.Type.Sliced;

            Button sectionButtonB = sectionButtonG.AddComponent<Button>();
            sectionButtonB.tabIdle = new Color32(36, 36, 36, 255);
            sectionButtonB.tabActive = new Color32(255, 255, 255, 255);

            GameObject sectionButtonGT = new GameObject("Text");
            sectionButtonGT.transform.parent = sectionButtonG.transform;

            Text sectionButtonT = sectionButtonGT.AddComponent<Text>();
            sectionButtonT.text = sections[i].name;
            sectionButtonT.color = new Color32(255, 255, 255, 255);
            sectionButtonT.font = font;
            sectionButtonT.resizeTextForBestFit = true;
            sectionButtonT.resizeTextMaxSize = 60;
            sectionButtonT.alignment = TextAnchor.MiddleCenter;

            sectionButtonG.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 200);

            sectionButtonGT.transform.localScale = new Vector3(1, 1, 1);
            sectionButtons.Add(sectionButtonB);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < sectionButtons.Count; i++)
        {
            if (sectionButtons[i].onClicked && sections[i] != null) { ChangeSection(i); }
        }

        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (itemButtons[i].onClicked && currentSection != null)
            {
                itemSprite.sprite = currentSection.items[i].sprite;
                itemName.text = currentSection.items[i].name;
                itemDescription.text = currentSection.items[i].desc;

                TweeningLibrary.FadeOut(section, 0.2f);
                TweeningLibrary.FadeIn(item, 0.2f);
            }
        }

        if (sectionBackButton.onClicked)
        {
            TweeningLibrary.FadeOut(section, 0.2f);
            TweeningLibrary.FadeIn(sectionsList, 0.2f);
        }
        else if (itemBackButton.onClicked)
        {
            TweeningLibrary.FadeOut(item, 0.2f);
            TweeningLibrary.FadeIn(section, 0.2f);
        }
    }

    public void ChangeSection(int id)
    {
        sectionDescription.text = sections[id].description;
        currentSection = sections[id];

        for (int j = 0; j < itemButtons.Count; j++) { Destroy(itemButtons[j].gameObject); }
        itemButtons.Clear();

        for (int p = 0; p < sections[id].items.Length; p++)
        {
            GameObject itemButtonG = new GameObject("Button");
            itemButtonG.transform.parent = itemList.transform;
            itemButtonG.transform.localScale = new Vector3(1, 1, 1);
            itemButtonG.AddComponent<HorizontalLayoutGroup>();

            Image itemButtonI = itemButtonG.AddComponent<Image>();
            itemButtonI.color = new Color32(36, 36, 36, 255);
            itemButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            itemButtonI.type = Image.Type.Sliced;

            Button itemButtonB = itemButtonG.AddComponent<Button>();
            itemButtonB.tabIdle = new Color32(36, 36, 36, 255);
            itemButtonB.tabActive = new Color32(255, 255, 255, 255);

            GameObject itemButtonGT = new GameObject("Text");
            itemButtonGT.transform.parent = itemButtonG.transform;

            Text itemButtonT = itemButtonGT.AddComponent<Text>();
            itemButtonT.text = sections[id].items[p].name;
            itemButtonT.color = new Color32(255, 255, 255, 255);
            itemButtonT.font = PreGenerationManager.instance.buttonFont;
            itemButtonT.resizeTextForBestFit = true;
            itemButtonT.resizeTextMaxSize = 60;
            itemButtonT.alignment = TextAnchor.MiddleCenter;

            itemButtonGT.transform.localScale = new Vector3(1, 1, 1);
            itemButtons.Add(itemButtonB);
        }

        TweeningLibrary.FadeOut(sectionsList, 0.2f);
        TweeningLibrary.FadeIn(section, 0.2f);
    }

    [System.Serializable]
    public class SectionStruct
    {
        [Header("The name")]
        public string name;
        [Header("Description of the section")]
        public string description;

        [Header("If we are using Standard Items then use this list")]
        public SectionItemStruct[] items;
    }

    [System.Serializable]
    public class SectionItemStruct
    {
        [Header("The item name")]
        public string name;
        [Header("Description of the item")]
        public string desc;
        [Header("The sprite")]
        public Sprite sprite;
    }
}
