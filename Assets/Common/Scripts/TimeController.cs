using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

    public static float currentScale = 1;

    public void StopTime()
    {
        Time.timeScale = 0;
        currentScale = Time.timeScale;
    }
    public void StartTime()
    {
        Time.timeScale = 1;
        currentScale = Time.timeScale;
    }
    public void MulTime(float mul)
    {
        Time.timeScale *= mul;
        currentScale = Time.timeScale;
    }
    public void LerpTime(float src, float dest, float time)
    {
        StartCoroutine(CoroutineLerpTime(src, dest, time));
    }
    IEnumerator CoroutineLerpTime(float src, float dest, float time)
    {
        float t = 0;
        while(time < Time.deltaTime)
        {
            t += Time.deltaTime / time;

            Time.timeScale = Mathf.Lerp(src, dest, t);
            currentScale = Time.timeScale;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }
}
