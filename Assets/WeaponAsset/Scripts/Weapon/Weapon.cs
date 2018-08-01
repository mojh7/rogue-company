using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;

public class Weapon : Item
{
    #region Variables
    public WeaponInfo originInfo;
    public WeaponInfo info;
    public WeaponView weaponView;
    public Animator animator;
    // enum State
    public WeaponState weaponState; // 무기 상태

    private AttackType attackType;
    private WeaponManager weaponManager;
    private Transform objTransform;
    private SpriteRenderer spriteRenderer;

    private CharacterInfo.OwnerType ownerType;
    private DelGetDirDegree ownerDirDegree;     // 소유자 각도
    private DelGetPosition ownerDirVec;         // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;            // 소유자 초기 위치(vector3)
    private Player player;

    private int chargedCount;
    private float timePerCharging;
    private float damageIncrementPerCharging;
    private bool canChargedAttack;              // 차징 공격 가능 여부, 초기에 true 상태
    private float chargedDamageIncreaseRate;    // 풀 차징 공격 데미지 상승률
    private Coroutine chargingUpdate;

    private BuffManager ownerBuff;

    private WeaponTargetEffect totalInfo;
    private WeaponTargetEffect effectInfo;

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

    #region initialization
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
        chargedCount = 0;
        timePerCharging = 0.1f;
        if (WeaponType.SPEAR == info.weaponType || WeaponType.CLUB == info.weaponType || WeaponType.SPORTING_GOODS == info.weaponType ||
            WeaponType.SWORD == info.weaponType || WeaponType.CLEANING_TOOL == info.weaponType || WeaponType.KNUCKLE == info.weaponType)
        {
            attackType = AttackType.MELEE;
        }
        else if (WeaponType.PISTOL == info.weaponType || WeaponType.SHOTGUN == info.weaponType || WeaponType.MACHINEGUN == info.weaponType ||
            WeaponType.SNIPER_RIFLE == info.weaponType || WeaponType.LASER == info.weaponType || WeaponType.BOW == info.weaponType ||
            WeaponType.TRASH == info.weaponType || WeaponType.WAND == info.weaponType || WeaponType.OTHER == info.weaponType)
        {
            attackType = AttackType.RANGED;
        }
    }
    #endregion

    #region Function

    public override void Active()
    {
        //무기 습득에 쓸거같음
    }
    

    /// <summary> weaponManager에 처음 등록될 때 onwer 정보 얻어오고 bulletPattern 정보 초기화 </summary>
    public void RegisterWeapon(WeaponManager weaponManager)
    {
        this.weaponManager = weaponManager;

        ownerType = weaponManager.GetOwnerType();
        if(OwnerType.Player == ownerType)
        {
            player = PlayerManager.Instance.GetPlayer();
        }
        ownerDirDegree = weaponManager.GetOwnerDirDegree();
        ownerDirVec = weaponManager.GetOwnerDirVec();
        ownerPos = weaponManager.GetOwnerPos();
        ownerBuff = weaponManager.GetOwnerBuff();
        totalInfo = ownerBuff.WeaponTargetEffectTotal[0];
        effectInfo = ownerBuff.WeaponTargetEffectTotal[(int)info.weaponType];
        

        // 공격 패턴(bulletPattern) 초기화
        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            info.bulletPatterns[i].Init(this);
        }
    }


    private bool HasCostForAttack()
    {
        if (AttackType.MELEE == attackType)
        {
            if (OwnerType.Player == ownerType && 0 == player.GetStamina() && 0 < info.staminaConsumption)
                return false;
        }
        else if (AttackType.RANGED == attackType)
        {
            if (info.ammo <= 0 && info.ammoCapacity >= 0)
                return false;
        }
        return true;
    }

    // startAttack -> attack

    public void StartAttack()
    {
        if (weaponState == WeaponState.Idle)
        {
            if (info.touchMode == TouchMode.Normal)
            {
                Attack(0f);
                if (ShortWeapon())
                {
                    // (1/총스테미너 크기) 스태미너 줄어들게 함
                    Stamina.Instance.StaminaMinus();
                }
                else
                {
                    Debug.Log("스태미너 안 쭐어든닷");
                }
            }
            else if(info.touchMode == TouchMode.Charge && weaponState == WeaponState.Idle)
            {
                if(false == HasCostForAttack())
                {
                    DebugX.Log("총알 혹은 스테미너 부족으로 인한 차징 공격 실패");
                    return;
                }
                UpdateWeaponBuff();
                weaponState = WeaponState.Charge;
                // 차징 코루틴 실행
                if (chargingUpdate == null)
                {
                    chargingUpdate = StartCoroutine(ChargingUpdate());
                }
            }
        }
    }

    // 실제 공격 함수, 무기 상태를 Attack으로 바꾸고 공격 패턴 한 사이클 실행.
    public void Attack(float damageIncreaseRate)
    {
        if(HasCostForAttack())
        {
            UpdateWeaponBuff();
            weaponState = WeaponState.Attack;
            weaponManager.UpdateAmmoView(info);
            PlayAttackAnimation();
            StartCoroutine(PatternCycle(damageIncreaseRate));
        }
        else
        {
            DebugX.Log("총알 혹은 스테미너 부족으로 인한 공격 실패");
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
            chargedDamageIncreaseRate = damageIncrementPerCharging * chargedCount;
            Debug.Log("차징 데미지 뻥튀기율 : " + chargedDamageIncreaseRate);
            Attack(chargedDamageIncreaseRate);
            chargedCount = 0;
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
            case AttackAniType.BLOW:
                animator.SetTrigger("blow");
                break;
            case AttackAniType.STRIKE:
                animator.SetTrigger("strike");
                break;
            case AttackAniType.SHOT:
                animator.SetTrigger("shot");
                break;
            default:
                break;
        }
    }

    public void UpdateWeaponBuff()
    {
        // 공격 속도 증가 
        info.cooldown = originInfo.cooldown * totalInfo.cooldownReduction * effectInfo.cooldownReduction;
        // 차징 속도 증가
        timePerCharging = 0.1f * totalInfo.chargeTimeReduction * effectInfo.chargeTimeReduction;
        // 차징 데미지 증가
        damageIncrementPerCharging = 0.1f * (totalInfo.chargingDamageIncrement + effectInfo.chargingDamageIncrement); 
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
                //CameraController.Instance.Shake(info.cameraShakeAmount, info.cameraShakeTime, info.cameraShakeType, ownerDirVec());
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
        while (chargedCount < info.chargeCountMax)
        {
            yield return YieldInstructionCache.WaitForSeconds(timePerCharging);
            chargedCount += 1;
            weaponManager.UpdateChargingUI((float)chargedCount / info.chargeCountMax);
        }
    }

    /// <summary>
    /// 근거리 무기인지 아닌지 확인하기 -> 근거리면 true 반환 : 스태미너 달게 만들기
    /// </summary>
    /// <returns></returns>
    private bool ShortWeapon()
    {
        // SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        switch (info.weaponType)
        {
            case WeaponType.SPEAR:
            case WeaponType.CLUB:
            case WeaponType.SPORTING_GOODS:
            case WeaponType.SWORD:
            case WeaponType.CLEANING_TOOL:
            case WeaponType.KNUCKLE:
                return true;
            default:
                return false;
        }
    }
    #endregion

    #region coroutine
    #endregion
}