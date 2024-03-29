﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGauge : MonoBehaviour {

    // frame, backGround, gaugeSpriteObj 3개는 inspector 창에서 붙였음

    [SerializeField]
    private GameObject frame;   // 검은색 틀
    [SerializeField]
    private GameObject backGround;  // 흰색 배경
    [SerializeField]
    private GameObject gaugeSpriteObj; // 차징 게이지 sprite
    [SerializeField]
    private GameObject fullChargedUI; // 풀 차징 표시 ui
    private Transform gaugeSpriteTrasnform;

    private Vector3 scaleVec;
    [SerializeField]
    private float fullChargedScaleX;    // 풀 차징시 gaugeSpriteObj에 scale X 값, 기본 1.8f
    // Use this for initialization
    void Start ()
    {
        scaleVec = new Vector3(0.0f, 0.2f, 1f);
        gaugeSpriteTrasnform = gaugeSpriteObj.GetComponent<Transform>();
        fullChargedScaleX = 1.8f;
        SetActiveChargeUI(false);
    }
	
    private void SetActiveChargeUI(bool boolean)
    {
        frame.SetActive(boolean);
        backGround.SetActive(boolean);
        gaugeSpriteObj.SetActive(boolean);
        fullChargedUI.SetActive(boolean);
    }

    /// <summary>
    /// 매개변수에 따른 차징 게이지 UI 업데이트
    /// </summary>
    /// <param name="chargedVaule"></param>
    public void UpdateChargeGauge(float chargedVaule)
    {
        // 연속으로 터치시 바로 차징 게이지 안뜨게 하기 위함
        if (chargedVaule <= 0.03f)
        {
            SetActiveChargeUI(false);
            return;
        }
        SetActiveChargeUI(true);
        fullChargedUI.SetActive(false);

        // 풀 차징
        if (chargedVaule >= 1.0f)
        {
            chargedVaule = 1.0f;
            fullChargedUI.SetActive(true);
            AudioManager.Instance.PlaySound(2, SOUNDTYPE.UI);
        }
        scaleVec.x = fullChargedScaleX * chargedVaule;
        gaugeSpriteTrasnform.localScale = scaleVec; 
    }
}
