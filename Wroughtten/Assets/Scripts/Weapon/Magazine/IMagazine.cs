using UnityEngine;

public interface IMagazine {
    GameObject FeedRound();
    void LoadRound(GameObject roundToLoad);
    void UpdateBulletPosition();
    int getBulletCount();

    bool getCanAcceptAmmo();
    void setCanAcceptAmmo(bool state);
    GameObject gameObject { get ; }
}