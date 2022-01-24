using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{

    [SerializeField] private Vector3 holsterPos;
    [SerializeField] private Transform holsterTransform;
    [SerializeField] public bool holsterEmpty = true;
    [SerializeField] public GameObject holsteredObject;
    [SerializeField] private bool hadRigidBody;
    public bool isLocked = false;


    void Update(){
        ManageObjects();
    }

    void Awake()
    {
        holsterPos = new Vector3(holsterTransform.transform.localPosition.x,holsterTransform.transform.localPosition.y,holsterTransform.transform.localPosition.z);
    }


    public bool isEmpty(){
        return holsterEmpty;
    }

    public void StoreObj(GameObject obj){
        print("Storing " + obj.name);
        if(!obj.TryGetComponent<Weapon>(out Weapon wpn)) return;
        if(obj==null || !holsterEmpty || isLocked)return;
        
        if(obj.TryGetComponent<InteractionProperties>(out InteractionProperties objPickupState)){
            if(!objPickupState.canPickup){
                return;
            }
        }else{
            return;
        }
            
        obj.transform.parent = gameObject.transform;
        obj.transform.localPosition = holsterTransform.localPosition;
        obj.transform.rotation = holsterTransform.rotation;
        holsteredObject = obj;
        
        holsterEmpty = false;
    }

    public void NullObj(){
        holsteredObject = null;
        holsterEmpty = true;
    }

    public GameObject RetrieveObject(){
        if(holsteredObject==null) return null; //incase empty gameobject
        if(holsteredObject.TryGetComponent<Rigidbody>(out Rigidbody rb)){
            rb.constraints = RigidbodyConstraints.None;
        }
        
        return holsteredObject;
    }

    private void ManageObjects(){
        if(holsteredObject!=null){
           if(holsteredObject.transform.parent != gameObject.transform){
                //print(holsteredObject.name + " is not a child of "+gameObject.name+" it is a child of "+holsteredObject.transform.parent);
                holsteredObject = null;
                holsterEmpty = true;
            }
        }
    }

    public Vector3 getOrigin(){
        return holsterPos;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,0.05f);
    }
}