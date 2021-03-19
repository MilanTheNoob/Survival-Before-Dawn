using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Singleton

    public static TutorialManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    [Header("The Tutorial World")]
    public PreGenerationManager.WorldDataStruct tutWorld;
    [Header("The UI Components")]
    public InfoBoxStruct infoBox;
    public UIControlsStruct controlsStruct;
    public List<TutStruct> tutInfos;

    public void EnableTutInfoInt(int tutId)
    {
        TweeningLibrary.FadeIn(infoBox.infoParent, 0.2f);

        infoBox.descText.text = tutInfos[tutId].desc;
        infoBox.image.sprite = tutInfos[tutId].img;

        controlsStruct.MoveJoystick.SetActive(tutInfos[tutId].controlsChoice.MoveJoystick);
        controlsStruct.CameraJoystick.SetActive(tutInfos[tutId].controlsChoice.CameraJoystick);

        controlsStruct.JumpButton.SetActive(tutInfos[tutId].controlsChoice.JumpButton);
        controlsStruct.MenuButton.SetActive(tutInfos[tutId].controlsChoice.MoveJoystick);
    }

    public void TriggerI() { PreGenerationManager.instance.GenerateWorld(tutWorld); }
    public void TriggerII() { EnableTutInfoInt(0); }

    public void DisableTutInfo()
    {
        TweeningLibrary.FadeOut(infoBox.infoParent, 0.2f);

        infoBox.descText.text = "";
        infoBox.image.sprite = null;
    }

    public void CompleteTutorial()
    {
        PreGenerationManager.instance.ResetWorld();
        SavingManager.SaveData.completedTutorial = true;
    }

    [System.Serializable]
    public class InfoBoxStruct
    {
        public GameObject infoParent;
        public Text descText;
        public Image image;
    }

    [System.Serializable]
    public class TutStruct
    {
        public string title;
        public string desc;
        public Sprite img;

        [Header("What controls are enabled when this struct is used")]
        public ControlsChoiceStruct controlsChoice;
    }

    [System.Serializable]
    public class UIControlsStruct
    {
        public GameObject MoveJoystick;
        public GameObject CameraJoystick;
        public GameObject JumpButton;
        public GameObject MenuButton;
    }

    [System.Serializable]
    public class ControlsChoiceStruct
    {
        public bool MoveJoystick = true;
        public bool CameraJoystick = true;
        public bool JumpButton = true;
        public bool MenuButton = true;
    }
}