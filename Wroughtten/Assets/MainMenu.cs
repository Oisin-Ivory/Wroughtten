using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] DBManager db;


    void Awake(){
        db = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<DBManager>(); 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play(){
        PlayerPrefs.SetFloat("score",120);
        SceneManager.LoadScene(2);
    }

    public void Exit(){
        Application.Quit();
    }
}
