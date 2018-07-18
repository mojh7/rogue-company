using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

/*
 * 
 */


// 아직 enum 정의 제대로 안해놓음.

//
//public enum ShotType { Charged, SemiAutoMatic, Automatic, Burst }
//public enum TouchType { Normal, Charged }



/* # 
 *  -
 * # WeaponView
 *  - 무기 외형 관리(scale, sprite, animation) 
 */

public class Weapon : Item {

    #region Variables
    public WeaponInfo originInfo;
    public WeaponInfo info;
    public WeaponView weaponView;
    public Animator animator;
    // enum State
    public WeaponState weaponState; // 무기 상태

    private WeaponManager weaponManager;
    private Transform objTransform;
    private SpriteRenderer spriteRenderer;

    private CharacterInfo.OwnerType ownerType;
    private DelGetDirDegree ownerDirDegree;   // 소유자 각도
    private DelGetPosition ownerDirVec; // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;    // 소유자 초기 위치(vector3)

    private float chargedTime;
    private bool canChargedAttack;  // 차징 공격 가능 여부, 초기에 true 상태
    private float ChargedAttackCooldown;        // 차지 공격 쿨타임
    private float chargedDamageIncreaseRate;    // 풀 차징 공격 데미지 상승률
    private Coroutine chargingUpdate;

    private BuffManager ownerBuff;

    [SerializeField] private int weaponId;
    

    // coroutine

    #endregion
    #region getter / setter
    public Sprite GetWeaponSprite() { return spriteRenderer.sprite; }
    public Transform ObjTransform { get { return objTransform; } set { objTransform = value; } }
    public CharacterInfo.OwnerType GetOwnerType() { return ownerType; }
    public DelGetDirDegree GetOwnerDirDegree() { return ownerDirDegree; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    public WeaponState GetWeaponState() { return weaponState; }
    public int GetWeaponId() { return weaponId; }
    #endregion

    #region UnityFunction
    void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
    }
    #endregion
    #region Function

    public override void Active()
    {
        //무기 습득에 쓸거같음
    }
    //6.02 이유성
    public void Init(WeaponInfo weaponInfo, CharacterInfo.OwnerType ownerType = CharacterInfo.OwnerType.Player)
    {
        this.ownerType = ownerType;
        // weaponInfo Clone
        info = weaponInfo.Clone();
        originInfo = weaponInfo;
        BaseInit();
    }

    /// <summary> DataStore에서 index 참조로 무기 정보 받아오기, weaponView class 초기화 </summary>
    public void Init(int weaponId, CharacterInfo.OwnerType ownerType = CharacterInfo.OwnerType.Player)
    {
        this.ownerType = ownerType;
        this.weaponId = weaponId;
        // id에 따른 무기 정보 받아오기
        info = DataStore.Instance.GetWeaponInfo(weaponId, ownerType).Clone();
        originInfo = DataStore.Instance.GetWeaponInfo(weaponId, ownerType);
        BaseInit();
    }

    /// <summary> weapon 초기화 기본 </summary>
    public void BaseInit()
    {
        objTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponView = new WeaponView(objTransform, spriteRenderer);
        weaponView.Init(info.sprite, info.scaleX, info.scaleY);
        weaponState = WeaponState.Idle;
        name = info.weaponName;
        // 무기 고유 변수들 초기화
        canChargedAttack = true;
        ChargedAttackCooldown = 20f;
        chargedTime = 0f;
        // 풀 차징 공격 (근거리, 원거리 별) 데미지 증가율
        if (info.weaponType == WeaponType.Gun || info.weaponType == WeaponType.ShotGun || info.weaponType == WeaponType.Laser)
        {
            chargedDamageIncreaseRate = 0.6f;
        }
        else if (info.weaponType == WeaponType.Blow || info.weaponType == WeaponType.Strike || info.weaponType == WeaponType.Swing)
        {
            chargedDamageIncreaseRate = 0.4f;
        }
    }

