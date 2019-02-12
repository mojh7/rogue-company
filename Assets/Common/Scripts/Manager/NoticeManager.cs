using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 임시

public class NoticeManager : MonoBehaviourSingleton<NoticeManager>
{
    public RectTransform bkgTransform;
    public Text txt;

    Coroutine showNoticeCoroutine;

    private void Awake()
    {
        bkgTransform.gameObject.SetActive(false);
        txt.enabled = false;
    }

    public void ShowNotice(string str)
    {
        txt.text = str;

        if(null != showNoticeCoroutine)
            StopCoroutine(showNoticeCoroutine);
        showNoticeCoroutine = StartCoroutine(ShowNoticeCoroutine());
        Debug.Log("ShowNotice");
    }

    private IEnumerator ShowNoticeCoroutine()
    {
        bkgTransform.gameObject.SetActive(true);
        txt.enabled = true;
        float scaleY = 0;
        float alpha = 0;
        Color color = Color.white;
        color.a = 0;
        bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, 0);
        txt.color = color;
        float showSpeed = 5f;
        float bkgHeight = 120f;
        // on
        while(bkgTransform.sizeDelta.y < bkgHeight)
        {
            bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);
            scaleY += showSpeed * bkgHeight * Time.fixedDeltaTime;
            if(alpha < 1f)
            {
                txt.color = color;
                alpha += showSpeed * Time.fixedDeltaTime;
                color.a = alpha;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        scaleY = bkgHeight;
        bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        // off
        while (bkgTransform.sizeDelta.y >= 0)
        {
            bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);
            scaleY -= showSpeed * bkgHeight * Time.fixedDeltaTime;
            if (alpha >= 0)
            {
                txt.color = color;
                alpha -= showSpeed * Time.fixedDeltaTime;
                color.a = alpha;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
        bkgTransform.gameObject.SetActive(false);
        txt.enabled = false;
    }
}
