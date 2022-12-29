using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
[CanEditMultipleObjects]
public class LevelGeneratorEditor : Editor
{
    string levelName = "Level";
    string levelPrefabsPath = "Assets/Prefabs/Levels";
    public override void OnInspectorGUI()
    {
        float labelWidth = 150f;

        DrawDefaultInspector();
        LevelGenerator levelGenerator = (LevelGenerator)target;
        //----- Generate Button-----//
        if (GUILayout.Button("Generate"))
        {
            levelGenerator.GenerateLevelFromFile();
        }
        //----- Clear Button-----//
        if (GUILayout.Button("Clear"))
        {
            levelGenerator.ClearLevel();
        }
        //----- Level Name Text Field -----//
        GUILayout.BeginHorizontal(); 
        GUILayout.Label("Level Name", GUILayout.Width(labelWidth)); 
       
        levelName = GUILayout.TextField(levelName); 
        GUILayout.EndHorizontal();
        //----- Level Prefab Path Text Field -----//
        GUILayout.BeginHorizontal(); 
        GUILayout.Label("Level Prefabs Path", GUILayout.Width(labelWidth)); 
        
        levelPrefabsPath = GUILayout.TextField(levelPrefabsPath); 
        GUILayout.EndHorizontal();
        //----- Save Button-----//
        if (GUILayout.Button("Save"))
        {
            levelGenerator.SaveLevel(levelPrefabsPath, levelName);
        }

    }
}

