using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{

    #region Singleton
    private static GameManager mInstance;

    private void Awake()
    {
        if (mInstance != null)
            Debug.Log("More than one instance of Inventory");
        mInstance = this;
    }
    public static GameManager Instance
    {
        get
        {
            return mInstance;
        }
    }
    #endregion


    public LevelScriptableObject[] levelScriptableObjects;
    public TextMeshProUGUI timeLeftText;
    public GuiControl playerGui;
    public EnemyManager enemyManager;
    private AudioSource audioSource;
    public AudioClip[] soundtracks;

    private Coroutine gameLoopCoroutine;

    int currentLevel;
    int timeLeft;
    int spawnSpeed;
    public static float masterVolume = 0.5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = masterVolume;
        currentLevel = 0;
        enemyManager.SetPrefabsTypes();
        StartWave();
    }

    private void StartWave()
    {
        timeLeft = levelScriptableObjects[currentLevel].waveTime;
        spawnSpeed = levelScriptableObjects[currentLevel].spawnSpeed;

        audioSource.clip = soundtracks[0];
        audioSource.Play();

        gameLoopCoroutine = StartCoroutine(LoopTimer());
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
        audioSource.clip = soundtracks[1];
        audioSource.Play();
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
        
        if (enemyManager.getNumEnemies() < levelScriptableObjects[currentLevel].enemyOneLimit  )
        {
            enemyManager.SpawnEnemiesRandomly(spawnSpeed, levelScriptableObjects[currentLevel].enemyOneLimit);
        }
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(afterDeathTimer());
    }

    private IEnumerator afterDeathTimer()
    {
        enemyManager.DespawnAllEnemies();
        StopCoroutine(gameLoopCoroutine);
        yield return new WaitForSeconds(3f);
        BackToMainMenu();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
