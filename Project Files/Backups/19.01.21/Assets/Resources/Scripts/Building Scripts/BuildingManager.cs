using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    #region Singleton

    // The instance of ourselves
    public static BuildingManager instance;

    // Called before Start
    private void Awake()
    {
        // Set the instance to ourselves
        instance = this;
    }

    #endregion

    [Header("UI Vars")]
    public GameObject buildButton;
    public GameObject cancelButton;
    public GameObject rotateButton;

    [Header("The layer for building")]
    public LayerMask layer;
    public int layerInt;

    [Header("How much does a structure peice stick to another one?")]
    public float tolerance = 1.5f;

    List<int> placedStructures = new List<int>();

    Camera cam;

    bool isBuilding;
    bool pauseBuilding;
    bool oldSnapped;
    bool oldBuilding;

    GameObject previewG;
    BuildPreview previewS;
    ItemSettings previewI;

    // Called at the beginning of the game
    void Start()
    {
        // Get the player camera
        cam = FindObjectOfType<Camera>();

        // Disable all the building UI
        buildButton.SetActive(false);
        cancelButton.SetActive(false);
        rotateButton.SetActive(false);

        // Load any building save data if called
        LoadData();
        TerrainGenerator.instance.ChunksAddedCallback += LoadData;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we are in building mode
        if (isBuilding)
        {
            // Fade in most of the UI
            if (isBuilding != oldBuilding) { TweeningLibrary.FadeIn(cancelButton, 0.1f); TweeningLibrary.FadeIn(rotateButton, 0.1f); oldBuilding = isBuilding; }

            // If the snap status of the structure is different then fade in the build button
            if (previewS.GetSnapped() != oldSnapped)
            {
                if (previewS.GetSnapped() == true) { TweeningLibrary.FadeIn(buildButton, 0.1f); } else { TweeningLibrary.FadeOut(buildButton, 0.1f); }
                oldSnapped = previewS.GetSnapped();
            }

            // If the building has been paused then check if the player has moved enough to unpause it
            if (pauseBuilding)
            {
                if (Mathf.Abs(InputManager.MouseX) >= tolerance || Mathf.Abs(InputManager.MouseY) >= tolerance) { pauseBuilding = false; }
            }
            else { BuildRay(); /* If not then start moving the structure elswhere */ }
        }
    }

    // Called to start a new build
    public bool NewBuild(ItemSettings i)
    {
        // Check if we are already building
        if (isBuilding)
            return false;

        // Instantiate a new structure and set it up
        previewG = Instantiate(i.gameObject, Vector3.zero, Quaternion.identity);
        previewS = previewG.GetComponent<BuildPreview>();
        previewI = i;
        previewG.layer = layerInt;

        // And we buildin now bois!
        isBuilding = true;

        // Make audio
        AudioManager.PlayBuild();

        // Return true
        return true;
    }

    // Rotate the structure
    public void RotateBuild()
    {
        // Check we are actually building
        if (!isBuilding)
            return;

        // Rotate the structure 90 degrees
        previewG.transform.eulerAngles = new Vector3(previewG.transform.eulerAngles.x, previewG.transform.eulerAngles.y + 90f, previewG.transform.eulerAngles.z);
    }

    // Called to cancel the build
    public void CancelBuild()
    {
        // Destroy the structure
        Destroy(previewG);

        // Reset al the vars
        previewG = null;
        previewS = null;
        previewI = null;

        isBuilding = false;
        oldBuilding = false;

        // Make audio
        AudioManager.PlayBuild();

        // Fade ou all the building UI
        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    // Called to finish the build (or place it)
    public void FinishBuild()
    {
        // Check if the structure is snapped
        if (!previewS.GetSnapped())
            return;

        // Reset the layer of the structure and place it
        previewG.layer = 0;
        previewS.Place();

        // Create a new structure data with values
        SavingManager.StructureData sd = new SavingManager.StructureData
        {
            name = previewI.name,
            pos = previewG.transform.position,
            rot = previewG.transform.rotation
        };
        // Add to the placed structures list
        placedStructures.Add(SavingManager.SaveFile.structures.Count);

        // Add the structure data to the structures list
        SavingManager.SaveFile.structures.Add(sd);

        // Remove the structure item from the player's inventory
        Inventory.instance.Destroy(previewI);

        // Reset all the vars
        previewG = null;
        previewS = null;
        previewI = null;

        isBuilding = false;
        oldBuilding = false;

        // Make audio
        AudioManager.PlayBuild();

        // Fade out all the building UI
        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    // Called to change the pauseBuilding var
    public static void PauseBuild(bool v) { instance.pauseBuilding = v; }

    // Moves the structure in front of the player with a raycast
    void BuildRay()
    {
        // Create a new ray output var
        RaycastHit hit;
        // Send out a raycast and move the structure properly
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 20f, ~layer)) { previewG.transform.position = hit.point; }
    }

    // Loads save data
    public void LoadData()
    {
        // Get the list of saved structures and cache it
        List<SavingManager.StructureData> saveData = SavingManager.SaveFile.structures;

        // Check if there are any structures in the save file
        if (saveData.Count > 0)
        {
            // Loop through them all
            for (int i = 0; i < SavingManager.SaveFile.structures.Count; i++)
            {
                if (!placedStructures.Contains(i) && TerrainGenerator.CanAddToNearestChunk(saveData[i].pos))
                {
                    // Spawn & place the structure in the world
                    StructureItemSettings si = Resources.Load<StructureItemSettings>("Prefabs/Interactable Items/" + saveData[i].name);
                    GameObject g = Instantiate(si.gameObject, saveData[i].pos, saveData[i].rot);
                    g.GetComponent<BuildPreview>().Place();

                    // Add the structure id to the placed structures
                    placedStructures.Add(i);
                }
            }
        }
    }

    // Waits a fram and loads data
    public IEnumerator WaitLoadDataI()
    {
        // Wait a frame
        yield return new WaitForSeconds(1f);
        // Load Data
        LoadData();
    }
}
