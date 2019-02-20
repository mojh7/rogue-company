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

    [SerializeField]
    private Image bkgImg;

    //dps, critical, cost
    [SerializeField]
    private Image costImg;
    [SerializeField]
    private Text[] infoTxt;

    private bool isShowing;

    private float lastShowingCallTime;

    private Coroutine maskAniCoroutine;
    private const float MASK_WIDTH_MAX = 500f;
    private const float MASK_WIDTH_MIN = 0f;
    private float maskWidth = 0f;

    private void Start()
    {
        Init();   
    }

    private void Init()
    {
        maskAniCoroutine = null;
        isShowing = false;
        maskObj.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ShowWeaponInfo(WeaponsData.Instance.GetWeaponInfo(Rating.E));
        }
    }

    public void ShowWeaponInfo(WeaponInfo info)
    {
        isShowing = true;
        bkgImg.sprite = bkgImgByRating[(int)info.rating - 1];
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
        lastShowingCallTime = Time.time;
        if(null == maskAniCoroutine)
        {
            maskAniCoroutine = StartCoroutine(MaskAnimation());
        }
    }

    //public void HideWeaponInfo()
    //{
    //    willHide = true;
    //    if (false == isActive || isHiding || isShowing)
    //    {
    //        return;
    //    }
    //    StartCoroutine(HideInfo());
    //}


    private IEnumerator MaskAnimation()
    {
        float showSpeed = 1500f;
        float hideSpeed = 1500f;
        float currentTime = 0;
        maskWidth = 0;
        maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MIN, 100f);
        maskObj.SetActive(true);
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (isShowing)
            {
                if (maskTransform.sizeDelta.x < MASK_WIDTH_MAX)
                {
                    maskWidth += Time.fixedDeltaTime * showSpeed;
                    maskTransform.sizeDelta = new Vector2(maskWidth, 100f);
                }

                if (maskTransform.sizeDelta.x > MASK_WIDTH_MAX)
                {
                    maskWidth = MASK_WIDTH_MAX;
                    maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MAX, 100f);
                    currentTime = Time.time;
                    lastShowingCallTime = Time.time;
                }
                else
                {
                    if (currentTime < lastShowingCallTime + 0.1f)
                    {
                        currentTime = Time.time;
                    }
                    else
                    {
                        isShowing = false;
                    }
                }
            }
            else
            {
                if (maskTransform.sizeDelta.x > MASK_WIDTH_MIN)
                {
                    maskWidth -= Time.fixedDeltaTime * hideSpeed;
                    maskTransform.sizeDelta = new Vector2(maskWidth, 100f);
                }
                else
                {
                    maskWidth = MASK_WIDTH_MIN;
                    maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MIN, 100f);
                    isShowing = false;
                    maskObj.SetActive(false);
                    maskAniCoroutine = null;
                    break;
                }
            }
        }
    }
}
