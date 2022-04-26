using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperClip : MonoBehaviour
{
    [SerializeField] Stack<GameObject> clipStack = new Stack<GameObject>();
    [SerializeField] int clipCapacity = 8;
    [SerializeField] Transform[] ammoStorePos = new Transform[5];

    [SerializeField]public bool isAttached = false;
    [SerializeField] BoxCollider loadCollider = null;
    [SerializeField]public string[] compAmmoTags;

    public int getClipCapacity(){
        return clipCapacity;
    }
    void Update(){
        UpdateBulletPosition();
    }
    int GetRounds(){
        return clipStack.Count;
    }
    public GameObject FeedRound(){
        if(clipStack.Count==0)return null;
        GameObject roundToFeed = clipStack.Pop();
        roundToFeed.GetComponent<Ammo>().isInMag = false;
        return roundToFeed;
    }

    public void UpdateBulletPosition(){
        if(clipStack.Count==0)return;
        int index = 0;
        foreach (GameObject round in clipStack){
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
                LoadRound(colObj.gameObject);
            }
            UpdateBulletPosition();
        
    }

    private bool checkAmmoComp(Ammo ammo){
        return Ammo.IsCompatableAmmoTypes(compAmmoTags,ammo.compAmmoTags);
    }
    public void LoadRound(GameObject roundToLoad){
        if(!checkAmmoComp(roundToLoad.GetComponent<Ammo>())) return;
        if(clipStack.Count < clipCapacity){
            if(roundToLoad.GetComponent<Ammo>().isInMag) return;
            clipStack.Push(roundToLoad);
            roundToLoad.transform.parent = gameObject.transform; //Make Parent the magazine, freeze rigidbody to stop movement
            roundToLoad.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            roundToLoad.GetComponent<CapsuleCollider>().enabled = false; //Disable collider when in the magazine
            roundToLoad.GetComponent<Ammo>().isInMag = true;
        }
        UpdateBulletPosition();
        
    }

    public IEnumerator EjectedClip(float timeTillCanAttach){
            yield return new WaitForSeconds(timeTillCanAttach);
            isAttached = false;
    }
}
