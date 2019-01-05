using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour {

    Character Owner;
    [SerializeField] private GameObject obj;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image delayHPImage;

    private float hpMax;
    private float oldHp;
    private float hp;

    public void Toggle()
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void Init(Character Owner)
    {
        this.Owner = Owner;
    }

    public void Notify()
    {
        hpMax = Owner.HPMax;
        oldHp = hp;
        hp = Owner.HP;
        hpImage.fillAmount = hp / hpMax;
        Debug.Log(oldHp + "," + hp + "," + hpMax);
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
            _image.fillAmount = temp/max;
            if (temp == _dest)
                break;
        }
    }
    
}
