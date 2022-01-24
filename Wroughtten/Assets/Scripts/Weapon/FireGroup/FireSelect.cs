using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSelect : MonoBehaviour
{
    [SerializeField] private FireSelectState[] selectorStates;
    [SerializeField] public bool locked = false;
    [SerializeField] int state;
    [SerializeField] int numStates;
    // Start is called before the first frame update
    void Start()
    {
        numStates = selectorStates.Length;
    }

    public void NextState(){
        if(locked)return;
        state++;
        state = state % numStates;
    }
    public FireSelectState getState(){
        return selectorStates[state];
    }
}
public enum FireSelectState{
    SAFE,
    SEMI,
    AUTO
}