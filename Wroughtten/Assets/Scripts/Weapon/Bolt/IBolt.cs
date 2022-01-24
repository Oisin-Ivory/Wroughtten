using UnityEngine;

public interface IBolt {
   
    void UpdateBoltPosition(float inputX,float inputY);
    void UpdateRoundPosition();
    void EjectRound();
    void SetFreezeState(bool state);
    bool GetFreezeState();
    bool GetIsHoldingOpen();
    int FireRound();
    bool getHeld();
    void setHeld(bool state);

    bool getClipAutoEject();
}
