using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelScriptableObject[] levelScriptableObjects;
    public TextMeshProUGUI timeLeftText;
    public GuiControl playerGui;
    public EnemyManager enemyManager;

    int currentLevel;
    int timeLeft;
    int spawnSpeed;

    void Start()
    {
        currentLevel = 0;
        StartWave();
    }

    private void StartWave()
    {
        timeLeft = levelScriptableObjects[currentLevel].waveTime;
        spawnSpeed = levelScriptableObjects[currentLevel].spawnSpeed;

        StartCoroutine(LoopTimer());
    }

    IEnumerator LoopTimer()
    {
        while (timeLeft > 0)
        {
            timeLeftText.text = timeLeft.ToString();
            TimerLoop();
            yield return new WaitForSeconds(1f);
            timeLeft -= 1;
        }

        // enter shop
        playerGui.ShopAppear();
        ShopManager.Instance.LoadNewItemsToShop();
        enemyManager.DespawnAllEnemies();

    }


    public void EnterNextWave()
    {
        currentLevel++;
        if(levelScriptableObjects.Length == currentLevel)
        {
            // end game
            return;
        }


        playerGui.ShopDisappear();

        //reset playerPosition???

        StartWave();
    }

    private void TimerLoop()
    {
        // spawn enemies up to a limit
        // limit will be defined in a scriptable object for each wave
        
        if (levelScriptableObjects[currentLevel].enemyOneLimit > enemyManager.getNumEnemies())
        {
            enemyManager.SpawnEnemiesRandomly(spawnSpeed);
        }
    }

}
