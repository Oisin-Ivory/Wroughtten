using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject weaponMagazineGameObj = null;
    [SerializeField] private IBolt weaponBolt = null;
    [SerializeField] private FireSelect weaponFireSelect;
    [SerializeField] private Vector3 weaponPosition;
    [SerializeField] private Vector3 weaponAdsPosition;
    [SerializeField] private Vector3 weaponOffsetPosition;
    private IMagazine weaponMagazine;
    [SerializeField] public bool detachableMagazine = false;

    [SerializeField] StripperClipAcceptor clipAcceptor;
    [SerializeField] public MagazineAcceptor magAcceptor;
    [SerializeField] Transform magazinePosition;
    private bool firing = false;


    public Vector3 getWeaponPosition(){
        return weaponPosition;
    }

    public Vector3 getWeaponAdsPosition(){
        return weaponAdsPosition;
    }
    void Awake(){
        //Time.timeScale = (0.1f);
        if(weaponMagazineGameObj!=null){
            weaponMagazine = weaponMagazineGameObj.GetComponent<IMagazine>();
        }

    }

    void Update(){
        weaponOffsetPosition = Vector3.Lerp(weaponOffsetPosition,Vector3.zero,Time.deltaTime);
        if(gameObject.transform.parent!=null && (gameObject.transform.parent.name=="HandL" || gameObject.transform.parent.name=="HandR"))
            gameObject.transform.localPosition = weaponOffsetPosition;
    }

    public void HandleInputs(){
        weaponBolt.setHeld(Input.GetButton("Jump"));
        if(weaponBolt.getHeld()){
            weaponBolt.UpdateBoltPosition(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
            if(clipAcceptor!=null){
                if(Input.GetAxis("Mouse Y")>1 && weaponBolt.getClipAutoEject()==true){
                    clipAcceptor.Ejectclip();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.C)){
            weaponFireSelect.NextState();
        }
           
       
        switch(weaponFireSelect.getState()){
            case FireSelectState.SAFE:
            
            break;
            case FireSelectState.SEMI:
                if(Input.GetMouseButtonDown(0)){
                    weaponBolt.FireRound();
                }
            break;
            case FireSelectState.AUTO:
                if(Input.GetMouseButton(0)){
                    weaponBolt.FireRound();
                }
            break;
        }  
    }

    public void RecoilWeapon(Vector3 dir,float magnitude){
         weaponOffsetPosition+=dir*magnitude;
    }

    public void setBolt(IBolt bolt){
        weaponBolt = bolt;
    }
    public IMagazine getMagazine(){
        return weaponMagazine;
    }
    public void setMagazine(IMagazine mag){
        weaponMagazineGameObj = mag.gameObject;
        weaponMagazine = mag;
    }

    public void nullMagazine(){
        weaponMagazineGameObj = null;
        weaponMagazine = null;
    }


}
