using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag=="Player"){
            GameObject gmo = GameObject.FindGameObjectWithTag("gamemanager");
            GameManager gm = gmo.GetComponent<GameManager>();
            gm.SaveCurrentScore();
            gm.GoToLevelStats();
        }
    }
}
