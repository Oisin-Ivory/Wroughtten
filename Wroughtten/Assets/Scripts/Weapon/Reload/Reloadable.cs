using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reloadable : MonoBehaviour
{
    [SerializeField] public Transform loadingPosition;
    [SerializeField] private AmmoType[] acceptableTypes;
    [SerializeField] private Transform[] acceptableTypePositions;

    public bool willAccept(Loadable ammoToLoad){
        foreach (AmmoType acceptableAmmo in acceptableTypes)
        {
            if(acceptableAmmo == ammoToLoad.ammoType){
                return true;
            }
        }

        return false;
    }

    public Transform getLoadingPosition(Loadable ammoToLoad){
        
        for(int i = 0; i < acceptableTypes.Length ; i++){
            if(acceptableTypes[i] == ammoToLoad.ammoType){
                return acceptableTypePositions[i];
            }
        }

        return null;
    }

}
