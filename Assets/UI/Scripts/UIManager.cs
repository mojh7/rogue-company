﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * ui manager 한개에서 ingame ui랑 기타 시작화면, 설정화면 ? 등등 다른 ui도 관리 할지
 * 
 * ingame 용이랑 기타 ui랑 구분 할지 아직 모르겠음.
 * 
 * ingame 용이랑 구분
 */
public class UIManager : MonoBehaviourSingleton<UIManager> {

    #region variables
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject ingamePanel;
    [SerializeField] private GameObject preventObj;
    [SerializeField] private GameObject menuObj;
    [SerializeField] private GameObject gameOverObj;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Text coinText;
    [SerializeField] private Text KeyText;
    [SerializeField] private GameObject clearObj;
    [SerializeField] private Image[] clearImage;
    [SerializeField] private GameObject warningUI;

    public BossHPUI bossHPUI;
    bool actived = false;
    bool isHide = true;
    #endregion

    #region function
    public void PlayMenuSound()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void OpenWarningUI()
    {
        warningUI.SetActive(true);
        UtilityClass.Invoke(this, CloseWarningUI, 1f);
    }

    private void CloseWarningUI()
    {
        warningUI.SetActive(false);
    }

    public void GameOverUI()
    {
        gameOverObj.SetActive(true);
        UIManager.Instance.SetActivedBool(true);
    }

    public void SetActivedBool(bool _actived)
    {
        actived = _actived;
    }

    public bool GetActived()
    {
        return actived;
    }

    public void SetCoinText(int _num)
    {
        if (null == coinText)
            return;
        coinText.text = _num.ToString();
    }

    public void SetKeyText(int _num)
    {
        if (null == KeyText)
            return;
        KeyText.text = _num.ToString();
    }

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
        if(preventObj.activeSelf)
            UIManager.Instance.SetActivedBool(false);
        else
            UIManager.Instance.SetActivedBool(true);
        preventObj.SetActive(!preventObj.activeSelf);
    }

    public void FadeInScreen()
    {
        FadeIn(fadeImage, 20);
    }

    void FadeIn(Image _img,int _interval)
    {
        StartCoroutine(CoroutineFadeIn(_img, _interval));
    }

    IEnumerator CoroutineFadeIn(Image _img,int _interval)
    {
        for (int i = _interval; i >= 0; i--)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
    }

    void FadeOut(Image _img, int _interval)
    {
        StartCoroutine(CoroutineFadeOut(_img, _interval));
    }

    IEnumerator CoroutineFadeOut(Image _img, int _interval)
    {
        for (int i = 0; i <= _interval; i++)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
    }

    public void ClearRoomUI(bool isActive)
    {
        clearObj.GetComponent<ClearRoomUI>().Clear();
    }

    public void BackToTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    #endregion

    #region UnityFunc
    private void Start()
    {
        SetCoinText(GameDataManager.Instance.GetCoin());
        SetKeyText(GameDataManager.Instance.GetKey());
    }
    #endregion
}
