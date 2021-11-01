using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperClipAcceptor : MonoBehaviour
{
    Weapon attachedWeapon = null;
    [SerializeField] Transform attachmentPoint  = null;
    [SerializeField] GameObject attachedClip = null;
    [SerializeField] bool clipAttached = false;
    BoxCollider attachmentTrigger = null;
    float timeSinceLastFeed = 0f;

    void Awake(){
        attachmentTrigger = gameObject.GetComponent<BoxCollider>();
    }
    void Update(){
        timeSinceLastFeed+=Time.deltaTime;
        if(clipAttached){
            if(Input.GetKeyDown("r")){
                FeedRounds(0,Input.GetAxis("Mouse Y"));
            }
        }
    }
    void OnTriggerEnter(Collider col){
        GameObject colGO = col.gameObject;
        if(colGO.TryGetComponent<StripperClip>(out StripperClip clipToAttach)){
            attachedClip = clipToAttach.gameObject;
            clipAttached = true;
            attachedClip.transform.parent = attachmentPoint; //Make Parent the magazine, freeze rigidbody to stop movement
            attachedClip.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            attachedClip.GetComponent<BoxCollider>().enabled = false; //Disable collider when in the magazine
            attachedClip.GetComponent<StripperClip>().isAttached = true;
        }

    }

    public void FeedRounds(float inputX, float inputY){
        //print("Moving Bolt " + inputY);
        if(Mathf.Abs(inputY)>1 && timeSinceLastFeed > 0.25f){
            attachedWeapon.getMagazine().LoadRound(attachedClip.GetComponent<StripperClip>().FeedRound());
            timeSinceLastFeed  = 0f;
        }
                
    }
}
