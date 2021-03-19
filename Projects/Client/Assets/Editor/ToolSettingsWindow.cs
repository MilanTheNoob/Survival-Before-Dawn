using System.Collections;
using UnityEditor;
using UnityEngine;

public class ToolItemSettingCreationWindows : EditorWindow
{
    string name;
    string longerName;
    string projName;
    string parentName;
    GameObject g;
    string desc;
    int sellValue;
    GameObject ga;
    GameObject model;

    [MenuItem("Editor Windows/ItemSetting Windows/Tools")]
    public static void ShowWindow()
    {
        GetWindow<ToolItemSettingCreationWindows>("Tool Setting Creation");
    }

    void OnGUI()
    {
        GUILayout.Label("Standard ONLY", EditorStyles.boldLabel);

        name = EditorGUILayout.TextField("Short name", name);
        longerName = EditorGUILayout.TextField("Descriptive name", longerName);
        parentName = EditorGUILayout.TextField("Category", parentName);

        GUILayout.Label("\nGameObject");
        g = (GameObject)EditorGUILayout.ObjectField(g, typeof(GameObject), false);
        GUILayout.Label("Prefab");
        model = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject), false);

        GUILayout.Label("\n");
        desc = EditorGUILayout.TextField("Item Description", desc);
        sellValue = EditorGUILayout.IntField("Sell Value", sellValue);

        if (GUILayout.Button("Create ItemSettings"))
        {
            projName = longerName.Replace(" ", "-").ToLower();

            string folderId = AssetDatabase.CreateFolder("Assets/Resources/Prefabs/Basic Objects/" + parentName, longerName);
            string folderPath = AssetDatabase.GUIDToAssetPath(folderId);

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(model), "Assets/Resources/Prefabs/_Misc/Meshes/" + projName + ".FBX");
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

            ToolItemSettings i = ScriptableObject.CreateInstance<ToolItemSettings>();

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
