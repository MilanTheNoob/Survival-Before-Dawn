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

    [Header("Components of the detail page")]
    public Button returnButton;
    public Button useButton;

    [Header("Text components")]
    public Text titleText;
    public Text descText;
    public Image itemImage;

    [HideInInspector]
    public ItemSettings currentItem;

    void Start()
    {
        useButton.enabled = false;
        itemImage.sprite = null;

        titleText.text = "";
        descText.text = "";
    }

    void Update()
    {
        if (useButton.onClicked && currentItem != null) { currentItem.Use(); InputManager.instance.ToggleUISectionsInt(0); }
        if (returnButton.onClicked) { InputManager.instance.ToggleUISectionsInt(2); }
    }

    // Called to set an item in the details page
    public void SetItem(ItemSettings itemSettings)
    {
        InputManager.instance.ToggleUISectionsInt(5);
        currentItem = itemSettings;

        if (itemSettings.isUsableItem)
        {
            useButton.enabled = true;
            useButton.gameObject.SetActive(true);
        } else 
        {
            useButton.enabled = false;
            useButton.gameObject.SetActive(false);
        }

        itemImage.sprite = itemSettings.icon;

        titleText.text = itemSettings.descName;
        descText.text = itemSettings.description;
    }
}
