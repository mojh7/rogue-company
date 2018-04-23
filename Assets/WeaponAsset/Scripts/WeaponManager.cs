using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponData;
using DelegateCollection;

/* onwer 하위 Object에 붙어서 무기 관리할 예정인 매니저 클래스
 * 
 * player는 최대 3개까지 무기 수용할 예정.
 */

public class WeaponManager : MonoBehaviour {

    #region variables
    
    [SerializeField]
    private Weapon[] equipWeaponSlot;   // 무기 장착 슬룻 (최대 3개)
    public int currentWeaponIndex;     // 현재 사용 무기 index
    private int weaponNumMax;           // 무기 장착 최대 갯수 
    private static WeaponManager instance = null;
    private Transform objTransform;
    private DelGetDirDegree ownerDirDegree;
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    

    #endregion
    #region getter
    public static WeaponManager Instance { get { return instance; } }
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
    #endregion
    #region setter
    #endregion
    #region UnityFunction
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        objTransform = GetComponent<Transform>();
    }
    // Use this for initialization
    void Start()
    {
        
        weaponNumMax = equipWeaponSlot.Length;  // 3
        Init();
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
        if (Player.Instance.GetRightDirection())
        {
            // 우측
            transform.rotation = Quaternion.Euler(0f, 0f, Player.Instance.GetDirDegree());
        }
        else
        {
            // 좌측
            transform.rotation = Quaternion.Euler(0f, 0f, Player.Instance.GetDirDegree() - 180f);
        }
    }
    #endregion

    #region Function
    public void Init()
    {
        // 방향, Position 리턴 함수 등록,나중에 어디에(onwer 누구냐에 따라서 다름, player, enmey, object) 붙는지에 따라 초기화
        // 지금은 테스트용으로 Player꺼 등록
        ownerDirDegree = Player.Instance.GetDirDegree;
        ownerDirVec = Player.Instance.GetRecenteInputVector;
        ownerPos = GetPosition;
        

        for (int i = 0; i < weaponNumMax; i++)
        {
            equipWeaponSlot[i].SetownerDirDegree(ownerDirDegree);
            equipWeaponSlot[i].SetOwnerPos(ownerPos);
            equipWeaponSlot[i].SetOwnerDirVec(ownerDirVec);
            equipWeaponSlot[i].Init();
        }
    }

    // 공격 버튼 누르는 중
    public void AttackButtonDown()
    {
        if (GetAttackAble())
        {
            equipWeaponSlot[currentWeaponIndex].StartAttack();
        }
    }

    // 공격 버튼 UP
    public void AttackButtonUP()
    {
        equipWeaponSlot[currentWeaponIndex].StopAttack();
    }
    #endregion

    // 해당 착용 무기 on, 비 착용 무기 off
    public void OnOffWeaponActive()
    {
        for(int i = 0; i < weaponNumMax; i++)
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
                currentWeaponIndex = (currentWeaponIndex + 1) % weaponNumMax;
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
            }
            // 이전 무기로 교체
            else
            {
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(false);
                currentWeaponIndex = (currentWeaponIndex - 1 + weaponNumMax) % weaponNumMax;
                equipWeaponSlot[currentWeaponIndex].gameObject.SetActive(true);
            }
        }
    }
}
