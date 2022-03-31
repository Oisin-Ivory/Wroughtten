using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AI Behaviour",menuName = "AI/Behaviour")]
public class AIBehaviour : ScriptableObject
{
    [Header("Enemy Weapons")]
    public float enemyWeaponAccuracy;
    public float maxAttackDistance;
    public float burstCount;
    public float timeBeforeBurst;
    [Header("Enemy Investigate")]
    public bool willInvestigate;
    public float timeSpendInvestigate;
    public float searchRadius;
    [Header("Enemy Cover")]
    public bool useCover;
    public CoverState coverState;
    public float maxCoverDistance;
    public float seekCoverBelowHealth;
    public float minDistanceIgnoreCover;
    
    [Header("Enemy Senses")]
    public float hearingDistance;
    public float viewDistance; 
    [Range(0,360)] public float viewAngle;
    public float timeBeforeGivingUpSearch = 10f;
    
    [Header("Enemy Tags")]
    public string[] targetListTag;

    public static bool isEnemy(string obj,string[] tags){
        foreach (string tag in tags){   
            if(obj == tag){
                return true;
            }
        }
        return false;
    }
}

public enum CoverState{
    NEVER,
    WOUNDED,
    ALWAYS
}
