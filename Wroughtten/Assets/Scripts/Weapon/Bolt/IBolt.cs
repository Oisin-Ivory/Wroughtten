using UnityEngine;

public interface IBolt {
    void UpdateBoltPosition(float inputX,float inputY);
    void UpdateRoundPosition();
    void EjectRound();
}