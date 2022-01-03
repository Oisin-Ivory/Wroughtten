using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject weaponMagazineGameObj = null;
    [SerializeField] public Vector3 weaponPosition;
    private IMagazine weaponMagazine;
    [SerializeField] bool detachableMagazine = false;

    void Awake(){
        if(weaponMagazineGameObj!=null){
            weaponMagazine = weaponMagazineGameObj.GetComponent<IMagazine>();
        }

    }
    void start(){

    }
    void Update(){

    }

    public IMagazine getMagazine(){
        if(!detachableMagazine) return (InternalMagazine)weaponMagazine;
        return (DetachableMagazine)weaponMagazine;
    }

}
