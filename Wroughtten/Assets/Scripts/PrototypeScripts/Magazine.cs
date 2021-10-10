using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{

    [SerializeField] Stack<GameObject> magazineStack = new Stack<GameObject>();
    [SerializeField] Transform ammoStorePos;
    [SerializeField] int magazineCapacity = 8;

    public bool isInWeapon = false;
    private BoxCollider ammoDeposit;

    [SerializeField] int magazinecount;
    void Update(){
        magazinecount = magazineStack.Count;
        UpdateTopBullet();
    }

    private void UpdateTopBullet(){
        foreach(GameObject round in magazineStack){
            if(round!=magazineStack.Peek()){
                round.GetComponent<MeshRenderer>().enabled = false;
            }else{
                round.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
    public int getMagAmmo(){
        return magazineStack.Count;
    }
    void Start(){
        ammoDeposit = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider colObj){

        //Debug.Log("Object entered trigger :" + colObj.name);
        if(magazineStack.Count < magazineCapacity && !isInWeapon){
            //Debug.Log("Testing to see ammo");
            if(colObj.TryGetComponent<Ammo>(out Ammo ammoToDeposit)){

                //Debug.Log("Object was ammo");

                magazineStack.Push(colObj.gameObject);
                colObj.transform.parent = gameObject.transform; //Make Parent the magazine, freeze rigidbody to stop movement
                colObj.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
                colObj.GetComponent<CapsuleCollider>().enabled = false; //Disable collider when in the magazine
                colObj.GetComponent<Ammo>().isInMag = true;
                //Debug.Log("Added "+colObj.name + " to magazine");
            }
        }
    }

    void UpdateAmmoPos(){
        //Debug.Log("there is " + magazineStack.Count + " in the mag");
        if(magazineStack.Count >0){
            foreach(GameObject ammo in magazineStack){
                ammo.transform.position = ammoStorePos.position;//Set ammo position to magazine
                ammo.transform.rotation = ammoStorePos.rotation;
            }
        }
    }

    public GameObject PassRound(){
        GameObject round = magazineStack.Pop();
        return round;
    }

    void FixedUpdate(){
        UpdateAmmoPos();
    }
    




}
