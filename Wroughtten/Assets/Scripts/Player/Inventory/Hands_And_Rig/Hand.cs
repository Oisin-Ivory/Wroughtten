using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    [SerializeField] private Vector3 originalHandPos;
    [SerializeField] public bool handEmpty = true;
    [SerializeField] public GameObject handObject;
    [SerializeField] private bool hadRigidBody;
    public bool isLocked = false;

    void Update(){
        ManageObjects();
    }

    void Awake()
    {
        originalHandPos = new Vector3(gameObject.transform.localPosition.x,gameObject.transform.localPosition.y,gameObject.transform.localPosition.z);
    }


    public bool isEmpty(){
        return handEmpty;
    }

    public void PickUpObject(GameObject obj){
        //print("picking up: "+obj.name);
        
        //print("hand empty: "+!handEmpty + "\nIsLocked: "+isLocked);
        if(obj==null || !handEmpty || isLocked)return;
        //print("pasted first: "+obj.name);
        if(obj.TryGetComponent<InteractionProperties>(out InteractionProperties objPickupState)){
            if(!objPickupState.canPickup){
                return;
            }
            //print("pasted second: "+obj.name);
        }else{
            return;
        }
            
        print("pasted third: "+obj.name);
        if(obj.TryGetComponent<Rigidbody>(out Rigidbody objRB)){
            objRB.constraints =
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            hadRigidBody = true;
        }else{
            hadRigidBody = false;
        }
        obj.transform.parent = gameObject.transform;
        obj.transform.rotation = new Quaternion(0,0,0,0);
        handObject = obj;
        
        print("picked up: "+obj.name);
        StartCoroutine(lerpObjToPosition(obj,Vector3.zero,0.25f));
        
        handEmpty = false;
    }

    public IEnumerator lerpObjToPosition(GameObject obj, Vector3 pos, float time){
        
        isLocked = true;
        float timeSpent = 0;
        while(timeSpent<time){
            //print("timeSpent: "+timeSpent);
            timeSpent+=Time.deltaTime;
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition,pos,timeSpent/time);
            yield return true;        
        }
        
        yield return true;
        isLocked = false;
    }

    public GameObject TransferItem(){
        if(handEmpty || isLocked) return null;
        GameObject obj = handObject;
        handObject.transform.parent = null;
        handObject = null;
        handEmpty = true;
        return obj;
    }


    public void DropObject(){
        if(handEmpty || isLocked)return;

        if(hadRigidBody){
            handObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        print("Setting parent to null in /Hand/DropObject");
        handObject.transform.parent = null;
        handObject = null;
        handEmpty = true;
    }

#region Hand Movement Lerps

    public IEnumerator lerpToLocalPosition(Vector3 pos, float time){
        isLocked = true;
        float timeSpent = 0;
        while(timeSpent<time){
            //print("timeSpent: "+timeSpent);
            timeSpent+=Time.deltaTime;
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition,pos,timeSpent/time);
            yield return true;        
        }
        
        yield return true;
        isLocked = false;
    }

    public IEnumerator lerpToPositionReload(Reloadable reloadable, float time){
        Vector3 pos = Vector3.zero;
        if(isLocked)yield break;
        isLocked = true;
        float timeSpent = 0;
        while(timeSpent<time){
            //print("timeSpent: "+timeSpent);
            timeSpent+=Time.deltaTime;
            if(handObject!=null){
                pos = reloadable.getLoadingPosition(handObject.GetComponent<Loadable>()).position;
            }
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,pos,timeSpent/time);
            yield return true;        
        }
        
        yield return true;
        ResetHandPos();
        isLocked = false;
    }

#endregion
    public void ResetHandPos(){
        StartCoroutine(lerpToLocalPosition(originalHandPos,1f));
    }
    private void ManageObjects(){
        if(handObject!=null){
           if(handObject.transform.parent != gameObject.transform){
                //print(handObject.name + " is not a child of "+gameObject.name+" it is a child of "+handObject.transform.parent);
                handObject = null;
                handEmpty = true;
            }
        }
    }

    public void UpdateWeaponPosition(bool ads){
        if(handObject==null)return;
        if(handObject.TryGetComponent<Weapon>(out Weapon wpn)){
            if(ads)
                wpn.gameObject.transform.localPosition = wpn.getWeaponAdsPosition();
            else
                wpn.gameObject.transform.localPosition = wpn.getWeaponPosition();
            
        }
    }

    public Vector3 getOrigin(){
        return originalHandPos;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,0.025f);
    }
}
