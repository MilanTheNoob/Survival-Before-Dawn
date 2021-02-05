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

    [Header("Layer for building")]
    public LayerMask layer;
    public int layerInt;

    [Header("Stick tolerance")]
    public float tolerance = 1.5f;

    List<Vector3> placedStructures = new List<Vector3>();

    Camera cam;

    bool isBuilding;
    bool pauseBuilding;
    bool oldSnapped;
    bool oldBuilding;

    GameObject previewG;
    BuildPreview previewS;
    ItemSettings previewI;

    void Start()
    {
        cam = FindObjectOfType<Camera>();

        buildButton.SetActive(false);
        cancelButton.SetActive(false);
        rotateButton.SetActive(false);

        LoadData();
    }

    void Update()
    {
        if (isBuilding)
        {
            if (isBuilding != oldBuilding) { TweeningLibrary.FadeIn(cancelButton, 0.1f); TweeningLibrary.FadeIn(rotateButton, 0.1f); oldBuilding = isBuilding; }

            if (previewS.GetSnapped() != oldSnapped)
            {
                if (previewS.GetSnapped() == true) { TweeningLibrary.FadeIn(buildButton, 0.1f); } else { TweeningLibrary.FadeOut(buildButton, 0.1f); }
                oldSnapped = previewS.GetSnapped();
            }

            if (pauseBuilding)
            {
                if (Mathf.Abs(InputManager.MouseX) >= tolerance || Mathf.Abs(InputManager.MouseY) >= tolerance) { pauseBuilding = false; }
            }
            else { BuildRay(); }
        }
    }

    public bool NewBuild(ItemSettings i)
    {
        if (isBuilding) { return false; }

        previewG = Instantiate(i.gameObject, Vector3.zero, i.gameObject.transform.rotation);
        previewS = previewG.GetComponent<BuildPreview>();
        previewI = i;
        previewG.layer = layerInt;

        isBuilding = true;
        AudioManager.PlayBuild();

        return true;
    }

    public void RotateBuild()
    {
        if (!isBuilding) { return; }
        previewG.transform.eulerAngles = new Vector3(previewG.transform.eulerAngles.x, previewG.transform.eulerAngles.y + 90f, previewG.transform.eulerAngles.z);
    }

    public void CancelBuild()
    {
        Destroy(previewG);

        previewG = null;
        previewS = null;
        previewI = null;

        isBuilding = false;
        oldBuilding = false;

        AudioManager.PlayBuild();

        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    public void FinishBuild()
    {
        if (!previewS.GetSnapped()) { return; }
        previewS.Place();

        StructureData sd = new StructureData
        {
            name = previewI.name,
            pos = previewG.transform.position,
            rot = previewG.transform.rotation
        };
        placedStructures.Add(previewG.transform.position);
        SavingManager.SaveFile.structures.Add(sd);

        Inventory.instance.Destroy(previewI);

        previewG = null;
        previewS = null;
        previewI = null;

        isBuilding = false;
        oldBuilding = false;

        AudioManager.PlayBuild();

        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    public static void PauseBuild(bool v) { instance.pauseBuilding = v; }

    void BuildRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 20f, ~layer)) { previewG.transform.position = hit.point; }
    }

    public void LoadData()
    {
        for (int i = 0; i < SavingManager.SaveFile.structures.Count; i++)
        {           
            if (!placedStructures.Contains(SavingManager.SaveFile.structures[i].pos) && TerrainGenerator.CanAddToNearestChunk(SavingManager.SaveFile.structures[i].pos))
            {
                StructureItemSettings si = Resources.Load<StructureItemSettings>("Prefabs/Interactable Items/" + SavingManager.SaveFile.structures[i].name);
                GameObject g = Instantiate(si.gameObject, SavingManager.SaveFile.structures[i].pos, SavingManager.SaveFile.structures[i].rot);
                g.GetComponent<BuildPreview>().Place();
                g.layer = layerInt;

                placedStructures.Add(SavingManager.SaveFile.structures[i].pos);
            }
            
        }
    }
}
