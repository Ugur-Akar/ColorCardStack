using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TakeSnapShot : MonoBehaviour
{
    //Settings
    public string folderPath = "./ScreenShots/";
    // Connections

    // State Variables
    int shotIndex;
    // Start is called before the first frame update
    void Start()
    {
        //InitConnections();
        InitState();
    }
    void InitConnections(){
    }
    void InitState(){
        shotIndex = 0;
        CheckResolution();
    }

    void CheckResolution()
    {
        string resStr = "" + Screen.width + "x" + Screen.height + "/";
        folderPath += resStr;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
    }

    void CheckKey()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            string dateStr = DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");

            string fileName = folderPath + "ss" + shotIndex + "_" + dateStr+".png";
            Debug.Log("File saved: " + fileName);
            ScreenCapture.CaptureScreenshot(fileName);
            shotIndex++;
        }
    } 
}

