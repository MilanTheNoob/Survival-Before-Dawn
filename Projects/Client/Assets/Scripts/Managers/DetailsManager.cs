using UnityEngine.UI;
using UnityEngine;

public class DetailsManager : MonoBehaviour
{
    #region Singleton

    public static DetailsManager instance;
    void Awake() { instance = this; }

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
        if (InputManager.instance != null)
        {
            if (useButton.onClicked && currentItem != null) { currentItem.Use(); InputManager.instance.ToggleUISectionsInt(0); }
            if (returnButton.onClicked) { InputManager.instance.ToggleUISectionsInt(2); }
        }
        else
        {
            if (useButton.onClicked && currentItem != null) { currentItem.Use(); MultiplayerInputManager.instance.ToggleUISectionsInt(0); }
            if (returnButton.onClicked) { MultiplayerInputManager.instance.ToggleUISectionsInt(2); }
        }
    }

    /// <summary>
    /// Shows information about an item in the details page
    /// </summary>
    /// <param name="itemSettings">The item to display info about</param>
    public void SetItem(ItemSettings itemSettings)
    {
        if (InputManager.instance != null) { InputManager.instance.ToggleUISectionsInt(5); } else { MultiplayerInputManager.instance.ToggleUISectionsInt(5); }
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
