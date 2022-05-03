using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] GameObject crosshairImage;
    [SerializeField] GameObject loadableImage;
    [SerializeField] GameObject reloadableImage;
    [SerializeField] Sprite crosshair;
    [SerializeField] Sprite interactable;
    [SerializeField] HandManager handManager;

    private Image crosshairImageSprite;

    void Awake(){
        crosshairImageSprite = crosshairImage.GetComponent<Image>();
    }
    void Update(){
        if(handManager.IsADS()){
            crosshairImage.SetActive(false);
            reloadableImage.SetActive(false);
            loadableImage.SetActive(false);
        }else{
            crosshairImage.SetActive(true);
            crosshairImageSprite.sprite = crosshair;
            GameObject go = handManager.GetObjectLookingAt();

            if(go==null)
                return;
            
            InteractionProperties ip;
            if(go.TryGetComponent<InteractionProperties>(out ip)){
                if(ip.canPickup){
                    crosshairImageSprite.sprite = interactable;
                }
            }

            Reloadable reloadable;
            Loadable loadable;

            reloadableImage.SetActive(go.TryGetComponent<Reloadable>(out reloadable));
            loadableImage.SetActive(go.TryGetComponent<Loadable>(out loadable));

        }
        
    }
    
}
