using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltAction : MonoBehaviour
{
    float boltProgress = 0f;
    bool boltStageOne = true;
    bool boltStageTwo = false;
    float stageOneProgress,stageTwoProgress = 0f;
    [SerializeField] float boltSpeedModifier = 2f;
    public Transform positionClosed,position1,position2;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        boltProgress = (stageOneProgress+stageTwoProgress)/2;
        if(Input.GetButtonDown("Jump")){
            Debug.Log("BoltProgress: "+boltProgress);
        }
        if(boltStageOne){
            //Debug.Log("Stage 1: "+stageOneProgress);
            //Debug.Log(Mathf.Abs(Input.GetAxis("Mouse X")));
            if(Mathf.Abs(Input.GetAxis("Mouse X"))>1)
                stageOneProgress += (Input.GetAxis("Mouse X")*-1) * Time.deltaTime * boltSpeedModifier;

            stageOneProgress = Mathf.Clamp(stageOneProgress,0,1);
            this.transform.position = Vector3.Lerp(positionClosed.position,position1.position,stageOneProgress);
            this.transform.rotation = Quaternion.Lerp(positionClosed.rotation,position1.rotation,stageOneProgress);
            if(stageOneProgress==1){
                boltStageOne = false;
                boltStageTwo = true;
            }
        }
        if(boltStageTwo){
            Debug.Log("Stage 2: "+stageTwoProgress);
            //Debug.Log(Mathf.Abs(Input.GetAxis("Mouse Y")));
            if(Mathf.Abs(Input.GetAxis("Mouse Y"))>1)
                stageTwoProgress += (Input.GetAxis("Mouse Y")*-1) * Time.deltaTime * boltSpeedModifier;

            if(stageTwoProgress<0){
                stageOneProgress = 0.99f;
                boltStageOne = true;
                boltStageTwo = false;
            }
            stageTwoProgress = Mathf.Clamp(stageTwoProgress,0,1);
            this.transform.position = Vector3.Lerp(position1.position,position2.position,stageTwoProgress);
            this.transform.rotation = Quaternion.Lerp(position1.rotation,position2.rotation,stageTwoProgress);
            
        }

    }
}
