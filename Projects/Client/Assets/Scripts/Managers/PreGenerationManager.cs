using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PreGenerationManager : MonoBehaviour
{
    public static PreGenerationManager instance;

    [Header("All the supported worlds")]
    public List<WorldDataStruct> worlds;

    [Header("Basic UI Components n stuff")]
    public GameObject worldsUIParent;
    public Font buttonFont;

    public delegate void WorldsDelegate();
    public WorldsDelegate WorldsCallback;

    [HideInInspector]
    public Dictionary<WorldDataStruct, Button> worldButtons;
    [HideInInspector]
    public WorldDataStruct currentWorldSettings;
    [HideInInspector]
    public bool isInWorld = false;

    int originalSeed;

    void Awake()
    {
        instance = this;
        for (int i = 0; i < worlds.Count; i++) { worlds[i].propsParent.SetActive(false); }
    }

    void Start()
    {
        WorldsCallback += UpdateWorldsList;
        WorldsCallback.Invoke();
    }

    void FixedUpdate()
    {
        /*
        for (int i = 0; i < worldButtons.Count; i++)
        {
            if (worldButtons[i].onClicked)
            {
                // If so, then get the associated world and store it
                currentWorldSettings = GetWorld(worldButtons[i].currentWorld);
            }
        }
        */
    }

    public void PlaySelectedWorld() { GenerateWorld(currentWorldSettings); }
    public void GenerateWorld(string worldName) { GenerateWorld(GetWorld(worldName)); }

    public void ResetWorld()
    {   
        if (currentWorldSettings != null && isInWorld)
        {
            TerrainGenerator.instance.heightMapSettings.noiseSettings.seed = originalSeed;
            TerrainGenerator.instance.generateType = TerrainGenerator.GenerateType.Standard;
            TerrainGenerator.instance.ResetChunks();

            InputManager.MovePlayer(new Vector3(0, 25, 0));
            currentWorldSettings.propsParent.SetActive(false);

            isInWorld = false;
            currentWorldSettings = null;
        }
    }

    public void GenerateWorld(WorldDataStruct world)
    {
        if (!isInWorld) { originalSeed = TerrainGenerator.instance.heightMapSettings.noiseSettings.seed; }

        InputManager.MovePlayer(world.pos);

        isInWorld = true;
        currentWorldSettings = world;
        world.propsParent.SetActive(true);

        TerrainGenerator.instance.heightMapSettings.noiseSettings.seed = world.seed;
        TerrainGenerator.instance.generateType = TerrainGenerator.GenerateType.PreGen;
        TerrainGenerator.instance.ResetChunks();
    }

    public WorldDataStruct GetWorld(string worldName)
    {
        for (int i = 0; i < worlds.Count; i++)
        {
            if (worlds[i].name == worldName)
                return worlds[i];
        }

        return null;
    }

    public void UpdateWorldsList()
    {
        /*
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
            Button worldButton = worldUI.AddComponent<Button>();
            worldButton.tabIdle = new Color32(36, 36, 36, 255);
            worldButton.tabActive = new Color32(255, 255, 255, 255);
            //worldButtons.Add(worldButton);

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
        */
    }

    [System.Serializable]
    public class WorldDataStruct
    {
        public GameObject propsParent;
        public Vector3 pos;
        public int seed;
        public string name;
    }
}
