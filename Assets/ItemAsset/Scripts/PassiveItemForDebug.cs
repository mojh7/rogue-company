
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponAsset;

public class PassiveItemForDebug : MonoBehaviourSingleton<PassiveItemForDebug>
{
    #region variables
    // 디버깅 관련
    [SerializeField]
    private GameObject passiveDebugObj;
    [SerializeField]
    private GameObject effectTotalViewObj;
    [SerializeField]
    private Image passiveSelectImage;
    [SerializeField]
    private GameObject passiveSlotPrefab;
    [SerializeField]
    private Text EffectTotalNameText;
    [SerializeField]
    private Text EffectTotalValueText;
    [SerializeField]
    private Text SelectPassiveIdText;
    [SerializeField]
    private Text SelectPassiveMemoText;

    [SerializeField]
    private Text viewTypeText;

    // effect Total text 변수 명, 효과
    private string variableNames;
    private List<string> variableValues;

    private int infoCurrentIndex;
    private int totalInfoIndexMax;

    private int currentIndex;
    private int passiveItemIndexMax;

    // 패시브 아이템 창 관련
    [SerializeField]
    private Transform standardPos;
    [SerializeField]
    private Transform passiveSlotsParent;
    [SerializeField]
    private int slotRow;
    [SerializeField]
    private int slotColumn;
    private int slotCountMax;
    [SerializeField]
    private Vector2 intervalPos;
    private PassiveSlot[] passiveSlots;

    private List<int> passiveSlotIds;
    private int passiveSlotIdsLength;

