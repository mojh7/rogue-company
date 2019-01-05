using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpbarUI : MonoBehaviour {
    [SerializeField] private GameObject obj;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image delayHPImage;
    [SerializeField] private Text hpText;
    Character Owner;

    float hp;
    float oldHp;
    float hpMax;

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void Init(Character Owner)
    {
        this.Owner = Owner;
        hp = Owner.HP;
        hpMax = hp;
        oldHp = hp;
        hpImage.fillAmount = hp / hpMax;
        hpText.text = hp + "/" + (int)hpMax;
    }

    public void SetHpMax()
    {
        this.hpMax = Owner.HPMax;
    }

    public void Notify()
    {
        hp = Owner.HP;
        if (hp < 0) return;
        oldHp = hp;
        hpImage.fillAmount = (int)hp / hpMax;
        hpText.text = (int)hp + "/" + (int)hpMax;
        StartCoroutine(CoroutineHP(oldHp, hp, hpMax, delayHPImage));
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
}
