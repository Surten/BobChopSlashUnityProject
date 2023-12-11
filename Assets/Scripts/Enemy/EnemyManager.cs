﻿using System.Collections;
using System.Collections.Generic;
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
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject expDrop;
    public GameObject coinDrop;

    public Transform playerTransform;
    public List<GameObject> enemyPrefab;

    public bool spawnOneEnemy = false;

    public float arenaRadius = 13f;

    public int maxBossCount = 1;
    private int curBossCount = 0;

    private void spawnOnEditorDemand()
    {
        SpawnEnemy(Vector3.zero);
        spawnOneEnemy = false;
    }

    public int getNumEnemies()
    {
        return enemies.Count;
    }


    public void SpawnEnemy(Vector3 position)
    {
        // Check if the list is not empty
        if (enemyPrefab.Count > 0)
        {
            // Generate a random index
            int randomIndex = Random.Range(0, enemyPrefab.Count);

            // Access the randomly selected GameObject
            GameObject randomEnemyPrefab = enemyPrefab[randomIndex];

            // Instantiate or use the selected prefab
            GameObject newEnemy = Instantiate(randomEnemyPrefab, position, Quaternion.identity);
            newEnemy.transform.parent = transform;
            newEnemy.GetComponent<EnemySmart>().SetNewTarget(playerTransform);
            enemies.Add(newEnemy);
        }
        else
        {
            Debug.LogError("Enemy Prefab list is empty. Add prefabs to the list.");
        }
    }

    public void SpawnEnemiesRandomly(int numEnemies)
    {
        float x, z;
        for(int i0 = 0; i0 < numEnemies; i0++)
        {
            x = 2 * arenaRadius * (UnityEngine.Random.value - 0.5f);
            z = 2 * arenaRadius * (UnityEngine.Random.value - 0.5f);
            Vector3 v = new Vector3(x, 1f, z);
            SpawnEnemy(v);
        }
    }

    public void DespawnAllEnemies()
    {
        foreach (GameObject go in enemies)
        {
            Destroy(go);
        }
        enemies.Clear();
    }


    void Start()
    {

    }

    public void onEnemyDeath(GameObject enemy, float despawnTime, int addedCoins, int addedExp)
    {
        //ShopManager.Instance.AddCoins(addedCoins);
        float r = 0.3f;
        _randomDropGameObjectAroundPos(coinDrop, enemy.transform.position, addedCoins, r);
        _randomDropGameObjectAroundPos(expDrop, enemy.transform.position, addedExp, r);
        enemies.Remove(enemy);
        Destroy(enemy, despawnTime);
    }

    public void _randomDropGameObjectAroundPos(GameObject dropItem, Vector3 pos_init, int amount, float radius) {
        for (int i0 = 0; i0 < amount; i0++)
        {
            Vector3 pos_temp = pos_init + new Vector3(radius * 2*(UnityEngine.Random.value - 0.5f), 0f, radius * 2*(UnityEngine.Random.value - 0.5f));
            Instantiate(dropItem, pos_temp, Quaternion.identity);
        }
    }

    void Update()
    {
        if (spawnOneEnemy) spawnOnEditorDemand();
        foreach (GameObject go in enemies) // Loop through each enemy
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
                    //UnityEngine.Debug.Log("Despawn Time" + e2.GetDespawnTime());
                    e2.animate();
                    onEnemyDeath(go, e2.GetDespawnTime(), e2.GetCoins(), e2.GetExp());
                    continue;
                }

                e2.Behaviour();
                continue;
            }
        }
    }

}
