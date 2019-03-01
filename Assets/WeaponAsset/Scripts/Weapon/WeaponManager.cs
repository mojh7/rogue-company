using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;
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
public class WeaponManager : MonoBehaviour
{

    #region constants
    private const int PLAYER_WEAPON_COUNT_MAX = 3;
    #endregion

    #region variables

    [Header("Main Weapon")]
    [SerializeField]
    private List<Weapon> equippedWeaponSlot;      // 무기 장착 슬룻
    /// <summary> 현재 사용 무기 index </summary>
    [SerializeField] private int currentWeaponIndex;
    /// <summary> 현재 장착된 무기 갯수 </summary>
    [SerializeField] private int weaponCount;
    /// <summary> 무기를 장착할 수 있는 최대 갯수 </summary>
    [SerializeField] private int weaponCountMax;
    /// <summary> 무기 drop and pick할 때 지정될 parnet object </summary>
    [SerializeField]
    private Transform registerPoint;

    [Header("Sub Weapon")]
    [SerializeField]
    private List<Weapon> equippedSubWeaponSlot;
    [SerializeField]
    private Transform subWeaponParentTransform;

    private Transform objTransform;
    private BuffManager ownerBuff;

    // 아직 owner중 object 고려 안 했음
    private Character owner;
    private Player player;
    private Enemy enemy;
    private CharacterInfo.OwnerType ownerType;
    // owner의 공격 방향 각도, 방향 벡터 와 현재 위치 함수이며 weapon->bulletPattern->bullet 방향으로 전달 함.
    private DelGetDirDegree ownerDirDegree;
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;


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

    // for Debug
    private int saveDataLength;
    // 공격 테스트용
    bool isReleaseSpaceBar = false;

    [Header("Player 전용 게임 시작시 무기 적용, True : 모든 무기 착용, Max = 모든 무기 갯수" +
        " False : startWeaponId 무기 1개 착용, Max = 3")]
    public bool equipAllWeapons = false;
    public int startWeaponId = 0;
    
    [SerializeField]
    private bool stopsUpdate;
    #endregion
    #region getter
    public WeaponState GetWeaponState() { return equippedWeaponSlot[currentWeaponIndex].GetWeaponState(); }
    public Vector3 GetPosition() { return objTransform.position; }
    public CharacterInfo.OwnerType GetOwnerType() { return ownerType; }
    public DelGetDirDegree GetOwnerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    public Enemy GetEnemy() { return enemy; }
    public int[] GetWeaponIds()
    {
        int[] weaponIds = new int[saveDataLength];
        for (int i = 0; i < weaponCount; i++)
        {
            weaponIds[i] = equippedWeaponSlot[i].GetWeaponId();
            //Debug.Log("i : " + i + ", 무기장착 id : " + weaponIds[i]);
        }
        for (int i = weaponCount; i < weaponCountMax; i++)
        {
            //Debug.Log("i : " + i + ", 무기 미 장착");
            weaponIds[i] = -1;
        }
        //Debug.Log(weaponCount + ", " + weaponCountMax + ", " + weaponIds[0] + ", " + weaponIds[1] + ", " + weaponIds[2] + ", savaDataLength : " + saveDataLength);
        return weaponIds;
    }
    public int[] GetWeaponAmmos()
    {
        int[] weaponAmmos = new int[saveDataLength];
        for (int i = 0; i < weaponCount; i++)
        {
            weaponAmmos[i] = equippedWeaponSlot[i].info.ammo;
        }
        // Debug.Log(weaponCount + ", " + weaponCountMax + ", " + weaponAmmos[0] + ", " + weaponAmmos[1] + ", " + weaponAmmos[2] + ", ");
        return weaponAmmos;
    }
    #endregion

    #region setter
    /// <summary> Owner 정보 등록 </summary>
    private void SetOwnerInfo(Character owner, CharacterInfo.OwnerType ownerType)
    {
        this.owner = owner;
        this.ownerType = ownerType;
        ownerDirDegree = owner.GetDirDegree;
        ownerDirVec = owner.GetDirVector;
        ownerPos = GetPosition;
        ownerBuff = owner.GetBuffManager();

        switch(ownerType)
        {
            case CharacterInfo.OwnerType.PLAYER:
                player = owner as Player;
                break;
            case CharacterInfo.OwnerType.ENEMY:
                enemy = owner as Enemy;
                break;
            default:
                break;
        }
    }
    #endregion

