using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManip : MonoBehaviour
{
    Vector3 direction;
    float moveSpeed;
    public Transform target;


    void Start()
    {

    }

    void Update()
    {
        direction = target.position - transform.position;
        direction.Normalize();

        transform.LookAt(target.position);
        moveSpeed = (target.position - transform.position).magnitude * 0.5f;
        transform.position += direction * Time.deltaTime * (moveSpeed);
    }
}
