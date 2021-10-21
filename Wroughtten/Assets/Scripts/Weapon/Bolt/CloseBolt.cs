using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBolt : MonoBehaviour, IBolt
{
    //1 is open - 0 is closed
    [SerializeField] float boltProgress = 0f;
    [SerializeField] float boltSpeedModifier = 2f;
    [SerializeField] Transform positionClosed,positionOpened;

    [SerializeField] float boltSpringStrenght = 1f;

    [SerializeField] bool isLocked = false;
    
    [SerializeField] bool isHeld = false;

    //Ammo
    [SerializeField] bool canTakeRound = false;
    [SerializeField] bool hasRound = false;
    [SerializeField] GameObject round;
    [SerializeField] GameObject roundPosition;

    //MagazineReference
    [SerializeField] Weapon weapon;

    [SerializeField] bool isRecoiling = false;
	[SerializeField] float recoilStrenght = -4f;

    [SerializeField] Vector3 ejectRoundsToward = Vector3.up;
    [SerializeField] Vector3 ejectRoundRotate = Vector3.up;
    [SerializeField] float ejectRoundForce = 134f;

    [SerializeField] bool willHoldOpenOnEmpty = true;

    bool holdingOpen=false;

    float minBoltPos = 0f;
    float maxBoltPos = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Time.timeScale = 0.1f;
    }   

    void Update(){
        isHeld = Input.GetButton("Jump");

        if(Input.GetMouseButtonDown(0) && boltProgress==0){
            RecoilBolt();

        }
        if(boltProgress > 0.9f){
            EjectRound();
        }

        if(isHeld && !isRecoiling)
            UpdateBoltPosition(0,Input.GetAxis("Mouse Y"));

        if(holdingOpen){
            if(boltProgress==1f){
                minBoltPos=0f;
            }
        }
        //Feed the round when bolt progress is .8-.85
        if(boltProgress < 0.85f && !hasRound && canTakeRound){

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
                if(weapon.getMagazine().getBulletCount()==0){
                    holdingOpen = true;
                    minBoltPos = 0.9f;
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
        isRecoiling = true;

    }

    public void EjectRound(){
        print("Ejecting Round");
        if(round==null){
            canTakeRound = true;
            return;
        }
        round.GetComponent<CapsuleCollider>().enabled = true;
        round.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //round.GetComponent<Ammo>().isInMag = false;
        round.transform.parent = null;
        //done Add force away from the saiga transform
        round.GetComponent<Rigidbody>().AddForce(gameObject.transform.up*132f + Vector3.right*Random.Range(10,40));
        StartCoroutine(round.GetComponent<Ammo>().EjectedRound(3f));
        round = null;
        hasRound = false;
        canTakeRound = true;
        weapon.getMagazine().UpdateBulletPosition();
    }

    public void UpdateRoundPosition(){
        round.transform.position = roundPosition.transform.position;
        round.transform.rotation = roundPosition.transform.rotation;
    }

    public void UpdateBoltPosition(float inputX, float inputY){
        //print("Moving Bolt " + inputY);
        if(Mathf.Abs(inputY)>1)
                boltProgress += (inputY*-1) * Time.deltaTime * boltSpeedModifier;

        boltProgress = Mathf.Clamp(boltProgress,minBoltPos,maxBoltPos);
        this.transform.position = Vector3.Lerp(positionClosed.position,positionOpened.position,boltProgress);
        this.transform.rotation = Quaternion.Lerp(positionClosed.rotation,positionOpened.rotation,boltProgress);
        
        if(round!=null) UpdateRoundPosition();
    }

    
    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ejectRoundsToward,ejectRoundForce);
    }
}