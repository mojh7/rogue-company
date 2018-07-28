using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponAsset;

public class PassiveItemForDebug : MonoBehaviour
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
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start ()
    {
        currentIndex = 0;
        infoCurrentIndex = 0;
        totalInfoIndexMax = (int)WeaponType.END;

        passiveItemIndexMax = DataStore.Instance.GetPassiveItemInfosLength();
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
                DebugX.Log("패시브 아이템 테스트창 off");
            }
            else
            {
                DebugX.Log("패시브 아이템 테스트창 on");
            }
            passiveDebugObj.SetActive(!passiveDebugObj.activeSelf);
            effectTotalViewObj.SetActive(passiveDebugObj.activeSelf);
        }
    }
    #endregion

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
            }
        }
    }

    public void ApplyPassiveForDebug()
    {
        if (passiveSlotIdsLength >= slotCountMax)
        {
            DebugX.Log("패시브 슬룻 꽉참. 아이템 적용 안됨.");
            return;
        }
        DebugX.Log(currentIndex + "번 패시브 아이템 사용 for debug");
        passiveSlotIds.Add(currentIndex);
        passiveSlotIdsLength += 1;
        UsableItemInfo passive = DataStore.Instance.GetPassiveItemInfo(currentIndex);
        for (int i = 0; i < passive.EffectApplyTypes.Length; i++)
        {
            passive.EffectApplyTypes[i].UseItem();
        }
        UpdatePassiveSlots();
        UpdateEffectTotalValueText();
    }

    public void UpdatePassiveSelectImage()
    {
        SelectPassiveIdText.text = "Id : " + currentIndex;
        SelectPassiveMemoText.text = DataStore.Instance.GetPassiveItemInfo(currentIndex).Notes;
        passiveSelectImage.sprite = DataStore.Instance.GetPassiveItemInfo(currentIndex).Sprite;
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

    public void UpdatePassiveSlots()
    {
        DebugX.Log("UpdatePassiveSlots : " + currentIndex + ", " + passiveSlotIdsLength);
        for(int i = 0; i < passiveSlotIdsLength; i++)
        {
            //DebugX.Log("a : " + i + ", " + passiveSlotIds[i]);
            passiveSlots[i].UpdatePassiveSlot(DataStore.Instance.GetPassiveItemInfo(passiveSlotIds[i]).Sprite);
        }
        for (int i = passiveSlotIdsLength; i < slotCountMax; i++)
        {
            //DebugX.Log("b : " + i);
            //passiveSlots[i].UpdatePassiveSlot(null);
        }
    }

    #endregion

    #region viewEffectInfo

    private void UpdateEffectTotalNameText()
    {
        variableNames = 
            "1.moveSpeedIncrement\n" +
            "2.rewardOfEndGameIncrement\n" +
            "3.discountRateOfVendingMachineItems\n" +
            "4.discountRateOfCafeteriaItems\n" +
            "5.discountRateAllItems\n" +
            "6.canDrainHp\n" +
            "-----\n" +
            "Weapon\n" +
            "1.bulletCountIncrement\n" +
            "2.criticalChanceIncrement\n" +

            "1.damageIncrement\n" +
            "2.knockBackIncrement\n" +
            "3.chargingDamageIncrement\n" +
            "4.gettingSkillGaugeIncrement\n" +
            "5.gettingStaminaIncrement\n" +
            "6.skillPowerIncrement\n" +
            "7.bulletScaleIncrement\n" +
            "8.bulletRangeIncrement\n" +
            "9.bulletSpeedIncrement\n" +

            "1.cooldownReduction\n" +
            "2.chargeTimeReduction\n" +
            "3.accuracyIncrement\n" +

            "1.increasePierceCount\n" +
            "2.becomesSpiderMine\n" +
            "3.bounceAble\n" +
            "4.shotgunBulletCanHoming\n" +
            "5.meleeWeaponsCanBlockBullet\n" +
            "6.meleeWeaponsCanReflectBullet\n" +
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
            characterTotal.rewardOfEndGameIncrement + "\n" +
            characterTotal.discountRateOfVendingMachineItems + "\n" +
            characterTotal.discountRateOfCafeteriaItems + "\n" +
            characterTotal.discountRateAllItems + "\n" +
            characterTotal.canDrainHp + "\n" +
            "---\n" +
            "Weapon\n" +
            weaponTotal.bulletCountIncrement + "\n" +
            weaponTotal.criticalChanceIncrement + "\n" +

            weaponTotal.damageIncrement + "\n" +
            weaponTotal.knockBackIncrement + "\n" +
            weaponTotal.chargingDamageIncrement + "\n" +
            weaponTotal.gettingSkillGaugeIncrement + "\n" +
            weaponTotal.gettingStaminaIncrement + "\n" +
            weaponTotal.skillPowerIncrement + "\n" +
            weaponTotal.bulletScaleIncrement + "\n" +
            weaponTotal.bulletRangeIncrement + "\n" +
            weaponTotal.bulletSpeedIncrement + "\n" +

            weaponTotal.cooldownReduction + "\n" +
            weaponTotal.chargeTimeReduction + "\n" +
            weaponTotal.accuracyIncrement + "\n" +

            weaponTotal.increasePierceCount + "\n" +
            weaponTotal.becomesSpiderMine + "\n" +
            weaponTotal.bounceAble + "\n" +
            weaponTotal.shotgunBulletCanHoming + "\n" +
            weaponTotal.meleeWeaponsCanBlockBullet + "\n" +
            weaponTotal.meleeWeaponsCanReflectBullet;

        EffectTotalValueText.text = variableValues;
    }
    #endregion
}
