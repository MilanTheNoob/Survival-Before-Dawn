using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    #region Singleton

    public static ToolsManager instance;
    void Awake() { instance = this; }

    #endregion

    [Header("Player tools")]
    public List<ToolsStruct> tools;
    public enum ToolType
    {
        None,
        Axe,
        Pickaxe,
        Shovel,
        Hammer,
        Pitchfork,
        Knife,
        Sword
    }

    [HideInInspector]
    public ToolType currentToolType;
    [HideInInspector]
    public ToolsStruct currentToolStruct;

    Animator anim;
    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

        currentToolType = ToolType.None;
        currentToolStruct = null;

        for (int i = 0; i < tools.Count; i++) { tools[i].toolsObject.SetActive(false); tools[i].beingUsed = false; }

        Inventory.instance.onItemChangedCallback += UpdateTools;
    }

    public void UpdateTools() { if (currentToolStruct != null && !Inventory.instance.items.Contains(currentToolStruct.scriptableObject)) { UnEquipTool(); } }

    public void EquipTool(ToolItemSettings requestedToolSettings)
    {
        for (int i = 0; i < tools.Count; i++)
        {
            if (tools[i].scriptableObject == requestedToolSettings)
            {
                tools[i].toolsObject.SetActive(true);

                currentToolType = tools[i].toolType;
                currentToolStruct = tools[i];
            }
            else { tools[i].toolsObject.SetActive(false); }
        }
    }

    public void UnEquipTool()
    {
        for (int i = 0; i < tools.Count; i++) { tools[i].toolsObject.SetActive(false); }

        currentToolType = ToolType.None;
        currentToolStruct = null;
    }

    public void SwingAnim() { anim.SetTrigger("Swing"); }

    [System.Serializable]
    public class ToolsStruct
    {
        public GameObject toolsObject;
        public ToolItemSettings scriptableObject;

        public ToolType toolType;

        [HideInInspector]
        public bool beingUsed;
    }
}