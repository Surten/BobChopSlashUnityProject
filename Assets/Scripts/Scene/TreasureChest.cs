using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform DropContainer;
    public int cointCount = 20;
    private void OnTriggerEnter(Collider other)
    {
        float radius = 0.5f;
        
        _randomDropGameObjectAroundPos(coinPrefab, transform.position, cointCount, radius);

        Destroy(gameObject, 0.5f);
    }

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
}
