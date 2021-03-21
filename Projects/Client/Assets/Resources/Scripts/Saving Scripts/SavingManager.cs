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
    public static SavingManager instance;

    public delegate void SaveGameDelegate();
    public static SaveGameDelegate SaveGameCallback;

    public static SaveFileStruct SaveFile;
    public static SaveDataStruct SaveData;

    public static string ip;
    public static int port;

    public static GameObject player;
    GameObject playerPrefab;

    string version = "v1.2.0-1_TEST";

    public enum GameStateEnum
    {
        None,
        Menu,
        Singleplayer,
        Multiplayer
    }
    static public GameStateEnum GameState;

    void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        instance = this;
    }

    void Start()
    {
        if (BinarySerializer.HasSaved("SurvivalBeforeDawn_CurrentSaveData_" + version + ".savedata"))
        {
            SaveData = BinarySerializer.Load<SaveDataStruct>("SurvivalBeforeDawn_CurrentSaveData_" + version + ".savedata");

            ProfileManager.instance.FirstTimeText.gameObject.SetActive(false);
            ProfileManager.instance.ReturnText.gameObject.SetActive(true);
            ProfileManager.instance.LegacyReturnText.gameObject.SetActive(false);
        }
        else if (BinarySerializer.HasSaved("SurvivalBeforeDawn_CurrentSaveData_V2.savedata") || BinarySerializer.HasSaved("SurvivalBeforeDawn_CurrentSaveData_V3.savedata"))
        {
            SaveData = new SaveDataStruct();

            ProfileManager.instance.FirstTimeText.gameObject.SetActive(false);
            ProfileManager.instance.ReturnText.gameObject.SetActive(false);
            ProfileManager.instance.LegacyReturnText.gameObject.SetActive(true);
        }
        else
        {
            SaveData = new SaveDataStruct();

            ProfileManager.instance.FirstTimeText.gameObject.SetActive(true);
            ProfileManager.instance.ReturnText.gameObject.SetActive(false);
            ProfileManager.instance.LegacyReturnText.gameObject.SetActive(false);
        }

        GameState = GameStateEnum.Menu;
    }

    public static void LoadSave(int saveId)
    {
        instance.playerPrefab = ProfileManager.instance.SkinData[SaveData.currentSkin].gameObject;

        GameState = GameStateEnum.Singleplayer;
        SaveFile = SaveData.SaveFiles[saveId];

        TransitionManager.ToSingleplayer();
    }

    public static void CreateSave(string name, string unParsedSeed, bool _LQGeneration)
    {
        instance.playerPrefab = ProfileManager.instance.SkinData[SaveData.currentSkin].gameObject;

        bool parsedSeed = int.TryParse(unParsedSeed, out int _seed);
        if (!parsedSeed) { _seed = UnityEngine.Random.Range(0, 9999999); }

        SaveFile = new SaveFileStruct
        {
            name = name,
            seed = _seed,
            LQGeneration = _LQGeneration,
            id = SaveData.SaveFiles.Count
        };

        GameState = GameStateEnum.Singleplayer;
        TransitionManager.ToSingleplayer();
    }

    public static void LoadServer(int serverId)
    {
        ip = SaveData.Servers[serverId].serverIp;
        port = SaveData.Servers[serverId].serverPort;

        GameState = GameStateEnum.Multiplayer;
        TransitionManager.ToMultiplayer();
    }

    public static void JoinServer(SavedServerStruct serverData)
    {
        SaveData.Servers.Add(serverData);

        ip = serverData.serverIp;
        port = serverData.serverPort;

        GameState = GameStateEnum.Multiplayer;
        TransitionManager.ToMultiplayer();
    }

    public static void SaveGame()
    {
        if (SaveFile != null)
        {
            try { SaveGameCallback.Invoke(); } catch { }

            for (int i = 0; i < SaveData.SaveFiles.Count; i++)
            {
                if (SaveData.SaveFiles[i].id == SaveFile.id)
                {
                    SaveData.SaveFiles[i] = SaveFile;
                    BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData_" + instance.version + ".savedata");
                    return;
                }
            }

            SaveData.SaveFiles.Add(SaveFile);
            BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData_" + instance.version + ".savedata");
        }
    }

    void OnApplicationPause()
    {
        if (GameState == GameStateEnum.Menu)
        {
            if (SaveData != null) { BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData_" + version + ".savedata"); }
        }
        else
        {
            SaveGame();
        }
    }

    void OnApplicationQuit()
    {
        if (GameState == GameStateEnum.Menu)
        {
            BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData_" + version + ".savedata");
        }
        else
        {
            SaveGame();
        }
    }

    public static void SetPlayer() { player = Instantiate(instance.playerPrefab, new Vector3(0f, 30f, 0f), Quaternion.identity); instance.StartCoroutine(EnablePlayerI()); }
    static IEnumerator EnablePlayerI() { player.SetActive(true); yield return new WaitForSeconds(3f); player.SetActive(true); }

    [Serializable]
    public class SaveFileStruct
    {
        public int id;
        public string name;

        public string firstSaveDate = DateTime.Now.ToString("dd-MM-yyyy");
        public string latestSaveDate = DateTime.Now.ToString("dd-MM-yyyy");

        public int seed = UnityEngine.Random.Range(0, 999999);
        public float funds = 0f;

        public List<string> inventoryItems = new List<string>();
        public List<StructureData> structures = new List<StructureData>();
        public List<float> vitals = new List<float>();

        public Dictionary<Vector2, ChunkData> Chunks = new Dictionary<Vector2, ChunkData>();
        public Dictionary<Vector3, StorageData> storage = new Dictionary<Vector3, StorageData>();

        public Vector3 playerPos = new Vector3(0f, 0f, 0f);
        public bool LQGeneration = false;
    }

    [Serializable]
    public class SaveDataStruct
    {
        public List<SavedServerStruct> Servers = new List<SavedServerStruct>();
        public List<SaveFileStruct> SaveFiles = new List<SaveFileStruct>();

        public bool milk_statue = false;
        public bool fragment_statue = false;
        public bool crystal_statue = false;

        public float MainAudioLevel = 0f;
        public float SFAudioLevel = 0f;
        public float MusicAudioLevel = 0f;

        public List<string> clickedAlerts = new List<string>();

        public bool completedProfile = false;
        public bool completedTutorial = true;
        public bool joinedMultiplayer = false;
        public bool completedSettings = false;

        public int currentSkin = 8;
        public string name = "";

        public GPlayDataStruct GPlayData = new GPlayDataStruct();
        public SettingsData SettingsData = new SettingsData();
    }

    [Serializable]
    public class GPlayDataStruct
    {
        public bool FirstTime = false;
        public bool Morning = false;
        public bool EarlyBird = false;
        public bool RIP = false;
        public bool RubyGem = false;
        public bool BuySkin = false;

        public int GameTime = 0;
        public bool OneHour = false;
        public bool FiveHour = false;

        public int LocalExploredChunks = 0;
        public int ExploredChunks = 0;

        public int LocalCraftedRubies = 0;
        public int CraftedRubies = 0;

        public int LocalDeaths = 0;
        public int Deaths = 0;
    }

    [Serializable]
    public class SettingsData
    {
        public int FPS = 30;
        public int RenderDistance = 80;

        public bool AA = true;
        public bool HDR = false;

        public bool Tonemapping = true;
        public bool DepthOfField = false;
        public bool MotionBlur = true;
        public bool Vignette = true;
        public bool Bloom = true;

        public float Sensitivity = 14;
    }
}

[Serializable]
public class ChunkData
{
    public List<PropData> Props = new List<PropData>();
}

[Serializable]
public class PropData
{
    public string Name;
    public Vector3 Position = new Vector3();
    public Vector3 Scale = new Vector3();
    public Quaternion Rotation = new Quaternion();
}

[Serializable]
public class StorageData
{
    public List<string> items = new List<string>();
}

[Serializable]
public class StructureData
{
    public string name;

    public Vector2 coord;
    public Vector3 pos;
    public Quaternion rot;
}

[Serializable]
public class SavedServerStruct
{
    public string localName;
    public string serverIp;
    public int serverPort;
    public bool official;
}
