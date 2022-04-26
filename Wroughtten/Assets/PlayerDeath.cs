using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    
    [SerializeField] GameObject camera;
    [SerializeField] Player_Mouse_Look look;
    [SerializeField] Player_Movement move;
    [SerializeField] GameObject deathScreen;
    [SerializeField] float timeToMove = 2;
    [SerializeField] Vector3 moveToPos;
    [SerializeField] LayerMask mask;
    bool isCalled = false;

    private void Awake(){
        camera = Camera.main.gameObject;
        look = camera.gameObject.GetComponent<Player_Mouse_Look>();
        move = GetComponent<Player_Movement>();
    }

    public void Die(){
        if(!isCalled){
            GetComponent<CharacterController>().enabled = false;
            look.enabled = false;
            move.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            
            camera.transform.parent = null;
            RaycastHit hit;
            if(Physics.Raycast(camera.transform.position,Vector3.down,out hit,100,mask)){
                moveToPos = hit.point+Vector3.up;
                Debug.DrawRay(camera.transform.position,Vector3.down,Color.magenta);
            }else{
                moveToPos = camera.transform.position;
            }

            

            deathScreen.SetActive(true);
            StartCoroutine(MoveToDeathPosition());
            isCalled = true;
        }
    }

    public void Retry(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }
    public IEnumerator MoveToDeathPosition(){
        
        float timeSpent = 0;
        while(timeSpent<timeToMove){
            //print("timeSpent: "+timeSpent);
            timeSpent+=Time.deltaTime;
            camera.transform.position = Vector3.Lerp(camera.transform.position,moveToPos,timeSpent/timeToMove);
            yield return true;        
        }
        yield return true;
        
    }
}
