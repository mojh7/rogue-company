using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameNoticeManager : MonoBehaviourSingleton<IngameNoticeManager>
{
    #region variables

    #region common
    [Header("common variables")]
    private const float MASK_WIDTH_MIN = 0f;
    private const float MASK_WIDTH_MAX = 498f;
    private const float MASK_HEIGHT_MIN = 0f;
    private const float MASK_HEIGHT_MAX = 136f;
    private float maskWidth = 0f;
    private Coroutine maskAniCoroutine;
    private bool isShowing;
    private float lastShowingCallTime;

    [SerializeField]
    private GameObject maskObj;
    [SerializeField]
    private Image bkgImg;
    [SerializeField]
    private RectTransform maskTransform;
    [SerializeField]
    private Sprite[] bkgImgByRating;
    #endregion

    #region weaponNotice
    [Header("weaponNotice variables")]
    [SerializeField]
    private GameObject weaponInfoNoticeObj;
    [SerializeField]
    private Sprite ammoImg;
    [SerializeField]
    private Sprite staminaImg;
    //dps, critical, cost
    [SerializeField]
    private Image costImg;
    [SerializeField]
    private Text[] infoTxt;
    #endregion

    #region itemNotice
    [Header("itemNotice variables")]
    [SerializeField]
    private GameObject itemInfoNoticeObj;
    [SerializeField]
    private Image itemBkgGlowImg;
    [SerializeField]
    private Image itemImg;
    [SerializeField]
    private Text itemDescTxt;
    #endregion

    #endregion

    #region unityFunc
    private void Start()
    {
        Init();   
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ShowInfoNotice(WeaponsData.Instance.GetWeaponInfo(Rating.E));
        }
    }
    #endregion

    #region func
    private void Init()
    {
        maskAniCoroutine = null;
        isShowing = false;
        maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MIN, MASK_HEIGHT_MAX);
        ActiveOffAllNotices();
        maskObj.SetActive(false);
    }

    private void ActiveOffAllNotices()
    {
        weaponInfoNoticeObj.SetActive(false);
        itemInfoNoticeObj.SetActive(false);
    }

    public void ShowInfoNotice(WeaponInfo info)
    {
        isShowing = true;
        ActiveOffAllNotices();
        weaponInfoNoticeObj.SetActive(true);

        bkgImg.sprite = bkgImgByRating[(int)info.rating - 1];
        infoTxt[0].text = info.dps.ToString();
        infoTxt[1].text = (info.criticalChance * 100f).ToString() + " %";
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

    public void ShowInfoNotice(UsableItemInfo info)
    {
        isShowing = true;
        ActiveOffAllNotices();
        itemInfoNoticeObj.SetActive(true);

        bkgImg.sprite = bkgImgByRating[(int)info.Rating - 1];
        itemImg.sprite = info.Sprite;
        itemBkgGlowImg.color = CommonConstants.Instance.RATING_TXT_COLOR[(int)info.Rating - 1];
        itemDescTxt.text = info.Notes;
        lastShowingCallTime = Time.time;
        if (null == maskAniCoroutine)
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
        maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MIN, MASK_HEIGHT_MAX);
        maskObj.SetActive(true);
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (isShowing)
            {
                if (maskTransform.sizeDelta.x < MASK_WIDTH_MAX)
                {
                    maskWidth += Time.fixedDeltaTime * showSpeed;
                    maskTransform.sizeDelta = new Vector2(maskWidth, MASK_HEIGHT_MAX);
                }

                if (maskTransform.sizeDelta.x > MASK_WIDTH_MAX)
                {
                    maskWidth = MASK_WIDTH_MAX;
                    maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MAX, MASK_HEIGHT_MAX);
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
                    maskTransform.sizeDelta = new Vector2(maskWidth, MASK_HEIGHT_MAX);
                }
                else
                {
                    maskWidth = MASK_WIDTH_MIN;
                    maskTransform.sizeDelta = new Vector2(MASK_WIDTH_MIN, MASK_HEIGHT_MAX);
                    isShowing = false;
                    maskObj.SetActive(false);
                    maskAniCoroutine = null;
                    break;
                }
            }
        }
    }
    #endregion
}
