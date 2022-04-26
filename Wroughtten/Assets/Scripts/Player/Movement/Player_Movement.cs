using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    
    [Header("Speed Settings")]
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float maxGroundSpeed = 4f;
    [SerializeField] private float groundSpeedIncrease = 2f;
    [SerializeField] private float groundSpeedDecrease = 4f;
    [SerializeField] private float airSpeedIncrease = 0.5f;

    
    [Header("Velocity and Gravity Settings")]
    private float gravity = -9.81f;
    private Vector3 velocity;
    [SerializeField] private float jumpHeight = 1f;

    
    [Header("Ground Settings")]
    public bool onGround = false;
    [SerializeField] private Transform groundCheck;


    [Header("Crouching Settings")]
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private float crouchHeight = 1.78f/2f;
    [SerializeField][Range(0,1)] private float crouchProgress = 0f;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private LayerMask mask;
    private float originHeight;
    

    void Awake(){
        originHeight = characterController.height;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    void Update(){

        if(Cursor.lockState==CursorLockMode.Locked){
            //print("Movbing");
            isCrouching = Input.GetKey(KeyCode.LeftControl);
            if(isCrouching){
                crouchProgress+=Time.deltaTime;
                crouchProgress = Mathf.Clamp(crouchProgress,0,1);
                characterController.height = Mathf.Lerp(characterController.height,crouchHeight,crouchProgress);
            }else{
                crouchProgress-=Time.deltaTime;
                crouchProgress = Mathf.Clamp(crouchProgress,0,1);
                characterController.height = Mathf.Lerp(originHeight,characterController.height,crouchProgress);;
            }
            capsuleCollider.height = characterController.height;
            onGround = Physics.CheckSphere(groundCheck.transform.position,0.1f,mask) ? true : false;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            

            if(onGround && currentSpeed > maxGroundSpeed){
                currentSpeed -= groundSpeedDecrease*Time.deltaTime;
            }else{
                if(currentSpeed < 2){
                    currentSpeed = maxGroundSpeed;
                }else{
                    currentSpeed += z * (airSpeedIncrease*Time.deltaTime);
                }
            }
            float   xSpeed = currentSpeed * x;
            float   zSpeed = currentSpeed * z;

            Vector3 move = (transform.right * xSpeed + transform.forward * zSpeed);

            
            characterController.Move(velocity * Time.deltaTime);
            

            characterController.Move(move * currentSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && onGround){

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.transform.position,0.1f);
    }
}
