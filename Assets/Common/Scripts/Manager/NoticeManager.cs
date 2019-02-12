using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 임시

public class NoticeManager : MonoBehaviourSingleton<NoticeManager>
{
    public Transform bkgTransform;
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
    }

    private IEnumerator ShowNoticeCoroutine()
    {
        bkgTransform.gameObject.SetActive(true);
        txt.enabled = true;
        float scaleY = 0;
        float alpha = 0;
        Color color = Color.white;
        color.a = 0;
        bkgTransform.localScale = new Vector3(bkgTransform.localScale.x, 0, 0);
        txt.color = color;
        while(bkgTransform.localScale.y < 150)
        {
            bkgTransform.localScale = new Vector3(bkgTransform.localScale.x, scaleY, 0);
            scaleY += 150f * Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        while (alpha < 255)
        {
            txt.color = color;
            alpha += 255f * Time.fixedDeltaTime;
            color.a = alpha;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return YieldInstructionCache.WaitForSeconds(2f);
        bkgTransform.gameObject.SetActive(false);
        txt.enabled = false;
    }
}
