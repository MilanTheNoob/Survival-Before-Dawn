using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour
{
    [Header("Basic UI Components")]
    public Button craftButton;
    public GameObject recipesUI;

    List<CraftingSettings> recipes = new List<CraftingSettings>();
    List<CraftingButton> recipeButtons = new List<CraftingButton>();

    CraftingSettings selectedR;


    // Start is called before the first frame update
    void Start()
    {
        // Add the updateRecipes func to the inventory Callback
        Inventory.instance.onItemChangedCallback += UpdateRecipes;

        // Load all the crafting recipes
        var trecipes = Resources.LoadAll("Scriptable Objects/Crafting Recipes", typeof(CraftingSettings)).Cast<CraftingSettings>();
        foreach (var recipe in trecipes) { recipes.Add(recipe); }
    }

    // Called every frame
    void Update()
    {
        // Loop through all the recipes
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            // Check if one of them has been clicked
            if (recipeButtons[i].onClicked) 
            {
                // Set the selected recipe button to the button that has been clicked
                selectedR = recipeButtons[i].currentRecipe;
            }
        }

        // Check if the craft button has been clicked
        if (craftButton.onClicked && selectedR != null)
        {
            // Craft the selected recipe
            selectedR.Craft();
            // Empty the var after
            selectedR = null;

            // Make audio
            AudioManager.PlayEquip();
        }
    }

    // Called whenever there is a change in the inventory
    void UpdateRecipes()
    {
        // Remove all the recipes UI
        for (int i = 0; i < recipesUI.transform.childCount; i++) { Destroy(recipesUI.transform.GetChild(i).gameObject); }

        // Remove all the recipes slots
        recipeButtons.Clear();

        // Cycke through all the recipes and see what is craftable
        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].CanCraft())
            {
                CraftingVariant recipe = recipes[i].GetCraftableVariant();

                // Create a new recipe ui parent
                GameObject recipeUI = new GameObject("Crafting Recipe UI - " + recipes[i].recipeName);

                // Set the parent and local pos correctly
                recipeUI.transform.parent = recipesUI.transform;

                // Add a new background image to it and set all the settings correctly
                Image recipeBGImage = recipeUI.AddComponent<Image>();
                recipeBGImage.color = new Color32(36, 36, 36, 255);
                recipeBGImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                recipeBGImage.type = Image.Type.Sliced;

                // Get the recipe ui RectTransform and set the size correctly
                recipeUI.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
                recipeUI.transform.localScale = new Vector3(1, 1, 1);

                // Add a new crafting button component
                CraftingButton recipeButton = recipeUI.AddComponent<CraftingButton>();
                recipeButton.tabIdle = new Color32(36, 36, 36, 255);
                recipeButton.tabActive = new Color32(255, 255, 255, 255);
                recipeButton.currentRecipe = recipes[i];

                // Add the ui object to the buttons list
                recipeButtons.Add(recipeButton);

                // Add a new HorizontalLayoutGroup and set it correctly
                HorizontalLayoutGroup recipeLayoutGroup = recipeUI.AddComponent<HorizontalLayoutGroup>();
                recipeLayoutGroup.spacing = 5;
                recipeLayoutGroup.childForceExpandWidth = false;
                recipeLayoutGroup.childControlWidth = false;

                for (int j = 0; j < recipes[i].recipes[0].Input.Length; j++)
                {
                    // Add a new input object ui
                    GameObject inputObjectGO = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Input[j].name);

                    // Set the transform and local pos correctly
                    inputObjectGO.transform.parent = recipeUI.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    // Add an image component to it and set the sprite
                    Image inputObjectImage = inputObjectGO.AddComponent<Image>();
                    inputObjectImage.sprite = recipe.Input[j].icon;

                    // Set the size of it correctly
                    inputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
                
                for (int j = 0; j < recipes[i].recipes[0].Tools.Length; j++)
                {
                    GameObject p = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Tools[j].name);
                    // Add a new input object ui
                    GameObject inputObjectGO = new GameObject("Child");

                    // Add an image component to it and set the sprite
                    Image bgImage = p.AddComponent<Image>();
                    bgImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                    bgImage.color = new Color32(0, 66, 66, 255);

                    // Set the transform and local pos correctly
                    p.transform.parent = recipeUI.transform;
                    p.transform.localScale = new Vector3(1, 1, 1);

                    // Set the size of it correctly
                    p.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                    // Set the transform and local pos correctly
                    inputObjectGO.transform.parent = p.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    // Add an image component to it and set the sprite
                    Image toolsObjectImage = inputObjectGO.AddComponent<Image>();
                    toolsObjectImage.sprite = recipe.Tools[j].icon;
                }

                // Add a new seperator object ui
                GameObject ioSeperatorObject = new GameObject("Input to Output Seperator");
                
                // Set the parent and local pos correctly
                ioSeperatorObject.transform.parent = recipeUI.transform;
                ioSeperatorObject.transform.localScale = new Vector3(1, 1, 1);
                
                // Add a new image component and set it up
                Image ioSeperatorImage = ioSeperatorObject.AddComponent<Image>();
                ioSeperatorImage.sprite = Resources.Load<Sprite>("UI/UI Icons/Arrow");
                ioSeperatorImage.color = new Color32(0, 66, 66, 255);

                // Set the size correctly
                ioSeperatorObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                for (int j = 0; j < recipes[i].recipes[0].Output.Length; j++)
                {
                    // Add a new output object ui
                    GameObject outputObjectGO = new GameObject("Recipe Output - " + recipes[i].recipes[0].Output[j].name);
                    
                    // Set the transform and local pos correctly
                    outputObjectGO.transform.parent = recipeUI.transform;
                    outputObjectGO.transform.localScale = new Vector3(1, 1, 1);
                    
                    // Add an image component to it and set the sprite
                    Image outputObjectImage = outputObjectGO.AddComponent<Image>();
                    outputObjectImage.sprite = recipes[i].recipes[0].Output[j].icon;
                    
                    // Set the size of it correctly
                    outputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
            }
        }
    }
}
