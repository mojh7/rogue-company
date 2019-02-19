using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoNotice : MonoBehaviour
{
    [SerializeField]
    private Sprite[] bkgImgByRating;

    [SerializeField]
    private Sprite ammoImg;
    [SerializeField]
    private Sprite staminaImg;

    [SerializeField]
    private GameObject maskObj;
    [SerializeField]
    private RectTransform maskTransform; 

    //dps, critical, cost
    [SerializeField]
    private Image costImg;
    [SerializeField]
    private Text[] infoTxt;

    private bool isActive;
    private bool isShowing;
    private bool isHiding;

    private void Start()
    {
        Init();   
    }

    private void Init()
    {
        isActive = false;
        isShowing = false;
        isHiding = false;
        maskObj.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ShowWeaponInfo(WeaponsData.Instance.GetWeaponInfo(Rating.E));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            HideWeaponInfo();
        }
    }

    public void ShowWeaponInfo(WeaponInfo info)
    {
        if (isActive || isShowing)
        {
            return;
        }
        isActive = true;
        infoTxt[0].text = info.dps.ToString();
        infoTxt[1].text = (info.criticalChance * 100f).ToString() + "%";
        if(Weapon.IsMeleeWeapon(info))
        {
            costImg.sprite = staminaImg;
            infoTxt[2].text = "-" + info.staminaConsumption.ToString();
        }
        else
        {
            costImg.sprite = ammoImg;
            infoTxt[2].text = info.ammoCapacity.ToString();
        }
        StartCoroutine(ShowInfo());
    }

    public void HideWeaponInfo()
    {
        if (false == isActive || isHiding || isShowing)
        {
            return;
        }
        StartCoroutine(HideInfo());
    }


    private IEnumerator ShowInfo()
    {
        isShowing = true;
        maskObj.SetActive(true);
        maskTransform.sizeDelta = new Vector2(0, 100f);
        float showSpeed = 1500f;
        float maskWidthMax = 500f;
        float maskWidth = 0f;
        while (maskTransform.sizeDelta.x < maskWidthMax)
        {
            maskTransform.sizeDelta = new Vector2(maskWidth, 100f);
            maskWidth += Time.fixedDeltaTime * showSpeed;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
        maskTransform.sizeDelta = new Vector2(maskWidthMax, 100f);
        isShowing = false;
    }

    private IEnumerator HideInfo()
    {
        isHiding = true;
        float hideSpeed = 1500f;
        float maskWidthMin = 0f;
        float maskWidth = 500f;
        while (maskTransform.sizeDelta.x > maskWidthMin)
        {
            maskTransform.sizeDelta = new Vector2(maskWidth, 100f);
            maskWidth -= Time.fixedDeltaTime * hideSpeed;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
        maskTransform.sizeDelta = new Vector2(maskWidthMin, 100f);
        isActive = false;
        isHiding = false;
        maskObj.SetActive(false);
    }
}
