using System.Collections;
using UnityEditor;
using UnityEngine;

public class ItemSettingCreationWindows : EditorWindow
{
    #region Singleton

    public static ItemSettingCreationWindows instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    string name;
    string longerName;
    string projName;
    string parentName;
    GameObject g;
    string desc;
    int sellValue;
    string modelType;
    GameObject ga;
    GameObject model;

    [MenuItem("Editor Windows/ItemSetting Windows/Standard")]
    public static void ShowWindow()
    {
        GetWindow<ItemSettingCreationWindows>("Item Setting Creation");
    }

    void OnGUI()
    {
        GUILayout.Label("Standard ItemSetting Creation Window", EditorStyles.boldLabel);

        GUILayout.Label("\nInput all values below, the output will \nbe store in 'Prefabs/Basic Objects/'. Please go to trello\nto fill in the names and description first.\n\nFinal thing, make sure to drag your model &\nmaterial into the correct folders\n\nThe short name for the item (one word)");
        name = EditorGUILayout.TextField("The short name: ", name);

        GUILayout.Label("\nThe longer and more descriptive name\n of the interactable. (needs to be unique)");
        longerName = EditorGUILayout.TextField("The longer name: ", longerName);

        GUILayout.Label("\nWhat is the parent name? (e.g. Rocks,\nFood, etc) Case sensitive");
        parentName = EditorGUILayout.TextField("The Parent name (type): ", parentName);

        GUILayout.Label("\nThe gameObject, must be from the Assets Folder");
        g = (GameObject)EditorGUILayout.ObjectField(g, typeof(GameObject), false);

        GUILayout.Label("\nThe Model that the gameObject derives from,\nmust be a model not prefab");
        GUILayout.Label("FBX ONLY", EditorStyles.boldLabel);
        model = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject), false);

        GUILayout.Label("\nWhat is the model type (e.g. fbx, obj, etc)");
        modelType = EditorGUILayout.TextField("The model type: ", modelType);

        GUILayout.Label("\nWhat is the description of the itemSettings,\ncan be descriptive");
        desc = EditorGUILayout.TextField("The description", desc);

        GUILayout.Label("\nThe amount you can trade in this item for to\na trader");
        sellValue = EditorGUILayout.IntField("The sell value: ", sellValue);

        if (GUILayout.Button("Create ItemSettings"))
        {
            projName = longerName.Replace(" ", "-").ToLower();

            string folderId = AssetDatabase.CreateFolder("Assets/Resources/Prefabs/Basic Objects/" + parentName, longerName);
            string folderPath = AssetDatabase.GUIDToAssetPath(folderId);

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(model), "Assets/Resources/Prefabs/_Misc/Meshes/" + projName + "." + modelType);
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(g), folderPath + "/" + projName + ".prefab");

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(model));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(g));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ga = Resources.Load<GameObject>("Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName);
            GameObject gp = Resources.Load<GameObject>("Prefabs/_Misc/Meshes/" + projName);
            if (ga.GetComponent<MeshFilter>() != null)
                ga.GetComponent<MeshFilter>().sharedMesh = gp.GetComponent<MeshFilter>().sharedMesh;

            AssetDatabase.CopyAsset("Assets/Resources/UI/Basic UI Shapes/100px Square.png", "Assets/Resources/Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName + ".png");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            g = null;
            model = null;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ItemSettings i = ScriptableObject.CreateInstance<ItemSettings>();

            i.name = projName;
            i.desc = name;
            i.descName = longerName;
            i.icon = Resources.Load<Sprite>("Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName);
            i.gameObject = ga;
            i.description = desc;
            i.sellValue = sellValue;
            i.buyValue = sellValue * 2;

            GameObject newG = Resources.Load<GameObject>("Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName);
            Interaction_InteractableItem newII = newG.AddComponent<Interaction_InteractableItem>();
            newII.itemSettings = i;

            if (newG.GetComponent<MeshCollider>() == null && newG.GetComponent<BoxCollider>() == null)
                newG.AddComponent<MeshCollider>();

            if (newG.GetComponent<MeshCollider>() != null) { newG.GetComponent<MeshCollider>().convex = true; newG.GetComponent<MeshCollider>().sharedMesh = Resources.Load<GameObject>("Prefabs/_Misc/Meshes/" + projName).GetComponent<MeshFilter>().sharedMesh; }

            AssetDatabase.CreateAsset(i, "Assets/Resources/Prefabs/Interactable Items/" + projName + ".asset");
            if (g != null) { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(g)); }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("\nClick on me to go to Trello", EditorStyles.linkLabel)) { Application.OpenURL("https://trello.com/b/IH8Ja9lC/project-survival-before-dawn"); }
    }
}
