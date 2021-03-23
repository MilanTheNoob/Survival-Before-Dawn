using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour
{
    [Header("Basic UI Components")]
    public GameObject recipesUI;

    List<CraftingSettings> recipes = new List<CraftingSettings>();
    Dictionary<int, Button> buttons = new Dictionary<int, Button>();

    bool dontCraft = false;

    void Start()
    {
        Inventory.instance.onItemChangedCallback += UpdateRecipes;
        var trecipes = Resources.LoadAll("Crafting Recipes", typeof(CraftingSettings)).Cast<CraftingSettings>();
        foreach (var recipe in trecipes) { recipes.Add(recipe); }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons.ElementAt(i).Value.onClicked && !dontCraft)
            {
                dontCraft = true;
                StartCoroutine(ResetCraftI());

                AudioManager.PlayEquip();
                recipes[buttons.ElementAt(i).Key].Craft();
            }
        }
    }

    IEnumerator ResetCraftI()
    {
        yield return new WaitForSeconds(0.2f);
        dontCraft = false;
    }

    void UpdateRecipes()
    {
        for (int i = 0; i < recipesUI.transform.childCount; i++) { Destroy(recipesUI.transform.GetChild(i).gameObject); }
        buttons.Clear();

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i].CanCraft())
            {
                CraftingVariant recipe = recipes[i].GetCraftableVariant();
                GameObject recipeUI = new GameObject("Crafting Recipe UI - " + recipes[i].recipeName);
                recipeUI.transform.parent = recipesUI.transform;

                Image recipeBGImage = recipeUI.AddComponent<Image>();
                recipeBGImage.color = new Color32(36, 36, 36, 255);
                recipeBGImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                recipeBGImage.type = Image.Type.Sliced;

                recipeUI.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
                recipeUI.transform.localScale = new Vector3(1, 1, 1);

                Button recipeButton = recipeUI.AddComponent<Button>();
                recipeButton.tabIdle = new Color32(36, 36, 36, 255);
                recipeButton.tabActive = new Color32(255, 255, 255, 255);

                buttons.Add(i, recipeButton);

                HorizontalLayoutGroup recipeLayoutGroup = recipeUI.AddComponent<HorizontalLayoutGroup>();
                recipeLayoutGroup.spacing = 5;
                recipeLayoutGroup.childForceExpandWidth = false;
                recipeLayoutGroup.childControlWidth = false;

                for (int j = 0; j < recipes[i].recipes[0].Input.Length; j++)
                {
                    GameObject inputObjectGO = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Input[j].name);

                    inputObjectGO.transform.parent = recipeUI.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    Image inputObjectImage = inputObjectGO.AddComponent<Image>();
                    inputObjectImage.sprite = recipe.Input[j].icon;

                    inputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
                
                for (int j = 0; j < recipes[i].recipes[0].Tools.Length; j++)
                {
                    GameObject p = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Tools[j].name);
                    GameObject inputObjectGO = new GameObject("Child");

                    Image bgImage = p.AddComponent<Image>();
                    bgImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                    bgImage.color = new Color32(0, 66, 66, 255);

                    p.transform.parent = recipeUI.transform;
                    p.transform.localScale = new Vector3(1, 1, 1);

                    p.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                    inputObjectGO.transform.parent = p.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    Image toolsObjectImage = inputObjectGO.AddComponent<Image>();
                    toolsObjectImage.sprite = recipe.Tools[j].icon;
                }

                GameObject ioSeperatorObject = new GameObject("Input to Output Seperator");
                
                ioSeperatorObject.transform.parent = recipeUI.transform;
                ioSeperatorObject.transform.localScale = new Vector3(1, 1, 1);
                
                Image ioSeperatorImage = ioSeperatorObject.AddComponent<Image>();
                ioSeperatorImage.sprite = Resources.Load<Sprite>("UI/UI Icons/Misc/Arrow");
                ioSeperatorImage.color = new Color32(255, 255, 255, 255);

                ioSeperatorObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                for (int j = 0; j < recipes[i].recipes[0].Output.Length; j++)
                {
                    GameObject outputObjectGO = new GameObject("Recipe Output - " + recipes[i].recipes[0].Output[j].name);
                    
                    outputObjectGO.transform.parent = recipeUI.transform;
                    outputObjectGO.transform.localScale = new Vector3(1, 1, 1);
                    
                    Image outputObjectImage = outputObjectGO.AddComponent<Image>();
                    outputObjectImage.sprite = recipes[i].recipes[0].Output[j].icon;
                    
                    outputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
            }
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            if (!recipes[i].CanCraft())
            {
                CraftingVariant recipe = recipes[i].recipes[0];

                GameObject recipeUI = new GameObject("Crafting Recipe UI - " + recipes[i].recipeName);
                recipeUI.transform.parent = recipesUI.transform;

                Image recipeBGImage = recipeUI.AddComponent<Image>();
                recipeBGImage.color = new Color32(66, 00, 00, 255);
                recipeBGImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                recipeBGImage.type = Image.Type.Sliced;

                recipeUI.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
                recipeUI.transform.localScale = new Vector3(1, 1, 1);

                HorizontalLayoutGroup recipeLayoutGroup = recipeUI.AddComponent<HorizontalLayoutGroup>();
                recipeLayoutGroup.spacing = 5;
                recipeLayoutGroup.childForceExpandWidth = false;
                recipeLayoutGroup.childControlWidth = false;

                for (int j = 0; j < recipes[i].recipes[0].Input.Length; j++)
                {
                    GameObject inputObjectGO = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Input[j].name);

                    inputObjectGO.transform.parent = recipeUI.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    Image inputObjectImage = inputObjectGO.AddComponent<Image>();
                    inputObjectImage.sprite = recipe.Input[j].icon;

                    inputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }

                for (int j = 0; j < recipes[i].recipes[0].Tools.Length; j++)
                {
                    GameObject p = new GameObject("Recipe Ingredient - " + recipes[i].recipes[0].Tools[j].name);
                    GameObject inputObjectGO = new GameObject("Child");

                    Image bgImage = p.AddComponent<Image>();
                    bgImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                    bgImage.color = new Color32(0, 66, 66, 255);

                    p.transform.parent = recipeUI.transform;
                    p.transform.localScale = new Vector3(1, 1, 1);

                    p.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                    inputObjectGO.transform.parent = p.transform;
                    inputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    Image toolsObjectImage = inputObjectGO.AddComponent<Image>();
                    toolsObjectImage.sprite = recipe.Tools[j].icon;
                }

                GameObject ioSeperatorObject = new GameObject("Input to Output Seperator");

                ioSeperatorObject.transform.parent = recipeUI.transform;
                ioSeperatorObject.transform.localScale = new Vector3(1, 1, 1);

                Image ioSeperatorImage = ioSeperatorObject.AddComponent<Image>();
                ioSeperatorImage.sprite = Resources.Load<Sprite>("UI/UI Icons/Misc/Arrow");
                ioSeperatorImage.color = new Color32(255, 255, 255, 255);

                ioSeperatorObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                for (int j = 0; j < recipes[i].recipes[0].Output.Length; j++)
                {
                    GameObject outputObjectGO = new GameObject("Recipe Output - " + recipes[i].recipes[0].Output[j].name);

                    outputObjectGO.transform.parent = recipeUI.transform;
                    outputObjectGO.transform.localScale = new Vector3(1, 1, 1);

                    Image outputObjectImage = outputObjectGO.AddComponent<Image>();
                    outputObjectImage.sprite = recipes[i].recipes[0].Output[j].icon;

                    outputObjectGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                }
            }
        }
    }
}
