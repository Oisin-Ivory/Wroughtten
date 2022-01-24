using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Mouse_Look : MonoBehaviour
{

    public Transform playerBody;

    public float mouseSensitivity = 200f;

    float xRotation = 0f;

    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity"); 
       
        if(mouseSensitivity==0)mouseSensitivity=200f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if(Cursor.lockState==CursorLockMode.Locked && (!Input.GetButton("Jump")||Input.GetKeyDown("r"))){
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
