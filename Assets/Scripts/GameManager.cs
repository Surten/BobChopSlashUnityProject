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
    public Transform playerTransform;
    private AudioSource audioSource;
    public AudioClip[] soundtracks;
    public GameObject[] scenes;

    private Coroutine gameLoopCoroutine;

    int currentScene;
    int currentLevel;
    int timeLeft;
    int spawnSpeed;
    public static float masterVolume = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = masterVolume;
        currentLevel = 0;
        currentScene = -1;
        deactivateAllScenes();
        StartWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            timeLeft = -1;
        }
    }

    private void StartWave()
    {
        
        timeLeft = levelScriptableObjects[currentLevel].waveTime;
        spawnSpeed = levelScriptableObjects[currentLevel].spawnSpeed;

        // Activate the New Scene
        if (currentScene != levelScriptableObjects[currentLevel].sceneLevel)
        {
            // Deativate Previous Scene
            if(currentScene != -1) scenes[currentScene].SetActive(false);

            // Activate Current Scene
            currentScene = levelScriptableObjects[currentLevel].sceneLevel;
            GameObject newScene = scenes[currentScene];
            newScene.SetActive(true);
            
            // Get Spawn Point for Player
            Transform childTransform = newScene.transform.Find("SpawnPoint");

            // Check if the child object was found
            if (childTransform != null)
            {
                // Access the child object
                GameObject childObject = childTransform.gameObject;
                playerTransform.position = childTransform.position;
                playerTransform.rotation = childTransform.rotation;
            }
            else
            {
                Debug.LogError("Child object not found.");
            }
        }

        RenderSettings.fog = levelScriptableObjects[currentLevel].fogEnabled;

        enemyManager.enemyPrefab = levelScriptableObjects[currentLevel].enemieTypes;
        enemyManager.SetPrefabsTypes();

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
            playerGui.OnPlayerWin();
            OnPlayerDeath();
            return;
        }


        playerGui.ShopDisappear();

        StartWave();
    }

    private void TimerLoop()
    {
        // spawn enemies up to a limit
        // limit will be defined in a scriptable object for each wave
        
        if (enemyManager.getNumEnemies() < levelScriptableObjects[currentLevel].enemyOneLimit  )
        {
            enemyManager.SpawnEnemiesRandomly(spawnSpeed, levelScriptableObjects[currentLevel].enemyOneLimit, levelScriptableObjects[currentLevel].enemyScalingFactor);
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

    private void deactivateAllScenes() {
        for (int i0 = 0; i0 < scenes.Length; i0++) {
            scenes[i0].SetActive(false);
        }
    }

}
