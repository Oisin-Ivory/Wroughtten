using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class BoltAction : MonoBehaviour, IBolt
{
    float boltProgress = 0f;
    [SerializeField]bool boltStageOne = true;
    [SerializeField]bool boltStageTwo = false;
    float stageOneProgress,stageTwoProgress = 0f;
    [SerializeField] float boltSpeedModifier = 2f;
    public Transform positionClosed,position1,position2;

    [SerializeField] GameObject roundPosition;
    [SerializeField] GameObject round;
    [SerializeField] bool canTakeRound = false;
    [SerializeField] bool hasRound = false;
    [SerializeField] float roundLaunchMultiplier = 134f;
    [SerializeField] Weapon weapon;

    [SerializeField] float ejectRoundAtBoltProgress = 1;
    [SerializeField] float feedRoundAtBoltProgress = 0.85f;

    [SerializeField] public bool autoEjectClip = false;

    #region firing
    [SerializeField] Transform barrellExit;
    [SerializeField] float weaponSpread;
    [SerializeField] float weaponSpreadDistance;

    [SerializeField] public bool isHeld = false;
    
    [SerializeField] public bool freezeBolt = false;
    [SerializeField] Vector3 recoilDir = Vector3.back;
    [SerializeField] float weaponRecoilForce = 0.2f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weapon.setBolt(this);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(boltProgress == ejectRoundAtBoltProgress){
            EjectRound();
        }

        if(canTakeRound && (boltProgress < feedRoundAtBoltProgress) && boltStageTwo && !hasRound){
            //Debug.Log("Feeding round if");
            if(weapon.getMagazine()!=null){
                round = weapon.getMagazine().FeedRound();
                //Debug.Log("Feeding round from mag");
            }

            if(round!=null){
                
                //Debug.Log("Round not null");
                canTakeRound = false;
                hasRound = true;
            }

        }

    }

    public void UpdateBoltPosition(float inputX, float inputY)
    {
        if(freezeBolt)return;

        boltProgress = (stageOneProgress+stageTwoProgress)/2;
        if(boltStageOne){
            //Debug.Log("Stage 1: "+stageOneProgress);
            //Debug.Log(Mathf.Abs(Input.GetAxis("Mouse X")));
            if(Mathf.Abs(inputX)>1)
                stageOneProgress += (inputX*-1) * Time.deltaTime * boltSpeedModifier;

            stageOneProgress = Mathf.Clamp(stageOneProgress,0,1);
            this.transform.position = Vector3.Lerp(positionClosed.position,position1.position,stageOneProgress);
            this.transform.rotation = Quaternion.Lerp(positionClosed.rotation,position1.rotation,stageOneProgress);
            if(stageOneProgress==1){
                boltStageOne = false;
                boltStageTwo = true;
            }
        }
        if(boltStageTwo){
            //Debug.Log("Stage 2: "+stageTwoProgress);
            //Debug.Log(Mathf.Abs(Input.GetAxis("Mouse Y")));
            if(Mathf.Abs(inputY)>1)
                stageTwoProgress += (inputY*-1) * Time.deltaTime * boltSpeedModifier;

            if(stageTwoProgress<0){
                stageOneProgress = 0.99f;
                boltStageOne = true;
                boltStageTwo = false;
            }
            stageTwoProgress = Mathf.Clamp(stageTwoProgress,0,1);
            this.transform.position = Vector3.Lerp(position1.position,position2.position,stageTwoProgress);
            this.transform.rotation = Quaternion.Lerp(position1.rotation,position2.rotation,stageTwoProgress);
            
        }
    }

    public void UpdateRoundPosition()
    { 
        round.transform.position = roundPosition.transform.position;
        round.transform.rotation = roundPosition.transform.rotation;
    }


    public void EjectRound()
    {
        //print("Ejecting Round");
        if(round==null){
            canTakeRound = true;
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
        canTakeRound = true;
        weapon.getMagazine().UpdateBulletPosition();
    }

    public void SetFreezeState(bool state)
    {
        freezeBolt = state;
    }

    public bool GetFreezeState()
    {
        
        return freezeBolt;
    }

    public bool GetIsHoldingOpen()
    {
        return boltProgress==1;
    }

     public int FireRound(){
         if(round==null)return -1;

        Ammo ammocmpt = round.GetComponent<Ammo>();
        if(ammocmpt.isSpent)return -1;

        if(boltProgress!=0) return 0;

        if(ammocmpt.Shoot()){
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

                    if(hitGameObject==null) return 1;

                    if(hitGameObject.TryGetComponent<DamageRelay>(out DamageRelay health)){

                      health.TakeDamage(round.GetComponent<Ammo>().ammoDamage);
                    }
                }
            }
            #endregion
        }
        return 1;
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
