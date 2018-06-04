using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour {

    [SerializeField] private GameObject obj;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image delayHPImage;

    float hp;
    float oldHp;
    float remainHp;

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void SetHpBar(float _hp)
    {
        hp = _hp;
        oldHp = hp;
        remainHp = hp;
        hpImage.fillAmount = remainHp / hp;
    }

    public void DecreaseHp(float _hp)
    {
        oldHp = remainHp;
        remainHp -= _hp;
        hpImage.fillAmount = remainHp / hp;
        StartCoroutine(CoroutineHP(oldHp, remainHp, hp, delayHPImage));
    }

    public void IncreaseHP(float _hp)
    {
        oldHp = remainHp;
        remainHp += Mathf.Clamp(remainHp + _hp, 0, hp);
        hpImage.fillAmount = remainHp / hp;
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
            _image.fillAmount = temp/max;
            if (temp == _dest)
                break;
        }
    }
}
