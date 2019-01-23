using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviourSingleton<TimeController> {

    private float currentScale = 1;
    private float oldScale = 1;

    private float playStartTime;
    private float elapsedTime;

    private bool isTimeLerp;
    private bool isTimeStop;

    private void Start()
    {
        elapsedTime = 0;
        isTimeLerp = false;
        isTimeStop = false;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void PlayStart()
    {
        playStartTime = elapsedTime;
    }
    public float GetPlayTime
    {
        get
        {
            return elapsedTime - playStartTime;
        }
    }

    public void StopTime()
    {
        isTimeStop = true;
        oldScale = currentScale;
        Time.timeScale = 0;
        currentScale = Time.timeScale;
    }
    public void StartTime()
    {
        isTimeStop = false;
        oldScale = currentScale;
        Time.timeScale = 1;
        currentScale = Time.timeScale;
    }
    public void MulTime(float mul)
    {
        oldScale = currentScale;
        Time.timeScale *= mul;
        currentScale = Time.timeScale;
    }
    public void LerpTimeScale(float src, float dest, float time)
    {
        if (src == 0 || dest == 0 || isTimeLerp || isTimeStop)
            return;
        isTimeLerp = true;
        StartCoroutine(CoroutineLerpTimeScale(src, dest, time));
    }
    IEnumerator CoroutineLerpTimeScale(float src, float dest, float time)
    {
        float startTime = Time.time;
        float tempElapsedTime = 0;
        while (time >= tempElapsedTime)
        {
            if (isTimeStop)
                break;
            tempElapsedTime = Time.time - startTime;

            Time.timeScale = Mathf.Lerp(src, dest, tempElapsedTime / time);
            currentScale = Time.timeScale;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        isTimeLerp = false;
    }
}
