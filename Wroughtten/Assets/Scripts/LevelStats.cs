using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelStats : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameManager gm;
    [SerializeField] private DBManager db;
    [SerializeField] private Text kills;
    [SerializeField] private Text time;
    [SerializeField] private Text score;
    [SerializeField] private Text leaderboard;


    void Start(){
        gameManager = GameObject.FindGameObjectWithTag("gamemanager");
        db = gameManager.GetComponent<DBManager>();
        gm = gameManager.GetComponent<GameManager>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        uploadScore();
        displayScores();
    }

    private void uploadScore(){
        gm.SaveScore();
    }

    private void displayScores(){
        Score currentScore = gm.getScore();
        kills.text = "Kills: " + currentScore.kills;
        time.text = "Time: " + currentScore.time;   
        score.text = "Score: " + currentScore.CalcScore();
        //db.AttemptGetScores(5,currentScore.scene,ref lb);
        StartCoroutine(db.getScores(10,currentScore.scene,result=>{

            leaderboard.text = FormatTable(result);
        }));
    }

    private string FormatTable(string data){

        string headings = "Name\tTime\t\tKills\tScore";

        string ret = headings+"\n";

        string[] rows = data.Split('|');
        foreach(string row in rows){
            string[] entries = row.Split(',');
            if(entries.Length<3)
                continue;
            for(int i=0 ; i < 4; i++ ){
                ret += entries[i]+"\t|\t";
            }
            ret += "\n";
        }
        return ret;
    }

    public void NextLevel(){
        int nextScene = gm.getScore().scene + 1; 
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (SceneManager.sceneCountInBuildSettings > nextScene){
            SceneManager.LoadScene(nextScene);
        }else{
            SceneManager.LoadScene(0);
        }
    }
}
