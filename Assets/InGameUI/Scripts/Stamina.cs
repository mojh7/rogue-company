using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviourSingleton<Stamina>
{

    private Image stamina;
    [HideInInspector]   
    public float totalStamina = 200;        // 아이템 먹으면 기본 스테미나 크기 증가
    [HideInInspector]
    public Stamina instance = null;

	void Awake () {
        stamina = GetComponent<Image>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            return;
        }
        Debug.Log(instance.name);
	}

    public void StaminaPlus()
    {
        if (stamina.fillAmount != 1.0f)
        {
            // 3초마다 한번씩 1씩 추가
            // 몬스터 죽을때마다 1씩 추가
            stamina.fillAmount += 1 / totalStamina;
        }
        else
        {
            return;
        }
    }

    public void StaminaMinus()
    {
        if (stamina.fillAmount >= 0 && stamina.fillAmount <= 1)
        {
            stamina.fillAmount -= 1 / totalStamina;
        }
        else if (stamina.fillAmount == 0)
        {
            Debug.Log("스태미너 0이에요 공격 ㄴㄴ");
        }
    }

    public bool StaminaState()
    {
        if (stamina.fillAmount >= 0 && stamina.fillAmount < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
