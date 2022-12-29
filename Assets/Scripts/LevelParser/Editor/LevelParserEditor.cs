using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelsParser))]
[CanEditMultipleObjects]
public class LevelParserEditor : Editor
{
    string levelName = "Level";
    string levelPrefabsPath = "Assets/Prefabs/Levels";
    public override void OnInspectorGUI()
    {
        float labelWidth = 150f;

        DrawDefaultInspector();
        LevelsParser levelParser = (LevelsParser)target;
        //----- Generate Button-----//
        if (GUILayout.Button("Parse Folder"))
        {
            levelParser.ProcessFilesInFolder();
        }
        

    }
}

