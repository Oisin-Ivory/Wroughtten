using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRigSlot : MonoBehaviour
{
    [SerializeField] private GameObject storedObject;
    

    public void StoreObject(GameObject obj){
        if(storedObject!=null)return;

        storedObject = obj;
        storedObject.transform.position = gameObject.transform.position;
        storedObject.transform.rotation = gameObject.transform.rotation;
        storedObject.transform.parent = gameObject.transform;
    }

    public GameObject RetrieveObject(){
        if(storedObject==null) return null; //incase empty gameobject
        if(storedObject.TryGetComponent<Rigidbody>(out Rigidbody rb)){
            rb.constraints = RigidbodyConstraints.None;
        }
        
        return storedObject;
    }

    public bool hasObject(){
        return !(storedObject==null);
    }
    public void NullObj(){
        storedObject = null;
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,0.05f);
    }
}
