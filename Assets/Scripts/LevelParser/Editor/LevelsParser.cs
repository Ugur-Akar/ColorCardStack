using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelsParser : MonoBehaviour
{
    //Settings

    // Connections
    public string levelFilesFolder = "Assets/Resources/Level";
    public string prefabsOutputFolder = "Assets/Prefabs/Levels/Gen 1";
    LevelGenerator levelGenerator;
    // State Variables
    
    // Start is called before the first frame update
    void Start()
    {
        InitConnections();
      
        //InitState();
    }
    void InitConnections(){
        levelGenerator = GetComponent<LevelGenerator>();
        
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ProcessFilesInFolder();
        }
    }

    public void ProcessFilesInFolder()
    {
        if (levelGenerator == null) levelGenerator = GetComponent<LevelGenerator>();

        DirectoryInfo info = new DirectoryInfo(levelFilesFolder);
        FileInfo[] fileInfo = info.GetFiles("*.csv", SearchOption.AllDirectories);
        
        for (int i=0; i< fileInfo.Length; i++)
        {
            levelGenerator.GenerateLevelFromFile(fileInfo[i].FullName);

            string prefabName = Path.GetFileNameWithoutExtension(fileInfo[i].FullName);

            levelGenerator.SaveLevel(prefabsOutputFolder, prefabName );
        }
           


        levelGenerator.GenerateLevelFromFile();
    }
}

