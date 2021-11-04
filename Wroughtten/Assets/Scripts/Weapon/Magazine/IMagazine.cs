using UnityEngine;

public interface IMagazine {
    GameObject FeedRound();
    void LoadRound(GameObject roundToLoad);
    void UpdateBulletPosition();
    int getBulletCount();

    GameObject gameObject { get ; }
}