using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // effect Total text 변수 명, 효과
    private string variableNames;
    private List<string> variableValues;

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

    #region function

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

    private void UpdateEffectTotalNameText()
    {
        variableNames ="--캐릭터 Effect Total Name value--\n" +
            "1.criticalChanceIncrement\n" +
            "2.moveSpeedIncrement\n" +
            "3.rewardOfEndGameIncrement\n" +
            "4.discountRateOfVendingMachineItems\n" +
            "5.discountRateOfCafeteriaItems\n" +
            "6.discountRateAllItems\n" +
            "7.canDrainHp\n" +
            "-----\n" +
            "Weapon\n" +
            "1.shotgunBulletCountIncrement\n" +

            "2.damageIncrement\n" +
            "3.knockBackIncrement\n" +
            "4.chargingAmountIncrement\n" +
            "5.gettingSkillGaugeIncrement\n" +
            "6.gettingStaminaIncrement\n" +
            "7.skillPowerIncrement\n" +
            "8.bulletScaleIncrement\n" +
            "9.bulletRangeIncrement\n" +
            "10.bulletSpeedIncrement\n" +

            "1.cooldownReduction\n" +
            "2.chargeTimeReduction\n" +
            "3.accuracyIncrement\n" +
            "4.shotgunsAccuracyIncrement\n" +

            "1.canIncreasePierceCount\n" +
            "2.becomesSpiderMine\n" +
            "3.bounceAble\n" +
            "4.shotgunBulletCanHoming\n" +
            "5.blowWeaponsCanBlockBullet\n" +
            "6.swingWeaponsCanReflectBullet\n" +
            "\n" +
            "\n";
        EffectTotalNameText.text = variableNames;
    }

    public void UpdateEffectTotalValueText()
    {
        CharacterTargetEffect characterTotal = PlayerBuffManager.Instance.BuffManager.CharacterTargetEffectTotal;
        WeaponTargetEffect weaponTotal = PlayerBuffManager.Instance.BuffManager.WeaponTargetEffectTotal;
        string variableValues = "\n" +
            characterTotal.criticalChanceIncrement + "\n" +
            characterTotal.moveSpeedIncrement + "\n" +
            characterTotal.rewardOfEndGameIncrement + "\n" +
            characterTotal.discountRateOfVendingMachineItems + "\n" +
            characterTotal.discountRateOfCafeteriaItems + "\n" +
            characterTotal.discountRateAllItems + "\n" +
            characterTotal.canDrainHp + "\n" +
            "---\n" +
            "Weapon\n" +
            weaponTotal.shotgunBulletCountIncrement + "\n" +
            weaponTotal.damageIncrement + "\n" +
            weaponTotal.knockBackIncrement + "\n" +
            weaponTotal.chargingAmountIncrement + "\n" +
            weaponTotal.gettingSkillGaugeIncrement + "\n" +
            weaponTotal.gettingStaminaIncrement + "\n" +
            weaponTotal.skillPowerIncrement + "\n" +
            weaponTotal.bulletScaleIncrement + "\n" +
            weaponTotal.bulletRangeIncrement + "\n" +
            weaponTotal.bulletSpeedIncrement + "\n" +

            weaponTotal.cooldownReduction + "\n" +
            weaponTotal.chargeTimeReduction + "\n" +
            weaponTotal.accuracyIncrement + "\n" +
            weaponTotal.shotgunsAccuracyIncrement + "\n" +

            weaponTotal.canIncreasePierceCount + "\n" +
            weaponTotal.becomesSpiderMine + "\n" +
            weaponTotal.bounceAble + "\n" +
            weaponTotal.shotgunBulletCanHoming + "\n" +
            weaponTotal.blowWeaponsCanBlockBullet + "\n" +
            weaponTotal.swingWeaponsCanReflectBullet;
        EffectTotalValueText.text = variableValues;
    }
    #endregion
}
