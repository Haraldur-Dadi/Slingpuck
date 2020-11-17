using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener {
    #if UNITY_ANDROID
    string gameID = "3798931";
    #elif UNITY_IOS
    string gameID = "3798930";
    #endif
    string rewardVideoID = "rewardedVideo";
    public Button rewardAdsBtn;

    public static AdManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            Advertisement.AddListener(this);
            Advertisement.Initialize(gameID);
            rewardAdsBtn.interactable = Advertisement.IsReady(rewardVideoID);
        }
    }

    public void ShowVideoAd() {
        // 2/5 chance to show an Ad, after every game
        if (Random.Range(0, 5) > 2) {
            Advertisement.Show();
        }
    }
    public void ShowRewardVideo() {
        Advertisement.Show(rewardVideoID);
    }

    public void OnUnityAdsReady(string placementId) {
        if (placementId == rewardVideoID) {
            rewardAdsBtn.interactable = true;
        }
    }
    public void OnUnityAdsDidStart(string placementId) {}
    public void OnUnityAdsDidError(string message) {}
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        if (placementId == rewardVideoID && showResult == ShowResult.Finished) {
            CurrencyManager.Instance.AddGold(50);
        }
    }
}
