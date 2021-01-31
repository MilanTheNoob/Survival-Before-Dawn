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

    public enum SectionType
    {
        ItemSettings,
        CraftingSettings,
        Standard
    }

    List<Button> itemButtons = new List<Button>();
    List<Button> sectionButtons = new List<Button>();

    SectionStruct currentSection;

    // Start is called before the first frame update
    void Start()
    {
        // Enable/disable UI components
        sectionsList.SetActive(true);
        section.SetActive(false);
        item.SetActive(false);

        // Loop through all the sections
        for (int i = 0; i < sections.Count; i++)
        {
            // Create a new gameObject and set vars
            GameObject sectionButtonG = new GameObject("Button");
            sectionButtonG.transform.parent = sectionsListView.transform;
            sectionButtonG.transform.localScale = new Vector3(1, 1, 1);
            sectionButtonG.AddComponent<HorizontalLayoutGroup>();

            // Add an Image to the gameObject
            Image sectionButtonI = sectionButtonG.AddComponent<Image>();
            sectionButtonI.color = new Color32(36, 36, 36, 255);
            sectionButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            sectionButtonI.type = Image.Type.Sliced;

            // Add a button and set vars
            Button sectionButtonB = sectionButtonG.AddComponent<Button>();
            sectionButtonB.tabIdle = new Color32(36, 36, 36, 255);
            sectionButtonB.tabActive = new Color32(255, 255, 255, 255);

            // Create a new gameObject as a child for the text
            GameObject sectionButtonGT = new GameObject("Text");
            sectionButtonGT.transform.parent = sectionButtonG.transform;

            // Add the text comp and set vars
            Text sectionButtonT = sectionButtonGT.AddComponent<Text>();
            sectionButtonT.text = sections[i].name;
            sectionButtonT.color = new Color32(255, 255, 255, 255);
            sectionButtonT.font = PreGenerationManager.instance.buttonFont;
            sectionButtonT.resizeTextForBestFit = true;
            sectionButtonT.resizeTextMaxSize = 60;
            sectionButtonT.alignment = TextAnchor.MiddleCenter;

            // Set the localScale (this is here cause of a bug/feature)
            sectionButtonGT.transform.localScale = new Vector3(1, 1, 1);
            // Add the item to the list
            sectionButtons.Add(sectionButtonB);
        }

        /*
        // Loop through all the sections
        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i].sectionType == SectionType.ItemSettings)
            {
                // Add all the interactables to the game
                var tinteractables = Resources.LoadAll("Prefabs/Interactable Items", typeof(ItemSettings)).Cast<ItemSettings>();
                foreach (var interactable in tinteractables) { sections[i].itemSettings.Add(interactable); }
            }
            else if (sections[i].sectionType == SectionType.CraftingSettings)
            {
                // Add all the crafting recipes to the game
                var trecipes = Resources.LoadAll("Scriptable Objects/Crafting Recipes", typeof(CraftingSettings)).Cast<CraftingSettings>();
                foreach (var recipe in trecipes) { sections[i].craftingSettings.Add(recipe); }
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        // Loop through all the section items
        for (int i = 0; i < sectionButtons.Count; i++)
        {
            // If the section button is clicked then change to respective section
            if (sectionButtons[i].onClicked && sections[i] != null) { ChangeSection(i); }
        }

        // Loop through all the item
        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (itemButtons[i].onClicked && currentSection != null)
            {
                // Set vars
                itemSprite.sprite = currentSection.items[i].sprite;
                itemName.text = currentSection.items[i].name;
                itemDescription.text = currentSection.items[i].desc;

                // Fade the components
                TweeningLibrary.FadeOut(section, 0.2f);
                TweeningLibrary.FadeIn(item, 0.2f);
            }
        }

        // If one of the back buttons have been pressed
        if (sectionBackButton.onClicked)
        {
            // Fade the comps
            TweeningLibrary.FadeOut(section, 0.2f);
            TweeningLibrary.FadeIn(sectionsList, 0.2f);
        }
        else if (itemBackButton.onClicked)
        {
            // Fade the comps
            TweeningLibrary.FadeOut(item, 0.2f);
            TweeningLibrary.FadeIn(section, 0.2f);
        }
    }

    // Called to change the section that is currently viewed
    public void ChangeSection(int id)
    {
        // Set the basic comps
        sectionDescription.text = sections[id].description;
        currentSection = sections[id];

        // Clear the UI list
        for (int j = 0; j < itemButtons.Count; j++) { Destroy(itemButtons[j].gameObject); }
        itemButtons.Clear();

        for (int p = 0; p < sections[id].items.Length; p++)
        {
            // Create a new gameObject and set vars
            GameObject itemButtonG = new GameObject("Button");
            itemButtonG.transform.parent = itemList.transform;
            itemButtonG.transform.localScale = new Vector3(1, 1, 1);
            itemButtonG.AddComponent<HorizontalLayoutGroup>();

            // Add an Image to the gameObject
            Image itemButtonI = itemButtonG.AddComponent<Image>();
            itemButtonI.color = new Color32(36, 36, 36, 255);
            itemButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            itemButtonI.type = Image.Type.Sliced;

            // Add a button and set vars
            Button itemButtonB = itemButtonG.AddComponent<Button>();
            itemButtonB.tabIdle = new Color32(36, 36, 36, 255);
            itemButtonB.tabActive = new Color32(255, 255, 255, 255);

            // Create a new gameObject as a child for the text
            GameObject itemButtonGT = new GameObject("Text");
            itemButtonGT.transform.parent = itemButtonG.transform;

            // Add the text comp and set vars
            Text itemButtonT = itemButtonGT.AddComponent<Text>();
            itemButtonT.text = sections[id].items[p].name;
            itemButtonT.color = new Color32(255, 255, 255, 255);
            itemButtonT.font = PreGenerationManager.instance.buttonFont;
            itemButtonT.resizeTextForBestFit = true;
            itemButtonT.resizeTextMaxSize = 60;
            itemButtonT.alignment = TextAnchor.MiddleCenter;

            // Set the localScale (this is here cause of a bug/feature)
            itemButtonGT.transform.localScale = new Vector3(1, 1, 1);
            // Add the item to the list
            itemButtons.Add(itemButtonB);
        }

        // Fade out comps
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
