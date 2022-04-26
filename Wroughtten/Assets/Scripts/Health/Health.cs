using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    public UnityEvent Event;
    public void TakeDamage(float damage){
        health -= damage;   
        if(isDead())
            Event.Invoke();
    }

    public float getHealth(){
        return health;
    } 
    public float getMaxHealth(){
        return maxHealth;
    }

    public bool isDead(){
        return health<=0;
    }

    public void Heal(int amount){
        //Debug.Log("Healing "+health);
        health += amount;
        health = Mathf.Clamp(health,0,maxHealth);
        
       // Debug.Log("Healed "+health);
    }
}
