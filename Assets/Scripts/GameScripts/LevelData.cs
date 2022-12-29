using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public List<int> startMoves;
    public List<int> otherMoves;
    public List<Material> materials;
    public List<float> cameraSize;
}

