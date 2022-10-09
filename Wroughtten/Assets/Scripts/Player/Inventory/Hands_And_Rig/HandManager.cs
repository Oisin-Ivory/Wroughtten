using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] Hand[] hands; 

    [SerializeField] Hand weaponHand;

    [SerializeField] float pickupDistance = 4f;
    private bool reloading = false;
    private float timeSinceReload = 10f;
    [SerializeField] private bool ADS = false;

    [SerializeField] ChestRigManager chestRigManager;


    public bool IsADS(){
        return ADS;
    }
    private void Update(){
        timeSinceReload+=Time.deltaTime;
        PickUpObject("e",1);
        PickUpObject("q",0);
        DropObject("x",1);
        DropObject("z",0);
        CheckForRigInput();
        if(Input.GetKeyDown("r")){
            Reload();
        }
        HandleInput();
    }


    public bool HasWeapon(){
        if(hands[0].handObject == null)
            return false;
            
        return hands[0].handObject.TryGetComponent<Weapon>(out Weapon weapon);
    }

    public Weapon GetWeapon(){
        return hands[0].handObject.GetComponent<Weapon>();
    }
    private void HandleInput()
    {
        if(!hands[0].isEmpty() && hands[1].isEmpty()){
            if(hands[0].handObject.TryGetComponent<Weapon>(out Weapon weapon)){
                weapon.HandleInputs();
            }
        }
        if(Input.GetMouseButtonDown(1)){
            ADS = !ADS;
            UpdateHandPosition();
        }
    }

    #region reloading
    private void Reload(){
        if(reloading)return;
        print("reloading");
        if(!hands[0].handEmpty && hands[1].handEmpty){
            if(hands[0].handObject.TryGetComponent<Weapon>(out Weapon wpn)){
                if(wpn.detachableMagazine){
                    wpn.magAcceptor.Ejectmag();
                }
            }
        }

        if(hands[0].handEmpty || hands[1].handEmpty){
            return;
        }
        if(hands[0].handObject.TryGetComponent<Reloadable>(out Reloadable reloadablescript) 
        && hands[1].handObject.TryGetComponent<Loadable>(out Loadable itemToLoad)){
            if(!reloadablescript.willAccept(itemToLoad))return;
            //print("reloading: "+hands[0].handObject.gameObject.name);
            
            if(timeSinceReload<1f)return;
            Debug.Log("Starting Coroutine");
            StartCoroutine(hands[1].lerpToPositionReload(reloadablescript,0.25f));
            timeSinceReload = 0;
        }
        
    }
#endregion

#region  objectPicking and Dropping
private void PickUpObject(string key,int hand){
        if(Input.GetKeyDown(key)){
            GameObject pickUpObj = GetObjectLookingAt();
            
            if(pickUpObj!=null){
                //print(pickUpObj.name);
                hands[hand].PickUpObject(pickUpObj);
            }
            UpdateHandPosition();
        }
    }

    private void DropObject(string key,int hand){
        if(Input.GetKeyDown(key)){
            hands[hand].DropObject();
            UpdateHandPosition();
        }
    }
    public GameObject GetObjectLookingAt(){
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

#endregion
    private void UpdateHandPosition(){
        print("updating hand position");
        if(hands[0].isEmpty()){
            hands[0].ResetHandPos();
        }
        if(hands[1].isEmpty()){
            hands[1].ResetHandPos();
        }
        if(!hands[0].isEmpty() && hands[1].isEmpty()){
            if(hands[0].handObject.TryGetComponent<Weapon>(out Weapon weapon)){
                if(ADS)
                    StartCoroutine(hands[0].lerpToLocalPosition(weapon.getWeaponAdsPosition(),0.25f));
                else
                    StartCoroutine(hands[0].lerpToLocalPosition(weapon.getWeaponPosition(),0.25f));
            }
        }else{
            hands[0].ResetHandPos();
        }
    }

#region ChestRig
    public GameObject TransferItem(int hand){
        if(hands[hand].isEmpty()) return null;
        
        return hands[hand].TransferItem();
    }

    public void RetrieveItem(int slot,int hand){
        GameObject retItem = chestRigManager.RetrieveItem(slot);
        if(retItem == null)return;
        hands[hand].PickUpObject(retItem);
    }

    public void RetrieveStoreItem(int slot, int hand){
        if(hands[hand].isLocked)return;
        if(hands[hand].isEmpty()){
            RetrieveItem(slot,hand);
        }else{
            if(hands[hand].handObject.GetComponent<InteractionProperties>().canStore){
                

                if(!chestRigManager.IsFree(slot)) return;
                chestRigManager.StoreObject(slot,TransferItem(hand));
            }
        }
        UpdateHandPosition();
    }

    private void CheckForRigInput(){
        int handIndex;
        if((!hands[1].isEmpty() || hands[0].isEmpty()) || (hands[0].handObject.TryGetComponent<Weapon>(out Weapon wpn) && hands[1].isEmpty())){
            handIndex = 1;
        }else{
            handIndex = 0;
        }

        
        for ( int i = 1; i < 9; ++i ){
            if ( Input.GetKeyDown( "" + i ) )
            {
                RetrieveStoreItem(i-1,handIndex);
                return;
            }
        }

        if(Input.GetKeyDown(KeyCode.H)){
            if(hands[0].isEmpty() && !hands[0].isLocked){
               // print("retrieving holstered item");
                GameObject retItem = chestRigManager.RetrieveHolstered();
                //Debug.Log("Retrieved item: "+retItem.name);
                if(retItem == null)return;
                hands[0].PickUpObject(retItem);
            }else if(!hands[0].isEmpty()){
//                print("holstered item: " + hands[0].handObject.name);
                if(hands[0].handObject.TryGetComponent<Weapon>(out Weapon holsterWeapon) && chestRigManager.HolsterIsEmpty()){
                    if(hands[0].handObject == null)return;
                    try{
                    chestRigManager.HolsterWeapon(TransferItem(0));
                    }catch{
                        print("Failed to store weapon due to rapid swap speed");
                    }
                }
            }
        }
    }
#endregion
}
