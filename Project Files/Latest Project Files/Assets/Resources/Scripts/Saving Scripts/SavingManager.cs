using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using System.Collections;

public class SavingManager : MonoBehaviour
{
    [Header("UI components in the profile scene")]
    public GameObject saveFilesView;

    [Space]

    public InputField newSFName;
    public PreviewButton newSFButton;

    [Space]

    public GameObject serversHolder;

    public PreviewButton playButton;
    public InputField nameField;
    public InputField serverField;
    public InputField portField;

    [Space]

    public Font font;

    public static SavingManager instance;

    public delegate void SaveGameDelegate();
    public static SaveGameDelegate SaveGameCallback;

    public static SaveFileStruct SaveFile;
    public static SaveDataStruct SaveData;

    public static string ip;
    public static int port;

    List<PreviewButton> saveFileButtons = new List<PreviewButton>();
    List<PreviewButton> buttons = new List<PreviewButton>();

    public enum GameStateEnum
    {
        None,
        Menu,
        Singleplayer,
        Multiplayer
    }
    static public GameStateEnum GameState;

    bool loadingServer;

    void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        instance = this;

        if (BinarySerializer.HasSaved("SurvivalBeforeDawn_CurrentSaveData_V2.savedata"))
        {
            SaveData = BinarySerializer.Load<SaveDataStruct>("SurvivalBeforeDawn_CurrentSaveData_V2.savedata");

            for (int i = 0; i < SaveData.SaveFileNames.Count; i++)
            {
                GameObject sfButtonG = new GameObject(SaveData.SaveFileNames[i]);
                sfButtonG.transform.parent = saveFilesView.transform;
                sfButtonG.AddComponent<HorizontalLayoutGroup>();
                sfButtonG.transform.localScale = new Vector3(1, 1, 1);

                Image sectionButtonI = sfButtonG.AddComponent<Image>();
                sectionButtonI.color = new Color32(36, 36, 36, 255);
                sectionButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                sectionButtonI.type = Image.Type.Sliced;

                PreviewButton sectionButtonB = sfButtonG.AddComponent<PreviewButton>();
                sectionButtonB.tabIdle = new Color32(36, 36, 36, 255);
                sectionButtonB.tabActive = new Color32(255, 255, 255, 255);

                GameObject sectionButtonGT = new GameObject("Text");
                sectionButtonGT.transform.parent = sfButtonG.transform;

                Text sectionButtonT = sectionButtonGT.AddComponent<Text>();
                sectionButtonT.text = SaveData.SaveFileNames[i];
                sectionButtonT.color = new Color32(255, 255, 255, 255);
                sectionButtonT.font = font;
                sectionButtonT.resizeTextForBestFit = true;
                sectionButtonT.resizeTextMaxSize = 60;
                sectionButtonT.alignment = TextAnchor.MiddleCenter;

                sectionButtonGT.transform.localScale = new Vector3(1, 1, 1);
                saveFileButtons.Add(sectionButtonB);
            }

            for (int i = 0; i < SaveData.Servers.Count; i++)
            {
                GameObject sfButtonG = new GameObject(SaveData.Servers[i].localName);
                sfButtonG.transform.parent = serversHolder.transform;
                sfButtonG.AddComponent<HorizontalLayoutGroup>();
                sfButtonG.transform.localScale = new Vector3(1, 1, 1);

                Image sectionButtonI = sfButtonG.AddComponent<Image>();
                sectionButtonI.color = new Color32(36, 36, 36, 255);
                sectionButtonI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
                sectionButtonI.type = Image.Type.Sliced;

                PreviewButton sectionButtonB = sfButtonG.AddComponent<PreviewButton>();
                sectionButtonB.tabIdle = new Color32(36, 36, 36, 255);
                sectionButtonB.tabActive = new Color32(255, 255, 255, 255);

                GameObject sectionButtonGT = new GameObject("Text");
                sectionButtonGT.transform.parent = sfButtonG.transform;

                Text sectionButtonT = sectionButtonGT.AddComponent<Text>();
                sectionButtonT.text = SavingManager.SaveData.Servers[i].localName;
                sectionButtonT.color = new Color32(255, 255, 255, 255);
                sectionButtonT.font = SavingManager.instance.font;
                sectionButtonT.resizeTextForBestFit = true;
                sectionButtonT.resizeTextMaxSize = 60;
                sectionButtonT.alignment = TextAnchor.MiddleCenter;

                sectionButtonGT.transform.localScale = new Vector3(1, 1, 1);
                buttons.Add(sectionButtonB);
            }
        }
        else
        {
            SaveData = new SaveDataStruct();
        }

