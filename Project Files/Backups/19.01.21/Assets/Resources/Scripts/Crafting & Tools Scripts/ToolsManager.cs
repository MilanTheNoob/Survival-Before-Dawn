using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    #region Singleton

    // The instance of ourselves
    public static ToolsManager instance;

    // Called before Start
    void Awake()
    {
        // Set the instance to ourselves
        instance = this;
    }

    #endregion

    [Header("The list of tools used")]
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
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the animator of the player
        anim = gameObject.GetComponent<Animator>();

        // Default all the variables
        currentToolType = ToolType.None;
        currentToolStruct = null;

        for (int i = 0; i < tools.Count; i++)
        {
            // Default all the tools
            tools[i].toolsObject.SetActive(false);
            tools[i].beingUsed = false;
        }

        // Add UpdateTools to inventory callback
        Inventory.instance.onItemChangedCallback += UpdateTools;
    }

    // Checks if the player dropped a tool he is using!
    public void UpdateTools()
    {
        if (currentToolStruct != null && !Inventory.instance.items.Contains(currentToolStruct.scriptableObject)) { UnEquipTool(); }
    }

    // Called to equip a certain tool
    public void EquipTool(ToolItemSettings requestedToolSettings)
    {
        // Loop through all the tools
        for (int i = 0; i < tools.Count; i++)
        {
            // If we have found the correct tool
            if (tools[i].scriptableObject == requestedToolSettings)
            {
                // Enable the tool and set it to the current tool
                tools[i].toolsObject.SetActive(true);

                // Set tool vars
                currentToolType = tools[i].toolType;
                currentToolStruct = tools[i];
            }
            else // If not
            {
                // Disable the tool
                tools[i].toolsObject.SetActive(false);
            }
        }
    }

    // Unequip a tool (or rather all of them)
    public void UnEquipTool()
    {
        // Loop through them all
        for (int i = 0; i < tools.Count; i++)
        {
            // Disable it   
            tools[i].toolsObject.SetActive(false);
        }

        // Set the current tool to nothing
        currentToolType = ToolType.None;
        currentToolStruct = null;
    }

    // Swing anim funcs
    public void SwingAnim() { anim.SetBool("Swing", true); StartCoroutine(SwingEnd()); }
    IEnumerator SwingEnd() { yield return 0; anim.SetBool("Swing", false); }

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