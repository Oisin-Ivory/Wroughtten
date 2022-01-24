using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBob : MonoBehaviour
{
    [SerializeField] GameObject objectToBob;
    [SerializeField] CharacterController playerCharCntrl;
    [SerializeField] float frequency;
    [SerializeField] float amplitude;
    [SerializeField] float speedMultiplier;
    [SerializeField] float maxBobSpeed;

    void Update(){
        speedMultiplier = getSpeed()/maxBobSpeed;
        checkMotion();
        resetPos();
    }
    private float getSpeed(){
        float speed = new Vector3(playerCharCntrl.velocity.x,0,playerCharCntrl.velocity.z).magnitude;
        return speed;
    }

    private Vector3 walkingMovement(){
        Vector3 pos = Vector3.zero;
        pos.x = (Mathf.Cos(Time.time * (frequency/2)) * amplitude * 2) * speedMultiplier;
        //pos.y = (Mathf.Sin(Time.time * frequency) * amplitude) * speedMultiplier;
        return pos;
    }

    private void checkMotion(){
        if(playerCharCntrl.isGrounded){
            return;
        }
        PlayMotion(walkingMovement());
    }

    private void resetPos(){
        if(objectToBob.transform.localPosition == Vector3.zero)return;

        objectToBob.transform.localPosition = Vector3.Lerp(objectToBob.transform.localPosition,Vector3.zero,Time.deltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        //Debug.Log("Chaning position" + motion);
        objectToBob.transform.localPosition += motion;

    }
}
