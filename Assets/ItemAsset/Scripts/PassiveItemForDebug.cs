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
    [SerializeField]
    private Vector2 intervalPos;
    private GameObject[,] passiveSlots;
    #endregion

    #region UnityFunc
    // Use this for initialization
    void Start ()
    {
        CreatePassiveSlots(standardPos.position);
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
        passiveSlots = new GameObject[slotRow, slotColumn];
        Vector3 currentPos = new Vector3();
        for (int y = 0; y < slotRow; y++)
        {
            for(int x = 0; x < slotColumn; x++)
            {
                currentPos.x = standardPos.x + x * intervalPos.x;
                currentPos.y = standardPos.y - y * intervalPos.y;
                passiveSlots[y, x] = Instantiate(passiveSlotPrefab);
                passiveSlots[y, x].transform.position = currentPos;
                passiveSlots[y, x].transform.SetParent(passiveSlotsParent);
            }
        }
    }

    public void ChangePassive()
    {

    }
    #endregion
}
