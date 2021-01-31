using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    [Header("UI components in the profile scene")]
    public GameObject saveFilesView;

    [Space]

    public InputField newSFName;
    public PreviewButton newSFButton;

    [Space]

    public Font font;

    public static SavingManager instance;

    public delegate void SaveGameDelegate();
    public static SaveGameDelegate SaveGameCallback;

    public static SaveFileStruct SaveFile;
    public static SaveDataStruct SaveData;

    public static string ip;
    public static int port;

    static readonly int saveVersion = 1;

    List<PreviewButton> saveFileButtons = new List<PreviewButton>();

    static TcpClient socket;

    static NetworkStream stream;
    static Packet receivedData;
    static byte[] receiveBuffer;

    static int dataBufferSize = 4096;

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

        if (BinarySerializer.HasSaved("SurvivalBeforeDawn_CurrentSaveData.savedata"))
        {
            SaveData = BinarySerializer.Load<SaveDataStruct>("SurvivalBeforeDawn_CurrentSaveData.savedata");

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
        }
        else
        {
            SaveData = new SaveDataStruct();
        }

        GameState = GameStateEnum.Menu;
    }

    private void Update()
    {
        if (GameState == GameStateEnum.Menu)
        {
            for (int i = 0; i < saveFileButtons.Count; i++)
            {
                if (saveFileButtons[i].onClicked)
                {
                    GameState = GameStateEnum.Singleplayer;
                    SaveFile = SaveData.SaveFiles[i];

                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                }
            }

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

    #region Multiplayer Code

    public static void LoadServer(string _ip, int _port)
    {
        ip = _ip;
        port = _port;

        print("load server called");

        socket = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };

        receiveBuffer = new byte[dataBufferSize];
        socket.BeginConnect(ip, port, instance.ConnectCallback, socket);
    }

    void ConnectCallback(IAsyncResult _result)
    {
        print("connect callback called");

        socket.EndConnect(_result);

        if (!socket.Connected)
            return;

        stream = socket.GetStream();
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        receivedData = new Packet();
    }

    void ReceiveCallback(IAsyncResult _result)
    {
        print("receive callback called");

            int _byteLength = stream.EndRead(_result);
            if (_byteLength <= 0) { return; }

            byte[] _data = new byte[_byteLength];
            Array.Copy(receiveBuffer, _data, _byteLength);

            HandleData(_data);
    }

    private void HandleData(byte[] _data)
    {
        print("handle data called");
        int _packetLength = 0;
        receivedData.SetBytes(_data);

        if (receivedData.UnreadLength() >= 4)
        {
            _packetLength = receivedData.ReadInt();
            if (_packetLength <= 0) { return; }
        }

        while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
        {
            byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using Packet _packet = new Packet(_packetBytes);
                int _packetId = _packet.ReadInt();
                if (_packetId <= 1) { FinishLoadServer(); }
            });

            _packetLength = 0;
            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) { return; }
            }
        }
    }

    void FinishLoadServer()
    {
        GameState = GameStateEnum.Multiplayer;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    #endregion

    public static void SaveGame()
    {
        SaveGameCallback.Invoke();

        bool found = false;
        for (int i = 0; i < SaveData.SaveFileNames.Count; i++)
        {
            if (SaveFile.name == SaveData.SaveFileNames[i]) { SaveData.SaveFiles[i] = SaveFile; found = true; }
        }

        if (!found) { SaveData.SaveFileNames.Add(SaveFile.name); SaveData.SaveFiles.Add(SaveFile); }
        BinarySerializer.Save(SaveData, "SurvivalBeforeDawn_CurrentSaveData.savedata");
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
    public class SaveFileStruct
    {
        public string name;

        public string firstSaveDate = System.DateTime.Now.ToString("dd-MM-yyyy");
        public string latestSaveDate = System.DateTime.Now.ToString("dd-MM-yyyy");

        public int seed = UnityEngine.Random.Range(0, 999999);
        public float funds = 0f;

        public List<string> inventoryItems = new List<string>();
        public List<StructureData> structures = new List<StructureData>();
        public List<float> vitals = new List<float>();
        public List<string> clickedAlerts = new List<string>();

        public Dictionary<Vector2, ChunkPropData> chunkData = new Dictionary<Vector2, ChunkPropData>();
        public Dictionary<Vector3, StorageData> storage = new Dictionary<Vector3, StorageData>();

        public Vector3 playerPos;

        public bool finishedTutorial = false;

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

    [System.Serializable]
    public class SaveDataStruct
    {
        public int version = saveVersion;

        public bool milk_statue = false;
        public bool fragment_statue = false;
        public bool crystal_statue = false;

        public List<string> SaveFileNames = new List<string>();
        public List<SaveFileStruct> SaveFiles = new List<SaveFileStruct>();
    }
}
