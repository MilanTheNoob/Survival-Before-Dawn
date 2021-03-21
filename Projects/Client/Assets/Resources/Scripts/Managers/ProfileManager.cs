using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Deals with the profile scene, mainly UI
/// </summary>
public class ProfileManager : MonoBehaviour
{
    #region Singleton

    public static ProfileManager instance;
    void Awake() { instance = this; }

    #endregion

    [Header("UI Components")]
    public GameObject SaveFileList;

    [Space]

    public Toggle LQGeneration;
    public InputField SaveSeed;
    public InputField SaveName;

    [Space]

    public GameObject ServerList;
    public InputField ServerName;
    public InputField ServerIp;
    public InputField ServerPort;

    [Space]

    public Text FirstTimeText;
    public Text ReturnText;
    public Text LegacyReturnText;

    [Space]

    public Button SkinSwapLeft;
    public Button SkinSwapRight;
    public Text SkinName;

    [Space]

    public Font BoldFont;
    public Font NormalFont;

    [Space]

    public Animator ProfileCam;

    [Space]

    public Button MultiplayerAccess;
    public Text MultiplayerAccessT;

    [Space]

    public Image ProfileImage;
    public Text ProfileName;
    public Text PSetupName;

    [Header("Skin Data")]
    public List<SkinDataStruct> SkinData;

    List<Button> SaveFileOptions = new List<Button>();
    List<Button> ServerOptions = new List<Button>();

    bool dontSwap;

    void Start()
    {
        PreviewInputManager.instance.ToggleUISectionsInt(6);
        PreviewSkin(SavingManager.SaveData.currentSkin);

        for (int i = 0; i < SaveFileList.transform.childCount; i++) { Destroy(SaveFileList.transform.GetChild(i).gameObject); }
        for (int i = 0; i < ServerList.transform.childCount; i++) { Destroy(ServerList.transform.GetChild(i).gameObject); }

        // Add all the save files & multiplayer
        for (int i = 0; i < SavingManager.SaveData.SaveFiles.Count; i++)
        {
            GameObject OptionG = new GameObject(SavingManager.SaveData.SaveFiles[i].name);
            OptionG.transform.parent = SaveFileList.transform;
            OptionG.AddComponent<VerticalLayoutGroup>();

            Image OptionI = OptionG.AddComponent<Image>();
            OptionI.color = new Color32(36, 36, 36, 255);
            OptionI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            OptionI.type = Image.Type.Sliced;

            Button OptionB = OptionG.AddComponent<Button>();
            OptionB.tabIdle = new Color32(36, 36, 36, 255);
            OptionB.tabActive = new Color32(255, 255, 255, 255);

            GameObject OptionNameG = new GameObject("Name");
            OptionNameG.transform.parent = OptionG.transform;
            OptionNameG.transform.localScale = Vector3.one;

            Text OptionNameT = OptionNameG.AddComponent<Text>();
            OptionNameT.text = SavingManager.SaveData.SaveFiles[i].name;
            OptionNameT.color = new Color32(255, 255, 255, 255);
            OptionNameT.font = BoldFont;
            OptionNameT.resizeTextForBestFit = true;
            OptionNameT.resizeTextMaxSize = 60;
            OptionNameT.alignment = TextAnchor.MiddleCenter;

            GameObject OptionDateG = new GameObject("Date");
            OptionDateG.transform.parent = OptionG.transform;
            OptionDateG.transform.localScale = Vector3.one;

            Text OptionDateT = OptionDateG.AddComponent<Text>();
            OptionDateT.text = SavingManager.SaveData.SaveFiles[i].latestSaveDate;
            OptionDateT.color = new Color32(255, 255, 255, 255);
            OptionDateT.font = NormalFont;
            OptionDateT.resizeTextForBestFit = true;
            OptionDateT.resizeTextMaxSize = 50;
            OptionDateT.alignment = TextAnchor.MiddleCenter;

            RectTransform OptionRT = OptionG.GetComponent<RectTransform>();
            OptionRT.sizeDelta = new Vector2(OptionRT.sizeDelta.x, 200);
            OptionG.transform.localScale = Vector3.one;

            SaveFileOptions.Add(OptionB);
        }

        for (int i = 0; i < SavingManager.SaveData.Servers.Count; i++)
        {
            GameObject ServerG = new GameObject(SavingManager.SaveData.Servers[i].localName);
            ServerG.transform.parent = ServerList.transform;
            ServerG.AddComponent<VerticalLayoutGroup>();

            Image ServerI = ServerG.AddComponent<Image>();
            ServerI.color = new Color32(36, 36, 36, 255);
            ServerI.sprite = Resources.Load<Sprite>("UI/Basic UI Shapes/100px Rounded Square Mild");
            ServerI.type = Image.Type.Sliced;

            Button ServerB = ServerG.AddComponent<Button>();
            ServerB.tabIdle = new Color32(36, 36, 36, 255);
            ServerB.tabActive = new Color32(255, 255, 255, 255);

            GameObject ServerNameG = new GameObject("Name");
            ServerNameG.transform.parent = ServerG.transform;
            ServerNameG.transform.localScale = Vector3.one;

            Text ServerNameT = ServerNameG.AddComponent<Text>();
            ServerNameT.text = SavingManager.SaveData.Servers[i].localName;
            ServerNameT.color = new Color32(255, 255, 255, 255);
            ServerNameT.font = BoldFont;
            ServerNameT.resizeTextForBestFit = true;
            ServerNameT.resizeTextMaxSize = 60;
            ServerNameT.alignment = TextAnchor.MiddleCenter;

            GameObject ServerIpG = new GameObject("IP");
            ServerIpG.transform.parent = ServerG.transform;
            ServerIpG.transform.localScale = Vector3.one;

            Text ServerIpT = ServerIpG.AddComponent<Text>();
            ServerIpT.text = SavingManager.SaveData.Servers[i].serverIp + ":" + SavingManager.SaveData.Servers[i].serverPort;
            ServerIpT.color = new Color32(255, 255, 255, 255);
            ServerIpT.font = NormalFont;
            ServerIpT.resizeTextForBestFit = true;
            ServerIpT.resizeTextMaxSize = 50;
            ServerIpT.alignment = TextAnchor.MiddleCenter;

            RectTransform OptionRT = ServerG.GetComponent<RectTransform>();
            OptionRT.sizeDelta = new Vector2(OptionRT.sizeDelta.x, 200);
            ServerG.transform.localScale = Vector3.one;

            ServerOptions.Add(ServerB);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < ServerOptions.Count; i++) { if (ServerOptions[i].onClicked) { SavingManager.LoadServer(i); } }
        for (int i = 0; i < SaveFileOptions.Count; i++) { if (SaveFileOptions[i].onClicked) { SavingManager.LoadSave(i); } }

        if (SkinSwapLeft.onClicked) { if (SavingManager.SaveData.currentSkin > 0) { PreviewSkin(SavingManager.SaveData.currentSkin - 1); } }
        if (SkinSwapRight.onClicked) { if (SavingManager.SaveData.currentSkin < SkinData.Count - 1) { PreviewSkin(SavingManager.SaveData.currentSkin + 1); } }
    }

