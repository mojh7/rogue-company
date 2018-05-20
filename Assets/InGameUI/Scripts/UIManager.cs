using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * ui manager 한개에서 ingame ui랑 기타 시작화면, 설정화면 ? 등등 다른 ui도 관리 할지
 * 
 * ingame 용이랑 기타 ui랑 구분 할지 아직 모르겠음.
 * 
 * ingame 용이랑 구분
 */
public class UIManager : MonoBehaviourSingleton<UIManager> {

    #region variables
    public Canvas canvas;
    public GameObject ingamePanel;
    public GameObject preventObj;
    public Image fadeImage;
    #endregion

    #region function

    public void TogglePreventObj()
    {
        if (preventObj == null)
            return;
        preventObj.SetActive(!preventObj.activeSelf);
    }

    public void ToggleUI()
    {
        FadeOut(fadeImage, 20);
    }

    void FadeOut(Image _img,int _interval)
    {
        StartCoroutine(CoroutineFadeOut(_img, _interval));
    }

    IEnumerator CoroutineFadeOut(Image _img,int _interval)
    {
        float time = (float)0.5f / _interval;

        for (int i = _interval; i >= 0; i--)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForSeconds(time);
        }
    }

    void FadeIn(Image _img, int _interval)
    {
        StartCoroutine(CoroutineFadeIn(_img, _interval));
    }

    IEnumerator CoroutineFadeIn(Image _img, int _interval)
    {
        float time = (float)0.5f / _interval;

        for (int i = 0; i <= _interval; i++)
        {
            _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, (float)i / _interval);
            yield return YieldInstructionCache.WaitForSeconds(time);
        }
    }
    #endregion
}
