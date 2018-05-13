using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponData;
using DelegateCollection;
using UnityEngine.UI;

/* onwer 하위 Object에 붙어서 무기 관리할 예정인 매니저 클래스
 * 
 * player는 최대 3개까지 무기 수용할 예정.
 */

public class WeaponManager : MonoBehaviour {

    #region variables

    [SerializeField]
    private List<Weapon> equipWeaponSlot;      // 무기 장착 슬룻 (최대 3개)
    [SerializeField]
    private int currentWeaponIndex;         // 현재 사용 무기 index
    private int weaponCount;               // 현재 장착된 무기 갯수 
    [SerializeField]
    private int weaponCountMax;            // 무기 장착 최대 갯수 
    private Transform objTransform;
    private DelGetDirDegree ownerDirDegree;
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private BuffManager ownerBuff;

    [SerializeField]
    private Transform registerPoint;
    //private Character owner로 해야 될 것 같지만 일단 Player owner;
    private Player owner;

    // 디버그용 차징 ui
    public GameObject chargedGaugeUI;
    public Slider chargedGaugeSlider;

    // 매 update 마다 바껴서 임시로 만듬
    private bool canPickAndDropWeapon = true;

    #endregion
    #region getter
    public bool GetAttackAble()
    {
        if(equipWeaponSlot[currentWeaponIndex].GetWeaponState() == WeaponState.Idle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector3 GetPosition() { return objTransform.position; }

    public DelGetDirDegree GetOwnerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    #endregion
    #region setter
    #endregion
    #region UnityFunction
    void Awake()
    {
        objTransform = GetComponent<Transform>();
    }
    // Use this for initialization
    void Start()
    {
        //weaponCountMax = 5;  // 원래 3인데 테스트용으로 inspecter창에서 값 받음;
        weaponCount = weaponCountMax;
        OnOffWeaponActive();
    }

    // 공격 테스트용
    bool isReleaseSpaceBar = false;

    // Update is called once per frame
    void Update()
    {
        //-------------------- 공격 테스트 용
        if(Input.GetKey(KeyCode.Space))
        {
            AttackButtonDown();
            isReleaseSpaceBar = true;
        }
        else if(isReleaseSpaceBar == true)
        {
            isReleaseSpaceBar = false;
            AttackButtonUP();
        }

        //---------------------------------

        // 바라보는 방향으로 무기 회전
        if (owner.GetRightDirection())
        {
            // 우측
            transform.rotation = Quaternion.Euler(0f, 0f, owner.GetDirDegree());
        }
        else
        {
            // 좌측
            transform.rotation = Quaternion.Euler(0f, 0f, owner.GetDirDegree() - 180f);
        }
    }
    #endregion

    #region Function
    public void Init(Player player)
    {
        owner = player;
        // Onwer 정보 등록
        // 방향, Position 리턴 함수 등록,나중에 어디에(onwer 누구냐에 따라서 다름, player, enmey, object) 붙는지에 따라 초기화
        // 지금은 테스트용으로 Player꺼 등록
        ownerDirDegree = player.GetDirDegree;
        ownerDirVec = player.GetRecenteInputVector;
        ownerPos = GetPosition;
        ownerBuff = player.GetBuffManager();

        for (int i = 0; i < weaponCountMax; i++)
        {
            // Debug 용으로 현재 장착된 무기들 인스펙터 창에서 설정된 wepaon id대로 초기화
            equipWeaponSlot[i].Init(-1);
            equipWeaponSlot[i].RegisterWeapon(this);
        }
    }

    /// <summary>
    /// 차징 공격에 사용되는 차징 게이지 UI Update
    /// </summary>
    /// <param name="chargedVaule"></param>
    public void UpdateChargingUI(float chargedVaule)
    {
        if(chargedVaule == 0)
        {
            chargedGaugeUI.SetActive(false);
            return;
        }
        chargedGaugeUI.SetActive(true);
        // 나중에 변수 하나둬서 position y값 지정할 수 있게 해서 어떤 owner에 붙여도 ui 올바른 위치에 뜨게 할 예정
        chargedGaugeSlider.value = chargedVaule; 
    }

    // 공격 버튼 누를 때, 누르는 중일 때
    public void AttackButtonDown()
    {
        if (GetAttackAble())
        {
            equipWeaponSlot[currentWeaponIndex].StartAttack();
        }
    }

    // 공격 버튼 뗐을 때
    public void AttackButtonUP()
    {
        equipWeaponSlot[currentWeaponIndex].StopAttack();
    }
    
    // 전체 무기 Active off, 현재 착용 무기만 on
    public void OnOffWeaponActive()
    {
        for(int i = 0; i < weaponCountMax; i++)
        {
            equipWeaponSlot[i].gameObject.SetActive(false);
        }
        equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
    }

    // 무기 교체, changeNextWepaon 값 true : 다음 무기, false : 이전 무기
    public void ChangeWeapon(bool changeNextWeapon)
    {
        if(equipWeaponSlot[currentWeaponIndex].GetWeaponState() == WeaponState.Idle)
        {
            // 다음 무기로 교체
            if (changeNextWeapon)
            {
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(false);
                currentWeaponIndex = (currentWeaponIndex + 1) % weaponCountMax;
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
            }
            // 이전 무기로 교체
            else
            {
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(false);
                currentWeaponIndex = (currentWeaponIndex - 1 + weaponCountMax) % weaponCountMax;
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
            }
        }
    }


    // return 값으로 나온 버려진 무기를 item Class에 넘겨서 Player 바로 밑에 버려진 아이템 구현

    /// <param name="weapon">추가 </param>
    /// <summary>
    /// 무기 습득
    /// 슬룻 남을 때 : 무기 습득하고 습득한 무기 착용
    /// 슬룻 꽉찰 때 : 습득 무기 착용과 동시에 버려진 무기 return
    /// </summary>
    /// <param name="pickedWeapon">습득한 무기</param>
    public void PickAndDropWeapon(Item pickedWeapon, GameObject itemContainer)
    {
        Weapon weapon = pickedWeapon as Weapon;
        // 매번 update마다 바껴서 일단 임시로 2초간 무기 줍고 버리기 delay줌
        if (!canPickAndDropWeapon) return;
        // 무기 습득하고 습득한 무기 착용
        if (weaponCount < weaponCountMax)
        {
            equipWeaponSlot.Add(weapon);
            weapon.ObjTransform.position = objTransform.position;
            weapon.ObjTransform.SetParent(objTransform);
            currentWeaponIndex = weaponCount++;
            OnOffWeaponActive();         
        }
        // 현재 착용중인 무기 버리고(return으로 내뱉음) 습득 무기로 바꾸고 장착
        else
        {
            Weapon dropedWeapon = equipWeaponSlot[currentWeaponIndex];
            equipWeaponSlot[currentWeaponIndex] = weapon;
            //pickedWeapon.ObjTransform.position = objTransform.position;
            //pickedWeapon.ObjTransform.rotation = objTransform.rotation;

            // pickedWeapon.ObjTransform.parent = objTransform;

            weapon.ObjTransform.SetParent(registerPoint, false);
            //pickedWeapon.ObjTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            weapon.RegisterWeapon(this);
            OnOffWeaponActive();
            itemContainer.GetComponent<ItemContainer>().Init(dropedWeapon);
            //dropedWeapon.ObjTransform.position = itemContainer.transform.position;
            dropedWeapon.ObjTransform.SetParent(itemContainer.transform, false);
        }
        StartCoroutine("PickAndDropWeaponDelay");
    }
    #endregion

    IEnumerable PickAndDropWeaponDelay()
    {
        canPickAndDropWeapon = false;
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        canPickAndDropWeapon = true;
    }
}
