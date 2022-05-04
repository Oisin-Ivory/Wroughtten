using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemoTools : MonoBehaviour
{
    [SerializeField] Health playerHealth;

    [SerializeField] GameObject godmodeIndicator;
    [SerializeField] GameObject timeIndicator;

    bool godMode = false;
    bool slowDown = false;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.O)){
            ToggleGodMode();
        }
        if(Input.GetKeyUp(KeyCode.P)){
            ToggleSlowDown();
        }
    }

    void ToggleGodMode(){
        godMode = !godMode;
        godmodeIndicator.SetActive(godMode);
        playerHealth.enabled = !godMode;
    }

    void ToggleSlowDown(){
        slowDown = !slowDown;
        timeIndicator.SetActive(slowDown);
        Time.timeScale = slowDown ? 0.1f : 1f;
        print(Time.timeScale);
    }
}
