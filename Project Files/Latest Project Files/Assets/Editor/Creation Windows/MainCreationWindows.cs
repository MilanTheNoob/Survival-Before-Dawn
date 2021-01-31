using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainCreationWindows : EditorWindow
{
    [MenuItem("Editor Windows/Creation Window")]

    // Called to render the window
    public static void ShowWindow()
    {
        GetWindow<MainCreationWindows>("Main Creation Window");
    }

    void OnGUI()
    {
        GUILayout.Label("The Main Creation Window", EditorStyles.boldLabel);
        GUILayout.Label("Use the following creation windows to create all basic items\n\nGeneration Windows:");

        if (GUILayout.Button("World Creation")) { GetWindow<WorldCreationWindows>("World Generation"); }

        GUILayout.Label("The ItemSetting Creation Windows, contain windows for different variants of itemSettings\n\nItemSetting Windows:");

        if (GUILayout.Button("Standard ItemSetting Creation")) { GetWindow<ItemSettingCreationWindows>("ItemSetting Creation"); }
        if (GUILayout.Button("Food ItemSetting Creation")) { GetWindow<FoodItemSettingCreationWindows>("FoodItemSetting Creation"); }
        if (GUILayout.Button("Structure ItemSetting Creation")) { GetWindow<StructureItemSettingCreationWindows>("StructureItemSetting Creation"); }
        if (GUILayout.Button("Tool ItemSetting Creation")) { GetWindow<ToolItemSettingCreationWindows>("ToolItemSetting Creation"); }
    }
}
