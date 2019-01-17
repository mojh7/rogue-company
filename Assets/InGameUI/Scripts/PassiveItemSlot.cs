using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveItemSlot : MonoBehaviourSingleton<PassiveItemSlot>, IPointerDownHandler
{
    public delegate void ActiveOffAllPassiveSlot();

    #region variables
    [SerializeField]
    private GameObject passiveItemSlot;
    [SerializeField]
    private GameObject passiveItemInfoView;
    //[SerializeField]
    //private Image passiveItemInfoViewImage;
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
    private int slotCountMax;
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
        CreatePassiveSlots(contentsParent);
    }
    #endregion

    #region PassiveItemSlot

    public void ClosePassiveItemSlot()
    {
        Debug.Log("z");
        passiveItemSlot.SetActive(false);
        passiveItemInfoView.SetActive(false);
        Time.timeScale = 1;
#if !UNITY_EDITOR
        UIManager.Instance.TogglePreventObj();
#endif
    }

    private void CreatePassiveSlots(Transform contentsParent)
    {
        passiveSlotIds = new List<int>();
        passiveSlotIdsLength = 0;
        passiveSlots = new PassiveSlot[slotCountMax];
        GameObject createdObj;
        Vector3 currentPos = new Vector3();
        for (int i = 0; i < slotCountMax; i++)
        {
            createdObj = Instantiate(passiveSlotPrefab);
            createdObj.name = "패시브 슬룻_" + i;
            createdObj.transform.position = currentPos;
            createdObj.transform.SetParent(contentsParent);
            passiveSlots[i] = createdObj.GetComponent<PassiveSlot>();
            activeOffAllPassiveSlot += passiveSlots[i].ActiveOffPassiveSlot;
            createdObj.transform.localScale = new Vector3(1, 1, 1);
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

    /// <summary>
    /// 패시브 아이템 정보 보기
    /// </summary>
    /// <param name="id"></param>
    public void ShowPassiveInfoView(int id)
    {
        passiveItemInfoView.SetActive(true);
        UsableItemInfo usableItemInfo = ItemsData.Instance.GetMiscItemInfo(id);
        //passiveItemInfoViewImage.sprite = usableItemInfo.Sprite;
        passiveItemInfoViewName.text = usableItemInfo.ItemName;
        passiveItemInfoViewNote.text = usableItemInfo.Notes;
        Vector3 pos = Input.mousePosition;
        if(pos.x < Screen.width * 0.5f)
        {
            pos.x += Screen.width * 0.15f;
        }
        else
        {
            pos.x -= Screen.width * 0.15f;
        }

        if (pos.y < Screen.height * 0.5f)
        {
            pos.y += Screen.height * 0.25f;
        }
        else
        {
            pos.y -= Screen.height * 0.25f;
        }
        passiveItemInfoView.transform.position = pos;
    }

    public void ClosePassiveInfoView()
    {
        passiveItemInfoView.SetActive(false);
    }

    /// <summary>
    /// 아이템 창(가방 모양) 터치 되었을 떄 아이템 창 UI On
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(1, SOUNDTYPE.UI);
        UpdatePassiveItemUI();
        passiveItemSlot.SetActive(true);
        //passiveItemInfoView.SetActive(true);
        Time.timeScale = 0;
        // unity pc에서 아래꺼 하고 아이템창에서 아이템 정보 보는 것이랑 닫기 버튼이 작동이 안됨.
#if !UNITY_EDITOR
        UIManager.Instance.TogglePreventObj();
#endif
    }

    #endregion
}
