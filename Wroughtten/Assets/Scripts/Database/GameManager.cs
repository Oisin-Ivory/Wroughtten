using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public string playerName;

    [SerializeField] string[] enemyTags;
    [SerializeField] int enemiesInSceneBeginning;
    [SerializeField] public int sceneIndex;
    [SerializeField] float timeInScene = 0;
    [SerializeField] DBManager db;

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        timeInScene = 0;
        GetScores();
        enemiesInSceneBeginning = getEnemiesInScene();
        sceneIndex = scene.buildIndex;
    }

    private int getEnemiesInScene(){
        int enemiesInScene = 0;
        foreach(string tag in enemyTags){
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            print(enemies.Length);
            Health enemyHealth;
            foreach(GameObject enemy in enemies){
                if(enemy.TryGetComponent<Health>(out enemyHealth)){
                    if(enemyHealth.getHealth() > 0){
                        enemiesInScene++;
                    }
                }
            } 
        }
        return enemiesInScene;
    }

    // Update is called once per frame
    void Update()
    {
        timeInScene += Time.deltaTime;

    }

    public void SaveScore(){
        
        int enemiesKilled = enemiesInSceneBeginning - getEnemiesInScene();
        Debug.Log(enemiesInSceneBeginning+"-"+getEnemiesInScene());
        Debug.Log(enemiesKilled);
        Score playerScore = new Score(playerName,timeInScene,enemiesKilled,SceneManager.GetActiveScene().buildIndex);
        db.SaveScore(playerScore,enemiesInSceneBeginning);
    }

    public string GetScores(){
        return db.getScores(playerName,SceneManager.GetActiveScene().buildIndex);
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
}
