using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingClass : MonoBehaviour {

    int framRate = 30;
    int qualityLevel;
    bool turnQuality;

    private void Awake()
    {
        Application.targetFrameRate = framRate;
        turnQuality = true;
    }

    void SetFrameRate(int i)
    {
        framRate = i;
        Application.targetFrameRate = framRate;
    }

    public void SetQualityLevel()
    {
        turnQuality = !turnQuality;
        if (turnQuality)
        {
            SetFrameRate(30);
            QualitySettings.SetQualityLevel(0);
        }
        else
        {
            SetFrameRate(-1);
            QualitySettings.SetQualityLevel(1);
        }
    }
}
