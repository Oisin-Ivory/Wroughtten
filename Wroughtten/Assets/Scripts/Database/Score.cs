using System;

public class Score{
    public string name;
    public float time, kills;
    public int scene;

    public Score(string name,float time,float kills,int scene){
        this.name = name;
        this.time = time;
        this.kills = kills;
        this.scene = scene;
    }

    public float CalcScore(int enemiesInScene){
        
        return ((this.kills/enemiesInScene)/this.time)*100;
    }

    public string toString(){
        return "Name: "+name+"\nTime: "+time+"\nKills: "+kills+"\nScene: "+scene;
    }

}
