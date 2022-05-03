using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Image healthImage;
    [SerializeField] Health health;

    private void Awake(){
        health = GetComponent<Health>();
    }

    private void Update(){
        healthImage.fillAmount = health.getHealth()/health.getMaxHealth();
    }
}
