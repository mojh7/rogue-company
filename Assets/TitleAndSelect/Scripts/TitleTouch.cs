using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 화면 터치하면 화면 깜빡거리면서 셀렉트이미지로 변경

public class TitleTouch : MonoBehaviour {

    public GameObject RestartButton;
    public GameObject StartNew;
    private bool isRestart = false;
    [SerializeField] private Text txt;
    [SerializeField] private Image image;
    private bool isHide = true;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image modeButton;
    [SerializeField] private Sprite[] modeImages;

    int index = 2;
    int mode = 0; //0 = normal, 1 = rush

    public void ChangeMode()
    {
        if(mode>=1)
        {
            mode = 0;
        }
        else
        {
            mode++;
        }

        GameStateManager.Instance.SetMode((GameStateManager.GameMode)mode);
        InitTitle();
    }

    public void ClickStart()
    {
        StartNew.SetActive(false);
        isRestart = false;
        FadeInScreen();
    }

    public void ClickRestart()
    {
        StartNew.SetActive(false);
        FadeInScreen();
        isRestart = true;
    }

    public void FadeInScreen()
    {
        txt.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
        FadeIn(fadeImage, 100);
    }

    void FadeIn(Image _img, int _interval)
    {
        StartCoroutine(CoroutineFadeIn(_img, _interval));
    }

    void InitTitle()
    {
        modeButton.sprite = modeImages[(int)GameStateManager.Instance.GetMode()];

        if (GameDataManager.Instance.LoadData())
            RestartButton.SetActive(true);
        else
            RestartButton.SetActive(false);
    }

    IEnumerator CoroutineFadeIn(Image _img, int _interval)
    {
        for (int i = _interval; i >= 0; i--)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        if (index == 0)
        {
            if (!isRestart)
                LoadSelect();
            else
                LoadInGame();
        }
        else
        {
            FadeIn(fadeImage, 20 * index);
            index--;
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadSelect()
    {
        GameStateManager.Instance.SetLoadsGameData(false);
        GameDataManager.Instance.ResetData();
        GameStateManager.Instance.LoadSelect();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.SetLoadsGameData(true);
        GameStateManager.Instance.LoadInGame();
    }

    private void Awake()
    {
        AudioManager.Instance.PlayMusic(0);
        InitTitle();
    }

    private void Update()
    {
        if (isHide)
        {
            Color color = txt.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            txt.color = color;
        }
        else
        {
            Color color = txt.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            txt.color = color;
        }
    }
}
