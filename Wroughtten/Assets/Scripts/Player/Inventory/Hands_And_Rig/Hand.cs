using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    [SerializeField] private Vector3 originalHandPos;
    [SerializeField] public bool handEmpty = true;
    [SerializeField] public GameObject handObject;
    [SerializeField] private bool hadRigidBody;
    private bool isLocked = false;


    void Update(){
        ManageObjects();
    }

    void Awake()
    {
        originalHandPos = new Vector3(gameObject.transform.localPosition.x,gameObject.transform.localPosition.y,gameObject.transform.localPosition.z);
    }


    public void PickUpObject(GameObject obj){
        if(obj==null || !handEmpty || isLocked)return;
        if(obj.TryGetComponent<PickUpAble>(out PickUpAble objPickupState)){
            if(!objPickupState.canPickup){
                return;
            }
        }else{
            return;
        }

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
        handObject.transform.localPosition = Vector3.zero;
        handEmpty = false;
    }

    public void DropObject(){
        if(handEmpty || isLocked)return;

        if(hadRigidBody){
            handObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        handObject.transform.parent = null;
        handObject = null;
        handEmpty = true;
    }

    public IEnumerator lerpToPosition(Transform pos, float time){
        isLocked = true;
        float timeSpent = 0;
        while(timeSpent<time){
            print("timeSpent: "+timeSpent);
            timeSpent+=Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,pos.transform.position,timeSpent/time);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation,pos.transform.rotation,timeSpent/time);
            yield return true;        
        }
        
        yield return true;
        gameObject.transform.localPosition = originalHandPos;
        isLocked = false;
    }

    private void ManageObjects(){
        if(handObject!=null){
           if(handObject.transform.parent != gameObject.transform){
                print(handObject.name + " is not a child of "+gameObject.name+" it is a child of "+handObject.transform.parent);
                handObject = null;
                handEmpty = true;
            }
        }
    }


    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,0.2f);
    }
}
