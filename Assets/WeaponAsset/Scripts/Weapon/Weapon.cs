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
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject muzzleFlashObj;
    // enum State
    public WeaponState weaponState; // 무기 상태

    private AttackType attackType;
    private WeaponManager weaponManager;
    private Transform objTransform;
    

    private CharacterInfo.OwnerType ownerType;
    private DelGetDirDegree ownerDirDegree;     // 소유자 각도
    private DelGetPosition ownerDirVec;         // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;            // 소유자 초기 위치(vector3)
    private Player player;
    private Enemy enemy;

    private float chargedTime;
    private float chargingSpeed;
    private float chargingDamageIncrement;
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
    public AttackType GetAttackType() { return attackType; }
    #endregion

    #region UnityFunction
    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y - 0.26f) * 100);
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
        weaponView = new WeaponView(objTransform, spriteRenderer);
        weaponView.Init(info.sprite, info.scaleX, info.scaleY);
        sprite = info.sprite;
        weaponState = WeaponState.Idle;
        name = info.weaponName;
        if (OwnerType.Enemy == ownerType)
        {
            //enemy
        }

        // 무기 고유 변수들 초기화
        canChargedAttack = true;
        chargedTime = 0;
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
        switch (ownerType)
        {
            case CharacterInfo.OwnerType.Player:
                player = PlayerManager.Instance.GetPlayer();
                break;
            case CharacterInfo.OwnerType.Enemy:
                enemy = weaponManager.GetEnemy();
                break;
            default:
                break;
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
    
    public void ShowMuzzleFlash()
    {
        if(info.showsMuzzleFlash)
        {
            muzzleFlashObj.SetActive(false);
            StopCoroutine(MuzzleFlash());
            StartCoroutine(MuzzleFlash());
        }
    }


    public bool HasCostForAttack()
    {
        if (AttackType.MELEE == attackType)
        {
            if (OwnerType.Player == ownerType && false == Stamina.Instance.IsConsumableStamina() && 0 < info.staminaConsumption)
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
            }
            else if (info.touchMode == TouchMode.Charge && weaponState == WeaponState.Idle)
            {
                if (false == HasCostForAttack())
                {
                    Debug.Log("총알 혹은 스테미너 부족으로 인한 차징 공격 실패");
                    return;
                }
                UpdateWeaponBuff();
                weaponState = WeaponState.Charge;
                // 차징 코루틴 실행
                if (null == chargingUpdate)
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
            PlayAttackAnimation();
            if (AttackType.MELEE == attackType)
                Stamina.Instance.ConsumeStamina(info.staminaConsumption);
            StartCoroutine(PatternCycle(damageIncreaseRate));
        }
        else
        {
            Debug.Log("총알 혹은 스테미너 부족으로 인한 공격 실패");
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
            chargedDamageIncreaseRate = chargingDamageIncrement * chargedTime;
            // Debug.Log("차징 데미지 뻥튀기율 : " + chargedDamageIncreaseRate);
            Attack(chargedDamageIncreaseRate);
            chargedTime = 0;
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
            case AttackAniType.PUNCH:
                animator.SetTrigger("punch");
                break;
            case AttackAniType.SWING:
                animator.SetTrigger("swing");
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
        chargingSpeed = 1f * (totalInfo.chargingSpeedIncrement + effectInfo.chargingSpeedIncrement);
        // 차징 데미지 증가
        chargingDamageIncrement = 1f * (totalInfo.chargingDamageIncrement + effectInfo.chargingDamageIncrement);
        // Debug.Log("무기 효과 업데이트 후 : " + totalInfo.chargingSpeedIncrement  + ", " + effectInfo.chargingSpeedIncrement + ", " + chargingSpeed + ", " + chargingDamageIncrement);
    }

    public void UseAmmo()
    {
        info.ammo -= 1;
        weaponManager.UpdateAmmoView(info);
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

    private IEnumerator MuzzleFlash()
    {
        muzzleFlashObj.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(0.07f);
        muzzleFlashObj.SetActive(false);
    }

    // 공격 패턴 한 사이클.
    private IEnumerator PatternCycle(float damageIncreaseRate)
    {
        if(0 < info.castingTime)
            yield return YieldInstructionCache.WaitForSeconds(info.castingTime);
        // 공격 한 사이클 실행
        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            for (int j = 0; j < info.bulletPatterns[i].GetExeuctionCount(); j++)
            {
                if(OwnerType.Enemy == ownerType)
                {
                    if(false == enemy.GetIsAcitveAttackAI())
                    {
                        //Debug.Log("공격 AI stop으로 인한 공격 사이클 멈춤");
                        Reload();
                        yield break;
                    }
                }
                if (false == HasCostForAttack() && AttackType.RANGED == attackType)
                {
                    //Debug.Log("공격 사이클 내에 총알 부족으로 인한 공격 멈춤");
                    Reload();
                    yield break;
                }

                ShowMuzzleFlash();
                // 공격 사운드 실행
                AudioManager.Instance.PlaySound(info.soundId);
                CameraController.Instance.Shake(info.cameraShakeAmount, info.cameraShakeTime);
                info.bulletPatterns[i].StartAttack(damageIncreaseRate, ownerType);
                if (info.bulletPatterns[i].GetDelay() > 0)
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
        while (chargedTime < info.chargeTimeMax)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            chargedTime += Time.fixedDeltaTime * chargingSpeed;
            weaponManager.UpdateChargingUI(chargedTime / info.chargeTimeMax);
            //Debug.Log(chargedTime + ", " + (chargedTime / info.chargeTimeMax));
        }
        chargedTime = info.chargeTimeMax;
        weaponManager.UpdateChargingUI(chargedTime / info.chargeTimeMax);
    }

    #endregion
}