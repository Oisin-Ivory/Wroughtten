using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] public CoverType coverType;
    [SerializeField] Transform coverTransform;
    [SerializeField] Transform shootingTransform;
    [SerializeField] public bool isInUse = false;
    [SerializeField] LayerMask mask;

    private void Start(){
        coverTransform = transform;
    }

    public CoverQuality EvalCoverPoint(GameObject target,string[] tags){
        RaycastHit coverHit;
        RaycastHit losHit;

        Physics.Linecast(coverTransform.position, target.transform.position+(Vector3.up * 1.75f),out coverHit,mask);
        bool hasCover;
        if(coverHit.collider!=null){
            hasCover = !(AIBehaviour.isEnemy(coverHit.collider.gameObject.tag,tags));
//            print("Cover:" + coverHit.collider.gameObject.name);
        }else{
            hasCover = false;
        }
        
        Physics.Linecast(shootingTransform.position, target.transform.position+(Vector3.up * 1.75f),out losHit,mask);
        bool hasSight;
        if(losHit.collider!=null){
//            print("LOS:" + losHit.collider.gameObject.name);
            hasSight = (AIBehaviour.isEnemy(losHit.collider.gameObject.tag,tags));
        }else{
            hasSight = false;
        }

  //      print("Cover: " + hasCover + " Sight: "+hasSight);
        Debug.DrawRay(coverTransform.position, target.transform.position - coverTransform.position,Color.red);
        Debug.DrawRay(shootingTransform.position, target.transform.position+(Vector3.up * 1.75f) - shootingTransform.position,Color.blue);

        if(hasCover && hasSight) return CoverQuality.COVERSIGHT;
        if(hasCover && !hasSight) return CoverQuality.COVER;
        return CoverQuality.NONE;
    }

    private void OnDrawGizmosSelected(){
        
        if(coverTransform==null)return;
        Gizmos.color = isInUse ? Color.green: Color.red;
        Gizmos.DrawSphere(coverTransform.position,0.5f);

        if(shootingTransform==null)return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(shootingTransform.position,0.5f);
    }
}

public enum CoverType{
    CROUCH,
    LEFT,
    RIGHT
}
public enum CoverQuality{
    COVERSIGHT,
    COVER,
    NONE
}