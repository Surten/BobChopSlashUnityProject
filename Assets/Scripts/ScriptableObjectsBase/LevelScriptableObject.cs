using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public int waveTime = 20;
    
    public int enemyOneLimit = 10;
    public int spawnSpeed = 1;

    public int sceneLevel = 1;
    public bool fogEnabled = false;

    public List<GameObject> enemieTypes;

    public float enemyScalingFactor = 1f;

}