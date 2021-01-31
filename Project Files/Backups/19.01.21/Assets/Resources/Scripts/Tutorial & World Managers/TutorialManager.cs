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

    // Called to enable the tutorial dialogue by name
    public void EnableTutInfo(string tutName)
    {
        // Make the UI parent active
        infoBox.infoParent.SetActive(true);

        // Loop through all the tutorial datas
        for (int i = 0; i < tutInfos.Count; i++)
        {
            if (tutInfos[i].title == tutName)
            {
                // Set the texts & image
                infoBox.titleText.text = tutInfos[i].title;
                infoBox.descText.text = tutInfos[i].desc;
                infoBox.image.sprite = tutInfos[i].img;

                // Toggle the buttons depending on tut info settings
                controlsStruct.MoveJoystick.SetActive(tutInfos[i].controlsChoice.MoveJoystick);
                controlsStruct.CameraJoystick.SetActive(tutInfos[i].controlsChoice.CameraJoystick);
                controlsStruct.JumpButton.SetActive(tutInfos[i].controlsChoice.JumpButton);
                controlsStruct.MenuButton.SetActive(tutInfos[i].controlsChoice.MoveJoystick);
            }
        }
    }

    // Called to enable the tutorial dialogue by id
    public void EnableTutInfoInt(int tutId)
    {
        print("hi");
        // Make the UI parent active
        TweeningLibrary.FadeIn(infoBox.infoParent, 0.2f);

        // Set the texts & image
        infoBox.titleText.text = tutInfos[tutId].title;
        infoBox.descText.text = tutInfos[tutId].desc;
        infoBox.image.sprite = tutInfos[tutId].img;

        // Toggle the buttons depending on tut info settings
        controlsStruct.MoveJoystick.SetActive(tutInfos[tutId].controlsChoice.MoveJoystick);
        controlsStruct.CameraJoystick.SetActive(tutInfos[tutId].controlsChoice.CameraJoystick);

        controlsStruct.JumpButton.SetActive(tutInfos[tutId].controlsChoice.JumpButton);
        controlsStruct.MenuButton.SetActive(tutInfos[tutId].controlsChoice.MoveJoystick);
    }

    // Some trigger functions
    public void TriggerI() { PreGenerationManager.instance.GenerateWorld(tutWorld); }
    public void TriggerII() { EnableTutInfoInt(0); }

    // Called to disable the tut info
    public void DisableTutInfo()
    {
        // Disable the UI parent
        TweeningLibrary.FadeOut(infoBox.infoParent, 0.2f);

        // Reset all the texts & images
        infoBox.titleText.text = "";
        infoBox.descText.text = "";
        infoBox.image.sprite = null;
    }

    // Func to complete the tutorial
    public void CompleteTutorial()
    {
        // Unrender the tut world
        PreGenerationManager.instance.ResetWorld();
        // Modify the save files saying the player finished the tut
        SavingManager.SaveFile.finishedTutorial = true;
    }

    [System.Serializable]
    public class InfoBoxStruct
    {
        public GameObject infoParent;
        public Text titleText;
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