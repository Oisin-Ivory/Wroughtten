using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Pickup_Inventory : MonoBehaviour
{

    #region Pickup
    [SerializeField] private float pickUpRange = 2f;
    #endregion
    #region LeftHand
    [SerializeField] private Transform leftHand; //Position for left hand
    [SerializeField] private bool leftHandEmpty = true;
    [SerializeField] private GameObject leftHandObject;
    
    #endregion

    #region RightHand
    [SerializeField] private Transform rightHand; //Position for right hand
    [SerializeField] private bool rightHandEmpty = true;
    [SerializeField] private GameObject rightHandObject;

    #endregion
    #region Weapon
    [SerializeField] private Transform weaponPosition;
    [SerializeField] private GameObject mainWeapon;
    [SerializeField] private bool weaponHolstered = false;
    #endregion
    #region Inventory
    [SerializeField] private GameObject[] inventoryArray = new GameObject[4];
    [SerializeField] private bool[] inventroySlotInUse ={false,false,false,false};

    #endregion
    [SerializeField] private Camera playerCamera;


    #region pickup


    private void PickUp(bool hand){
        RaycastHit hit;
        Debug.DrawRay(playerCamera.transform.position,playerCamera.transform.forward,Color.blue,Mathf.Infinity);
        if(Physics.Raycast(playerCamera.transform.position,playerCamera.transform.forward, out hit, pickUpRange)){
            GameObject hitGameObject = hit.transform.gameObject;

            if(hitGameObject==null) return;

            if(hitGameObject.TryGetComponent<Saiga>(out Saiga saigaScript)){
                if(rightHandEmpty)
                    PickUpWeapon(saigaScript.gameObject);
            }

            if(hitGameObject.TryGetComponent<Magazine>(out Magazine magToPickUp)){
                if(!magToPickUp.isInWeapon){
                PickUpMagazine(magToPickUp.gameObject,hand);
                }

            }
            if(hitGameObject.TryGetComponent<Ammo>(out Ammo roundToPickUp)){
                if(!roundToPickUp.isInMag){
                PickUpMagazine(roundToPickUp.gameObject,hand);
                }
            }
            if(hitGameObject.TryGetComponent<Spawner_Button>(out Spawner_Button button)){
                button.SpawnObject();
            }
            if(hitGameObject.TryGetComponent<Scoring_StartButton>(out Scoring_StartButton startButton)){
                startButton.StartCourse();
            }
            if(hitGameObject.TryGetComponent<Scoring_Reset>(out Scoring_Reset resetButton)){
                resetButton.Reset();
            }


        }
    }
    private void PickUpWeapon(GameObject weapon){ //todo both hands must be empty

        weapon.GetComponent<Saiga>().isEquiped = true;
        weapon.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
        mainWeapon = weapon;
        mainWeapon.transform.position = weaponPosition.position;
        mainWeapon.transform.rotation = weaponPosition.rotation;
        mainWeapon.transform.parent = weaponPosition;
        rightHandEmpty=false;
                
            
        
    }

    private void PickUpMagazine(GameObject magazine,bool hand){

        if(hand && rightHandEmpty){
            rightHandEmpty = false;
            rightHandObject = magazine;
            magazine.GetComponent<Rigidbody>().constraints =    RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            magazine.transform.position = rightHand.position;
            magazine.transform.rotation = rightHand.rotation;
            magazine.transform.parent = rightHand;

        }

        if(!hand && leftHandEmpty){
            leftHandEmpty = false;
            leftHandObject = magazine;
            magazine.GetComponent<Rigidbody>().constraints =    RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            magazine.transform.position = leftHand.position;
            magazine.transform.rotation = leftHand.rotation;
            magazine.transform.parent = leftHand;

        }
    }

    private void PickUpAmmo(GameObject ammo,bool hand){

        if(hand && rightHandEmpty){
            rightHandEmpty = false;
            rightHandObject = ammo;
            ammo.GetComponent<Rigidbody>().constraints =    RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            ammo.transform.position = rightHand.position;
            ammo.transform.rotation = rightHand.rotation;
            ammo.transform.parent = rightHand;

        }

        if(!hand && leftHandEmpty){
            leftHandEmpty = false;
            leftHandObject = ammo;
            ammo.GetComponent<Rigidbody>().constraints =    RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            ammo.transform.position = leftHand.position;
            ammo.transform.rotation = leftHand.rotation;
            ammo.transform.parent = leftHand;

        }
    }
    #endregion
    
    #region drop

    private void Drop(bool hand){
        if(!weaponHolstered && mainWeapon!=null){
            mainWeapon.GetComponent<Saiga>().isEquiped = false;
            mainWeapon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            mainWeapon.transform.parent = null;
            mainWeapon = null;
            rightHandEmpty = true;
            return;
        }
        if(!rightHandEmpty && hand){
            rightHandObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            rightHandObject.transform.parent = null;
            rightHandObject = null;
            rightHandEmpty = true;
            return;
        }
        if(!leftHandEmpty && !hand){
            leftHandObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            leftHandObject.transform.parent = null;
            leftHandObject = null;
            leftHandEmpty = true;
            return;
        }

    }

    #endregion
    
    #region inventory
    
    private void InventoryManageItem(int index){
        //Add item as parent to inventory array gameobject, disable rigidbody etc..

        //if hand is full and slot is used return
        //if hand is empty and slot contains item retrieve
        //if hand is fill and slot is empty store

        if(!leftHandEmpty && inventroySlotInUse[index])return;
        if(leftHandEmpty && inventroySlotInUse[index]){
            //Debug.Log("attempting to retrieve item "+inventoryArray[index].transform.GetChild(1).gameObject.name);

            InventoryRetrieveItem(index);
            return;
        }
        if(!leftHandEmpty&&!inventroySlotInUse[index]){
            InventoryStoreItem(index);
            return;
        }

       

    }

    private void InventoryRetrieveItem(int index){
        
        leftHandObject = inventoryArray[index].transform.GetChild(1).gameObject;
        // Debug.Log("Left handObject is " + leftHandObject.name);
        leftHandObject.transform.parent = leftHand.transform;
        leftHandObject.transform.position = leftHand.transform.position;
        leftHandObject.transform.rotation = leftHand.transform.rotation;
        inventroySlotInUse[index] = false;
        leftHandEmpty = false;
    }
    private void InventoryStoreItem(int index){
        leftHandObject.transform.parent = inventoryArray[index].transform;
        leftHandObject.transform.position = inventoryArray[index].transform.position;
        leftHandObject.transform.rotation = inventoryArray[index].transform.rotation;
        inventroySlotInUse[index] = true;
        leftHandObject = null;
        leftHandEmpty = true;
    }

    private void HolsterWeapon(){
        //UnHolstering Weapon
        
        if(rightHandEmpty && weaponHolstered){ //holster must make right hand empty/full
            weaponHolstered = false;
            mainWeapon.SetActive(true);
            rightHandEmpty = false;
            return;
        }
        //Holster weapon
        //if weapon isn't holstered
        if(!weaponHolstered && mainWeapon!=null){
            weaponHolstered = true;
            mainWeapon.SetActive(false);
            rightHandEmpty = true;

        }
    }

    #endregion

    #region Inputs

    private void ManageInputs(){
        #region Pickup
        if(Input.GetButtonDown("Interact")){
            PickUp(true);
        }

        if(Input.GetButtonDown("InteractLeft")){
            PickUp(false);
        }
        #endregion
        #region Drop
        if(Input.GetButtonDown("Drop")){
            //if(rightHandEmpty)
            Drop(true);
        }
        if(Input.GetButtonDown("DropLeft")){
            //if(rightHandEmpty)
            Drop(false);
        }
        #endregion
        #region holster
        #region inventory
        if(Input.GetButtonDown("Store1")){
            InventoryManageItem(0);
        }
        if(Input.GetButtonDown("Store2")){
            InventoryManageItem(1);
        }
        if(Input.GetButtonDown("Store3")){
            InventoryManageItem(2);
        }
        if(Input.GetButtonDown("Store4")){
            InventoryManageItem(3);
        }

        #endregion
        if(Input.GetButtonDown("Holster")){
            HolsterWeapon();
        }
        #endregion
    }
    #endregion
    void Update(){
        if(Cursor.lockState==CursorLockMode.Locked){
            ManageInputs();
        }
        ManageObjects();
    }

    private void ManageObjects(){
        if(rightHandObject!=null){
           // Debug.Log("Parent is :"+rightHandObject.transform.parent.name);
           // Debug.Log("right hand is :"+rightHand.gameObject.name);
           // Debug.Log(rightHandObject.transform.parent == rightHand?"they are the same":"they're not");
            if(rightHandObject.transform.parent != rightHand){
                rightHandObject = null;
                rightHandEmpty = true;
            }
        }
        if(leftHandObject!=null){
            if(leftHandObject.transform.parent != leftHand){
                leftHandObject = null;
                leftHandEmpty = true;
            }
        }
    }

    void FixedUpdate(){
        if(mainWeapon!=null){
            mainWeapon.transform.rotation = playerCamera.transform.rotation;
        }
    }

}
