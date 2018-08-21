using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveItemSlot : MonoBehaviourSingleton<PassiveItemSlot>, IPointerDownHandler
{
    private delegate void ActiveOffAllPassiveSlot();

    #region variables
    [SerializeField]
    private GameObject passiveItemSlot;
    
    // 아이템 슬룻
    [SerializeField]
    private GameObject passiveSlotPrefab;

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

    private ActiveOffAllPassiveSlot activeOffAllPassiveSlot;
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start()
    {
        slotCountMax = slotRow * slotColumn;
        CreatePassiveSlots(standardPos.position);
    }
    #endregion

    #region PassiveItemSlot

    public void ClosePassiveItemSlot()
    {
        passiveItemSlot.SetActive(false);
        Time.timeScale = 1;
        UIManager.Instance.TogglePreventObj();
    }

    private void CreatePassiveSlots(Vector3 standardPos)
    {
        passiveSlotIds = new List<int>();
        passiveSlotIdsLength = 0;
        passiveSlots = new PassiveSlot[slotCountMax];
        GameObject createdObj;
        Vector3 currentPos = new Vector3();
        for (int y = 0; y < slotRow; y++)
        {
            for (int x = 0; x < slotColumn; x++)
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

    public void AddPassiveItem()
    {

    }

    public void UpdatePassiveItemUI()
    {
        activeOffAllPassiveSlot();
        for(int i = 0; i < PlayerBuffManager.Instance.BuffManager.PassiveIds.Count; i++)
        {
            passiveSlots[i].UpdatePassiveSlot(DataStore.Instance.GetMiscItemInfo(PlayerBuffManager.Instance.BuffManager.PassiveIds[i]).Sprite);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdatePassiveItemUI();
        passiveItemSlot.SetActive(true);
        Time.timeScale = 0;
        UIManager.Instance.TogglePreventObj();
    }

    #endregion
}
