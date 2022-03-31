using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AI Weapon",menuName = "AI/Weapon")]
public class AIWeapon : ScriptableObject
{
   public float damage = 13f;
   public float spread = 3f;
   public float spreadDistance = 1f;
   public float rateOfFire = 0.25f;
   public bool doesSpread = false;
   public float range = 100f;
   public float pelletCount = 1f;
   public int magCount = 30;
   public AnimatorOverrideController anim;
   public Vector3 transformPos;
   public Quaternion rotationPos;
   public GameObject weapon;
   public float reloadTime;
}
