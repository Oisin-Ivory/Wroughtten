using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    
    [Header("Components ")]
    [SerializeField] GameObject meshGameObj;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Health health;
    
    [Header("AI States")]
    [SerializeField] AIState state;
    [SerializeField] AICombatState combatState;
    [SerializeField] AICoverState coverState;
    [SerializeField] bool hasLOS;

    [Header("AI InitialStates States")]
    [SerializeField] AIState initialState = AIState.IDLE;
    [SerializeField] AICombatState initialcombatState = AICombatState.COVER;
    [SerializeField] AICoverState initialcoverState = AICoverState.NOCOVERPOINT;
    
    [Header("AI and Weapon Attributes")]
    [SerializeField] AIBehaviour aiBehaviour;
    [SerializeField] AIWeapon aiWeapon;
    [SerializeField] Transform weaponBarell;
    
    [Header("Targeting and Cover")]
    [SerializeField] GameObject activeTarget;
    [SerializeField] GameObject coverPoint;
    [SerializeField] Vector3 targetPos;
    [SerializeField] LayerMask mask;
    private float timeSinceLastShot = Mathf.Infinity;
    [SerializeField] float timeSinceKnowledgeOfTarget = 0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {   
        timeSinceLastShot+=Time.deltaTime;

        hasLOS = HasLosTarget();


        if(state != AIState.COMBAT){
            SearchForTarget();
            if(state == AIState.PATROL){

            }
        }

        if(state==AIState.COMBAT){
            CombatState();
        }
    }


    private void CombatState(){
        coverState = UpdateCoverState();
        if(Vector3.Distance(transform.position,targetPos) > 1.5f)
            gameObject.transform.LookAt(targetPos);
        if(hasLOS || Vector3.Distance(activeTarget.transform.position,transform.position)<aiBehaviour.hearingDistance){
            targetPos = activeTarget.transform.position;
            timeSinceKnowledgeOfTarget = 0;
        }else{
            timeSinceKnowledgeOfTarget+=Time.deltaTime;
        }
        if(timeSinceKnowledgeOfTarget>aiBehaviour.timeBeforeGivingUpSearch){
            state = initialState;
            combatState = initialcombatState;
            coverState = initialcoverState;
            timeSinceKnowledgeOfTarget = 0;
            activeTarget = null;
            return;
        }
        if(combatState == AICombatState.COVER){
            if(Vector3.Distance(transform.position,activeTarget.transform.position)>aiBehaviour.minDistanceIgnoreCover){
            if(aiBehaviour.useCover && coverState != AICoverState.ATCOVER){
                if(coverState == AICoverState.NOCOVERPOINT){
                    coverPoint = SeekCoverPoint();
                    if(coverPoint == null){
                        if(hasLOS){
                            combatState = AICombatState.SHOOT;
                        }else{
                            //Debug.Log("Doesn't have LOS");
                            combatState = AICombatState.ADVANCE;
                        }
                    }else{
                        agent.SetDestination(coverPoint.transform.position);
                    }
                }
                if(coverState == AICoverState.NOTATCOVER){
                    agent.SetDestination(coverPoint.transform.position);
                    GameObject newCP = SearchBetterCover();
                    if(newCP!=null){
                        AsignNewCover(newCP);
                    }
                }
            }
            if(coverState == AICoverState.ATCOVER){
                combatState = AICombatState.SHOOT;
                GameObject newCP = SearchBetterCover();
                if(newCP!=null){
                    AsignNewCover(newCP);
                }
            }
            }else{
                combatState = AICombatState.ADVANCE;
            }
        }
        if(combatState == AICombatState.ADVANCE){
            if(hasLOS){
                if(Vector3.Distance(transform.position,activeTarget.transform.position) < aiBehaviour.maxAttackDistance){
                    combatState = AICombatState.SHOOT;
                }
            }else if(Vector3.Distance(transform.position,activeTarget.transform.position) > aiBehaviour.maxAttackDistance || health.getHealth() > aiBehaviour.seekCoverBelowHealth){
                agent.SetDestination(targetPos);
            }else{
                combatState = AICombatState.COVER;
            }
        }

        if(combatState == AICombatState.SHOOT){
            if(hasLOS && aiBehaviour.maxAttackDistance > Vector3.Distance(transform.position,activeTarget.transform.position)){
                ShootTarget();
            }else{
                combatState = AICombatState.ADVANCE;
            }
        }

    }

    private void AsignNewCover(GameObject newCP){
        coverPoint.GetComponent<CoverPoint>().isInUse = false;
        newCP.GetComponent<CoverPoint>().isInUse = true;
        coverPoint = newCP;
    }

    public void ForceCombatAndTarget(AIState state, GameObject target){
        this.state = state;
        this.activeTarget = target;
    }


    private AICoverState UpdateCoverState(){
        if(coverPoint==null) return AICoverState.NOCOVERPOINT;
        return (Vector3.Distance(transform.position,coverPoint.transform.position) < 1f) ? AICoverState.ATCOVER : AICoverState.NOTATCOVER;
    }

    private void ShootTarget(){
        if(activeTarget == null)return;
        if(timeSinceLastShot < aiWeapon.rateOfFire) return;
        transform.LookAt(targetPos);

        RaycastHit hit;
        Vector3 targetHitPoint = Vector3.zero;
        Transform adjustedTransform = activeTarget.transform;
        adjustedTransform.position += Vector3.up;
        weaponBarell.LookAt(adjustedTransform);
        for(int i = 0; i < aiWeapon.pelletCount ; i++){
                 Vector3 forwardVector;
                 if(aiWeapon.doesSpread){
                    Vector3 deviation3D = Random.insideUnitCircle * aiWeapon.spread; // make some deviation
                    Quaternion rot = Quaternion.LookRotation(Vector3.forward * aiWeapon.range + deviation3D);//get rotation
                    forwardVector = weaponBarell.transform.rotation * rot * Vector3.forward; // apply rotation
                }else{
                    targetHitPoint = weaponBarell.transform.rotation * (Vector3.forward * aiWeapon.spreadDistance) +  (Random.insideUnitSphere*aiWeapon.spread);
                    
                 }

                Debug.DrawRay(weaponBarell.transform.position,targetHitPoint, Color.red,aiWeapon.range);
                timeSinceLastShot = 0f;
                if(Physics.Raycast(weaponBarell.transform.position,targetHitPoint, out hit, aiWeapon.range,mask)){
                    print("Hit: " + hit.collider.gameObject.name);
                    GameObject hitGameObject = hit.transform.gameObject;

                    if(hitGameObject==null) return;
                    //print("getting damage relat");
                    if(hitGameObject.TryGetComponent<DamageRelay>(out DamageRelay health)){
                        health.TakeDamage(aiWeapon.damage);
                        if(health.GetHealth().gameObject.TryGetComponent<AIController>(out AIController ai)){
                          ai.ForceCombatAndTarget(AIState.COMBAT,gameObject);
                        }
                    }
                }
            }
    }
 

    private GameObject SearchBetterCover(){
        //print("evaling cover");
        GameObject oldCP = coverPoint;
        CoverQuality oldCPQuality = oldCP.GetComponent<CoverPoint>().EvalCoverPoint(activeTarget,aiBehaviour.targetListTag);
        List<GameObject> allCoverClose = GetCloseCoverPoints();

        foreach(GameObject cp in allCoverClose){
            CoverQuality newCPQuality = cp.GetComponent<CoverPoint>().EvalCoverPoint(activeTarget,aiBehaviour.targetListTag);
            //Debug.Log("New CP Quality: "+newCPQuality+ " > Old CP Quality: "+oldCPQuality+" = " + (newCPQuality > oldCPQuality));

            if(newCPQuality < oldCPQuality){
                return cp;
            }
            
        }
        return null;
    }
    private GameObject SeekCoverPoint(){
        //print("Seeking Cover Point");
        List<GameObject> coverPoints = GetCloseCoverPoints();
        GameObject bestCP = null;
        foreach (GameObject cp in coverPoints){
            
            switch (cp.GetComponent<CoverPoint>().EvalCoverPoint(activeTarget,aiBehaviour.targetListTag))
            {
                case CoverQuality.COVERSIGHT:
                    cp.GetComponent<CoverPoint>().isInUse = true;
                    return cp;
                case CoverQuality.COVER:
                    bestCP = cp;
                break;

            }
        }
        
        if(bestCP==null){
            //print("null");
            return null;
        }
        print(bestCP.gameObject.name);
        bestCP.GetComponent<CoverPoint>().isInUse = true;
        return bestCP;
    }

    private List<GameObject> GetCloseCoverPoints(){
        Collider[] objs = Physics.OverlapSphere(transform.position,aiBehaviour.maxCoverDistance);
        List<GameObject> coverPoints = new List<GameObject>();
        foreach(Collider obj in objs){
            if(obj.TryGetComponent<CoverPoint>(out CoverPoint cp)){
                coverPoints.Add(obj.gameObject);
            }
        }
        return coverPoints;
    }

    #region Sight
    private bool isEnemy(string tag){
        foreach (string str in aiBehaviour.targetListTag){
            if(str == tag){
                return true;
            }
        }
        return false;
    }
    private void SearchForTarget(){
        //Looking
        Collider[] objs = Physics.OverlapSphere(transform.position,aiBehaviour.viewDistance);
        foreach(Collider obj in objs){
            //print(obj.name +" is a child of " + obj.transform.root.name +" : "+ gameObject.name);
            if(Object.ReferenceEquals(obj.transform.root, gameObject.transform)) continue;

            Transform target = obj.gameObject.transform;
            Vector3 targetDir = (target.position - meshGameObj.transform.position).normalized;

            if(Vector3.Angle(meshGameObj.transform.forward,targetDir) < aiBehaviour.viewAngle/2){
                RaycastHit hit;
                if(Physics.Linecast(meshGameObj.transform.position+(Vector3.up),target.position+(Vector3.up),out hit,mask)){
                    
                    if(isEnemy(hit.collider.gameObject.tag)){
                        Debug.Log(hit.collider.gameObject.name +" enemy status: " + isEnemy(hit.collider.gameObject.tag));
                        activeTarget = target.root.gameObject;
                        targetPos = activeTarget.transform.position;
                        state = AIState.COMBAT;
                        return;
                    }
                }
            }

        }


        //Listening
        Collider[] hearingObjs = Physics.OverlapSphere(transform.position,aiBehaviour.hearingDistance);
        foreach(Collider obj in hearingObjs){
            if(Object.ReferenceEquals(obj.transform.root, gameObject.transform)) continue;
            GameObject target = obj.gameObject;
            if(isEnemy(target.tag)){
                activeTarget = target.transform.root.gameObject;
                targetPos = activeTarget.transform.position;
                state = AIState.COMBAT;
                return;
            }
        }
    }

    private bool HasLosTarget(){
        if(activeTarget == null) return false;
        RaycastHit hit;
        //Physics.Linecast(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.78f),out hit,mask);
        Physics.Raycast(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.78f) - transform.position,out hit,aiBehaviour.viewDistance,mask);
        Debug.DrawRay(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.78f) - transform.position,Color.cyan);
        
       if(hit.collider == null) return false;
        
        Vector3 targetDir = (activeTarget.transform.position - meshGameObj.transform.position).normalized;
        if(Vector3.Angle(meshGameObj.transform.forward,targetDir) < aiBehaviour.viewAngle/2){
            return Object.ReferenceEquals(hit.collider.gameObject,activeTarget);
        }
        return false;
        //print(hit.collider.gameObject.name);
        //return AIBehaviour.isEnemy(hit.collider.gameObject.tag,aiBehaviour.targetListTag);
    }
    private Vector3 DirFromAngle(float angleInDeg,bool globalAngle){
        if(!globalAngle){
            angleInDeg+=meshGameObj.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDeg *Mathf.Deg2Rad),0,Mathf.Cos(angleInDeg *Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected(){
        if(aiBehaviour==null)return;
        Vector3 viewAngleA = DirFromAngle(-aiBehaviour.viewAngle/2,false);
        Vector3 viewAngleB = DirFromAngle(aiBehaviour.viewAngle/2,false);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(meshGameObj.transform.position,aiBehaviour.viewDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(meshGameObj.transform.position,aiBehaviour.minDistanceIgnoreCover);
        Gizmos.DrawWireSphere(meshGameObj.transform.position,aiBehaviour.maxCoverDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(meshGameObj.transform.position,meshGameObj.transform.position + viewAngleA * aiBehaviour.viewDistance);
        Gizmos.DrawLine(meshGameObj.transform.position,meshGameObj.transform.position + viewAngleB * aiBehaviour.viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meshGameObj.transform.position,aiBehaviour.hearingDistance);
        if(targetPos!=null){
            Gizmos.DrawCube(targetPos,new Vector3(1,1,1));
        }
    }

    #endregion
}

public enum AIState{
    IDLE,
    PATROL,
    COMBAT
}

public enum AICombatState{
    SHOOT,
    ADVANCE,
    COVER,
}

public enum AICoverState{
    NOCOVERPOINT,
    NOTATCOVER,
    ATCOVER,

}