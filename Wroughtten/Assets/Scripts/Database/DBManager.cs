using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class DBManager : MonoBehaviour{

    private string result = "";
    public void SaveScore(Score score,int enemiesAtStart){
        StartCoroutine(PostScores(score,enemiesAtStart));
    }
    public string getScores(string name, int scene){
        StartCoroutine(GetScores(name,scene));
        return "";
        // int ittermax = 10000;
        // int itter = 0;
        // while(result==""){
        //     if(itter > 1000000000){
        //         Debug.Log("too long");
        //         return "";
        //     }
        //     itter++;
        // }
        // Debug.Log(itter);
        // string returnval = result;
        // result = "";
        // return returnval;
    }

    IEnumerator PostScores(Score score,int enemiesAtStart){
        string url = "20083797.000webhostapp.com/scores.php";
        url += "?name="+score.name+"&time="+score.time+"&kills="+score.kills+"&score="+score.CalcScore(enemiesAtStart)+"&scene="+score.scene;
        Debug.Log(url);
        UnityWebRequest hs_get = UnityWebRequest.Get(url);
        yield return hs_get.SendWebRequest();
        if (hs_get.error != null)
            Debug.Log("There was an error posting the high score: "
                    + hs_get.error);
        else
        {
            string dataText = hs_get.downloadHandler.text;
            Debug.Log(dataText);
        }
    }

    IEnumerator GetScores(string name,int scene){
        string url = "20083797.000webhostapp.com/getscores.php";
        url += "?name="+name+"&scene="+scene;
        UnityWebRequest hs_get = UnityWebRequest.Get(url);
        yield return hs_get.SendWebRequest();
        if (hs_get.error != null)
            Debug.Log("There was an error getting the high score: "
                    + hs_get.error);
        else
        {
            string dataText = hs_get.downloadHandler.text;
            Debug.Log(dataText);
        }
    }

}