using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRelay : MonoBehaviour
{
    
    [Header("Damage & Health")]
    [SerializeField] Health health;
    [SerializeField] float damageMultiplier = 1;

    [Header("Blood Effect")]
    [SerializeField] GameObject bloodEffectPrefab;
    [SerializeField] float magnitude;
    [SerializeField] float additionalUpForce = 2f;
    [SerializeField] float maxForce = 2f;
    [SerializeField] float minForce = -2f;
    private bool hasBloodEffect = false;
    [SerializeField] Material bloodyMaterial;
    private bool hasTextureEffect = false;
    [SerializeField] GameObject relatedLimb;
    private bool isDamaged = false;

    void Awake(){
        hasBloodEffect = bloodEffectPrefab!=null;
        hasTextureEffect = bloodyMaterial!=null && relatedLimb!=null;
    }

    void Update(){
        
    }

    public void TakeDamage(float damage){
        health.TakeDamage(damage*damageMultiplier);
    }

    public void TakeDamage(float damage,RaycastHit hit){
        health.TakeDamage(damage*damageMultiplier);
        if(hasBloodEffect)
            spurtBlood(hit);
        if(hasTextureEffect && !isDamaged){
            Renderer objRender = relatedLimb.GetComponent<Renderer>();
            Material[] objMat = objRender.materials;
            Material[] newMat = new Material[objMat.Length+1];
            int matIndex = 0;
            foreach(Material mat in objMat){
                newMat[matIndex] = objMat[matIndex];
                matIndex++;
            }
            newMat[objMat.Length] = bloodyMaterial;
            objRender.materials = newMat;
            isDamaged = true;
        }
    }

    private void spurtBlood(RaycastHit hit){
        GameObject bloodEffectGameObject = Instantiate(bloodEffectPrefab,hit.point,Quaternion.identity);
        Rigidbody rb = bloodEffectGameObject.GetComponent<Rigidbody>();
        Vector3 calcDir = (Vector3.Cross(hit.point,hit.normal)*magnitude);
        rb.velocity = new Vector3(Mathf.Clamp(calcDir.x,minForce,2),Mathf.Clamp(calcDir.y,minForce,maxForce)+additionalUpForce,Mathf.Clamp(calcDir.z,minForce,maxForce));

    }

    public Health GetHealth(){
        return health;
    }
}
