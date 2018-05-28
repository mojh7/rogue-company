using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
/* 
 * Ctrl K,F - 자동 들여 쓰기
 * Ctrl K,C - 주석 처리
 * Ctrl K,U - 주석 해제
 * Ctrl U   - 소문자 만들기
 * Ctrl M,M - 해당 범위 축소 / 확장
 */

/// <summary>
/// Onwer(Player, Enmey, Object) 하위 자식 오브젝트로 붙어서
/// Weapon을 담고 쓸 수 있도록 관리하는 Class, Player는 3개 쓸 예정
/// </summary>
public class WeaponManager : MonoBehaviour {

    #region variables

    [SerializeField]
    private List<Weapon> equipWeaponSlot;      // 무기 장착 슬룻
    /// <summary> 현재 사용 무기 index </summary>
    [SerializeField]
    private int currentWeaponIndex;
    /// <summary> 현재 장착된 무기 갯수 </summary>
    private int weaponCount;
    /// <summary> 무기를 장착할 수 있는 최대 갯수 </summary>
    [SerializeField]
    private int weaponCountMax;
    private Transform objTransform;
    private BuffManager ownerBuff;

    // 아직 owner중 object 고려 안 했음
    private Character owner;
    private OwnerType ownerType;
    // owner의 공격 방향 각도, 방향 벡터 와 현재 위치 함수이며 weapon->bulletPattern->bullet 방향으로 전달 함.
    private DelGetDirDegree ownerDirDegree;
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;

    /// <summary> 무기 drop and pick할 때 지정될 parnet object </summary>
    [SerializeField]
    private Transform registerPoint;

    /// <summary> 차징 게이지 script</summary> 
    public ChargeGauge chargeGauge; // inspector 창에서 붙임

    // 매 update 마다 무기가 바껴서 임시로 delay 만듬
    private bool canPickAndDropWeapon = true;


    /// <summary> owner 각도에 따라 좌우 바뀔 때 weaponManager scale에 적용 할 vector </summary>
    private Vector3 scaleVector;
    /// <summary> owner 각도에 따라 좌우 바뀔 때 weaponManager locla position에 적용 할 vector </summary>
    private Vector3 posVector;
    // 우측 바라보고 있을 때의 weaponManager의 position 초기 x값, 좌측 볼 때 - 이 값을 지정 해줘야 됨.
    private float rightDirectionPosX;

    #endregion
    #region getter
    /// <summary> weapon State 참조하여 Idle인지 확인 후 공격 가능 여부 리턴 </summary>
    /// <returns></returns>
    
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
    public OwnerType GetOwnerType() { return ownerType; }
    public DelGetDirDegree GetOwnerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    #endregion
    #region setter
    public void SetOwnerType(OwnerType ownerType) { this.ownerType = ownerType; }
    #endregion
    #region UnityFunction
    void Awake()
    {
        objTransform = GetComponent<Transform>();
        scaleVector = new Vector3(1f, 1f, 1f);
        // Debug.Log("local pos : " + objTransform.localPosition); 디버그는 반올림으로 소숫점 1자리까지만 나오는 듯 x값 0.15 => 디버그 0.2, x값 0.25 => 디버그 0.3로 나옴
        posVector = objTransform.localPosition;   // weaponManager 초기 local Position 값, 좌 우 바뀔 때 x값만 -, + 부호로 바꿔줘서 사용
        rightDirectionPosX = posVector.x;
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

        // 바라보는 방향으로 무기 회전 및 position, scale 조정
        if (owner.GetRightDirection())
        {
            // 우측
            scaleVector.x = 1f;
            posVector.x = rightDirectionPosX;
            objTransform.localPosition = posVector;
            objTransform.localScale = scaleVector;
            objTransform.rotation = Quaternion.Euler(0f, 0f, ownerDirDegree());
        }
        else
        {
            // 좌측
            scaleVector.x = -1f;
            posVector.x = -rightDirectionPosX;
            objTransform.localPosition = posVector;
            objTransform.localScale = scaleVector;
            objTransform.rotation = Quaternion.Euler(0f, 0f, ownerDirDegree() - 180f);
        }
    }
    #endregion

