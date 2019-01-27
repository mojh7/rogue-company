using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 화면 터치하면 화면 깜빡거리면서 셀렉트이미지로 변경

public class TitleTouch : MonoBehaviour
{

    public GameObject RestartButton;
    public GameObject StartNew;
    private bool isRestart = false;
    [SerializeField]
    private Text touch;
    [SerializeField]
    private Image image;
    private bool isHide = true;
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private Image modeButton;
    [SerializeField]
    private Sprite[] modeImages;
    [SerializeField]
    private GameObject creditUI;
    [SerializeField]
    private RectTransform titleTransform;
    private Vector3 titleScale;

    int index = 2;
    int mode = 0; //0 = normal, 1 = rush

    #region unityFunc
    private void Awake()
    {
        AudioManager.Instance.PlayMusic(0);
        InitTitle();
    }

    private void Update()
    {
        if (isHide)
        {
            Color color = touch.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            touch.color = color;
        }
        else
        {
            Color color = touch.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            touch.color = color;
        }
    }
    #endregion

    #region func
    public void OpenCreditUI()
    {
        creditUI.SetActive(true);
    }

    public void CloseCreditUI()
    {
        creditUI.SetActive(false);
    }

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
        touch.gameObject.SetActive(false);
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

        if (GameDataManager.Instance.LoadData(GameDataManager.UserDataType.INGAME))
            RestartButton.SetActive(true);
        else
            RestartButton.SetActive(false);

        titleScale = titleTransform.localScale;
        StartCoroutine(RepeatTitleScaleLargerAndSmaller());
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadSelect()
    {
        GameStateManager.Instance.SetLoadsGameData(false);
        GameDataManager.Instance.ResetData(GameDataManager.UserDataType.INGAME);
        GameStateManager.Instance.LoadSelect();
    }

    public void LoadInGame()
    {
        GameStateManager.Instance.SetLoadsGameData(true);
        GameStateManager.Instance.LoadInGame();
    }
    #endregion

    IEnumerator RepeatTitleScaleLargerAndSmaller()
    {
        float time = 0;
        while(true)
        {
            titleTransform.localScale = titleScale * (1f + 0.05f * Mathf.Sin(time));
            time += Time.fixedDeltaTime * 4.5f;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
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
}
