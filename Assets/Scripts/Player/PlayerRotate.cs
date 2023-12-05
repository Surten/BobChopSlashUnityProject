using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform playerHead;

    public float xRotation = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;        
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseWheel = Input.mouseScrollDelta.y * 0.1f;

        Vector3 playerDir = playerHead.position - transform.position;
        Vector3 futureCameraPosition = transform.position + playerDir* mouseWheel;


        if ((playerHead.position - futureCameraPosition).magnitude > 1f && (playerHead.position - futureCameraPosition).magnitude < 6f)
            transform.position = futureCameraPosition;


        if (xRotation + mouseY > -40f && xRotation + mouseY < 90f)
        {
            transform.RotateAround(playerHead.position, playerBody.right, mouseY);
            xRotation = transform.eulerAngles.x;
            if (xRotation > 180f) xRotation -= 360f;
        }

        /*transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);*/
        transform.LookAt(playerHead.position);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
