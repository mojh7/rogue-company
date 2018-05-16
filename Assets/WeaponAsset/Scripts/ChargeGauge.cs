using System.Collections;
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
    private Transform gaugeSpriteTrasnform;

    private Vector3 scaleVec;
    [SerializeField]
    private float fullChargedScaleX;    // 풀 차징시 gaugeSpriteObj에 scale X 값, 기본 1.8f
    // Use this for initialization
    void Start ()
    {
        scaleVec = new Vector3(0f, 0.2f, 1f);
        gaugeSpriteTrasnform = gaugeSpriteObj.GetComponent<Transform>();
        fullChargedScaleX = 1.8f;
        frame.SetActive(false);
        backGround.SetActive(false);
        gaugeSpriteObj.SetActive(false);
    }
	
    public void UpdateChargeGauge(float chargedVaule)
    {
        if (chargedVaule <= 0.05f)
        {
            frame.SetActive(false);
            backGround.SetActive(false);
            gaugeSpriteObj.SetActive(false);
            return;
        }
        frame.SetActive(true);
        backGround.SetActive(true); 
        gaugeSpriteObj.SetActive(true);

        if (chargedVaule > 1.0f) chargedVaule = 1.0f;
        scaleVec.x = fullChargedScaleX * chargedVaule;
        gaugeSpriteTrasnform.localScale = scaleVec; 
    }
}
