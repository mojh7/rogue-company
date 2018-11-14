using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpbarUI : MonoBehaviour {
    [SerializeField] private GameObject obj;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image delayHPImage;
    [SerializeField] private Text hpText;

    
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
        hpText.text = remainHp + "/" + maxHp;
    }

    public void DecreaseHp(float _hp)
    {
        if (_hp < 0) return;
        oldHp = remainHp;
        remainHp = _hp;
        hpImage.fillAmount = remainHp / hp;
        hpText.text = remainHp + "/" + maxHp;
        StartCoroutine(CoroutineHP(oldHp, remainHp, hp, delayHPImage));
    }

    public void IncreaseHP(float _hp)
    {
        if (_hp < 0) return;
        oldHp = remainHp;
        remainHp = _hp;
        hpImage.fillAmount = remainHp / hp;
        hpText.text = remainHp + "/" + maxHp;
        if (remainHp > maxHp)
        {
            hp = remainHp;
            maxHp = hp;
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
}
