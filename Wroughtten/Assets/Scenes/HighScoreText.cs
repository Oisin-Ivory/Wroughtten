using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighScoreText : MonoBehaviour
{
    [SerializeField] TextMesh text;
    [SerializeField] GameManager gm;
    
    void Start(){
        StartCoroutine(GetScores(gm.playerName,gm.sceneIndex));
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
            SetText(dataText);
            Debug.Log(dataText);
        }
    }
    
    private void SetText(string highscore){
        string displayText = "";
        string[] values = highscore.Split(',');
        if(values.Length < 4) return;
        displayText += "Name____Time____Kills___Score\n";
        displayText += values[0] + "____"+values[1]+"____"+values[2] + "____"+values[3];
        text.text = displayText;
    }   
}
