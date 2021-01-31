using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PreGenerationManager : MonoBehaviour
{
    #region Singleton

    public static PreGenerationManager instance;

    #endregion

    [Header("All the supported worlds")]
    public List<WorldDataStruct> worlds;

    [Header("Basic UI Components n stuff")]
    public GameObject worldsUIParent;
    public Font buttonFont;

    public delegate void WorldsDelegate();
    public WorldsDelegate WorldsCallback;

    [HideInInspector]
    public List<WorldButton> worldButtons;
    [HideInInspector]
    public WorldDataStruct currentWorldSettings;
    [HideInInspector]
    public bool isInWorld = false;

    int originalSeed;

    // Called before Start
    void Awake()
    {
        // Set our insgleton's instance
        instance = this;

        // Loop through all the worlds and disable them for now
        for (int i = 0; i < worlds.Count; i++) { worlds[i].propsParent.SetActive(false); }
    }

    // Called at the beginning of the game
    void Start()
    {
        // Add the UpdateWorldsList func to the worlds callback
        WorldsCallback += UpdateWorldsList;
        // Invoke the callback
        WorldsCallback.Invoke();
    }

    // Called per frame
    void Update()
    {
        // Loop through the world buttons
        for (int i = 0; i < worldButtons.Count; i++)
        {
            // Check if the button is clicked
            if (worldButtons[i].onClicked)
            {
                // If so, then get the associated world and store it
                currentWorldSettings = GetWorld(worldButtons[i].currentWorld);
            }
        }
    }

    // Plays the selected world
    public void PlaySelectedWorld() { GenerateWorld(currentWorldSettings); }
    // Called to generate a specified world (by string)
    public void GenerateWorld(string worldName) { GenerateWorld(GetWorld(worldName)); }

    // Resets the world
    public void ResetWorld()
    {   
        if (currentWorldSettings != null && isInWorld)
        {
            // Set all the TerrainGenerator params back to default
            TerrainGenerator.instance.heightMapSettings.noiseSettings.seed = originalSeed;
            TerrainGenerator.instance.generateType = TerrainGenerator.GenerateType.Standard;
            TerrainGenerator.instance.ResetChunks();
            StartCoroutine(BuildingManager.instance.WaitLoadDataI());

            // Reset the player pos
            InputManager.instance.player.GetComponent<CharacterController>().enabled = false;
            InputManager.instance.player.transform.position = new Vector3(0, 25, 0);
            InputManager.instance.player.GetComponent<CharacterController>().enabled = true;

            // Disable the world in the scene
            currentWorldSettings.propsParent.SetActive(false);

            // Reset state vars, etc
            isInWorld = false;
            currentWorldSettings = null;
        }
    }

    // Called to generate
    public void GenerateWorld(WorldDataStruct world)
    {
        // Get the original seed if we are not in a world
        if (!isInWorld)
            originalSeed = TerrainGenerator.instance.heightMapSettings.noiseSettings.seed;

        // Basic vars being set
        isInWorld = true;
        currentWorldSettings = world;

        // Set the TerrainGenerator params
        TerrainGenerator.instance.heightMapSettings.noiseSettings.seed = world.seed;
        TerrainGenerator.instance.generateType = TerrainGenerator.GenerateType.PreGen;
        TerrainGenerator.instance.ResetChunks();

        // Enable the world in the scene
        world.propsParent.SetActive(true);
    }

    // Returns a world 
    public WorldDataStruct GetWorld(string worldName)
    {
        // Loop through all the worlds
        for (int i = 0; i < worlds.Count; i++)
        {
            // Return a world once we find it
            if (worlds[i].name == worldName)
                return worlds[i];
        }

        // Return null if the for loop falls through
        return null;
    }

    // Updates the list of worlds
    public void UpdateWorldsList()
    {
        // Loop through all the UI children and destroy them
        for (int i = 0; i < worldsUIParent.transform.childCount; i++) { Destroy(worldsUIParent.transform.GetChild(i).gameObject); }
        // Clear the buttons list
        worldButtons.Clear();

        // Loop through all the worlds
        for (int i = 0; i < worlds.Count; i++)
        {
            // Create a new empty UI parent
            GameObject worldUI = new GameObject("World Settings UI - " + worlds[i].name);

            // Set the parent & localScale
            worldUI.transform.parent = worldsUIParent.transform;
            worldUI.transform.localScale = new Vector3(1, 1, 1);

            // Create a new Image comp
            Image worldBGImage = worldUI.AddComponent<Image>();
            worldBGImage.color = new Color32(36, 36, 36, 255);
            worldBGImage.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            worldBGImage.type = Image.Type.Sliced;

            // Set the sizeDelta
            worldUI.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);

            // Add a new WorldButton
            WorldButton worldButton = worldUI.AddComponent<WorldButton>();
            worldButton.tabIdle = new Color32(36, 36, 36, 255);
            worldButton.tabActive = new Color32(255, 255, 255, 255);
            worldButton.currentWorld = worlds[i].name;

            // Add the WorldButton to the buttons list
            worldButtons.Add(worldButton);

            // Add a HLG
             worldUI.AddComponent<HorizontalLayoutGroup>();
            // Create a new child for text
            GameObject worldUITxt = new GameObject(worlds[i].name + "'s Description");

            // Set the child's parent & localScale
            worldUITxt.transform.parent = worldUI.transform;
            worldUITxt.transform.localScale = new Vector3(1, 1, 1);

            // Add a text comp
            Text UITxtComp = worldUITxt.AddComponent<Text>();

            // Set all the basic vars
            UITxtComp.text = worlds[i].name;
            UITxtComp.color = new Color32(255, 255, 255, 255);
            UITxtComp.font = buttonFont;
            UITxtComp.resizeTextForBestFit = true;
            UITxtComp.resizeTextMaxSize = 60;
            UITxtComp.alignment = TextAnchor.MiddleLeft;
        }
    }

    [System.Serializable]
    public class WorldDataStruct
    {
        public GameObject propsParent;
        public int seed;
        public string name;
    }
}
