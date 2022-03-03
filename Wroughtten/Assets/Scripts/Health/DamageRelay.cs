using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRelay : MonoBehaviour
{
    
    [SerializeField] Health health;
    [SerializeField] float damageMultiplier = 1;
     public void TakeDamage(float damage){
        health.TakeDamage(damage*damageMultiplier);
    }

    public Health GetHealth(){
        return health;
    }
}
