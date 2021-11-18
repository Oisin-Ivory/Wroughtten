using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] Hand[] hands; 

    [SerializeField] Hand weaponHand;

    [SerializeField] float pickupDistance = 4f;
    private bool reloading = false;

    private void Update(){
        PickUpObject("e",1);
        PickUpObject("q",0);
        DropObject("x",1);
        DropObject("z",0);
        if(Input.GetKeyDown("r")){
            Reload();
        }
    }

    private void Reload(){
        if(reloading)return;
        if(hands[0].handEmpty || hands[1].handEmpty){
            return;
        }
        if(hands[0].handObject.TryGetComponent<Reloadable>(out Reloadable reloadablescript) 
        && hands[1].handObject.TryGetComponent<Loadable>(out Loadable itemToLoad)){
            if(!reloadablescript.willAccept(itemToLoad))return;
            print("reloading: "+hands[0].handObject.gameObject.name);
            reloading = true;
            StartCoroutine(hands[1].lerpToPosition(reloadablescript.getLoadingPosition(itemToLoad),1f));
            reloading = false;
        }
        
    }

private void PickUpObject(string key,int hand){
        if(Input.GetKeyDown(key)){
            GameObject pickUpObj = GetObjectLookingAt();
            
            if(pickUpObj!=null)
                print(pickUpObj.name);
                hands[hand].PickUpObject(pickUpObj);
        }
    }

    private void DropObject(string key,int hand){
        if(Input.GetKeyDown(key)){
            GameObject pickUpObj = GetObjectLookingAt();
            
            if(pickUpObj!=null)
                print(pickUpObj.name);
                hands[hand].DropObject();
        }
    }
    private GameObject GetObjectLookingAt(){
        Debug.DrawRay(Camera.main.transform.position,
                            Camera.main.transform.forward,Color.green);
        RaycastHit hit;
        if(Physics.Raycast( Camera.main.transform.position,
                            Camera.main.transform.forward,
                            out hit,pickupDistance)){
            return hit.collider.gameObject;
        }
        return null;
    }
}
