using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 임시

public class NoticeManager : MonoBehaviourSingleton<NoticeManager>
{
    [SerializeField]
    private GameObject noticeUiObj;
    [SerializeField]
    private RectTransform bkgTransform;
    [SerializeField]
    private Text noticeTxt;

    Coroutine showNoticeCoroutine;

    private void Awake()
    {
        noticeUiObj.SetActive(false);
    }

    public void ShowNotice(string str)
    {
        noticeTxt.text = str;

        if(null != showNoticeCoroutine)
            StopCoroutine(showNoticeCoroutine);
        showNoticeCoroutine = StartCoroutine(ShowNoticeCoroutine());
        Debug.Log("ShowNotice");
    }

    private IEnumerator ShowNoticeCoroutine()
    {
        noticeUiObj.SetActive(true);
        float scaleY = 0;
        float alpha = 0;
        Color color = Color.white;
        color.a = 0;
        bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, 0);
        noticeTxt.color = color;
        float showSpeed = 5f;
        float bkgHeight = 120f;
        // show
        while(bkgTransform.sizeDelta.y < bkgHeight)
        {
            bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);
            scaleY += showSpeed * bkgHeight * Time.fixedDeltaTime;
            if(alpha < 1f)
            {
                noticeTxt.color = color;
                alpha += showSpeed * Time.fixedDeltaTime;
                color.a = alpha;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        scaleY = bkgHeight;
        bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        // hide
        while (bkgTransform.sizeDelta.y >= 0)
        {
            bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);
            scaleY -= showSpeed * bkgHeight * Time.fixedDeltaTime;
            if (alpha >= 0)
            {
                noticeTxt.color = color;
                alpha -= showSpeed * Time.fixedDeltaTime;
                color.a = alpha;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
        noticeUiObj.SetActive(true);
    }
}
