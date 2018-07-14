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
    private Text EffectTotalText;
    [SerializeField]
    private Text SelectPassiveIdText;
    [SerializeField]
    private Text SelectPassiveMemoText;

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
        }
    }
    #endregion

    #region function
    private void CreatePassiveSlots(Vector3 standardPos)
    {
        Debug.Log("row : " + slotRow + ", col : " + slotColumn + ", slot Max : " + slotCountMax);
        passiveSlotIds = new List<int>();
        passiveSlotIdsLength = 0;
        passiveSlots = new PassiveSlot[slotCountMax];
        GameObject createdObj;
        Vector3 currentPos = new Vector3();
        for (int y = 0; y < slotRow; y++)
        {
            for(int x = 0; x < slotColumn; x++)
            {
                Debug.Log("x : " + x + ", y : " + y);
                currentPos.x = standardPos.x + x * intervalPos.x;
                currentPos.y = standardPos.y - y * intervalPos.y;
                createdObj = Instantiate(passiveSlotPrefab);
                createdObj.transform.position = currentPos;
                createdObj.transform.SetParent(passiveSlotsParent);
                passiveSlots[y * slotRow + x] = createdObj.GetComponent<PassiveSlot>();
            }
        }
    }

    public void ApplyPassiveForDebug()
    {
        Debug.Log(currentIndex + "번 패시브 아이템 사용 for debug");
        passiveSlotIds.Add(currentIndex);
        passiveSlotIdsLength += 1;
        UsableItemInfo passive = DataStore.Instance.GetPassiveItemInfo(currentIndex);
        for (int i = 0; i < passive.EffectApplyTypes.Length; i++)
        {
            passive.EffectApplyTypes[i].UseItem();
        }
        UpdatePassiveSlots();
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
            Debug.Log("껄껄 : " + i + ", " + passiveSlotIds[i]);
            passiveSlots[i].UpdatePassiveSlot(DataStore.Instance.GetPassiveItemInfo(passiveSlotIds[i]).Sprite);
        }
        for (int i = passiveSlotIdsLength; i < slotCountMax; i++)
        {
            Debug.Log("껄껄2 : " + i);
            passiveSlots[i].UpdatePassiveSlot(null);
        }
    }
    #endregion
}
