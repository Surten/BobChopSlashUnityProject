using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treasureChest : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform DropContainer;
    private void OnTriggerEnter(Collider other)
    {
        float radius = 0.5f;
        for (int i0 = 0; i0 < 20; i0++)
        {
            Vector3 pos_temp = transform.position + new Vector3(radius * 2 * (UnityEngine.Random.value - 0.5f), 0f, radius * 2 * (UnityEngine.Random.value - 0.5f));
            GameObject obj_temp = Instantiate(coinPrefab, pos_temp, Quaternion.identity);
            obj_temp.transform.parent = DropContainer.transform;
        }
        Destroy(gameObject);
    }
}
