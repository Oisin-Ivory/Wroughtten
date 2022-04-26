using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    
    [SerializeField] int heal = 25;
    [SerializeField] Vector3 rotAmount = new Vector3(0,1,0);

    void Update(){
        transform.Rotate(rotAmount * Time.deltaTime);
    }
    void OnTriggerEnter(Collider col){
        GameObject go = col.gameObject;
        if(go.tag.Equals("Player")){
            go.GetComponent<Health>().Heal(heal);
            Destroy(this.gameObject);
        }
    }
}
