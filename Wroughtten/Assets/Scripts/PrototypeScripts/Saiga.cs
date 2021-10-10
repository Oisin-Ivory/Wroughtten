using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saiga : MonoBehaviour
{
    #region Mag Variables
    [SerializeField] GameObject magazine;
    [SerializeField] Transform magAttachPoint;
    private bool magazineInWeapon = false;

    #endregion

    #region Chamber Variables
    [SerializeField] GameObject chamberedRound;
    [SerializeField] Transform boltTransform;
    private bool roundInChamber = false;

    [SerializeField] private Transform barrellExit;

    #endregion

    #region Animation
    private Saiga_Animation saigaAnimator;
    #endregion

    #region Firing

    float timeSinceLastShot = 0f;
    [SerializeField] float fireRate = 0.8f;
    int fireMode = 0; // 0-safe 1-auto 2-semi
    float range = 300.0f; 
    [SerializeField] float maxSpread = 1f;

    #endregion

    #region PlayerAction
    public bool isEquiped = false;
    #endregion
    
    #region Shell Transforms
    [SerializeField] Transform shellEjection;
    #endregion
    
    #region Partical Effects
    [SerializeField] private ParticleSystem[] particles;

    #endregion
    
    #region Sound
    [SerializeField] private AudioSource gunShot;
    [SerializeField] private AudioSource boltAction;

    [SerializeField] private AudioSource safteyAction;
    #endregion

    #region Saftey

    private float[] safteyPositions = new float[3]{0,-20,-25};
    [SerializeField] private GameObject safteyLever;

    #endregion
    #region Magazine
    private void OnTriggerEnter(Collider colObj){
        //Debug.Log("Object entered trigger :" + colObj.name);
        if(!magazineInWeapon){
            if(colObj.TryGetComponent<Magazine>(out Magazine magToAttach)){

                //Debug.Log("Object was Magazine");

                magToAttach.gameObject.transform.parent = gameObject.transform;
                magazine = magToAttach.gameObject;

                colObj.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY| 
                                                                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
                colObj.GetComponent<BoxCollider>().enabled = false; //Disable collider when in the magazine
                magazineInWeapon = true;
                magToAttach.isInWeapon = true;
                
            }
        }
    }

    private void DetachMag(){
        if(magazine==null)return;
        magazine.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.None;
        magazine.GetComponent<BoxCollider>().enabled = true;
        magazine.GetComponent<Magazine>().isInWeapon = false;
        magazine.transform.position += new Vector3(0f,-0.5f,0f);
        magazine.transform.parent = null;
        magazine = null;
        magazineInWeapon = false;
    }

    #endregion

#region Chamber

    private void ChamberRound(){
        if(fireMode==0)return;
        //Method should be called when shooting LIVE ammunition & can be called manually to cycle bolt.
        /*
        Debug.Log("Attempting to Chamber Round");
        Debug.Log("The mag contains " + magazine.GetComponent<Magazine>().getMagAmmo() + " rounds");
        Debug.Log(magazineInWeapon ? "There is a magazine in the weapon" : "There isn't a mag in the weapon");
        Debug.Log(magazineInWeapon ? "There is a round in the chamber" : "There isn't a round in the chamber");
        */
        //Nothing in chamber or mag, can't do anything

        //Debug.Log("requested animator");
        saigaAnimator.RunBoltAnimation();
        boltAction.pitch = (Random.Range(0.8f, 1f));
        boltAction.Play();
        if(!roundInChamber && !magazineInWeapon) return;

        
        
        timeSinceLastShot = 0f; // Any time the bolt is minipulated it needs to go back.
        //No round in chamber && mag /w ammo
        if(!roundInChamber && magazineInWeapon && magazine.GetComponent<Magazine>().getMagAmmo() > 0){
            //Debug.Log("magCondition1");
            //Debug.Log("Mag has ammo");
            chamberedRound = magazine.GetComponent<Magazine>().PassRound();
            chamberedRound.transform.parent = boltTransform.transform; // Collider and Rigidbody are Off
            roundInChamber = true;
            return;
        }
        //Round in chamber, but no round to feed in
        if(roundInChamber && (!magazineInWeapon || magazine.GetComponent<Magazine>().getMagAmmo() == 0)){
            //Debug.Log("magCondition2");
            //Debug.Log("Mag has no ammo");
            EjectChamberedRound();
            return;
        }

        //Round in chamber, and Rounds to feed in
        if(roundInChamber && magazineInWeapon && magazine.GetComponent<Magazine>().getMagAmmo() > 0){
            //Debug.Log("magCondition3");
            //Debug.Log("Mag has ammo");
            EjectChamberedRound();
            chamberedRound = magazine.GetComponent<Magazine>().PassRound();
            chamberedRound.transform.parent = boltTransform.transform; // Collider and Rigidbody are Off
            roundInChamber = true;
            return;
        }
        

    }

    private void EjectChamberedRound(){
        if(!roundInChamber) return;
        chamberedRound.GetComponent<CapsuleCollider>().enabled = true;
        chamberedRound.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        chamberedRound.GetComponent<Ammo>().isInMag = false;
        chamberedRound.transform.parent = null;
        //done Add force away from the saiga transform
        chamberedRound.transform.position = shellEjection.transform.position;
        chamberedRound.GetComponent<Rigidbody>().AddForce(shellEjection.transform.forward*3f);
        chamberedRound = null;
        roundInChamber = false;
        
    }

#endregion

#region Firing

    private void PullTrigger(){
        if(timeSinceLastShot < fireRate) return;

        if(fireMode == 0) return;

        if(!chamberedRound) return;

        if(!chamberedRound.GetComponent<Ammo>().isSpent){

            if(chamberedRound.GetComponent<Ammo>().Shoot()){
                #region Raycasting
                RaycastHit hit;
                //Todo add partical and sound effects
                for(int i = 0; i < chamberedRound.GetComponent<Ammo>().ammoPelletCount ; i++){
                    
                    Vector3 deviation3D = Random.insideUnitCircle * maxSpread; // make some deviation
                    Quaternion rot = Quaternion.LookRotation(Vector3.forward * range + deviation3D);//get rotation
                    Vector3 forwardVector = barrellExit.transform.rotation * rot * Vector3.forward; // apply rotation
                    Debug.DrawRay(barrellExit.transform.position,forwardVector*range, Color.red,Mathf.Infinity);
                    foreach(ParticleSystem particle in particles){
                        particle.Play();
                    }
                    gunShot.pitch = (Random.Range(0.8f, 1f));
                    gunShot.Play();
                    if(Physics.Raycast(barrellExit.transform.position,forwardVector, out hit, range)){
                        GameObject hitGameObject = hit.transform.gameObject;
                        if(hitGameObject==null) return ;
                        if(hitGameObject.TryGetComponent<Health>(out Health health)){
                            health.takeDamage(chamberedRound.GetComponent<Ammo>().ammoDamage);
                            //Debug.Log("Target should have took damage");
                        }else if(hitGameObject.TryGetComponent<DamageRelay>(out DamageRelay damageRelay)){
                            damageRelay.takeDamage(chamberedRound.GetComponent<Ammo>().ammoDamage);
                        }
                    }
                }
                #endregion
                //Debug.Log("Bang!");
                ChamberRound();
                
                //ChamberRound();
                timeSinceLastShot = 0f;
            }
        }
    }

#endregion

#region FireSelector

    private void CycleFireSelection(){
        fireMode++;
        if(fireMode>=3){
            fireMode=0;
        }
        safteyAction.pitch = (Random.Range(0.8f, 1f));
        safteyAction.Play();
        Quaternion rotation = Quaternion.Euler(safteyPositions[fireMode], 0, 0);
        safteyLever.transform.localRotation = rotation;
    }
#endregion

#region Inputs

    void InputManagement(){
        if(Input.GetButtonDown("Jump")){
            DetachMag();
        }
        if(Input.GetButtonDown("chargingHandle")){
            ChamberRound();
        }
        if(fireMode==2){
            if(Input.GetButtonDown("Fire1")){
                PullTrigger();
            }
        }else{
            if(Input.GetButton("Fire1")){
                PullTrigger();
            }
        }
         if(Input.GetButtonDown("Fire3")){
             CycleFireSelection();
         }
    }
#endregion

#region Update function

    void Update(){
        if(Cursor.lockState==CursorLockMode.Locked){
            if(isEquiped){
                
                InputManagement();
            }
            timeSinceLastShot+=Time.deltaTime;
        }
    }
    void FixedUpdate(){
        
        if(magazineInWeapon){
            //Debug.Log("there is " + magazine.name + " in the saiga");
            magazine.transform.position = magAttachPoint.position;
            magazine.transform.rotation = magAttachPoint.rotation;
        }
        if(roundInChamber){
            chamberedRound.transform.position = boltTransform.position;
            chamberedRound.transform.rotation = boltTransform.rotation;
        }
    }

#endregion

#region Start function
    void Start(){
        saigaAnimator = gameObject.GetComponent<Saiga_Animation>();
    }
#endregion
}
