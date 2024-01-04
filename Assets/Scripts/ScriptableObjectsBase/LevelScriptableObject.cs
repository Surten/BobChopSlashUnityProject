using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public int waveTime = 20;
    
    public int enemyOneLimit = 10;
    public int spawnSpeed = 1;

    public Vector3 playerStartPosition;
    public bool fogEnabled = false;

    public List<GameObject> enemieTypes;

    public float enemyScalingFactor = 1f;

    public bool enforceNewPosition = true;

}