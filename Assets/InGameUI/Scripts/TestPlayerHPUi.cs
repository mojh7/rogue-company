using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerHPUi : MonoBehaviour {
    [SerializeField] private GameObject obj;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image delayHPImage;

    float hp;
    float oldHp;
    float remainHp;
    float maxHp;

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void SetHpBar(float _hp)
    {
        hp = _hp;
        maxHp = hp;
        oldHp = hp;
        remainHp = hp;
        hpImage.fillAmount = remainHp / hp;
    }

    public void DecreaseHp(float _hp)
    {
        oldHp = remainHp;
        remainHp = _hp;
        hpImage.fillAmount = remainHp / hp;
        StartCoroutine(CoroutineHP(oldHp, remainHp, hp, delayHPImage));
    }

    public void IncreaseHP(float _hp)
    {
        oldHp = remainHp;
        remainHp = _hp;
        hpImage.fillAmount = remainHp / hp;
        if (remainHp > maxHp)
        {
            hp = remainHp;
            maxHp = hp;
        }
        else
        {
            StartCoroutine(CoroutineHP(oldHp, remainHp, hp, hpImage));
        }
    }

    IEnumerator CoroutineHP(float _src, float _dest, float max, Image _image)
    {
        float temp;
        float t = 0;
        while (true)
        {
            yield return YieldInstructionCache.WaitForEndOfFrame;
            t += Time.deltaTime / 1f;

            temp = Mathf.Lerp(_src, _dest, t);
            _image.fillAmount = temp / max;
            if (temp == _dest)
                break;
        }
    }

    private void Update()
    {
        // 디버그용
        if (Input.GetKeyDown(KeyCode.Z))
        {
            remainHp--;
            DecreaseHp(remainHp);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            remainHp++;
            IncreaseHP(remainHp);
        }
    }

    // hp ui 갱신
    /*
    public void UpdateHPUI(float hp)
    {
        if (hp < 0) return;
        int remainder = (int)hp % column;

        // hp가 최대 표시 갯수를 넘을 때
        if (hp >= column)
        {
            overHpTextObj.SetActive(false);
            for (int i = 0; i < column; i++)
            {
                hpObjList[i].SetActive(true);
                hpImageList[i].sprite = hpFullSprite;
            }
            if (hp > column)
            {
                overHpTextObj.SetActive(true);
                // 반 칸 짜리 체력이 존재 할 경우
                if (hp - (int)hp == 0.5)
                {
                    //hpImageList[0].sprite = hpHalfSprite;
                    hpImageList[column - 1].sprite = hpHalfSprite;
                    overHp = (int)hp - column + 1;
                }
                else // 반 칸 짜리 체력이 존재 하지 않을 경우
                {
                    overHp = (int)hp - column;
                }

                overHpText.text = "+ " + overHp.ToString();
            }
        }
        else // column 수 이하 일 때 체력 ui 표시
        {
            overHpTextObj.SetActive(false);
            for (int i = 0; i < remainder; i++)
            {
                hpObjList[i].SetActive(true);
                hpImageList[i].sprite = hpFullSprite;

            }
            for (int i = remainder; i < column; i++)
            {
                hpObjList[i].SetActive(false);
            }

            // 0.5 소수 유무 판단 후 hp half 표시
            if (hp - (int)hp == 0.5)
            {
                hpObjList[remainder].SetActive(true);
                hpImageList[remainder].sprite = hpHalfSprite;
            }
        }
    }*/
}
