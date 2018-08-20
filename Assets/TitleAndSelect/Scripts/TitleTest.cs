using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 화면 터치하면 화면 깜빡거리면서 셀렉트이미지로 변경

public class TitleTest : MonoBehaviour {

    [SerializeField] private Text txt;
    [SerializeField] private Image image;
    private bool isHide = true;

    [SerializeField] private Image fadeImage;

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

    IEnumerator CoroutineFadeIn(Image _img, int _interval)
    {
        for (int i = _interval; i >= 0; i--)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        LoadSelect();
    }

    private void LoadSelect()
    {
        GameStateManager.Instance.LoadSelect();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            FadeInScreen();
        }

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
