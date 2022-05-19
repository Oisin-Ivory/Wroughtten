using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBolt : MonoBehaviour, IBolt
{
    //1 is open - 0 is closed
    [Header("Bolt Settings")]
    [SerializeField] Weapon weapon;
    [SerializeField] float boltProgress = 0f;
    [SerializeField] public float boltSpeedModifier = 2f;
    [SerializeField] float boltSpringStrenght = 1f;
    [SerializeField] float ejectRoundAtBoltProgress = 0.85f;
    [SerializeField] float feedRoundAfterBoltProgress = 0.7f;
    float minBoltPos = 0f;
    float maxBoltPos = 1f;
    [SerializeField] float holdBoltOpenAt = 0.9f;
    [SerializeField] public bool autoEjectClip = false;
    [Header("Bolt & Round Positions")]

    [SerializeField] Transform positionClosed;
    [SerializeField] Transform positionOpened;
    
    [SerializeField] GameObject roundPosition;
    [Header("Bolt States")]
    [SerializeField] bool isLocked = false;
    [SerializeField] bool isHeld = false;
    [SerializeField] bool willHoldOpenOnEmpty = true;  
    [SerializeField] bool lockBoltOnEmptyMag = true;
    [SerializeField] public bool holdingOpen = false;
    [SerializeField] public bool freezeBolt = false;
    [SerializeField] bool isRecoiling = false;

    [Header("Round Settings")]
    [SerializeField] GameObject round;
    [SerializeField] bool canTakeRound = false;
    [SerializeField] bool hasRound = false;
    [SerializeField] float roundLaunchMultiplier = 134f;
    [SerializeField] Vector3 ejectRoundsToward = Vector3.up;
    [SerializeField] Vector3 ejectRoundRotate = Vector3.up;
    [SerializeField] float ejectRoundForce = 134f;

    [Header("Recoil Settings")]
    [SerializeField] float recoilStrenght = -4f;
    [SerializeField] Vector3 recoilDir = Vector3.back;
    [SerializeField] float weaponRecoilForce = 0.1f;
    [Header("Firing")]
    
    [SerializeField] Transform barrellExit;
    [SerializeField] float weaponSpread;
    [SerializeField] float weaponSpreadDistance;
    [Header("Sound")]
    [SerializeField] WeaponSoundManager sound;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weapon.setBolt(this);
        sound = weapon.gameObject.GetComponent<WeaponSoundManager>();
        //Time.timeScale = 0.1f;
    }   

    void Update(){
        //isHeld = Input.GetButton("Jump");

        
        if(boltProgress > holdBoltOpenAt){
            EjectRound();
        }

        

        //if(isHeld && !isRecoiling)
        //    UpdateBoltPosition(0,Input.GetAxis("Mouse Y"));

        if(holdingOpen){
            if(boltProgress==1f){
                holdingOpen = false;
                isLocked = false;
                minBoltPos=0f;
            }else if(boltProgress >= holdBoltOpenAt && (weapon.getMagazine()==null || weapon.getMagazine().getBulletCount()==0)){
                minBoltPos = holdBoltOpenAt;
            }
        }
        if(lockBoltOnEmptyMag){
            if(boltProgress==0f && (weapon.getMagazine()==null || weapon.getMagazine().getBulletCount()==0) ){
                holdingOpen = true;
                isLocked = true;
            }
        }

        if(boltProgress == 1){
            canTakeRound = true;
        }
        //Feed the round when bolt progress is .8-.85
        if((boltProgress < ejectRoundAtBoltProgress) && (boltProgress > feedRoundAfterBoltProgress) && !hasRound && canTakeRound){

            if(weapon.getMagazine()!=null)
                round = weapon.getMagazine().FeedRound();

            if(round!=null){
                canTakeRound = false;
                hasRound = true;
            }

        }
        //print(boltProgress);
        if(isRecoiling){
            if(boltProgress!=1){
                isHeld = true;
                UpdateBoltPosition(0,recoilStrenght);
            }else{
                isRecoiling=false;
                if(weapon.getMagazine()==null && willHoldOpenOnEmpty){
                    holdingOpen = true;
                    minBoltPos = holdBoltOpenAt;
                }else if(weapon.getMagazine().getBulletCount()==0 && willHoldOpenOnEmpty){
                    holdingOpen = true;
                    minBoltPos = holdBoltOpenAt;
                }
				
            }
			//UpdateCanTakeRound();
            //return;
        }

        
        if(!isHeld && !isRecoiling){
            UpdateBoltPosition(0,boltSpringStrenght);
        }
    }
	
    private void RecoilBolt(){
        if(hasRound)
            isRecoiling = true;

    }

    public void EjectRound(){
        //print("Ejecting Round");
        if(round==null){
            //canTakeRound = true;
            return;
        }
        round.GetComponent<CapsuleCollider>().enabled = true;
        round.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //round.GetComponent<Ammo>().isInMag = false;
        round.transform.parent = null;
        //done Add force away from the saiga transform
        round.GetComponent<Rigidbody>().AddForce(gameObject.transform.up*roundLaunchMultiplier + Vector3.right*Random.Range(10,40));
        StartCoroutine(round.GetComponent<Ammo>().EjectedRound(3f));
        round = null;
        hasRound = false;
        //canTakeRound = true;
        if(weapon.getMagazine()!=null)
            weapon.getMagazine().UpdateBulletPosition();
    }


    public void UpdateRoundPosition(){
        round.transform.position = roundPosition.transform.position;
        round.transform.rotation = roundPosition.transform.rotation;
    }

    public void UpdateBoltPosition(float inputX, float inputY){
        if(freezeBolt)return;
        float dt = Time.deltaTime;
        
        dt = Mathf.Clamp(dt,0,0.007f);
        //print("Moving Bolt " + inputY);
        if(Mathf.Abs(inputY)>1)
                boltProgress += (inputY*-1) * dt * boltSpeedModifier;

        boltProgress = Mathf.Clamp(boltProgress,minBoltPos,maxBoltPos);
        this.transform.position = Vector3.Lerp(positionClosed.position,positionOpened.position,boltProgress);
        this.transform.rotation = Quaternion.Lerp(positionClosed.rotation,positionOpened.rotation,boltProgress);
        
        if(round!=null) UpdateRoundPosition();
    }


    public int FireRound(){
        if(round==null){
            if(boltProgress == 0) canTakeRound = false;
            return -1;
            }
        Ammo ammocmpt = round.GetComponent<Ammo>();
        if(ammocmpt.isSpent){
            return -1;
            }

        if(boltProgress!=0){return 0;}

        if(ammocmpt.Shoot()){
            sound.ShootAudio(transform.position);
            weapon.RecoilWeapon(recoilDir,weaponRecoilForce*ammocmpt.recoilForceMultiplier);
             #region Raycasting
             RaycastHit hit;
             Vector3 targetHitPoint = Vector3.zero;
             //Todo add partical and sound effects
             for(int i = 0; i < ammocmpt.ammoPelletCount ; i++){
                 Vector3 forwardVector;
                 if(ammocmpt.spread){
                    Vector3 deviation3D = Random.insideUnitCircle * weaponSpread; // make some deviation
                    Quaternion rot = Quaternion.LookRotation(Vector3.forward * ammocmpt.range + deviation3D);//get rotation
                    forwardVector = barrellExit.transform.rotation * rot * Vector3.forward; // apply rotation
                }else{
                    targetHitPoint = barrellExit.transform.rotation * (Vector3.forward * weaponSpreadDistance) +  (Random.insideUnitSphere*weaponSpread);
                    
                 }

                Debug.DrawRay(barrellExit.transform.position,targetHitPoint, Color.red,Mathf.Infinity);

                if(Physics.Raycast(barrellExit.transform.position,targetHitPoint, out hit, ammocmpt.range)){
                    
                    
 
                    GameObject hitGameObject = hit.transform.gameObject;

                    // if(hitGameObject==null){
                    //     canTakeRound = true;
                    //     return 1;
                    // }

                    if(hitGameObject.TryGetComponent<DamageRelay>(out DamageRelay health)){
                      health.TakeDamage(round.GetComponent<Ammo>().ammoDamage,hit);
                      if(health.GetHealth().gameObject.TryGetComponent<AIController>(out AIController ai)){
                          ai.ForceCombatAndTarget(AIState.COMBAT,weapon.transform.parent.parent.parent.parent.gameObject);
                      }
                    }
                }
            }
            RecoilBolt();
            #endregion
        }
        canTakeRound = true;
        return 1;
    }



    
    
    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ejectRoundsToward,0.5f);
    }

    public bool GetFreezeState(){
        return freezeBolt;
    }
    
    public bool GetIsHoldingOpen(){
        return holdingOpen;
    }

    public void SetFreezeState(bool state){
        freezeBolt = state;
    }
    public bool getHeld(){
         return isHeld;
    }
    public void setHeld(bool state){
        isHeld = state;
    }

    public bool getClipAutoEject(){
        return autoEjectClip;
    }
}
