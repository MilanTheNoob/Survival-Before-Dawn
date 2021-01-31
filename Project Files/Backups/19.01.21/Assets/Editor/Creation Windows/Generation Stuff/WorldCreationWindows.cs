using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldCreationWindows : EditorWindow
{
    int seed;
    string name;

    [MenuItem("Editor Windows/Generation Stuff/World Generation")]

    // Called to render a window
    public static void ShowWindow()
    {
        GetWindow<WorldCreationWindows>("World Generation");
    }

    // Called for a GUI
    void OnGUI()
    {
        // Sum labels
        GUILayout.Label("Used to create a pre-gen level (or world)", EditorStyles.boldLabel);
        GUILayout.Label("Go to playmode, set the seed & name of the level/world.\n\nThen create the world in the new gameObject,\n copy the world you created and exit playmode.\n then paste it using the WorldsManager.cs\n");

        // Input fields
        seed = (int)EditorGUILayout.IntField("The seed", seed);

        // Button with code for once clicked
        if (GUILayout.Button("Empty World"))
        {
            // Create new GameObject and set params
            GameObject worldParent = new GameObject();
            worldParent.transform.parent = GameObject.Find("Pre-Generated Worlds").transform;
            worldParent.transform.name = "New Pre-Gen Level";

            // Get the TerrainGenerator instance
            TerrainGenerator terrainGenerator = TerrainGenerator.instance;

            // Set the terrain params
            terrainGenerator.heightMapSettings.noiseSettings.seed = seed;
            terrainGenerator.generateType = TerrainGenerator.GenerateType.PreGen;
            terrainGenerator.ResetChunks();
        }
    }
}
