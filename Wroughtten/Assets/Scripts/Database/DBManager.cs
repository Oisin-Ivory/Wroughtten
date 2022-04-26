using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.Security.Cryptography;

public class DBManager : MonoBehaviour{
    [SerializeField] string playerName = null;
    string mainURL = "https://20083797.000webhostapp.com/wroughtten/";
    private GameObject errorText;
    void Awake(){
        errorText = GameObject.Find("Error_Text");
        errorText.SetActive(false);
    }
    void Start(){
        DontDestroyOnLoad(this.gameObject);
    }

    public string GetPlayerName(){
        return playerName;
    }

    public void attemptSaveScore(Score score,int enemies){
        StartCoroutine(SaveScore(score,enemies,result=>{
            
        }));
    }

    public void attemptLogIn(string userName,string password){
        StartCoroutine(LogIn(userName,GetHashString(password)));
    }

    public void attemptRegister(string userName,string password){
        StartCoroutine(Register(userName,GetHashString(password)));
    }

    public static byte[] GetHash(string inputString){
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString){
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }


    IEnumerator LogIn(string userName,string password){
        string url = mainURL+"login.php";
        url += "?username="+userName+"&password="+password;
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www.text);
        if(www.text!="Failed"){
            string[] credentials = www.text.Split(' ');
            playerName = credentials[0];
            GetComponent<GameManager>().playerName = playerName;
            Destroy(GameObject.Find("Login"));
        }else{
            StartCoroutine(displayError());
        }
    }

    public IEnumerator displayError(){
        
        errorText.SetActive(true);
        yield return new WaitForSeconds(5);
        errorText.SetActive(false);
    }

    IEnumerator Register(string userName,string password){
        string url = mainURL+"register.php";
        url += "?username="+userName+"&password="+password;
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www.text);
        if(www.text!="Failed"){
            string[] credentials = www.text.Split(' ');
            playerName = credentials[0];
            StartCoroutine(LogIn(userName,password));
        }
    }

    public IEnumerator SaveScore(Score score,int enemiesAtStart,System.Action<bool> callbackOnFinish){
        string url = mainURL+"uploadScore.php";
        url += "?name="+score.name+"&time="+score.time+"&kills="+score.kills+"&score="+score.CalcScore()+"&scene="+score.scene;
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www.text);
        callbackOnFinish(true);
    }

    // public void AttemptGetScores(int numScores,int scene,ref String stringToBuild){
    //     string finalStr = "";
    //     StartCoroutine(getScores(numScores,scene,(result)=>{
    //         finalStr = result;
    //     }));
    //     while(finalStr == ""){}
    //     stringToBuild = finalStr;
    // }

    public IEnumerator getScores(int numScores,int scene,System.Action<String> callbackOnFinish){
        string url = mainURL+"getScores.php";
        url += "?num="+numScores+"&scene="+scene;
        WWW www = new WWW(url);
        while(!www.isDone){}
        yield return www.text;
        callbackOnFinish(www.text);
    }

}