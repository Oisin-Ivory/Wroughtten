using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    
    [Header("Components ")]
    [SerializeField] GameObject meshGameObj;
    [SerializeField] GameObject headMeshGameObj;
    [SerializeField] GameObject torsoMeshGameObj;
    [SerializeField] GameObject legsMeshGameObj;
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
    [SerializeField] Vector3 startingPos;
    [SerializeField] Quaternion startingRot;
    
    [Header("AI and Weapon Attributes")]
    [SerializeField] AIBehaviour aiBehaviour;
    [SerializeField] AIWeapon aiWeapon;
    [SerializeField] Transform weaponBarrel;
    
    [Header("Targeting and Cover")]
    [SerializeField] GameObject activeTarget;
    [SerializeField] GameObject coverPoint;
    [SerializeField] Vector3 targetPos;
    [SerializeField] LayerMask mask;
    [SerializeField] float timeSinceKnowledgeOfTarget = 0f;
    [SerializeField] float timeSpentInvestigating = 0f;

    
    [Header("Enemy Pathing")]

    [SerializeField] private Path path;
    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private float timeSpentWaiting = 0f;
    [SerializeField] private Waypoint currentWaypoint;

    [Header("Animation")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator torsoAnimator;
    [SerializeField] private Animator legsAnimator;
    [SerializeField] private GameObject weaponBone;

    [Header("Weapon State")]
    [SerializeField] int maxMag;
    [SerializeField] int magCount;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float timeSpentReloading = 0f;
    [SerializeField] bool isReloading = false;
    [SerializeField] bool isShooting = false;
    [SerializeField] float burstShotsFired = 0f;
    [SerializeField] float timeSinceLastBurst = Mathf.Infinity;
    [SerializeField] private float timeSinceLastShot = Mathf.Infinity;
    


    private void Patrol(){
        if(Vector3.Distance(transform.position,currentWaypoint.transform.position) > 1f){
            //print("not close");
            agent.SetDestination(currentWaypoint.transform.position);
        }else{
            //print("is close");
            timeSpentWaiting += Time.deltaTime;
            if(timeSpentWaiting > currentWaypoint.waitTime){
                print("done waiting");
                System.Tuple<Waypoint, int> nextWP = path.GetNextWaypoint(waypointIndex);
                currentWaypoint = nextWP.Item1;
                waypointIndex = nextWP.Item2;
                timeSpentWaiting = 0;
            }
        }
    }


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        startingPos = transform.position;

    }

    void Start(){
        if(path!=null)
            currentWaypoint = path.GetWaypoint(waypointIndex);
        state = initialState;
        combatState = initialcombatState;
        coverState = initialcoverState;

        maxMag = aiWeapon.magCount;
        magCount = maxMag;

        torsoAnimator.runtimeAnimatorController = aiWeapon.anim;

        GameObject weapon = Instantiate(aiWeapon.weapon);
        weapon.transform.parent = weaponBone.transform;
        weapon.transform.localPosition = aiWeapon.transformPos;
        weapon.transform.localRotation = aiWeapon.rotationPos;
        reloadTime = aiWeapon.reloadTime;

        weaponBarrel = weapon.transform.Find("barrel");
    }

    public void Investigate(Vector3 position){
        Debug.Log(gameObject.name+" Investigating");
        if(state == AIState.COMBAT) return;
        if(!aiBehaviour.willInvestigate) return;
        state = AIState.INVESTIGATE;
        targetPos = position + Random.insideUnitSphere*aiBehaviour.searchRadius;
        agent.SetDestination(position);
    }
    
    void Update()
    {   
        legsAnimator.SetBool("moving",agent.velocity.magnitude>0);
        timeSinceLastShot+=Time.deltaTime;
        timeSinceLastBurst+=Time.deltaTime;

        hasLOS = HasLosTarget();

        if(isReloading){
            timeSpentReloading += Time.deltaTime;
            if(timeSpentReloading > reloadTime){
                Reload();
            }
        }

        if(state != AIState.COMBAT){
            SearchForTarget();
            if(state==AIState.INVESTIGATE){
                timeSpentInvestigating+=Time.deltaTime;
                if(timeSpentInvestigating>aiBehaviour.timeSpendInvestigate){
                    state = initialState;
                    agent.SetDestination(startingPos);
                }
            }
            if(state == AIState.PATROL){
                Patrol();
            }
        }

        if(state==AIState.COMBAT){
            CombatState();
        }
    }


    private void CombatState(){
        coverState = UpdateCoverState();
        if(Vector3.Distance(transform.position,targetPos) > 1.5f)
            torsoMeshGameObj.transform.LookAt(targetPos);
        if(activeTarget!=null && (hasLOS || Vector3.Distance(activeTarget.transform.position,transform.position)<aiBehaviour.hearingDistance)){
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
                Vector3 lookAtPos = new Vector3(activeTarget.transform.position.x,meshGameObj.transform.position.y,activeTarget.transform.position.z);
                meshGameObj.transform.LookAt(lookAtPos);
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
            if(hasLOS && (Vector3.Distance(transform.position,activeTarget.transform.position) < aiBehaviour.maxAttackDistance)){
                combatState = AICombatState.SHOOT;
            }else if(Vector3.Distance(transform.position,activeTarget.transform.position) > aiBehaviour.maxAttackDistance || health.getHealth() > aiBehaviour.seekCoverBelowHealth){
                agent.SetDestination(targetPos);
                meshGameObj.transform.localRotation = Quaternion.identity;
            }else{
                combatState = AICombatState.COVER;
            }
        }

        if(combatState == AICombatState.SHOOT){
            if(hasLOS && aiBehaviour.maxAttackDistance > Vector3.Distance(transform.position,activeTarget.transform.position)){
                agent.SetDestination(transform.position);

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

    public void Shoot(){
        
        if(timeSinceLastShot < aiWeapon.rateOfFire)
            return;
        RaycastHit hit;
        Vector3 targetHitPoint = Vector3.zero;
        Transform adjustedTransform = activeTarget.transform;
        weaponBarrel.LookAt(adjustedTransform.position + Vector3.up);
        
        magCount--;
        burstShotsFired++;
        isShooting = false;

        for(int i = 0; i < aiWeapon.pelletCount ; i++){
            Vector3 forwardVector;
            if(aiWeapon.doesSpread){
                Vector3 deviation3D = Random.insideUnitCircle * aiWeapon.spread; // make some deviation
                Quaternion rot = Quaternion.LookRotation(Vector3.forward * aiWeapon.range + deviation3D);//get rotation
                forwardVector = weaponBarrel.transform.rotation * rot * Vector3.forward; // apply rotation
            }else{
                targetHitPoint = weaponBarrel.transform.rotation * ( (Vector3.forward * aiWeapon.spreadDistance) + (Vector3.forward * aiBehaviour.enemyWeaponAccuracy)) +  (Random.insideUnitSphere*aiWeapon.spread);
             }

            Debug.DrawRay(weaponBarrel.transform.position,targetHitPoint, Color.red,aiWeapon.range);
            timeSinceLastShot = 0f;
            if(Physics.Raycast(weaponBarrel.transform.position,targetHitPoint, out hit, aiWeapon.range,mask)){
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

    private void ShootTarget(){
        if(activeTarget == null)return;
        if(timeSinceLastShot < aiWeapon.rateOfFire) return;
        if(isReloading){
            return;
        }

        if(burstShotsFired > aiBehaviour.burstCount){
            timeSinceLastBurst = 0;
            burstShotsFired = 0;
        }

        if(timeSinceLastBurst < aiBehaviour.timeBeforeBurst){
            return;
        }
        

        if(magCount <= 0){
            isReloading = true;
            torsoAnimator.SetTrigger("reload");
            return;
        }
        
        if(isShooting)
            return;

        
        Debug.Log("shooting");
        isShooting = true;
        torsoAnimator.SetTrigger("shoot");  
        
        
        transform.LookAt(targetPos);

        

    }


    public void Reload(){
        magCount = maxMag;
        isReloading = false;
        timeSpentReloading = 0;
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
//        Debug.Log(hasLOS);
        RaycastHit hit;
        //Physics.Linecast(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.78f),out hit,mask);
        Physics.Raycast(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.5f) - transform.position,out hit,aiBehaviour.viewDistance,mask);
        Debug.DrawRay(transform.position+(Vector3.up*1.78f),activeTarget.transform.position+(Vector3.zero*1.5f) - transform.position,Color.cyan);
        
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


    public void Die(){
        agent.isStopped = true;
        torsoAnimator.SetTrigger("die");
        legsAnimator.SetTrigger("die");
        this.enabled = false;
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
        Gizmos.DrawWireSphere(meshGameObj.transform.position,aiBehaviour.maxAttackDistance);
        if(targetPos!=null){
            Gizmos.DrawCube(targetPos,new Vector3(1,1,1));
        }
    }

    #endregion
}

public enum AIState{
    IDLE,
    PATROL,
    COMBAT,
    INVESTIGATE
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