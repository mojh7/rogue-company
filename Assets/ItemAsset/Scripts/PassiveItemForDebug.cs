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
    private Image passiveSelectImage;
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
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start ()
    {
        CreatePassiveSlots(standardPos.position);
        slotCountMax = slotRow * slotColumn;
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
                createdObj.transform.position = currentPos;
                createdObj.transform.SetParent(passiveSlotsParent);
                passiveSlots[y * slotRow + x] = createdObj.GetComponent<PassiveSlot>();
            }
        }
    }

    public void ChangePassive()
    {

    }

    public void UpdatePassiveSlots()
    {
        int passiveLength = PlayerBuffManager.Instance.BuffManager.PassiveEffectsLength;
        for(int i = 0; i < passiveLength; i++)
        {
           // passiveSlots[i].UpdatePassiveSlot(PlayerBuffManager.Instance.BuffManager.PassiveEffects[i].Sprite);
        }
        for (int i = passiveLength; i < slotCountMax; i++)
        {
            passiveSlots[i].UpdatePassiveSlot(null);
        }
    }
    #endregion
}