    /// <summary>
    /// Moves from the welcome page to the menu or profile
    /// </summary>
    public void FinishWelcome()
    {
        if (SavingManager.SaveData.completedProfile)
        {
            ProfileCam.SetTrigger("ToMenu");
            PreviewInputManager.instance.ToggleUISectionsInt(0);
        }
        else
        {
            ProfileCam.SetTrigger("ToProfile");
            PreviewInputManager.instance.ToggleUISectionsInt(2);
        }
    }

    /// <summary>
    /// Creates a save file from current input fields
    /// </summary>
    public void CreateSave() { SavingManager.CreateSave(SaveName.text, SaveSeed.text, LQGeneration.isOn); }
    /// <summary>
    /// Joins a new server based of input fields
    /// </summary>
    public void JoinServer()
    {
        bool parsedPort = int.TryParse(ServerPort.text, out int port);
        if (!parsedPort) { port = 26950; }

        SavedServerStruct server = new SavedServerStruct
        {
            serverIp = ServerIp.text,
            serverPort = port,
            localName = ServerName.text
        };

        SavingManager.JoinServer(server);
    }

    /// <summary>
    /// Previews a skin
    /// </summary>
    /// <param name="id">The id of the skin</param>
    public void PreviewSkin(int id)
    {
        if (!dontSwap)
        {
            SkinName.text = SkinData[id].skinName;
            for (int i = 0; i < SkinData.Count; i++) { if (id == i) { SkinData[i].profilePreview.SetActive(true); } else { SkinData[i].profilePreview.SetActive(false); } }
            SavingManager.SaveData.currentSkin = id;

            dontSwap = true;
            StartCoroutine(ResetSwapI());
        }
    }

    IEnumerator ResetSwapI()
    {
        yield return new WaitForSeconds(0.2f);
        dontSwap = false;
    }

    /// <summary>
    /// Finish profile setup
    /// </summary>
    public void FinishProfile()
    {
        ProfileCam.SetTrigger("ToMenu");
        PreviewInputManager.instance.ToggleUISectionsInt(0);

        SavingManager.SaveData.name = ProfileName.text;
        SavingManager.SaveData.completedProfile = true;
    }

    [System.Serializable]
    public class SkinDataStruct
    {
        public GameObject gameObject;
        public GameObject profilePreview;
        public string skinName;
    }
}
