using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static EnemySmart;
using static UnityEngine.CullingGroup;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public Transform playerTransform;
    public GameObject enemyPrefab;
    public Enemy1ScriptableObject enemy1ScriptableObject;
    public Enemy2ScriptableObject enemy2ScriptableObject;

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
        newEnemy.GetComponent<EnemySmart>().SetNewTarget(playerTransform);
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

    public void onEnemyDeath(GameObject enemy, float despawnTime, int coins)
    {
        ShopManager.Instance.AddCoins(coins);
        enemies.Remove(enemy);
        Destroy(enemy, despawnTime);
    }

    void Update()
    {
        if (spawnOneEnemy) spawnOnEditorDemand();
        foreach (GameObject go in enemies) // Loop through each enemy
        {
            EnemyCapsuleSmart e1 = go.GetComponent<EnemyCapsuleSmart>();
            if (e1 != null)
            {
                CapsuleEnemy(e1, go);
                continue;
            }

            EnemyZombieSmart e2 = go.GetComponent<EnemyZombieSmart>();
            if (e2 != null)
            {
                ZombieEnemy(e2, go);
                continue;
            }
        }
    }

    void CapsuleEnemy(EnemyCapsuleSmart e, GameObject go) {
        bool isStateChanged = e.GetIsStateChanged();
        float textSizeMult = 2f;
        Color textColor = Color.black;
        bool showflg = false;

        enemy1ScriptableObject = e.enemyScriptableObject;
        if (e.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
        {
            if (isStateChanged)
            {
                e.ShowFloatingText("(✖╭╮✖)", textColor, textSizeMult, showflg);
                e.ResetIsStateChanged();
            }
            e.renderTextureColor();
            onEnemyDeath(go, e.GetDespawnTime(), enemy1ScriptableObject.coinsDropOnDeath);
            return;
        }

        if (e.GetEnemyState() == EnemySmart.EnemyState.Staggering)
        { // If the enemy is staggering, do nothing;
            if (isStateChanged) e.ResetIsStateChanged();
            return;
        }

        float targetDistance = (playerTransform.position - e.transform.position).magnitude; // Check for the distance between player and enemy
        if (targetDistance > enemy1ScriptableObject.awarenessAwareRange) // No movement
        {
            if (e.isIdleJumping)
            {
                if (isStateChanged)
                {
                    e.ShowFloatingText("♪(┌・。・)┌", textColor, textSizeMult * 4f, showflg);
                    e.ResetIsStateChanged();
                }
                e.SetEnemyState(EnemySmart.EnemyState.Jumping);
            }
            else
            {
                if (isStateChanged)
                {
                    e.ShowFloatingText("ヽ(•‿•)ノ", textColor, textSizeMult * 4f, showflg);
                    e.ResetIsStateChanged();
                }
                e.SetEnemyState(EnemySmart.EnemyState.Idle);
            }

            return;
        }
        else if (targetDistance > enemy1ScriptableObject.awarenessChaseRange) // Rotate towards player
        {
            if (isStateChanged)
            {
                e.ShowFloatingText("(☉_☉)", textColor, textSizeMult * 2f, showflg);
                e.LoadWavFile(e.Sound2Int(EnemyCapsuleSmart.SoundState.Alert));
                e.ResetIsStateChanged();
            }
            e.SetEnemyState(EnemySmart.EnemyState.Rotating);
            return;
        }
        else if (targetDistance > enemy1ScriptableObject.awarenessAttackRange) // Chase Player
        {
            if (isStateChanged)
            {
                e.ShowFloatingText("ヽ(ಠ_ಠ)ノ", textColor, textSizeMult, showflg);
                if (e.isCharging) e.LoadWavFile(e.Sound2Int(EnemyCapsuleSmart.SoundState.Running));
                e.ResetIsStateChanged();
            }
            if (e.isCharging)
            {
                e.SetEnemyState(EnemySmart.EnemyState.Running);
            }
            else
            {
                e.SetEnemyState(EnemySmart.EnemyState.Walking);
            }

            return;
        }
        else // Attack player
        {
            if (isStateChanged)
            {
                e.ResetIsStateChanged();
            }

            if (e.canExplode)
            {
                e.SetEnemyState(EnemySmart.EnemyState.Explode);
                e.LoadWavFile(0);
            }
            else
            {
                e.SetEnemyState(EnemySmart.EnemyState.Attack);
            }

            return;
        }
    }

    void ZombieEnemy(EnemyZombieSmart e, GameObject go)
    {
        bool isStateChanged = e.GetIsStateChanged();

        enemy2ScriptableObject = e.enemyScriptableObject;
        if (e.GetEnemyState() == EnemySmart.EnemyState.Dead) // If dead, remove data and remove body
        {
            if (isStateChanged) e.ResetIsStateChanged();
            onEnemyDeath(go, e.GetDespawnTime(), enemy2ScriptableObject.coinsDropOnDeath);
            return;
        }

        if (e.GetEnemyState() == EnemySmart.EnemyState.Staggering)
        { // If the enemy is staggering, do nothing;
            if (isStateChanged) e.ResetIsStateChanged();
            return;
        }

        float targetDistance = (playerTransform.position - e.transform.position).magnitude; // Check for the distance between player and enemy
        if (targetDistance > enemy2ScriptableObject.awarenessAwareRange) // No movement
        {
            if (isStateChanged) e.ResetIsStateChanged();
            e.SetEnemyState(EnemySmart.EnemyState.Idle);
            return;
        }
        else if (targetDistance > enemy2ScriptableObject.awarenessChaseRange) // Rotate towards player
        {
            if (isStateChanged) e.ResetIsStateChanged();
            e.SetEnemyState(EnemySmart.EnemyState.Rotating);
            return;
        }
        else if (targetDistance > enemy2ScriptableObject.awarenessAttackRange) // Chase Player
        {
            if (isStateChanged) e.ResetIsStateChanged();
            if (e.isCharging) e.SetEnemyState(EnemySmart.EnemyState.Running);
            else e.SetEnemyState(EnemySmart.EnemyState.Walking);
            return;
        }
        else // Attack player
        {
            if (isStateChanged) e.ResetIsStateChanged();
            e.SetEnemyState(EnemySmart.EnemyState.Attack);
            return;
        }
    }
}
