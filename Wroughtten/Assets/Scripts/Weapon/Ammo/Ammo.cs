using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private Mesh spentMesh;
    [SerializeField] private MeshFilter meshFilter;

    public bool isSpent = false;
    public int ammoPelletCount = 12; // ammount of pelletes in shot
    public float ammoDamage = 25f;
    [SerializeField] public bool spread = false;
    [SerializeField] public float range = 200;
    [SerializeField] public bool isInMag = false;

    public bool Shoot(){
        if(isSpent) return false;

        meshFilter.mesh = spentMesh;
        isSpent = true;
        return true;
    }

    public IEnumerator EjectedRound(float timeTillCanFeed){
            yield return new WaitForSeconds(timeTillCanFeed);
            isInMag = false;
        }
}
