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
    [SerializeField] Score currentScore;

    void Awake(){
        db = GetComponent<DBManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        if(SceneManager.GetActiveScene().buildIndex < 2)
            return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(SceneManager.GetActiveScene().buildIndex < 2)
            return;
        timeInScene = 0;
        enemiesInSceneBeginning = getEnemiesInScene();
        sceneIndex = scene.buildIndex;
    }

    private int getEnemiesInScene(){
        int enemiesInScene = 0;
        foreach(string tag in enemyTags){
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            Debug.Log(enemies.Length + " Enemies found");
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

    public void SaveCurrentScore(){
        int enemiesKilled = enemiesInSceneBeginning - getEnemiesInScene();
        currentScore = new Score(playerName,timeInScene,enemiesKilled,enemiesInSceneBeginning,SceneManager.GetActiveScene().buildIndex);
    }

    public float getTime(){
        return timeInScene;
    }
    public float getKills(){
        return timeInScene;
    }
    public Score getScore(){
       return currentScore;
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex < 2)
            return;
        timeInScene += Time.deltaTime;

    }

    public void SaveScore(){
        db.attemptSaveScore(currentScore,enemiesInSceneBeginning);
    }



    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void beginGame(){
        SceneManager.LoadScene(2);
    }

    public void GoToLevelStats(){
        SceneManager.LoadScene(1);
    }
    
}
