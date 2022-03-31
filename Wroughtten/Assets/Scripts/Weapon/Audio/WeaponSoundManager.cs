using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [Header("Shooting")]
    [SerializeField] AudioClip shoot;
    [SerializeField] float shootAudioDistance;

    [Header("Bolt")]
    [SerializeField] AudioClip boltForward;
    [SerializeField] AudioClip boltBackward;
    [SerializeField] AudioClip boltStageTransition;

    public void ShootAudio(Vector3 hitPosition){
        audioSource.PlayOneShot(shoot);
        Collider[] objectsInHearingDist = Physics.OverlapSphere(hitPosition,shootAudioDistance);
        foreach(Collider col in objectsInHearingDist){
            AIController aiController;
            if(col.gameObject.TryGetComponent<AIController>(out aiController)){
                aiController.Investigate(hitPosition);
            }
            
        }
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position,shootAudioDistance);
    }
}
