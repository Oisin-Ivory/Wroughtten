using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalMagazine : MonoBehaviour, IMagazine
{

    [SerializeField] Stack<GameObject> magazineStack = new Stack<GameObject>();
    [SerializeField] int magazineCapacity = 8;
    [SerializeField] Transform[] ammoStorePos = new Transform[5];
    private BoxCollider ammoDeposit;
    [SerializeField] int magazinecount;
    [SerializeField] public bool canAcceptAmmo = true;
    [SerializeField]public string[] compAmmoTags;
    void Awake(){
        ammoDeposit = gameObject.GetComponent<BoxCollider>();
    }
    void Update(){
        UpdateBulletPosition();
    }

    public void UpdateBulletPosition(){
        if(magazineStack.Count==0)return;
        int index = 0;
        foreach (GameObject round in magazineStack){
            //print("This is round: "+round.name);
            if(index < ammoStorePos.Length){
                //print("Storing round "+round.name+" at position "+ammoStorePos[index].name);
                round.transform.position = ammoStorePos[index].position;
                round.transform.rotation = ammoStorePos[index].rotation;
            }else{
                round.transform.position = ammoStorePos[ammoStorePos.Length-1].position;
                round.transform.rotation = ammoStorePos[ammoStorePos.Length-1].rotation;
            }
            index++;
        }
    }

    private void OnTriggerEnter(Collider colObj){
        //print("Collider2D with" + colObj.name);
            if(colObj.TryGetComponent<Ammo>(out Ammo ammoToDeposit)){
                if(!Ammo.IsCompatableAmmoTypes(compAmmoTags,ammoToDeposit.compAmmoTags))return;
                LoadRound(colObj.gameObject);
            }
            UpdateBulletPosition();
        
    }

    public void LoadRound(GameObject roundToLoad){
        if(!canAcceptAmmo)return;
        if(magazineStack.Count < magazineCapacity){
            if(roundToLoad.GetComponent<Ammo>().isInMag)return;
            print("loadingRound: "+roundToLoad.name);
            magazineStack.Push(roundToLoad);
            roundToLoad.transform.parent = this.transform; //Make Parent the magazine, freeze rigidbody to stop movement
            roundToLoad.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            roundToLoad.GetComponent<CapsuleCollider>().enabled = false; //Disable collider when in the magazine
            roundToLoad.GetComponent<Ammo>().isInMag = true;
        }
        UpdateBulletPosition();
        
    }

    public GameObject FeedRound(){
        if(magazineStack.Count==0)return null;
        return magazineStack.Pop();
    }
    
    public int getBulletCount(){
        return magazineStack.Count;
    }

    public bool getCanAcceptAmmo()
    {
        return canAcceptAmmo;
    }

    public void setCanAcceptAmmo(bool state)
    {
        canAcceptAmmo = state;
    }

    public string[] getCompAmmoTags()
    {
        return compAmmoTags;
    }
}
    