        GameState = GameStateEnum.Menu;
    }

    void FixedUpdate()
    {
        if (GameState == GameStateEnum.Menu)
        {
            if (playButton == null) { Destroy(gameObject); }

            for (int i = 0; i < saveFileButtons.Count; i++)
            {
                if (saveFileButtons[i].onClicked)
                {
                    GameState = GameStateEnum.Singleplayer;
                    SaveFile = SaveData.SaveFiles[i];

                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                }
            }

            if (playButton.onClicked && !loadingServer)
            {
                loadingServer = true;

                SavedServerStruct server = new SavedServerStruct();
                server.localName = nameField.text;
                server.serverIp = serverField.text;
                server.official = false;

                int port = 26950;

                if (portField.text.Length > 0) { int.TryParse(portField.text, out port); }

                server.serverPort = port;
                SaveData.Servers.Add(server);

                SaveGame();

                StartCoroutine(LoadServer(serverField.text, 26950));
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].onClicked)
                {
                    ip = SaveData.Servers[i].serverIp;
                    port = SaveData.Servers[i].serverPort;

                    GameState = GameStateEnum.Multiplayer;
                    SceneManager.LoadScene(2, LoadSceneMode.Single);
                }
            }

            if (newSFButton == null) { newSFButton = GameObject.Find("Create Singleplayer Play Button").GetComponent<PreviewButton>(); }

            if (newSFButton.onClicked)
            {
                if (newSFName.text.Length < 1 || newSFName.text == null)
                {
                    PreviewInputManager.ShowError("New Save File can't be created without a name");
                }
                else
                {
                    GameState = GameStateEnum.Singleplayer;
                    SaveFile = new SaveFileStruct();
                    SaveFile.name = newSFName.text;

                    if (SaveData.milk_statue) { SaveFile.inventoryItems.Add(PreviewIAPManager.instance.iaps[0].item.name); }
                    if (SaveData.fragment_statue) { SaveFile.inventoryItems.Add(PreviewIAPManager.instance.iaps[1].item.name); }
                    if (SaveData.crystal_statue) { SaveFile.inventoryItems.Add(PreviewIAPManager.instance.iaps[2].item.name); }

                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                }
            }
        }
    }

    IEnumerator LoadServer(string _ip, int _port)
    {
        yield return new WaitForSeconds(0.6f);

        ip = _ip;
        port = _port;

        GameState = GameStateEnum.Multiplayer;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public static void SaveGame()
    {
        if (SaveGameCallback != null) { SaveGameCallback.Invoke(); }

        if (SaveFile != null)
        {
            bool found = false;
            for (int i = 0; i < SaveData.SaveFileNames.Count; i++) { if (SaveFile.name == SaveData.SaveFileNames[i]) { SaveData.SaveFiles[i] = SaveFile; found = true; } }
            if (!found) { SaveData.SaveFileNames.Add(SaveFile.name); SaveData.SaveFiles.Add(SaveFile); }
        }
        BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData_V2.savedata");
    }

    void OnApplicationPause() { if (SaveFile != null) { SaveGame(); } }
    void OnApplicationQuit() { if (SaveFile != null) { SaveGame(); } }
}

[System.Serializable]
public class StructureData
{
    public string name;

    public Vector2 coord;
    public Vector3 pos;
    public Quaternion rot;
}

[System.Serializable]
public class ChunkPropData
{
    public List<string> propName = new List<string>();
    public List<Vector3> position = new List<Vector3>();
    public List<Quaternion> rotation = new List<Quaternion>();
}

[System.Serializable]
public class StorageData
{
    public List<string> items = new List<string>();
}

[System.Serializable]
public class HeightMapData
{
    public Dictionary<Vector2, int> values = new Dictionary<Vector2, int>();
}

[System.Serializable]
public class SavedServerStruct
{
    public string localName;
    public string serverIp;
    public int serverPort;
    public bool official;
}

[System.Serializable]
public class SaveFileStruct
{
    public string name;

    public string firstSaveDate = DateTime.Now.ToString("dd-MM-yyyy");
    public string latestSaveDate = DateTime.Now.ToString("dd-MM-yyyy");

    public int seed = UnityEngine.Random.Range(0, 999999);
    public float funds = 0f;

    public List<string> inventoryItems = new List<string>();
    public List<StructureData> structures = new List<StructureData>();
    public List<float> vitals = new List<float>();
    public List<string> clickedAlerts = new List<string>();

    public Dictionary<Vector2, ChunkPropData> chunkData = new Dictionary<Vector2, ChunkPropData>();
    public Dictionary<Vector3, StorageData> storage = new Dictionary<Vector3, StorageData>();

    public Vector3 playerPos = new Vector3(0f, 0f, 0f);
    public bool finishedTutorial = true;
}

[System.Serializable]
public class SaveDataStruct
{
    public List<SavedServerStruct> Servers = new List<SavedServerStruct>();

    public bool milk_statue = false;
    public bool fragment_statue = false;
    public bool crystal_statue = false;

    public List<string> SaveFileNames = new List<string>();
    public List<SaveFileStruct> SaveFiles = new List<SaveFileStruct>();

    public float MainAudioLevel = 0f;
    public float SFAudioLevel = 0f;
    public float MusicAudioLevel = 0f;

    public int FPSLimit = 30;

    public bool aa = false;
    public bool hdr = false;
    public bool dr = false;

    public int vd = 1;
    public int pp = 2;
}
