using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DBLogin : MonoBehaviour
{
    [SerializeField] DBManager db;
    [SerializeField] Text username;
    [SerializeField] Text password;
    [SerializeField] GameObject errorText;
    
    void Awake(){
        db = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<DBManager>();
    }
    void Start(){
        if(db.GetPlayerName()!=""){
            Destroy(GameObject.Find("Login"));
        }
    }
    public void AttemptLogin(){
        db.attemptLogIn(username.text,password.text);
    }    
    public void AttemptRegister(){
        db.attemptRegister(username.text,password.text);
        Debug.Log(username.text + " : "+password.text);
    }
    
    
}