    #region Function
    public void Init(Character owner, OwnerType ownerType)
    {
        SetOwnerInfo(owner, ownerType);
        
        // 0526 땜빵
        if(OwnerType.Enemy == ownerType)
        {
            equipWeaponSlot[0].Init(Random.Range(0, 4), ownerType);
        }

        for (int i = 0; i < weaponCountMax; i++)
        {
            // Debug 용으로 현재 장착된 무기들 인스펙터 창에서 설정된 wepaon id대로 초기화
            equipWeaponSlot[i].Init(-1, ownerType);
            equipWeaponSlot[i].RegisterWeapon(this);
        }
    }
    
    /// <summary>
    /// Owner 정보 등록
    /// </summary>
    /// <param name="owner"></param>
    public void SetOwnerInfo(Character owner, OwnerType ownerType)
    {
        this.owner = owner;
        this.ownerType = ownerType;
        ownerDirDegree = owner.GetDirDegree;
        ownerDirVec = owner.GetDirVector;
        ownerPos = GetPosition;
        if(OwnerType.Player == ownerType)
        {
            Player player = owner as Player;
            ownerBuff = player.GetBuffManager();
        }
    }

    /// <summary>
    /// 차징 공격에 사용되는 차징 게이지 UI Update
    /// </summary>
    /// <param name="chargedVaule"></param>
    public void UpdateChargingUI(float chargedVaule)
    {
        chargeGauge.UpdateChargeGauge(chargedVaule);
    }

    /// <summary>
    /// 공격 버튼 누를 때, 누르는 중일 때 
    /// </summary>
    public void AttackButtonDown()
    {
        if (GetAttackAble())
        {
            equipWeaponSlot[currentWeaponIndex].StartAttack();
        }
    }

    /// <summary>
    /// 공격 버튼 뗐을 때
    /// </summary>
    public void AttackButtonUP()
    {
        //if(equipWeaponSlot[currentWeaponIndex].GetWeaponState() == WeaponState.PickAndDrop)
        //{
        //    equipWeaponSlot[currentWeaponIndex].SetWeaponState(WeaponState.Idle);
        //}
        equipWeaponSlot[currentWeaponIndex].StopAttack();
    }

    /// <summary>
    /// 전체 무기 object Active off, 현재 착용 무기만 on 
    /// </summary>
    public void OnOffWeaponActive()
    {
        for(int i = 0; i < weaponCountMax; i++)
        {
            equipWeaponSlot[i].gameObject.SetActive(false);
            equipWeaponSlot[i].SetWeaponState(WeaponState.Idle);
        }
        equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// 무기 교체, changeNextWepaon 값 true : 다음 무기, false : 이전 무기 
    /// </summary>
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


    /// <summary>
    /// 무기 습득 : 슬룻 남을 때 = 무기 습득하고 습득한 무기 착용 / 슬룻 꽉찰 때 = 습득 무기 착용과 동시에 버려진 무기
    /// </summary>
    /// <param name="pickedWeapon">얻어서 장착할 무기</param>
    /// <param name="itemContainer"></param>
    public void PickAndDropWeapon(Item pickedWeapon)
    {
        
        Weapon weapon = pickedWeapon as Weapon;
        if (weapon == null)
            return;
        // 매번 update마다 바껴서 일단 임시로 2초간 무기 줍고 버리기 delay줌
        if (!canPickAndDropWeapon) return;
        // 무기 습득하고 습득한 무기 착용
        if (weaponCount < weaponCountMax)
        {
            equipWeaponSlot.Add(weapon);
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.RegisterWeapon(this);
            currentWeaponIndex = weaponCount++;
            OnOffWeaponActive();         
        }
        // 현재 착용중인 무기 버리고 습득 무기로 바꾸고 장착
        else
        {
            Weapon dropedWeapon = equipWeaponSlot[currentWeaponIndex];
            equipWeaponSlot[currentWeaponIndex] = weapon;
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.RegisterWeapon(this);
            OnOffWeaponActive();
            GameObject obj = ItemManager.Instance.CreateItem(dropedWeapon, transform.position);
            dropedWeapon.ObjTransform.SetParent(obj.transform, false);
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
