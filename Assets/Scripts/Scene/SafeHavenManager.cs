using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeHavenManager : MonoBehaviour
{
    public List<GameObject> safeHavens;
    private bool activeHaven;
    private float timer;
    public float maxSafeTime;
    private int index;

    // Start is called before the first frame update
    void OnEnable()
    {
        activeHaven = false;
        foreach (GameObject go in safeHavens) {
            SafeHaven sf = go.GetComponent<SafeHaven>();
            sf.deactivateHaven();
            go.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!activeHaven) // Randomly Activate a Safe Haven
        {
            index = Random.Range(0, safeHavens.Count);
            safeHavens[index].SetActive(true);
            SafeHaven sf = safeHavens[index].GetComponent<SafeHaven>();
            sf.setTimeToBlink(3f);
            sf.activateHaven(maxSafeTime);
            activeHaven = true;
        }

        if (timer > maxSafeTime) // Deativate a Safe Haven
        {
            safeHavens[index].SetActive(false);
            SafeHaven sf = safeHavens[index].GetComponent<SafeHaven>();
            sf.deactivateHaven();
            activeHaven = false;
            timer = 0;
        }

        
    }
}
