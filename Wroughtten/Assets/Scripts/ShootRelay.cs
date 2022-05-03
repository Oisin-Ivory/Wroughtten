using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRelay : MonoBehaviour
{
    [SerializeField] private AIController ai;
    public void Shoot(){
        ai.Shoot();
    }
}
