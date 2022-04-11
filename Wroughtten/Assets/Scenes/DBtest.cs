using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DBtest : MonoBehaviour
{
    [SerializeField] GameManager gm;

    private void OnTriggerEnter(Collider col){
        Debug.Log(col.gameObject.name);
        if(col.gameObject.tag == "Player"){
            gm.SaveScore();
            SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        }
    }
}
