using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DetailsManager : MonoBehaviour
{
    #region Singleton

    // The instance of ourselves
    public static DetailsManager instance;

    // Called before Start
    void Awake()
    {
        // Set the instance to ourselves
        instance = this;
    }

    #endregion

    [Header("The button components of the detail page")]
    public Button returnButton;
    public Button useButton;

    [Header("The text components")]
    public Text titleText;
    public Text descText;
    public Image itemImage;

    [HideInInspector]
    public ItemSettings currentItem;

    // Start is called before the first frame update
    void Start()
    {
        // Reset all the buttons & texts
        useButton.enabled = false;
        itemImage.sprite = null;

        titleText.text = "";
        descText.text = "";
    }

    // Called every frame
    void Update()
    {
        // Call sum code if the use button is clicked
        if (useButton.onClicked && currentItem != null)
        {
            // Use the current item
            currentItem.Use();
            // Change which UI to choose
            InputManager.instance.ToggleUISectionsInt(0);
        }

        // Change Ui if the return button is called
        if (returnButton.onClicked) { InputManager.instance.ToggleUISectionsInt(2); }
    }

    // Called to set an item in the details page
    public void SetItem(ItemSettings itemSettings)
    {
        // USED ONLY TO GET RID OF THE DETAILS PAGE
        // If it is an item to use then use it not read description
        //if (itemSettings.isUsableItem) { itemSettings.Use(); return; }

        // Change the current UI
        InputManager.instance.ToggleUISectionsInt(5);

        // Set the current item
        currentItem = itemSettings;

        // To enable the use button or not, that is the question
        if (itemSettings.isUsableItem)
        {
            useButton.enabled = true;
            useButton.gameObject.SetActive(true);
        } else 
        {
            useButton.enabled = false;
            useButton.gameObject.SetActive(false);
        }

        // Set the sprites & texts
        itemImage.sprite = itemSettings.icon;

        titleText.text = itemSettings.descName;
        descText.text = itemSettings.description;
    }
}
