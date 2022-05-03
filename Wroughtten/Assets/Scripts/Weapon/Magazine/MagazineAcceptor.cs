using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineAcceptor : MonoBehaviour
{
    [SerializeField] Weapon attachedWeapon = null;
    [SerializeField] Transform attachmentPoint  = null;
    [SerializeField] GameObject attachedMagazine = null;
    [SerializeField] bool magazineAttached = false;
    [SerializeField] float magazineLaunchMultiplier = 75f;
    [SerializeField] Vector3 magazineEjectDir = Vector3.zero;
    float timeSinceLastFeed = 0f;
    [SerializeField]public string[] compAmmoTags;

    void Update(){
        if(attachedMagazine==null)return;
        attachedMagazine.gameObject.transform.localPosition = Vector3.zero;
        attachedMagazine.transform.rotation = attachmentPoint.rotation; 
        attachedMagazine.transform.parent = attachmentPoint;   
    }

    void OnTriggerEnter(Collider col){
        print(col.gameObject.name);
        GameObject colGO = col.gameObject;
        if(colGO.TryGetComponent<DetachableMagazine>(out DetachableMagazine magazineToAttach)){
            if(magazineToAttach.isAttached)return;
            if(!Ammo.IsCompatableAmmoTypes(magazineToAttach.compAmmoTags,compAmmoTags))return;
            print(col.gameObject.name + " is compatable");
        
            attachedMagazine = magazineToAttach.gameObject;
            magazineAttached = true;
            attachedMagazine.transform.parent = attachmentPoint; //Make Parent the magazine, freeze rigidbody to stop movement
            attachedMagazine.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            attachedMagazine.GetComponent<BoxCollider>().enabled = false; //Disable collider when in the magazine
            attachedMagazine.GetComponent<DetachableMagazine>().isAttached = true;
            attachedMagazine.gameObject.transform.localPosition = Vector3.zero;
            attachedMagazine.gameObject.transform.localRotation = Quaternion.identity;
            attachedWeapon.setMagazine(magazineToAttach);
           }

    }

    public void Ejectmag(){
        print("ejecting");
        if(attachedMagazine==null)return;
        
        magazineAttached = false;
        attachedMagazine.transform.parent = null;
        attachedMagazine.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.None;
        attachedMagazine.GetComponent<BoxCollider>().enabled = true;
        attachedMagazine.GetComponent<Rigidbody>().AddForce(gameObject.transform.up*magazineLaunchMultiplier + magazineEjectDir*Random.Range(10,40));
        
        StartCoroutine(attachedMagazine.GetComponent<DetachableMagazine>().EjectedMagazine(1f));
        attachedMagazine = null;
        attachedWeapon.nullMagazine();
    }
}
