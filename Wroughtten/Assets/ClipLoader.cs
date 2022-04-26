using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipLoader : MonoBehaviour
{
    [SerializeField] StripperClip clipToLoad;
    [SerializeField] GameObject bulletToLoad;

    void Awake(){
        StripperClip clip;
        gameObject.TryGetComponent<StripperClip>(out clip);
        clipToLoad = clip;
    }
    void Start()
    {
        LoadMagazine();
    }

    private void LoadMagazine(){
        for(int i = 0; i < clipToLoad.getClipCapacity();i++){
            GameObject bullet = Instantiate(bulletToLoad);
            clipToLoad.LoadRound(bullet);
        }
    }
}
