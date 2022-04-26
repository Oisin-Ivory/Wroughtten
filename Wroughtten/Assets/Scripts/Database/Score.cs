using System;

[System.Serializable]
public class Score{
    public string name;
    public float time;
    public int scene,enemiesAtStart,kills;

    public Score(string name,float time,int kills,int enemiesAtStart,int scene){
        this.name = name;
        this.time = time;
        this.kills = kills;
        this.scene = scene;
        this.enemiesAtStart = enemiesAtStart;
    }

    public float CalcScore(){
        
        return (((float)this.kills/(float)enemiesAtStart)/this.time)*100;
    }

    public string toString(){
        return "Name: "+name+"\nTime: "+time+"\nKills: "+kills+"\nScene: "+scene;
    }

}
