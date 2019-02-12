using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 임시

public class NoticeManager : MonoBehaviourSingleton<NoticeManager>
{
    public RectTransform bkgTransform;
    public Text txt;

    private void Awake()
    {
        bkgTransform.gameObject.SetActive(false);
        txt.enabled = false;
    }

    public void ShowNotice(string str)
    {
        txt.text = str;
        StopCoroutine(ShowNoticeCoroutine());
        StartCoroutine(ShowNoticeCoroutine());
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
        while(bkgTransform.sizeDelta.y < 150)
        {
            bkgTransform.sizeDelta = new Vector2(bkgTransform.sizeDelta.x, scaleY);
            scaleY += 4 * 150f * Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        while (alpha < 255)
        {
            txt.color = color;
            alpha += 510f * Time.fixedDeltaTime;
            color.a = alpha;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        bkgTransform.gameObject.SetActive(false);
        txt.enabled = false;
    }
}
