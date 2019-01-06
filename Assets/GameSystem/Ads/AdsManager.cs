using UnityEngine;
using System;

namespace GoogleMobileAds.Api
{
    public class AdsManager : MonoBehaviourSingleton<AdsManager>
    {
        private RewardBasedVideoAd rewardBasedVideo;

        [SerializeField]
        string ads_App_Id;
        #region base
        public void Start()
        {
#if UNITY_ANDROID
            string appId = ads_App_Id;
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif

            MobileAds.Initialize(appId);

            // Get singleton reward based video ad reference.
            this.rewardBasedVideo = RewardBasedVideoAd.Instance;

            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
            this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
            this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
            this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
            this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
            this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
            this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

            this.RequestRewardBasedVideo();
        }

        private void RequestRewardBasedVideo()
        {
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-1798048662997653/5160629473";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

            AdRequest request = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
            this.rewardBasedVideo.LoadAd(request, adUnitId);
        }
        #endregion
        #region RewardBasedVideo callback handlers

        public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
        }

        public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
        }

        public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        }

        public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        }

        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            MonoBehaviour.print(
                "HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
        }

        public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        }

        #endregion
        private void ShowRewardBasedVideo()
        {
            if (this.rewardBasedVideo.IsLoaded())
            {
                this.rewardBasedVideo.Show();
            }
            else
            {
                MonoBehaviour.print("Reward based video ad is not ready yet");
            }
        }

        public void Show()
        {
            ShowRewardBasedVideo();
        }
    }

}
