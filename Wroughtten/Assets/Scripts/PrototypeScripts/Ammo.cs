using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private Mesh spentMesh;

    public bool isSpent = false;
    public int ammoPelletCount = 12; // ammount of pelletes in shot
    public float ammoDamage = 25f;

    [SerializeField] public bool isInMag = false;

    public bool Shoot(){
        if(isSpent) return false;

        gameObject.GetComponent<MeshFilter>().mesh = spentMesh;
        isSpent = true;
        return true;
    }

    public IEnumerator EjectedRound(float timeTillCanFeed){
            yield return new WaitForSeconds(timeTillCanFeed);
            isInMag = false;
        }
}
