using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
public class StripperClipAcceptor : MonoBehaviour
{
    [SerializeField] Weapon attachedWeapon = null;
    [SerializeField] Transform attachmentPoint  = null;
    [SerializeField] GameObject attachedClip = null;
    [SerializeField] bool clipAttached = false;
    [SerializeField] float clipLaunchMultiplier = 75f;
    float timeSinceLastFeed = 0f;
    IBolt weaponBolt = null;
    [SerializeField]public string[] compAmmoTags;

    void Awake(){
        //Time.timeScale = (0.1f);
        weaponBolt = gameObject.GetComponent<IBolt>();
    }
    void Update(){
        timeSinceLastFeed+=Time.deltaTime;
        if(clipAttached){
            if(Input.GetKey("r")){
                FeedRounds(0,Input.GetAxis("Mouse Y"));
            }
        }
        if(attachedClip==null)return;
        attachedClip.gameObject.transform.localPosition = Vector3.zero;
        attachedClip.transform.rotation = attachmentPoint.rotation;    
    
    }
    void OnTriggerEnter(Collider col){
        if(!weaponBolt.GetIsHoldingOpen())return;
        print(col.gameObject.name);
        GameObject colGO = col.gameObject;
        if(colGO.TryGetComponent<StripperClip>(out StripperClip clipToAttach)){
            if(!Ammo.IsCompatableAmmoTypes(clipToAttach.compAmmoTags,compAmmoTags))return;
            if(clipToAttach.isAttached)return;
            attachedClip = clipToAttach.gameObject;
            clipAttached = true;
            attachedClip.transform.parent = attachmentPoint; //Make Parent the magazine, freeze rigidbody to stop movement
            attachedClip.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            attachedClip.GetComponent<BoxCollider>().enabled = false; //Disable collider when in the magazine
            attachedClip.GetComponent<StripperClip>().isAttached = true;
            clipToAttach.gameObject.transform.localPosition = Vector3.zero;
            attachedClip.transform.rotation = attachmentPoint.rotation; 
            weaponBolt.SetFreezeState(true);
        }

    }

    public void FeedRounds(float inputX, float inputY){
        if(!clipAttached)return;
        //print("Moving Bolt " + inputY);
        if(inputY<0 && timeSinceLastFeed > 0.05f){
            print("Feeding Round");
            GameObject roundToAdd = attachedClip.GetComponent<StripperClip>().FeedRound();
            if(roundToAdd==null)return;
            attachedWeapon.getMagazine().LoadRound(roundToAdd);

            attachedWeapon.getMagazine().UpdateBulletPosition();
            attachedClip.GetComponent<StripperClip>().UpdateBulletPosition();
            timeSinceLastFeed  = 0f;
        }
        if(inputY>2){
            print("Ejecting Clip");
           Ejectclip();
        }
                
    }

    public void Ejectclip(){
        if(attachedClip==null)return;
        
        clipAttached = false;
        attachedClip.transform.parent = null;
        attachedClip.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.None;
        attachedClip.GetComponent<BoxCollider>().enabled = true;
        attachedClip.GetComponent<Rigidbody>().AddForce(gameObject.transform.up*clipLaunchMultiplier + Vector3.right*Random.Range(10,40));
        
        StartCoroutine(attachedClip.GetComponent<StripperClip>().EjectedClip(1f));
        attachedClip = null;
        weaponBolt.SetFreezeState(false);
    }
}
