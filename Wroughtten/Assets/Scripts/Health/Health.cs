using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 100;
    public UnityEvent Event;
    public void TakeDamage(float damage){
        health -= damage;   
        if(isDead())
            Event.Invoke();
    }

    public bool isDead(){
        return health<=0;
    }
}