    #region unityFunc
    void Awake()
    {
        objTransform = GetComponent<Transform>();
        scaleVector = new Vector3(1f, 1f, 1f);
        // Debug.Log("local pos : " + objTransform.localPosition); 디버그는 반올림으로 소숫점 1자리까지만 나오는 듯 x값 0.15 => 디버그 0.2, x값 0.25 => 디버그 0.3로 나옴
        posVector = objTransform.localPosition;   // weaponManager 초기 local Position 값, 좌 우 바뀔 때 x값만 -, + 부호로 바꿔줘서 사용
        rightDirectionPosX = posVector.x;
        equippedWeaponSlot = new List<Weapon>();
        equippedSubWeaponSlot = new List<Weapon>();
    }

    void Update()
    {
        if (stopsUpdate) return;

        //-------------------- Player 공격 테스트 용
#if UNITY_EDITOR
        if (OwnerType.PLAYER == ownerType)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                AttackButtonDown();
                isReleaseSpaceBar = true;
            }
            else if (isReleaseSpaceBar == true)
            {
                isReleaseSpaceBar = false;
                AttackButtonUP();
            }
        }
#endif

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

    void FixedUpdate()
    {
        for (int i = 0; i < equippedSubWeaponSlot.Count; i++)
        {
            equippedSubWeaponSlot[i].UpdateBehavior();
        }
    }
    #endregion

    #region Initialization
    public void Init(Character owner, OwnerType ownerType, int[] startingWeaponInfos)
    {
        SetOwnerInfo(owner, ownerType);
        if (OwnerType.PLAYER == ownerType)
        {
            Weapon weapon;
            equippedWeaponSlot = new List<Weapon>();
            currentWeaponIndex = 0;
            weaponCount = 0;
            // 튜토리얼 씬
            if (true == DebugSetting.Instance.isTutorialScene)
            {
                weaponCountMax = PLAYER_WEAPON_COUNT_MAX;
            }

            // 튜토리얼씬이 아니고 로드 게임이 아닐 때 무기 셋팅
            else if (false == GameStateManager.Instance.GetLoadsGameData())
            {
                // list내 모든 무기 착용
                if(WeaponEquipType.ALL == DebugSetting.Instance.weaponEquipType)
                {
                    weaponCountMax = WeaponsData.Instance.GetWeaponInfosLength();
                    weaponCount = weaponCountMax;
                    Debug.Log("모든 무기 착용, CountMax : " + weaponCountMax);

                    for (int i = 0; i < weaponCountMax; i++)
                    {
                        weapon = ObjectPoolManager.Instance.CreateWeapon(i) as Weapon;
                        equippedWeaponSlot.Add(weapon);
                        weapon.ObjTransform.SetParent(registerPoint, false);
                        weapon.RegisterWeapon(this);
                    }
                }
                //DebugSetting에 기입된 id 무기만 생성
                else if(WeaponEquipType.DEBUG == DebugSetting.Instance.weaponEquipType)
                {
                    weaponCountMax = DebugSetting.Instance.startingWeaponInfos.Length;
                    for (int i = 0; i < DebugSetting.Instance.startingWeaponInfos.Length; i++)
                    {
                        if (-1 == DebugSetting.Instance.startingWeaponInfos[i])
                            continue;
                        weapon = ObjectPoolManager.Instance.CreateWeapon(DebugSetting.Instance.startingWeaponInfos[i]) as Weapon;
                        equippedWeaponSlot.Add(weapon);
                        weapon.ObjTransform.SetParent(registerPoint, false);
                        weapon.RegisterWeapon(this);
                        weaponCount += 1;
                    }
                }

                else if(WeaponEquipType.PLAYER_INFO == DebugSetting.Instance.weaponEquipType)
                {
                    weaponCountMax = PLAYER_WEAPON_COUNT_MAX;
                    for (int i = 0; i < startingWeaponInfos.Length; i++)
                    {
                        weapon = ObjectPoolManager.Instance.CreateWeapon(startingWeaponInfos[i]) as Weapon;
                        equippedWeaponSlot.Add(weapon);
                        weapon.ObjTransform.SetParent(registerPoint, false);
                        weapon.RegisterWeapon(this);
                        weaponCount += 1;
                        if (weaponCount == 3)
                            return;
                    }
                }

                else if (WeaponEquipType.RANDOM == DebugSetting.Instance.weaponEquipType)
                {
                    Debug.Log(DebugSetting.Instance.weaponEquipType);
                    weaponCountMax = 3;
                    weapon = ObjectPoolManager.Instance.CreateWeapon(Random.Range(0, 19)) as Weapon;
                    equippedWeaponSlot.Add(weapon);
                    weapon.ObjTransform.SetParent(registerPoint, false);
                    weapon.RegisterWeapon(this);
                    weapon = ObjectPoolManager.Instance.CreateWeapon(Random.Range(19, 38)) as Weapon;
                    equippedWeaponSlot.Add(weapon);
                    weapon.ObjTransform.SetParent(registerPoint, false);
                    weapon.RegisterWeapon(this);
                    weapon = ObjectPoolManager.Instance.CreateWeapon(Random.Range(38, 57)) as Weapon;
                    equippedWeaponSlot.Add(weapon);
                    weapon.ObjTransform.SetParent(registerPoint, false);
                    weapon.RegisterWeapon(this);
                    weaponCount = 3;
                }
                //Debug.Log("새 게임 무기 초기화, weaponCnt, cntMax = " + weaponCount + ", " + weaponCountMax);
            }
            // 저장된 데이터를 로드한 게임 일 때
            else
            {
                int[] weaponIds = GameDataManager.Instance.GetWeaponIds();
                int[] weaponAmmos = GameDataManager.Instance.GetWeaponAmmos();
                weaponCountMax = weaponIds.Length;
                weaponCount = 0;
                for (int i = 0; i < weaponCountMax; i++)
                {
                    if (weaponIds[i] >= 0)
                    {
                        weapon = ObjectPoolManager.Instance.CreateWeapon(weaponIds[i]) as Weapon;
                        //Debug.Log("i : " + i + ", id : " + weaponIds[i] + ", ammo : " + weaponAmmos[i] + ", Name : " + weapon.GetName());
                        equippedWeaponSlot.Add(weapon);
                        weapon.ObjTransform.SetParent(registerPoint, false);
                        weapon.RegisterWeapon(this);
                        weapon.info.ammo = weaponAmmos[i];
                        weaponCount += 1;
                    }
                }
                Debug.Log("로드 게임 무기 초기화 weaponcnt, cntMax :" + weaponCount + ", " + weaponCountMax);
            }

            saveDataLength = weaponCountMax;
        }
        UpdateCurrentWeapon();
    }

    public void Init(Character owner, EnemyData enemyData)
    {
        if (enemyData.WeaponInfo.Count == 0)
        {
            stopsUpdate = true;
            return;
        }
        SetOwnerInfo(owner, OwnerType.ENEMY);
        currentWeaponIndex = 0;
        if (DebugSetting.Instance.equipsEnemyDataWeapon)
        {
            for (int i = 0; i < enemyData.WeaponInfo.Count; i++)
                EquipWeapon(enemyData.WeaponInfo[i]);
        }
        else
        {
            EquipWeapon(WeaponsData.Instance.GetWeaponInfo(DebugSetting.Instance.enemyEquipWeaponId, OwnerType.ENEMY));
            // EquipWeapon(DataStore.Instance.GetWeaponInfo(Random.Range(0, DataStore.Instance.GetEnemyWeaponInfosLength()), OwnerType.ENEMY));
        }
        UpdateCurrentWeapon();
    }
    #endregion

    #region Function


    public bool FillUpAmmo()
    {
        return equippedWeaponSlot[currentWeaponIndex].FillUpAmmo();
    }

    public bool WeaponEmpty()
    {
        if (0 == weaponCount)
            return true;
        else
            return false;
    }

    public void HideWeapon()
    {
        if (WeaponEmpty())
            return;
        equippedWeaponSlot[currentWeaponIndex].Hide();
    }

    public void RevealWeapon()
    {
        if (WeaponEmpty())
            return;
        equippedWeaponSlot[currentWeaponIndex].Reveal();
    }

    /// <summary> 차징 공격에 사용되는 차징 게이지 UI Update </summary>
    public void UpdateChargingUI(float chargedVaule)
    {
        chargeGauge.UpdateChargeGauge(chargedVaule);
    }

    public void AttackWeapon(int index)
    {
        if (WeaponState.Idle == equippedWeaponSlot[currentWeaponIndex].GetWeaponState())
        {
            if (index != currentWeaponIndex)
            {
                currentWeaponIndex = index;
                UpdateCurrentWeapon();
            }
            equippedWeaponSlot[currentWeaponIndex].StartAttack();
        }
    }

    /// <summary> 공격 버튼 누를 때, 누르는 중일 때 </summary>
    public void AttackButtonDown()
    {
        if (WeaponEmpty())
            return;
        equippedWeaponSlot[currentWeaponIndex].StartAttack();

        for(int i = 0; i < equippedSubWeaponSlot.Count; i++)
        {
            equippedSubWeaponSlot[i].StartAttack();
        }
    }

    /// <summary> 공격 버튼 뗐을 때 </summary>
    public void AttackButtonUP()
    {
        if (WeaponEmpty())
            return;
        equippedWeaponSlot[currentWeaponIndex].StopAttack();
        for (int i = 0; i < equippedSubWeaponSlot.Count; i++)
        {
            equippedSubWeaponSlot[i].StopAttack();
        }
    }

    /// <summary> 현재 착용 무기 대해서 내용 업데이트, 현재 착용 무기 외의 모든 무기 off </summary>
    public void UpdateCurrentWeapon()
    {
        if (WeaponEmpty())
            return;

        for (int i = 0; i < weaponCount; i++)
        {
            equippedWeaponSlot[i].gameObject.SetActive(false);
        }
        equippedWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
        if(OwnerType.PLAYER == ownerType)
        {
            ControllerUI.Instance.WeaponSwitchButton.UpdateWeaponSprite(equippedWeaponSlot[currentWeaponIndex].GetWeaponSprite());
            // 0827 윤아 추가
            int nextWeaponIndex, prevWeaponIndex;
            if (currentWeaponIndex == 0)
            {
                nextWeaponIndex = currentWeaponIndex + 1;
                prevWeaponIndex = weaponCount - 1;
            }
            else if (currentWeaponIndex == (weaponCount - 1))
            {
                nextWeaponIndex = 0;
                prevWeaponIndex = currentWeaponIndex - 1;
            }
            else
            {
                nextWeaponIndex = currentWeaponIndex + 1;
                prevWeaponIndex = currentWeaponIndex - 1;
            }
            if (weaponCount >= 2)
            {
                ControllerUI.Instance.WeaponSwitchButton.UpdateNextWeaponSprite(equippedWeaponSlot[nextWeaponIndex].GetWeaponSprite());
                ControllerUI.Instance.WeaponSwitchButton.UpdatePrevWeaponSprite(equippedWeaponSlot[prevWeaponIndex].GetWeaponSprite());
                ControllerUI.Instance.WeaponSwitchButton.UpdateAmmoView(equippedWeaponSlot[currentWeaponIndex].info);
            }
        }
    }

    public void UpdateAmmoView(WeaponInfo info)
    {
        if(OwnerType.PLAYER == ownerType)
        {
            ControllerUI.Instance.WeaponSwitchButton.UpdateAmmoView(info);
        }
    }

    /// <summary> 무기 교체, changeNextWepaon 값 true : 다음 무기, false : 이전 무기 </summary>
    public void ChangeWeapon(bool changeNextWeapon)
    {
        if (WeaponState.Idle == equippedWeaponSlot[currentWeaponIndex].GetWeaponState())
        {
            if (weaponCount == 1) return;

            // 다음 무기로 교체
            if (changeNextWeapon)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % weaponCount;
            }
            // 이전 무기로 교체
            else
            {
                currentWeaponIndex = (currentWeaponIndex - 1 + weaponCount) % weaponCount;
            }
            UpdateCurrentWeapon();
        }
    }

    public void TutorialWeapon(Item item)
    {
        Weapon weapon = item as Weapon;
        if (weaponCount < weaponCountMax)
        {
            equippedWeaponSlot.Add(weapon);
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.RegisterWeapon(this);
            weaponCount++;
            UpdateCurrentWeapon();
        }
    }

    /// <summary>
    /// 무기 습득 : 슬룻 남을 때 = 무기 습득하고 습득한 무기 착용, 
    /// 슬룻 꽉찰 때 = 습득 무기 착용과 동시에 버려진 무기 </summary>
    /// <param name="pickedWeapon">얻어서 장착할 무기</param>
    public bool PickAndDropWeapon(Item pickedWeapon)
    {
        Weapon weapon = pickedWeapon as Weapon;
        if(WeaponEmpty())
        {
            equippedWeaponSlot.Add(weapon);
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.ObjTransform.localScale = weapon.GetScale();
            weapon.RegisterWeapon(this);
            currentWeaponIndex = weaponCount++;
            UpdateCurrentWeapon();

            return true;
        }
        // canPickAndDropWeapon 매번 update마다 바껴서 일단 임시로 1초간 무기 줍고 버리기 delay줌
        if (weapon == null || WeaponState.Idle != equippedWeaponSlot[currentWeaponIndex].GetWeaponState() || !canPickAndDropWeapon)
            return false;

        // 무기 습득하고 습득한 무기 착용
        if (weaponCount < weaponCountMax)
        {
            equippedWeaponSlot.Add(weapon);
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.RegisterWeapon(this);
            currentWeaponIndex = weaponCount++;
            UpdateCurrentWeapon();      
        }
        // 현재 착용중인 무기 버리고 습득 무기로 바꾸고 장착
        else
        {
            Weapon dropedWeapon = equippedWeaponSlot[currentWeaponIndex];
            equippedWeaponSlot[currentWeaponIndex] = weapon;
            weapon.ObjTransform.SetParent(registerPoint, false);
            weapon.RegisterWeapon(this);
            UpdateCurrentWeapon();
            GameObject obj = ItemManager.Instance.CreateItem(dropedWeapon, transform.position);
            dropedWeapon.ObjTransform.SetParent(obj.transform, false);
            dropedWeapon.ObjTransform.localScale = dropedWeapon.GetScale();
        }
        StartCoroutine(PickAndDropWeaponDelay());
        weapon.ObjTransform.localRotation = Quaternion.identity;
        weapon.ObjTransform.localScale = weapon.GetScale();
        //weapon.ObjTransform.localScale = new Vector3(Mathf.Abs(weapon.ObjTransform.localScale.x), weapon.ObjTransform.localScale.y, weapon.ObjTransform.localScale.z);
        return true;
    }

    //0603 유성, 0807 모 수정
    public void EquipWeapon(WeaponInfo weaponInfo)
    {
        weaponCountMax += 1;
        weaponCount += 1;
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon();
        weapon.Init(weaponInfo, ownerType);
        equippedWeaponSlot.Add(weapon);
        weapon.ObjTransform.SetParent(registerPoint, false);
        weapon.RegisterWeapon(this);
    }

    //0603 유성, 0807 모 수정
    public void EquipSubWeapon(WeaponInfo weaponInfo)
    {
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon();
        weapon.Init(weaponInfo, ownerType);
        equippedSubWeaponSlot.Add(weapon);
        weapon.ObjTransform.SetParent(subWeaponParentTransform, false);
        weapon.RegisterSubWeapon(this);
    }

    /// <summary> Enemy용, 장착 된 무기 회수하여 오브젝트 풀로 되돌림 </summary>
    public void RemoveAllWeapons()
    {
        weaponCountMax = 0;
        weaponCount = 0;
        for(int i = 0; i < equippedWeaponSlot.Count; i++)
            ObjectPoolManager.Instance.DeleteWeapon(equippedWeaponSlot[i].gameObject);
        for (int i = 0; i < equippedSubWeaponSlot.Count; i++)
            ObjectPoolManager.Instance.DeleteWeapon(equippedSubWeaponSlot[i].gameObject);
        equippedWeaponSlot = new List<Weapon>();
        equippedSubWeaponSlot = new List<Weapon>();
    }

    #endregion

    IEnumerator PickAndDropWeaponDelay()
    {
        canPickAndDropWeapon = false;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        canPickAndDropWeapon = true;
    }
}