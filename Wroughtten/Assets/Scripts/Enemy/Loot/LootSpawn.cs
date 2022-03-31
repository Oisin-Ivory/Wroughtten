using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] lootToSpawn;

    public void SpawnLoot(){
        float offset = 0;
        foreach (GameObject obj in lootToSpawn){
            Instantiate(obj,transform.position + getRandDir()*offset ,Quaternion.identity);
            offset+=0.25f;
        }
        Destroy(this);
    }

    private Vector3 getRandDir(){
        int dir = Random.Range(0,3);
        switch(dir){
            case 0:
                return Vector3.back;
            case 1:
                return Vector3.left;
            case 2:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }
}
