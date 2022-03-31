using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTimer : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] float timeAlive = 0f;

    // Update is called once per frame
    void Update(){
        timeAlive+=Time.deltaTime;
        if(timeAlive > duration){
            Destroy(this.gameObject);
        }
    }
}
