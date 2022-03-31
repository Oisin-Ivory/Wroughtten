using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagLoader : MonoBehaviour
{
    [SerializeField] IMagazine magToLoad;
    [SerializeField] GameObject bulletToLoad;

    void Awake(){
        IMagazine mag;
        gameObject.TryGetComponent<IMagazine>(out mag);
        magToLoad = mag;
    }
    void Start()
    {
        LoadMagazine();
    }

    private void LoadMagazine(){
        for(int i = 0; i < magToLoad.getMagazineCapacity();i++){
            GameObject bullet = Instantiate(bulletToLoad);
            magToLoad.LoadRound(bullet);
        }
    }
}
