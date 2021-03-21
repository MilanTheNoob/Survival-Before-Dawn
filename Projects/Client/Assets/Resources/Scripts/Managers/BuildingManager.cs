using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    #region Singleton

    public static BuildingManager instance;
    void Awake() { instance = this; }

    #endregion

    [Header("UI Vars")]
    public GameObject buildButton;
    public GameObject cancelButton;
    public GameObject rotateButton;

    [Header("Stick tolerance")]
    public float tolerance = 0f;

    List<Vector3> placedStructures = new List<Vector3>();

    public Camera cam;

    bool isBuilding;
    bool pauseBuilding;
    bool oldSnapped;
    bool oldBuilding;

    GameObject previewG;
    BuildPreview previewS;
    ItemSettings previewI;

    GameObject dropG;
    ItemSettings dropI;

    #region Unity Funcs

    void Start()
    {
        buildButton.SetActive(false);
        cancelButton.SetActive(false);
        rotateButton.SetActive(false);

        if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer) { LoadData(); }
    }

    void FixedUpdate()
    {
        if (cam == null) { cam = FindObjectOfType<Camera>(); }

        if (isBuilding)
        {
            if (dropG == null)
            {
                if (isBuilding != oldBuilding)
                {
                    TweeningLibrary.FadeIn(cancelButton, 0.1f);
                    TweeningLibrary.FadeIn(rotateButton, 0.1f);
                    oldBuilding = isBuilding;

                    if (previewS.GetSnapped()) { TweeningLibrary.FadeIn(buildButton, 0.1f); }
                }

                if (previewS.GetSnapped() != oldSnapped)
                {
                    if (previewS.GetSnapped() == true) { TweeningLibrary.FadeIn(buildButton, 0.1f); } else { TweeningLibrary.FadeOut(buildButton, 0.1f); }
                    oldSnapped = previewS.GetSnapped();
                }

                if (pauseBuilding)
                {
                    if (InputManager.instance != null)
                    {
                        if (Mathf.Abs(InputManager.MouseX) >= tolerance || Mathf.Abs(InputManager.MouseY) >= tolerance) { pauseBuilding = false; }
                    }
                    else
                    {
                        if (Mathf.Abs(MultiplayerInputManager.MouseX) >= tolerance || Mathf.Abs(MultiplayerInputManager.MouseY) >= tolerance) { pauseBuilding = false; }
                    }
                }
                else { BuildRay(); }
            }

            if (dropG != null) { BuildRay(); }

            if (dropG != null) { dropG.layer = 10; } 
        }
    }

    #endregion

    #region Basic Building Funcs

    public bool NewBuild(ItemSettings i)
    {
        if (isBuilding) { return false; }

        previewG = Instantiate(i.gameObject, Vector3.zero, i.gameObject.transform.rotation);
        previewG.name = i.name;
        previewS = previewG.GetComponent<BuildPreview>();
        previewI = i;
        previewG.layer = 10;

        isBuilding = true;
        AudioManager.PlayBuild();

        return true;
    }

    public void RotateBuild()
    {
        if (!isBuilding) { return; }

        if (previewG != null)
        {
            previewG.transform.eulerAngles = new Vector3(previewG.transform.eulerAngles.x, previewG.transform.eulerAngles.y + 90f, previewG.transform.eulerAngles.z);
        }
        else
        {
            dropG.transform.eulerAngles = new Vector3(dropG.transform.eulerAngles.x, dropG.transform.eulerAngles.y + 90f, dropG.transform.eulerAngles.z);
        }
    }

    public void CancelBuild()
    {
        if (previewG != null)
        {
            Destroy(previewG);

            previewG = null;
            previewS = null;
            previewI = null;
        }
        else
        {
            Destroy(dropG);

            dropG = null;
            dropI = null;
        }

        isBuilding = false;
        oldBuilding = false;

        AudioManager.PlayBuild();

        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    #endregion

    public void FinishBuild()
    {
        if (previewG != null)
        {
            if (!previewS.GetSnapped()) { return; }

            if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
            {
                StructureData sd = new StructureData
                {
                    name = previewI.name,
                    pos = previewG.transform.position,
                    rot = previewG.transform.rotation
                };
                placedStructures.Add(previewG.transform.position);
                SavingManager.SaveFile.structures.Add(sd);

                Inventory.instance.Destroy(previewI);
            }
            else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
            {
                ClientSend.AddStructure(previewG);
                Destroy(previewG);
            }

            previewG = null;
            previewS = null;
            previewI = null;
        }
        else
        {
            if (SavingManager.GameState == SavingManager.GameStateEnum.Singleplayer)
            {
                PropData propData = new PropData
                {
                    Name = dropI.gameObject.name,
                    Position = dropG.transform.position,
                    Rotation = dropG.transform.rotation,
                    Scale = dropG.transform.localScale
                };
                SavingManager.SaveFile.Chunks[TerrainGenerator.GetNearestChunk(propData.Position)].Props.Add(propData);

                Inventory.instance.Destroy(dropI);
                dropG.layer = 9;
                dropG.GetComponent<Collider>().enabled = true;

                if (!dropI.ignoreGravity) { dropG.AddComponent<Rigidbody>(); }
                TerrainGenerator.AddToNearestChunk(dropG, TerrainGenerator.ChildType.Prop);
            }
            else if (SavingManager.GameState == SavingManager.GameStateEnum.Multiplayer)
            {
                // TODO: Add multiplayer support
            }

            dropG = null;
            dropI = null;
        }

        isBuilding = false;
        oldBuilding = false;

        AudioManager.PlayBuild();

        TweeningLibrary.FadeOut(buildButton, 0.1f);
        TweeningLibrary.FadeOut(cancelButton, 0.1f);
        TweeningLibrary.FadeOut(rotateButton, 0.1f);
    }

    public bool StartDropItem(ItemSettings i)
    {
        if (isBuilding) { return false; }

        InputManager.instance.ToggleUISectionsInt(0);
        TweeningLibrary.FadeIn(buildButton, 0.1f);
        TweeningLibrary.FadeIn(cancelButton, 0.1f);
        TweeningLibrary.FadeIn(rotateButton, 0.1f);

        dropG = Instantiate(i.gameObject);
        dropG.layer = 10;
        dropG.GetComponent<Collider>().enabled = false;
        dropG.transform.eulerAngles = Vector3.zero;
        dropG.name = i.gameObject.name;
        dropI = i;

        isBuilding = true;

        return true;
    }

    public static void PauseBuild(bool v) { instance.pauseBuilding = v; }

    void BuildRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 20f, ~10)) 
        { 
            if (previewG != null)
            {
                previewG.transform.position = hit.point;
            }
            else
            {
                dropG.transform.position = hit.point;
            }
        }
    }

    #region Loading

    public void LoadData()
    {
        for (int i = 0; i < SavingManager.SaveFile.structures.Count; i++)
        {           
            if (!placedStructures.Contains(SavingManager.SaveFile.structures[i].pos) && TerrainGenerator.CanAddToNearestChunk(SavingManager.SaveFile.structures[i].pos))
            {
                StructureItemSettings si = Resources.Load<StructureItemSettings>("Prefabs/Interactable Items/" + SavingManager.SaveFile.structures[i].name);
                GameObject g = Instantiate(si.gameObject, SavingManager.SaveFile.structures[i].pos, SavingManager.SaveFile.structures[i].rot);
                g.GetComponent<BuildPreview>().Place();

                placedStructures.Add(SavingManager.SaveFile.structures[i].pos);
            }
            
        }
    }

    #endregion
}
