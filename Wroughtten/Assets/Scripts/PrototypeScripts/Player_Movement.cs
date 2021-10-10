using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float groundSpeed = 6f;
    //[SerializeField] private float airSpeed = 12f;
    private float gravity = -9.81f;
    private Vector3 velocity;
    //[SerializeField] private float jumpHeight = 1f;
    public bool onGround = false;
    [SerializeField] private Transform groundCheck;


    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.transform.position,0.1f);
    }
    void Update(){

        if(Cursor.lockState==CursorLockMode.Locked){
            onGround = Physics.CheckSphere(groundCheck.transform.position,0.1f) ? true : false;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            
            characterController.Move(velocity * Time.deltaTime);
            

            characterController.Move(move * groundSpeed * Time.deltaTime);

            // if (Input.GetButtonDown("Jump") && onGround){

            //     velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // }

            velocity.y += gravity * Time.deltaTime;
        }
    }
}
