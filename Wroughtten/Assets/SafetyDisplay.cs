using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafetyDisplay : MonoBehaviour
{
   [SerializeField] Sprite safe;
   [SerializeField] Sprite semi;
   [SerializeField] Sprite auto;
   [SerializeField] HandManager hm;
   [SerializeField] Image image;

   void Update(){
       if(hm.HasWeapon()){
           Weapon wpn = hm.GetWeapon();

           switch (wpn.getSafteyState()){
            case FireSelectState.SAFE:
                image.sprite = safe;
            break;
            case FireSelectState.SEMI:
                image.sprite = semi;
                
            break;
            case FireSelectState.AUTO:
                image.sprite = auto;
            break;
           }
       }
   }
}
