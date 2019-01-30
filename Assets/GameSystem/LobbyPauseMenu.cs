using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPauseMenu : MonoBehaviourSingleton<LobbyPauseMenu> {

    #region variables
    [SerializeField] private GameObject preventObj;
    [SerializeField] private GameObject menuObj;
    [SerializeField] private Text coinText;
    [SerializeField] private Text KeyText;

    bool actived = false;
    bool isHide = true;
    #endregion

    #region unityFunc
    private void Awake()
    {
        preventObj.SetActive(false);
    }
    #endregion

    #region func
    public void PlayMenuSound()
    {
            return;
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void SetActivedBool(bool _actived)
    {
        actived = _actived;
    }

    public bool GetActived()
    {
        return actived;
    }

    //public void SetCoinText(int _num)
    //{
    //    if (null == coinText)
    //        return;
    //    coinText.text = _num.ToString();
    //}

    //public void SetKeyText(int _num)
    //{
    //    if (null == KeyText)
    //        return;
    //    KeyText.text = _num.ToString();
    //}

    public void ReturnTitle()
    {
        TimeController.Instance.StartTime();
        GameStateManager.Instance.LoadTitle();
    }

    public void ToggleMenu()
    {
        PlayMenuSound();
        if (menuObj.activeSelf)
        {
            TimeController.Instance.StartTime();
        }
        else
        {
            TimeController.Instance.StopTime();
        }
        TogglePreventObj();
        menuObj.SetActive(!menuObj.activeSelf);
    }

    public void TogglePreventObj()
    {
        if (preventObj == null)
            return;
        if (preventObj.activeSelf)
            UIManager.Instance.SetActivedBool(false);
        else
            UIManager.Instance.SetActivedBool(true);
        preventObj.SetActive(!preventObj.activeSelf);
    }
    #endregion

    #region UnityFunc
    //private void Start()
    //{
    //    SetCoinText(GameDataManager.Instance.GetCoin());
    //    SetKeyText(GameDataManager.Instance.GetKey());
    //}
    #endregion
}
