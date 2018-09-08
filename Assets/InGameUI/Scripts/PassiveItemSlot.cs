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
    [SerializeField]
    private GameObject passiveItemInfoView;
    [SerializeField]
    private Image passiveItemInfoViewImage;
    [SerializeField]
    private Text passiveItemInfoViewName;
    [SerializeField]
    private Text passiveItemInfoViewNote;
    // 아이템 슬룻
    [SerializeField]
    private GameObject passiveSlotPrefab;

    // 패시브 아이템 창 관련
    [SerializeField]
    private Transform contentsParent;
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

    [SerializeField]
    private Sprite emptySprite;
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start()
    {
        slotCountMax = slotRow * slotColumn;
        CreatePassiveSlots(contentsParent);
    }
    #endregion

    #region PassiveItemSlot

    public void ClosePassiveItemSlot()
    {
        passiveItemSlot.SetActive(false);
        passiveItemInfoView.SetActive(false);
        Time.timeScale = 1;
        UIManager.Instance.TogglePreventObj();
    }

    private void CreatePassiveSlots(Transform contentParent)
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
                createdObj = Instantiate(passiveSlotPrefab);
                createdObj.name = "패시브 슬룻 " + (y * slotRow + x);
                createdObj.transform.position = currentPos;
                createdObj.transform.SetParent(contentParent);
                passiveSlots[y * slotColumn + x] = createdObj.GetComponent<PassiveSlot>();
                activeOffAllPassiveSlot += passiveSlots[y * slotColumn + x].ActiveOffPassiveSlot;
                createdObj.transform.localScale = new Vector3(1, 1, 1);
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
            passiveSlots[i].UpdatePassiveSlot(PlayerBuffManager.Instance.BuffManager.PassiveIds[i]);
        }
    }

    public void ShowPassiveInfoView(int id)
    {
        UsableItemInfo usableItemInfo = DataStore.Instance.GetMiscItemInfo(id);
        passiveItemInfoViewImage.sprite = usableItemInfo.Sprite;
        passiveItemInfoViewName.text = usableItemInfo.ItemName;
        passiveItemInfoViewNote.text = usableItemInfo.Notes;
    }

    /// <summary>
    /// 아이템 창(가방 모양) 터치 되었을 떄 아이템 창 UI On
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        UpdatePassiveItemUI();
        passiveItemSlot.SetActive(true);
        passiveItemInfoView.SetActive(true);
        Time.timeScale = 0;
        UIManager.Instance.TogglePreventObj();
    }

    #endregion
}