    /// <summary> weaponManager에 처음 등록될 때 onwer 정보 얻어오고 bulletPattern 정보 초기화 </summary>
    public void RegisterWeapon(WeaponManager weaponManager)
    {
        this.weaponManager = weaponManager;

        ownerType = weaponManager.GetOwnerType();

        ownerDirDegree = weaponManager.GetOwnerDirDegree();
        ownerDirVec = weaponManager.GetOwnerDirVec();
        ownerPos = weaponManager.GetOwnerPos();
        ownerBuff = weaponManager.GetOwnerBuff();

        // 공격 패턴(bulletPattern) 초기화
        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            info.bulletPatterns[i].Init(this);
        }
    }

    // StartAttack -> Attack -> PatternCycle -> Reload
    // 공격 시도, onwer 따라 다름

    // Player : 공격 버튼 터치 했을 때, 함수명 바뀔 예정
    // Enemy  :
    public void StartAttack()
    {
        // 공격 가능 여부 판단 후 공격(탄 부족, 무기 스왑 도중, 공격 딜레이, 기타 공격 불가능 상태)
        if (weaponState == WeaponState.Idle)
        {
            // TouchMode에 따른 다른 행동 Normal : 일반 공격 실행, Charge : 차징
            if (info.touchMode == TouchMode.Normal)
            {
                // 공격 함수 실행
                Attack(0f);
            }
            else if(info.touchMode == TouchMode.Charge && weaponState == WeaponState.Idle)
            {
                if(info.ammo <= 0 && info.ammoCapacity >= 0)
                {
                    Debug.Log("총알 부족으로 인한 차징 공격 실패");
                    return;
                }
                UpdateWeaponBuff();
                weaponState = WeaponState.Charge;
                // 차징 코루틴 실행
                if (chargingUpdate == null)
                {
                    chargingUpdate = StartCoroutine("ChargingUpdate");
                }
            }
        }
    }

    // 실제 공격 함수, 무기 상태를 Attack으로 바꾸고 공격 패턴 한 사이클 실행.
    public void Attack(float damageIncreaseRate)
    {
        // 사용 할 수 있는 탄약이 존재 하거나, 탄약 최대치가 무한대 인 경우 (일단 info.ammoCapacity = -1 일 때 무한대로 놓고 있음)
        // 일단 1회 공격 사이클 돌릴 때 무조건 info.ammo -1 처리
        if(info.ammo > 0 || info.ammoCapacity < 0)
        {
            UpdateWeaponBuff();
            weaponState = WeaponState.Attack;
            info.ammo -= 1;
            // weaponManager 에서 onwerType = player 인 것만 실행 되게 체크함
            weaponManager.UpdateAmmoView(info);
            // 공격 애니메이션 실행
            // 공격 타입에 따른 공격 실행 (원거리 : 탄 뿌리기 후 cost(탄) 소모)
            PlayAttackAnimation();
            StartCoroutine(PatternCycle(damageIncreaseRate));
        }
        else
        {
            Debug.Log("총알 부족으로 인한 공격 실패");
        }
    }


    // 공격 시도 중지, onwer랑 공격 방식에 따라 달라짐.
    // player : 공격 버튼 터치 후 땠을 때, 차징 공격같은 경우 오히려 여기서 공격. 함수 명 바뀔 예정
    //          레이저 같은 경우 공격 중지
    public void StopAttack()
    {
        if (info.touchMode == TouchMode.Normal)
        {
            for (int i = 0; i < info.bulletPatternsLength; i++)
            {
                info.bulletPatterns[i].StopAttack();
            }
        }
        else if (info.touchMode == TouchMode.Charge && weaponState == WeaponState.Charge)
        {
            // 차징 중지 후 공격 실행, 풀 차징 시에만 추가 효과 얻음
            if (chargedTime + 0.01f >= info.chargeTime)
            {
                // 풀 차징
                Attack(chargedDamageIncreaseRate);
            }
            else
            {
                // 풀 차징하지 않음
                Attack(0f);
            }
            chargedTime = 0f;
            // 차징 update coroutine 종료
            if(chargingUpdate != null)
            {
                StopCoroutine(chargingUpdate);
                chargingUpdate = null;
            }
            // 차징 UI off
            weaponManager.UpdateChargingUI(0);
        }
    }

    // 재장전
    public void Reload()
    {
        // 공격 딜레이에 재 장전
        StartCoroutine(ReloadTime());
    }

    // 공격 애니메이션 play
    public void PlayAttackAnimation()
    {        
        // 근거리는 휘두르기, 찌르기 애니메이션
        // 총은 반동 주기
        switch (info.attackAniType)
        {
            case AttackAniType.Blow:
                animator.SetTrigger("blow");
                break;
            case AttackAniType.Strike:
                animator.SetTrigger("strike");
                break;
            case AttackAniType.Shot:
                animator.SetTrigger("shot");
                break;
            default:
                break;
        }
    }

    public void UpdateWeaponBuff()
    {
        // 9. 공격속도 증가 +n% (무기 재사용 시간 감소 n% 감소), n 미정 
        info.cooldown = originInfo.cooldown * ownerBuff.WeaponTargetEffectTotal.cooldownReduction;
        info.chargeTime = originInfo.chargeTime * ownerBuff.WeaponTargetEffectTotal.chargeTimeReduction;
    }


    // 공격 패턴 한 사이클.
    private IEnumerator PatternCycle(float damageIncreaseRate)
    {
        // 공격 한 사이클 실행
        for (int i = 0; i < info.bulletPatternsLength; i++)
        { 
            for(int j = 0; j < info.bulletPatterns[i].GetExeuctionCount(); j++)
            {
                // 공격 사운드 실행
                AudioManager.Instance.PlaySound(info.soundId);
                CameraController.Instance.Shake(info.cameraShakeAmount, info.cameraShakeTime, info.cameraShakeType, ownerDirVec());
                info.bulletPatterns[i].StartAttack(damageIncreaseRate, ownerType);
                if(info.bulletPatterns[i].GetDelay() > 0)
                {
                    yield return YieldInstructionCache.WaitForSeconds(info.bulletPatterns[i].GetDelay());
                }
            }
        }
        Reload(); 
    } 

    // 재장전 코루틴, cooldown 시간 이후의 WeaponState가 Idle로 됨.
    private IEnumerator ReloadTime()
    {
        weaponState = WeaponState.Reload;
        yield return YieldInstructionCache.WaitForSeconds(info.cooldown);
        weaponState = WeaponState.Idle;
    }

    /// <summary>
    /// 차징 시작 코루틴
    /// </summary>
    private IEnumerator ChargingUpdate()
    {
        // 무기별로 chargeTime 다름 0517 기준으로 근거리 2.5~3.5초, 원거리 3~4.5초

        // 2.019999, 2와 같은 부동 소수점 계산 문제 생겨서 info.chargeTime에다가 WaitForseconds / 2 값 만큼 빼줬음 
        while (chargedTime < info.chargeTime - 0.01f)
        {
            // 0.02f => 차징 ui 갱신 시간, 숫자가 작을 수록 빠른 주기로 갱신됨.
            chargedTime += 0.02f;
            weaponManager.UpdateChargingUI(chargedTime / info.chargeTime);
            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }
    }

    // 차징 공격 쿨타임 코루틴, 20초
    private IEnumerator EnterChargingAttackCooldown()
    {
        canChargedAttack = false;
        yield return YieldInstructionCache.WaitForSeconds(ChargedAttackCooldown);
        canChargedAttack = true;
    }
    #endregion

    #region coroutine
    #endregion
}