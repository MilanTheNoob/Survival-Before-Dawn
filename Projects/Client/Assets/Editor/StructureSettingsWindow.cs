using System.Collections;
using UnityEditor;
using UnityEngine;

public class StructureSettingsWindow : EditorWindow
{
    string name;
    string longerName;
    string projName;
    string parentName;
    GameObject g;
    string desc;
    int sellValue;
    int structureType;
    GameObject ga;
    GameObject model;

    [MenuItem("Editor Windows/Structure")]
    public static void ShowWindow()
    {
        GetWindow<StructureSettingsWindow>("Structures");
    }

    void OnGUI()
    {
        GUILayout.Label("Structure Settings ONLY", EditorStyles.boldLabel);

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

        GUILayout.Label("\nStrcture Type");
        structureType = EditorGUILayout.IntField("1 - Foundation, 2 - Wall, 3 - Furniture", structureType);

        if (GUILayout.Button("Create"))
        {
            projName = longerName.Replace(" ", "-").ToLower();

            string folderId = AssetDatabase.CreateFolder("Assets/Resources/Prefabs/Basic Objects/" + parentName, longerName);
            string folderPath = AssetDatabase.GUIDToAssetPath(folderId);

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(model), "Assets/Resources/Prefabs/_Misc/Meshes/" + projName + ".fbx");
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

            StructureItemSettings i = ScriptableObject.CreateInstance<StructureItemSettings>();

            i.name = projName;
            i.desc = name;
            i.descName = longerName;
            i.icon = Resources.Load<Sprite>("Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName);
            i.gameObject = ga;
            i.description = desc;
            i.sellValue = sellValue;
            i.buyValue = sellValue * 2;

            GameObject newG = Resources.Load<GameObject>("Prefabs/Basic Objects/" + parentName + "/" + longerName + "/" + projName);
            BuildPreview newBP = newG.AddComponent<BuildPreview>();

            if (structureType == 1) { newBP.structureType = BuildPreview.StructureType.Foundation; }
            else if (structureType == 2) { newBP.structureType = BuildPreview.StructureType.Wall; }
            else if (structureType == 3) { newBP.structureType = BuildPreview.StructureType.Furniture; }
            else if (structureType == 4) { newBP.structureType = BuildPreview.StructureType.Storage; }

            if (newG.GetComponent<MeshCollider>() == null && newG.GetComponent<BoxCollider>() == null)
                newG.AddComponent<MeshCollider>();

            if (newG.GetComponent<MeshCollider>() != null) { newG.GetComponent<MeshCollider>().convex = true; newG.GetComponent<MeshCollider>().sharedMesh = Resources.Load<GameObject>("Prefabs/_Misc/Meshes/" + projName).GetComponent<MeshFilter>().sharedMesh; }

            AssetDatabase.CreateAsset(i, "Assets/Resources/Prefabs/Interactable Items/" + projName + ".asset");
            if (g != null) { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(g)); }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
