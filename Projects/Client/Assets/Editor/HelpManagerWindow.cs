using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class HelpManagerWindow : EditorWindow
{
    [MenuItem("Editor Windows/Help")]
    public static void ShowWindow()
    {
        GetWindow<HelpManagerWindow>("Help");
    }

    private void OnGUI()
    {
        GUILayout.Label("Help", EditorStyles.boldLabel);
        GUILayout.Label("Generates info for the HelpManager");

        if (GUILayout.Button("Generate"))
        {
            HelpManager hm = GameObject.FindObjectOfType<HelpManager>();
            hm.sections.Clear();

            List<string> itemTypes = new List<string>(Directory.EnumerateDirectories("Assets\\Resources\\Prefabs\\Basic Objects"));
            Debug.Log(itemTypes[0]);

            for (int i = 0; i < itemTypes.Count; i++)
            {
                List<HelpManager.SectionItemStruct> items = new List<HelpManager.SectionItemStruct>();
                List<string> itemsDirs = new List<string>(Directory.EnumerateDirectories(itemTypes[i]));

                string folderName = itemTypes[i].Replace("Assets\\Resources\\Prefabs\\Basic Objects\\", "");

                for (int j = 0; j < itemsDirs.Count; j++)
                {
                    string name = itemsDirs[j].Replace("Assets\\Resources\\Prefabs\\Basic Objects\\" + folderName + "\\", "").ToLower().Replace(" ", "-");
                    ItemSettings itemI = Resources.Load<ItemSettings>("Prefabs/Interactable Items/" + name);

                    Debug.Log(name);

                    HelpManager.SectionItemStruct item = new HelpManager.SectionItemStruct();
                    item.name = itemI.descName;
                    item.sprite = itemI.icon;
                    item.desc = itemI.description;

                    items.Add(item);
                }

                HelpManager.SectionStruct section = new HelpManager.SectionStruct();
                section.name = "Interactable Items - " + folderName;
                section.items = items.ToArray();

                hm.sections.Add(section);
            }

            List<HelpManager.SectionItemStruct> recipes = new List<HelpManager.SectionItemStruct>();
            List<CraftingSettings> tRecipes = new List<CraftingSettings>();

            var trecipes = Resources.LoadAll("Scriptable Objects/Crafting Recipes", typeof(CraftingSettings)).Cast<CraftingSettings>();
            foreach (var recipe in trecipes) { tRecipes.Add(recipe); }

            for (int j = 0; j < tRecipes.Count; j++)
            {
                HelpManager.SectionItemStruct recipe = new HelpManager.SectionItemStruct();
                recipe.name = tRecipes[j].recipeName;
                recipe.desc = tRecipes[j].recipeDesc;
                recipe.sprite = Resources.Load<Sprite>("UI/UI Icons/Action Icons/crafting");

                recipes.Add(recipe);
            }

            HelpManager.SectionStruct craftingSection = new HelpManager.SectionStruct();
            craftingSection.name = "Crafting Recipes";
            craftingSection.description = "Crafting allows players to make items with others, to go to the Crafting Menu click on the Shortcut Button > Crafting. Click on one of the recipes on the side and click Craft to make something!";
            craftingSection.items = recipes.ToArray();

            hm.sections.Add(craftingSection);
        }
    }
}
