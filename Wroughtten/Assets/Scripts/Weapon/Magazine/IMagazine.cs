using UnityEngine;

public interface IMagazine {
    GameObject FeedRound();
    void LoadRound(GameObject roundToLoad);
    void UpdateBulletPosition();
    int getBulletCount();
    int getMagazineCapacity();
    bool getCanAcceptAmmo();
    void setCanAcceptAmmo(bool state);
    string[] getCompAmmoTags();
    GameObject gameObject { get ; }
}