    private delegate void ActiveOffAllPassiveSlot();
    private ActiveOffAllPassiveSlot activeOffAllPassiveSlot;
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start ()
    {
        currentIndex = 0;
        infoCurrentIndex = 0;
        totalInfoIndexMax = (int)WeaponType.END;

        passiveItemIndexMax = DataStore.Instance.GetMiscItemInfosLength();
        slotCountMax = slotRow * slotColumn;
        
        CreatePassiveSlots(standardPos.position);
        UpdatePassiveSelectImage();
        UpdateEffectTotalNameText();
        UpdateEffectTotalValueText();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(passiveDebugObj.activeSelf)
            {
                Debug.Log("패시브 아이템 테스트창 off");
            }
            else
            {
                Debug.Log("패시브 아이템 테스트창 on");
            }
            passiveDebugObj.SetActive(!passiveDebugObj.activeSelf);
            effectTotalViewObj.SetActive(passiveDebugObj.activeSelf);
        }
    }
    #endregion

    // 임시로 만든 함수
    public void OnOffPassiveItems()
    {
        passiveDebugObj.SetActive(!passiveDebugObj.activeSelf);
        effectTotalViewObj.SetActive(passiveDebugObj.activeSelf);
    }


    #region PassiveItemSlot

    private void CreatePassiveSlots(Vector3 standardPos)
    {
        passiveSlotIds = new List<int>();
        passiveSlotIdsLength = 0;
        passiveSlots = new PassiveSlot[slotCountMax];
        GameObject createdObj;
        Vector3 currentPos = new Vector3();
        for (int y = 0; y < slotRow; y++)
        {
            for(int x = 0; x < slotColumn; x++)
            {
                currentPos.x = standardPos.x + x * intervalPos.x;
                currentPos.y = standardPos.y - y * intervalPos.y;
                createdObj = Instantiate(passiveSlotPrefab);
                createdObj.name = "패시브 슬룻 " + (y * slotRow + x);
                createdObj.transform.position = currentPos;
                createdObj.transform.SetParent(passiveSlotsParent);
                passiveSlots[y * slotColumn + x] = createdObj.GetComponent<PassiveSlot>();
                activeOffAllPassiveSlot += passiveSlots[y * slotColumn + x].ActiveOffPassiveSlot;
            }
        }
    }

    public void ApplyPassiveForDebug()
    {
        Debug.Log(currentIndex + "번 패시브 아이템 사용 for debug");
        UsableItemInfo info = DataStore.Instance.GetMiscItemInfo(currentIndex);
        PlayerBuffManager.Instance.BuffManager.RegisterUsableItemInfo(info);
    }

    public void UpdatePassiveSelectImage()
    {
        SelectPassiveIdText.text = "Id : " + currentIndex;
        SelectPassiveMemoText.text = DataStore.Instance.GetMiscItemInfo(currentIndex).Notes;
        passiveSelectImage.sprite = DataStore.Instance.GetMiscItemInfo(currentIndex).Sprite;
    }

    public void SelectPassiveUp()
    {
        currentIndex = (currentIndex - 1 + passiveItemIndexMax) % passiveItemIndexMax;
        UpdatePassiveSelectImage();
    }

    public void SelectPassiveDown()
    {
        currentIndex = (currentIndex + 1 + passiveItemIndexMax) % passiveItemIndexMax;
        UpdatePassiveSelectImage();
    }

    public void UpdatePassiveItemUI()
    {
        //Debug.Log("UpdatePassiveSlots : " + currentIndex + ", " + passiveSlotIdsLength);
        UpdateEffectTotalValueText();
        activeOffAllPassiveSlot();
        for (int i = 0; i < PlayerBuffManager.Instance.BuffManager.PassiveIds.Count; i++)
        {
            passiveSlots[i].UpdatePassiveSlot(PlayerBuffManager.Instance.BuffManager.PassiveIds[i]);
        }
    }
    #endregion


    #region viewEffectInfo
    public void UpdateEffectTotalNameText()
    {
        variableNames =
            "1.moveSpeedIncrement\n" +
            "2.hpMaxRatio\n" +
            "3.hpRatio\n" +
            "4.charScale\n" +
            "5.skillGage\n" +
            "6.gettingStaminaMultiple\n" +
            "7.staminaMaxRatio\n" +
            "8.canDrainHp\n" +
            "9.isNotConsumeStamina\n" +
            "10.isNotConsumeAmmo\n" +
            "-----\n" +
            "Weapon\n" +
            "1.bulletCountIncrement\n" +
            "2.criticalChanceIncrement\n" +
            "3.criticalDamageIncrement\n" +
            "1.damageIncrement\n" +
            "2.knockBackIncrement\n" +
            "3.chargingSpeedIncrement\n" +
            "4.chargingDamageIncrement\n" +
            "5.gettingSkillGaugeIncrement\n" +
            "6.bulletScaleIncrement\n" +
            "7.bulletRangeIncrement\n" +
            "8.bulletSpeedIncrement\n" +
            
            "1.decreaseDamageAfterPierceReduction\n" +
            "2.cooldownReduction\n" +
            "3.accuracyIncrement\n" +

            "1.increasePierceCount\n" +
            "2.becomesSpiderMine\n" +
            "3.bounceAble\n" +
            "4.shotgunBulletCanHoming\n" +
            "5.canHoming\n" +
            "6.meleeWeaponsCanBlockBullet\n" +
            "7.meleeWeaponsCanReflectBullet\n" +
            "\n" +
            "\n";
        EffectTotalNameText.text = variableNames;
    }

    public void ChangeViewEffectTotal(bool nextType)
    {
        if(nextType)
            infoCurrentIndex = (infoCurrentIndex + 1) % totalInfoIndexMax;
        else
            infoCurrentIndex = (infoCurrentIndex - 1 + totalInfoIndexMax) % totalInfoIndexMax;
        UpdateEffectTotalValueText();
    }

    public void UpdateEffectTotalValueText()
    {
        viewTypeText.text = ((WeaponType)infoCurrentIndex).ToString();
        CharacterTargetEffect characterTotal = PlayerBuffManager.Instance.BuffManager.CharacterTargetEffectTotal;
        WeaponTargetEffect weaponTotal = PlayerBuffManager.Instance.BuffManager.WeaponTargetEffectTotal[infoCurrentIndex];
        string variableValues =
            characterTotal.moveSpeedIncrement + "\n" +
            characterTotal.hpMaxRatio + "\n" +
            //characterTotal.hpRatio + "\n" +
            characterTotal.charScale + "\n" +
            characterTotal.skillGage + "\n" +
            characterTotal.gettingStaminaMultiple + "\n" +
            characterTotal.staminaMaxRatio + "\n" +

            characterTotal.canDrainHp + "\n" +
            characterTotal.isNotConsumeStamina + "\n" +
            characterTotal.isNotConsumeAmmo + "\n" +
            "---\n" +
            "Weapon\n" +
            weaponTotal.bulletCountIncrement + "\n" +
            weaponTotal.criticalChanceIncrement + "\n" +
            weaponTotal.criticalDamageIncrement + "\n" +

            weaponTotal.damageIncrement + "\n" +
            weaponTotal.knockBackIncrement + "\n" +
            weaponTotal.chargingSpeedIncrement + "\n" +
            weaponTotal.chargingDamageIncrement + "\n" +
            weaponTotal.gettingSkillGaugeIncrement + "\n" +
            weaponTotal.bulletScaleIncrement + "\n" +
            weaponTotal.bulletRangeIncrement + "\n" +
            weaponTotal.bulletSpeedIncrement + "\n" +

            weaponTotal.decreaseDamageAfterPierceReduction + "\n" +
            weaponTotal.cooldownReduction + "\n" +
            weaponTotal.accuracyIncrement + "\n" +

            weaponTotal.increasePierceCount + "\n" +
            weaponTotal.becomesSpiderMine + "\n" +
            weaponTotal.bounceAble + "\n" +
            weaponTotal.shotgunBulletCanHoming + "\n" +
            weaponTotal.canHoming + "\n" +
            weaponTotal.meleeWeaponsCanBlockBullet + "\n" +
            weaponTotal.meleeWeaponsCanReflectBullet;

        EffectTotalValueText.text = variableValues;
    }
    #endregion
}
