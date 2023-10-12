using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public int waveTime = 20;
    
    public int enemyOneLimit = 10;
    public int spawnSpeed = 1;

}