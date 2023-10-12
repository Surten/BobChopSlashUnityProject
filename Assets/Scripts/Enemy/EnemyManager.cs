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
        for(int i = 0; i < numEnemies; i++)
        {
            x = (UnityEngine.Random.value - 0.5f) * arenaRadius * 2;
            z = (UnityEngine.Random.value - 0.5f) * arenaRadius * 2;
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
        foreach (GameObject go in enemies)
        {
            EnemySmartAF e = go.GetComponent<EnemySmartAF>();

            if (e.GetEnemyState() == EnemySmartAF.EnemyState.Dead)
            {
                onEnemyDeath(go);
                return;
            }

            float targetDistance = (playerTransform.position - e.transform.position).magnitude;
            if (targetDistance > enemyScriptableObject.awarenessAwareRange)
            {
                e.SetEnemyState(EnemySmartAF.EnemyState.Idle);
            } 
            else if (targetDistance > enemyScriptableObject.awarenessChaseRange)
            {
                e.SetEnemyState(EnemySmartAF.EnemyState.Rotating);
            }
            else if (targetDistance > enemyScriptableObject.awarenessAttackRange)
            {
                e.SetEnemyState(EnemySmartAF.EnemyState.Walking);
            }
            else
            {
                e.SetEnemyState(EnemySmartAF.EnemyState.Attack);
            }
        }

    }
}
