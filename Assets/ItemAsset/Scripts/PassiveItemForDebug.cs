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
            Debug.Log("패시브 슬룻 꽉참. 아이템 적용 안됨.");
            return;
        }
        Debug.Log(currentIndex + "번 패시브 아이템 사용 for debug");
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
        Debug.Log("UpdatePassiveSlots : " + currentIndex + ", " + passiveSlotIdsLength);
        for(int i = 0; i < passiveSlotIdsLength; i++)
        {
            //Debug.Log("a : " + i + ", " + passiveSlotIds[i]);
            passiveSlots[i].UpdatePassiveSlot(DataStore.Instance.GetPassiveItemInfo(passiveSlotIds[i]).Sprite);
        }
        for (int i = passiveSlotIdsLength; i < slotCountMax; i++)
        {
            //Debug.Log("b : " + i);
            //passiveSlots[i].UpdatePassiveSlot(null);
        }
    }

    private void UpdateEffectTotalNameText()
    {
        variableNames ="--캐릭터 Effect Total Name value--\n" +
            "criticalChanceIncrease\n" +
            "moveSpeedIncrease\n" +
            "rewardOfEndGameIncrease\n" +
            "discountRateOfVendingMachineItems\n" +
            "discountRateOfCafeteriaItems\n" +
            "discountRateAllItems\n" +
            "canDrainHp\n" +
            "-----\n" +
            "Weapon\n" +
            "shotgunBulletCountIncrease\n" +
            "damageIncrease\n" +
            "knockBackIncrease\n" +
            "chargeDamageIncrease\n" +
            "bulletScaleIncrease\n" +
            "bulletRangeIncrease\n" +
            "bulletSpeedIncrease\n" +
            "cooldownReduction\n" +
            "chargeTimeReduction\n" +
            "accuracyIncrease\n" +
            "shotgunsAccuracyIncrease\n" +
            "canIncreasePierceCount\n" +
            "becomesSpiderMine\n" +
            "bounceAble\n" +
            "shotgunBulletCanHoming\n" +
            "blowWeaponsCanBlockBullet\n" +
            "swingWeaponsCanReflectBullet\n" +
            "\n" +
            "\n";
        EffectTotalNameText.text = variableNames;
    }

    public void UpdateEffectTotalValueText()
    {
        CharacterTargetEffect characterTotal = PlayerBuffManager.Instance.BuffManager.CharacterTargetEffectTotal;
        WeaponTargetEffect weaponTotal = PlayerBuffManager.Instance.BuffManager.WeaponTargetEffectTotal;
        string variableValues = "\n" +
            characterTotal.criticalChanceIncrease + "\n" +
            characterTotal.moveSpeedIncrease + "\n" +
            characterTotal.rewardOfEndGameIncrease + "\n" +
            characterTotal.discountRateOfVendingMachineItems + "\n" +
            characterTotal.discountRateOfCafeteriaItems + "\n" +
            characterTotal.discountRateAllItems + "\n" +
            characterTotal.canDrainHp + "\n" +
            "---\n" +
            "Weapon\n" +
            weaponTotal.shotgunBulletCountIncrease + "\n" +
            weaponTotal.damageIncrease + "\n" +
            weaponTotal.knockBackIncrease + "\n" +
            weaponTotal.chargeDamageIncrease + "\n" +
            weaponTotal.bulletScaleIncrease + "\n" +
            weaponTotal.bulletRangeIncrease + "\n" +
            weaponTotal.bulletSpeedIncrease + "\n" +
            weaponTotal.cooldownReduction + "\n" +
            weaponTotal.chargeTimeReduction + "\n" +
            weaponTotal.accuracyIncrease + "\n" +
            weaponTotal.shotgunsAccuracyIncrease + "\n" +

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
