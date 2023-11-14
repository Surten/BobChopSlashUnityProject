using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public Transform playerTransform;
    public GameObject enemyPrefab;
    public Enemy1ScriptableObject enemyScriptableObject;

    public bool spawnOneEnemy = false;

    private float arenaRadius = 30f;

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
        newEnemy.GetComponent<EnemySmartAF>().SetNewTarget(playerTransform);
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

    public void onEnemyDeath(GameObject enemy)
    {
        ShopManager.Instance.AddCoins(enemyScriptableObject.coinsDropOnDeath);
        enemies.Remove(enemy);
        Destroy(enemy, 3f);
    }

    void Update()
    {
        if (spawnOneEnemy) spawnOnEditorDemand();
        foreach (GameObject go in enemies) // Loop through each enemy
        {
            EnemySmartAF e = go.GetComponent<EnemySmartAF>();

            if (e.GetEnemyState() == EnemySmartAF.EnemyState.Dead) // If dead, remove data and remove body
            {
                onEnemyDeath(go);
                return;
            }

            float targetDistance = (playerTransform.position - e.transform.position).magnitude; // Check for the distance between player and enemy
            if (targetDistance > enemyScriptableObject.awarenessAwareRange) // No movement
            {
                if (e.isIdleJumping)
                {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Jumping);
                }
                else
                {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Idle);
                }

                continue;
            } 
            else if (targetDistance > enemyScriptableObject.awarenessChaseRange) // Rotate towards player
            {
                e.SetEnemyState(EnemySmartAF.EnemyState.Rotating);
                continue;
            }
            else if (targetDistance > enemyScriptableObject.awarenessAttackRange) // Chase Player
            {
                
                if (e.isCharging)
                {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Charging);
                }
                else
                {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Walking);
                }

                continue;
            }
            else // Attack player
            {
                if (e.canExplode)
                {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Explode);
                }
                else {
                    e.SetEnemyState(EnemySmartAF.EnemyState.Attack);
                }

                continue;
            }
        }

    }
}
