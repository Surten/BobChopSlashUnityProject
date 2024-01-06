using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static EnemySmart;
using static UnityEditor.PlayerSettings;
using static UnityEngine.CullingGroup;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject expDrop;
    public GameObject coinDrop;
    public GameObject DropContainer;

    public Transform playerTransform;
    public List<GameObject> enemyPrefab;
    private List<GameObject> commonEnemyPrefab = new List<GameObject>();
    private List<GameObject> bossEnemyPrefab = new List<GameObject>();

    public bool spawnOneEnemy = false;

    public float spawnableRadius = 13f;
    public float nonSpawnableRadius = 3f;
    private float validRadius = 0f;

    public int maxBossCount = 1;
    private int curBossCount = 0;

    public GameObject spawnEffectCommon;
    public GameObject spawnEffectBoss;

    /* Start and Update Functions */

    void Start()
    {
        validRadius = spawnableRadius - nonSpawnableRadius;
        if (validRadius < 0.5f) validRadius = 2f;
    }

    void Update()
    {
        if (spawnOneEnemy) spawnOnEditorDemand();

        // Loop through each enemy
        foreach (GameObject go in new List<GameObject>(enemies)) // Avoid error of removing enemy from a collection while in use
        {
            EnemyCapsuleSmart e1 = go.GetComponent<EnemyCapsuleSmart>();
            if (e1 != null)
            {
                bool isStateChanged = e1.GetIsStateChanged();
                if (e1.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
                {
                    if (isStateChanged)
                    {
                        float textSizeMult = 2f;
                        Color textColor = Color.black;
                        bool showflg = true;
                        e1.ShowFloatingText("(✖╭╮✖)", textColor, textSizeMult, showflg);
                        e1.ResetIsStateChanged();
                        e1.animate();
                    }
                    onEnemyDeath(go, e1.GetDespawnTime(), e1.GetCoins(), e1.GetExp());
                    continue;
                }

                e1.Behaviour();
                continue;
            }

            EnemyZombieSmart e2 = go.GetComponent<EnemyZombieSmart>();
            if (e2 != null)
            {
                bool isStateChanged = e2.GetIsStateChanged();

                if (e2.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
                {
                    if (isStateChanged) e2.ResetIsStateChanged();
                    e2.animate();
                    onEnemyDeath(go, e2.GetDespawnTime(), e2.GetCoins(), e2.GetExp());
                    continue;
                }

                e2.Behaviour();
                continue;
            }

            EnemyHumanoidSmart e3 = go.GetComponent<EnemyHumanoidSmart>();
            if (e3 != null)
            {
                bool isStateChanged = e3.GetIsStateChanged();

                if (e3.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
                {
                    if (isStateChanged) e3.ResetIsStateChanged();
                    e3.animate();
                    onEnemyDeath(go, e3.GetDespawnTime(), e3.GetCoins(), e3.GetExp());
                    continue;
                }

                e3.Behaviour();
                continue;
            }

            EnemyWolfSmart e4 = go.GetComponent<EnemyWolfSmart>();
            if (e4 != null)
            {
                bool isStateChanged = e4.GetIsStateChanged();

                if (e4.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
                {
                    if (isStateChanged) e4.ResetIsStateChanged();
                    e4.animate();
                    onEnemyDeath(go, e4.GetDespawnTime(), e4.GetCoins(), e4.GetExp());
                    continue;
                }

                e4.Behaviour();
                continue;
            }
        }
    }

    /* Load Prefabs */
    public void SetPrefabsTypes()
    {
        bossEnemyPrefab.Clear();
        commonEnemyPrefab.Clear();
        foreach (GameObject go in enemyPrefab)
        {
            if (_checkIsBoss(go))
            {
                bossEnemyPrefab.Add(go);
            }
            else
            {
                commonEnemyPrefab.Add(go);
            }
        }
        // Debug.Log("Boss Enemy Count: " + bossEnemyPrefab.Count);
        // Debug.Log("Common Enemy Count: " + commonEnemyPrefab.Count);
    }

    /* Spawn and Despawn Functions */
    private void spawnOnEditorDemand()
    {
        SpawnEnemy(Vector3.zero);
        spawnOneEnemy = false;
    }

    public void SpawnEnemy(Vector3 position, float scalingFactor = 1.0f)
    {

        // Check if the list is not empty
        if (enemyPrefab.Count != 0) 
        {
            if ((bossEnemyPrefab.Count != 0) && curBossCount < maxBossCount)
            {
                
                _SpawnPrefabEnemy(bossEnemyPrefab, position, scalingFactor);
                _PlaySpawnEffect(spawnEffectBoss, position);
                curBossCount++;
                //Debug.LogError("Boss Detected. Add prefabs to the list.");
            }
            else if (commonEnemyPrefab.Count != 0)
            {
                _SpawnPrefabEnemy(commonEnemyPrefab, position, scalingFactor);
                _PlaySpawnEffect(spawnEffectCommon, position);
            }
            else {
                Debug.LogError("Common Enemy Type (non-boss) not available. Add prefabs to the list.");
            }
        }
        else
        {
            Debug.LogError("Enemy Prefab list is empty. Add prefabs to the list.");
        }
    }

    private void _SpawnPrefabEnemy(List<GameObject> prefab, Vector3 position, float scalingFactor = 1.0f) {
        // Generate a random index
        int randomIndex = Random.Range(0, prefab.Count);

        // Access the randomly selected GameObject
        GameObject randomEnemyPrefab = prefab[randomIndex];

        // Instantiate or use the selected prefab
        GameObject newEnemy = Instantiate(randomEnemyPrefab, position, Quaternion.identity);
        newEnemy.transform.parent = transform;
        newEnemy.GetComponent<EnemySmart>().SetNewTarget(playerTransform);
        newEnemy.GetComponent<EnemySmart>().SetScalingFactor(scalingFactor);
        newEnemy.GetComponent<EnemyHitable>().ScaleHealth(scalingFactor);

        enemies.Add(newEnemy);
    }

    public void SpawnEnemiesRandomly(int numEnemies, int maxEnemies, float scalingFactor = 1.0f)
    {
        float x, z;
        if (enemies.Count >= maxEnemies) return;

        if ((enemies.Count + numEnemies) > maxEnemies) numEnemies = maxEnemies - enemies.Count;

        for (int i0 = 0; i0 < numEnemies; i0++)
        {
            
            x = 2 * validRadius * (UnityEngine.Random.value - 0.5f) + nonSpawnableRadius + playerTransform.position.x;
            z = 2 * validRadius * (UnityEngine.Random.value - 0.5f) + nonSpawnableRadius + playerTransform.position.z;
            Vector3 v = new Vector3(x, 0.1f, z);

            if(_isSpawnLocationValid(v, 2f)) SpawnEnemy(v, scalingFactor);
        }
    }

    public int getNumEnemies()
    {
        return enemies.Count;
    }

    public void DespawnAllEnemies()
    {
        foreach (GameObject go in enemies)
        {
            Destroy(go);
        }
        enemies.Clear();
        curBossCount = 0;
    }

    public void onEnemyDeath(GameObject enemy, float despawnTime, int addedCoins, int addedExp)
    {
        float r = 0.3f;
        _randomDropGameObjectAroundPos(coinDrop, enemy.transform.position, addedCoins, r);
        _randomDropGameObjectAroundPos(expDrop, enemy.transform.position, addedExp, r);
        enemies.Remove(enemy);
        Destroy(enemy, despawnTime);
    }

    /* Particle Effects */

    void _PlaySpawnEffect(GameObject spawnEffect, Vector3 position)
    {
        // Instantiate the particle effect at the spawn location
        GameObject particleEffectInstance = Instantiate(spawnEffect, position, Quaternion.Euler(-90f, 0f, 0f));

        // Access the ParticleSystem component if needed
        ParticleSystem particleSystem = particleEffectInstance.GetComponent<ParticleSystem>();

        // Play the particle effect
        if (particleSystem != null)
        {
            particleSystem.Play();

            // Destroy the GameObject after the duration of the particle system
            Destroy(particleEffectInstance, particleSystem.main.duration);
        }
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/

    public void _randomDropGameObjectAroundPos(GameObject dropItem, Vector3 pos_init, int amount, float radius)
    {
        for (int i0 = 0; i0 < amount; i0++)
        {
            Vector3 pos_temp = pos_init + new Vector3(radius * 2 * (UnityEngine.Random.value - 0.5f), 0f, radius * 2 * (UnityEngine.Random.value - 0.5f));
            GameObject obj_temp = Instantiate(dropItem, pos_temp, Quaternion.identity);
            obj_temp.transform.parent = DropContainer.transform;
            Destroy(obj_temp, 20.0f);
        }
    }

    private bool _checkIsBoss(GameObject go)
    {
        EnemyCapsuleSmart e1 = go.GetComponent<EnemyCapsuleSmart>();
        if (e1 != null) return e1.enemyScriptableObject.isBoss;
        EnemyZombieSmart e2 = go.GetComponent<EnemyZombieSmart>();
        if (e2 != null) return e2.enemyScriptableObject.isBoss;
        EnemyHumanoidSmart e3 = go.GetComponent<EnemyHumanoidSmart>();
        if (e3 != null) return e3.enemyScriptableObject.isBoss;
        EnemyWolfSmart e4 = go.GetComponent<EnemyWolfSmart>();
        if (e4 != null) return e4.enemyScriptableObject.isBoss;
        return false;
    }

    private bool _isSpawnLocationValid(Vector3 spawnPoint, float maxRaycastDistance = 2f)
    {
        // Create a ray from the spawn point in the upward direction
        Ray ray = new Ray(spawnPoint, Vector3.up);

        // Perform the raycast
        return !Physics.Raycast(ray, maxRaycastDistance);
    }
}
