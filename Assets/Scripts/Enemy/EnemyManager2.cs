using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static EnemySmartAF2;
using static UnityEngine.CullingGroup;

public class EnemyManager2 : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public Transform playerTransform;
    public GameObject enemyPrefab;
    public Enemy2ScriptableObject enemyScriptableObject;

    public bool spawnOneEnemy = false;

    public float arenaRadius = 13f;

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
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.transform.parent = transform;
        newEnemy.GetComponent<EnemySmartAF2>().SetNewTarget(playerTransform);
        enemies.Add(newEnemy);
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

    public void onEnemyDeath(GameObject enemy, float despawnTime)
    {
        ShopManager.Instance.AddCoins(enemyScriptableObject.coinsDropOnDeath);
        enemies.Remove(enemy);
        Destroy(enemy, despawnTime);
    }

    void Update()
    {
        if (spawnOneEnemy) spawnOnEditorDemand();
        foreach (GameObject go in enemies) // Loop through each enemy
        {
            EnemySmartAF2 e = go.GetComponent<EnemySmartAF2>();

            bool isStateChanged = e.GetIsStateChanged();

            if (e.GetEnemyState() == EnemySmartAF2.EnemyState.Dead) // If dead, remove data and remove body
            {
                if (isStateChanged) e.ResetIsStateChanged();
                onEnemyDeath(go, e.GetDespawnTime());
                return;
            }

            if (e.GetEnemyState() == EnemySmartAF2.EnemyState.Staggering) { // If the enemy is staggering, do nothing;
                if (isStateChanged) e.ResetIsStateChanged();
                return;
            }

            float targetDistance = (playerTransform.position - e.transform.position).magnitude; // Check for the distance between player and enemy
            if (targetDistance > enemyScriptableObject.awarenessAwareRange) // No movement
            {
                if (isStateChanged) e.ResetIsStateChanged();
                e.SetEnemyState(EnemySmartAF2.EnemyState.Idle);
                continue;
            } 
            else if (targetDistance > enemyScriptableObject.awarenessChaseRange) // Rotate towards player
            {
                if (isStateChanged) e.ResetIsStateChanged();
                e.SetEnemyState(EnemySmartAF2.EnemyState.Rotating);
                continue;
            }
            else if (targetDistance > enemyScriptableObject.awarenessAttackRange) // Chase Player
            {
                if (isStateChanged) e.ResetIsStateChanged();
                if (e.isCharging) e.SetEnemyState(EnemySmartAF2.EnemyState.Running);
                else e.SetEnemyState(EnemySmartAF2.EnemyState.Walking);
                continue;
            }
            else // Attack player
            {
                if (isStateChanged) e.ResetIsStateChanged();
                e.SetEnemyState(EnemySmartAF2.EnemyState.Attack);
                continue;
            }
        }

    }
}
