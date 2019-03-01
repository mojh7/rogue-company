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
    [SerializeField]
    private Transform muzzleFlashTransform;
    // enum State
    public WeaponState weaponState; // 무기 상태

    private AttackType attackType;
    private WeaponManager weaponManager;
    private Transform objTransform;
    

    protected CharacterInfo.OwnerType ownerType;
    private DelGetDirDegree ownerDirDegree;     // 소유자 각도
    private DelGetPosition ownerDirVec;         // 소유자 각도 벡터(vector3)
    private DelGetPosition ownerPos;            // 소유자 초기 위치(vector3)
    private Player player;
    private Enemy enemy;

    // for Sub Weapon
    private Vector3 additinalPos;
    private float satelliteAngle;

    private float chargedTime;
    private float chargingSpeed;
    private float chargingDamageIncrement;
    private bool canChargedAttack;              // 차징 공격 가능 여부, 초기에 true 상태
    private float chargedDamageIncreaseRate;    // 풀 차징 공격 데미지 상승률
    private Coroutine chargingUpdate;

    private BuffManager ownerBuff;

    private WeaponTargetEffect totalInfo;
    private WeaponTargetEffect effectInfo;

    private bool isFullCharged;
    [SerializeField] protected int weaponId;


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
    public Character GetOwnerPlayer() { return player; }
    public Character GetOwnerEnemy() { return enemy; }
    public WeaponState GetWeaponState() { return weaponState; }
    public int GetWeaponId() { return weaponId; }
    public AttackType GetAttackType() { return attackType; }

    public float GetSubWeaponDirDegree() { return satelliteAngle; }
    public Vector3 GetSubWeaponDirVec() { return Vector3.right.RotateRadians(satelliteAngle); }
    
    public Vector3 GetScale() { return new Vector3(info.scaleX, info.scaleY, 1f); }
    public Vector3 GetMuzzlePos()
    {
        if (-90 <= ownerDirDegree() && ownerDirDegree() < 90)
        {
            return ownerPos() + ownerDirVec() * info.addDirVecMagnitude + MathCalculator.VectorRotate(ownerDirVec(), 90) * info.additionalVerticalPos;
        }
        else
        {
            return ownerPos() + ownerDirVec() * info.addDirVecMagnitude + MathCalculator.VectorRotate(ownerDirVec(), -90) * info.additionalVerticalPos;
        }
    }
    #endregion

    #region unityFunc
    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt((transform.position.y - 0.5f) * 100);
    }
    #endregion

    #region initialization

    //6.02 이유성
    public void Init(WeaponInfo weaponInfo, OwnerType ownerType = OwnerType.PLAYER)
    {
        this.ownerType = ownerType;
        // data save / load를 위한 weapon id 설정
        this.weaponId = weaponInfo.GetWeaponId();
        // weaponInfo Clone
        info = weaponInfo.Clone();
        originInfo = weaponInfo;
        BaseInit();
    }

    /// <summary> WeaponsData에서 index 참조로 무기 정보 받아오기, weaponView class 초기화 </summary>
    public void Init(int weaponId, OwnerType ownerType = OwnerType.PLAYER)
    {
        this.ownerType = ownerType;
        this.weaponId = weaponId;
        // id에 따른 무기 정보 받아오기
        originInfo = WeaponsData.Instance.GetWeaponInfo(weaponId, ownerType);
        info = originInfo.Clone();
        BaseInit();
    }

    /// <summary> weapon 초기화 기본 </summary>
    private void BaseInit()
    {
        objTransform = GetComponent<Transform>();
        weaponView = new WeaponView(objTransform, spriteRenderer);
        weaponView.Init(info.sprite, info.scaleX, info.scaleY);
        sprite = info.sprite;
        weaponState = WeaponState.Idle;
        name = info.weaponName;
        /*
        if (OwnerType.ENEMY == ownerType)
        {
            //enemy
        }
        */

        // 무기 고유 변수들 초기화
        canChargedAttack = true;
        chargedTime = 0;
        if(IsMeleeWeapon(info))
        {
            attackType = AttackType.MELEE;
        }
        else
        {
            attackType = AttackType.RANGED;
        }
    }
    #endregion

    #region func
    public bool FillUpAmmo()
    {
        // oo 총 이거나 총알 꽉찼을 때, 근거리 무기일 때 false
        if(-1 == info.ammoCapacity || info.ammo == info.ammoCapacity || AttackType.MELEE == attackType)
        {
            return false;
        }
        else
        {
            info.ammo = info.ammoCapacity;
            weaponManager.UpdateAmmoView(info);
            return true;
        }
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }

    public void Reveal()
    {
        spriteRenderer.enabled = true;
    }

    public override void Active()
    {
        //무기 습득에 쓸거같음
    }

    private void InitBulletPattern()
    {
        for (int i = 0; i < info.bulletPatterns.Length; i++)
        {
            info.bulletPatterns[i].Init(this);
        }
        for (int i = 0; i < info.bulletPatternsWhenFullyCharged.Length; i++)
        {
            info.bulletPatternsWhenFullyCharged[i].Init(this);
        }
    }

    /// <summary> weaponManager에 처음 등록될 때 onwer 정보 얻어오고 bulletPattern 정보 초기화 </summary>
    public void RegisterWeapon(WeaponManager weaponManager)
    {
        this.weaponManager = weaponManager;

        ownerType = weaponManager.GetOwnerType();
        switch (ownerType)
        {
            case CharacterInfo.OwnerType.PLAYER:
                player = PlayerManager.Instance.GetPlayer();
                break;
            case CharacterInfo.OwnerType.ENEMY:
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

        InitBulletPattern();
    }
    
    public void RegisterSubWeapon(WeaponManager weaponManager)
    {
        Debug.Log("RegisterSubWeapon");
        this.weaponManager = weaponManager;
        ownerType = weaponManager.GetOwnerType();
        switch (ownerType)
        {
            case OwnerType.PLAYER:
                player = PlayerManager.Instance.GetPlayer();
                break;
            case OwnerType.ENEMY:
                enemy = weaponManager.GetEnemy();
                break;
            default:
                break;
        }

        additinalPos = Vector3.zero;
        satelliteAngle = 0;
        ownerDirDegree = weaponManager.GetOwnerDirDegree();//GetSubWeaponDirDegree;
        ownerDirVec = weaponManager.GetOwnerDirVec();//GetSubWeaponDirVec;
        ownerPos = () => { return weaponManager.GetOwnerPos()() + additinalPos; };
        ownerBuff = weaponManager.GetOwnerBuff();
        totalInfo = ownerBuff.WeaponTargetEffectTotal[0];
        effectInfo = ownerBuff.WeaponTargetEffectTotal[(int)info.weaponType];

        InitBulletPattern();
        AutoMoving();
    }

    public void ShowMuzzleFlash()
    {
        if(info.showsMuzzleFlash)
        {
            muzzleFlashObj.SetActive(false);
            StopCoroutine(MuzzleFlash());
            StartCoroutine(MuzzleFlash());
        }
        if(MuzzleFlashType.NONE != info.muzzleFlashType && MuzzleFlashType.BASIC != info.muzzleFlashType)
        {
            ParticleManager.Instance.PlayParticle(info.muzzleFlashName, GetMuzzlePos());
        }
    }

    public bool HasCostForAttack()
    {
        if (AttackType.MELEE == attackType)
        {
            if (OwnerType.PLAYER == ownerType && false == Stamina.Instance.IsConsumableStamina() && 0 < info.staminaConsumption
                && false == PlayerManager.Instance.GetPlayer().IsNotConsumeStamina)
                return false;
        }
        else if (AttackType.RANGED == attackType)
        {
            if (info.ammo <= 0 && info.ammoCapacity >= 0 && false == PlayerManager.Instance.GetPlayer().IsNotConsumeAmmo)
                return false;
        }
        return true;
    }

    #region attack func

    // startAttack -> attack
    public void StartAttack()
    {
        if (weaponState == WeaponState.Idle)
        {
            if (info.touchMode == TouchMode.NORMAL)
            {
                Attack(0f);
            }
            else if (info.touchMode == TouchMode.CHARGE && weaponState == WeaponState.Idle)
            {
                if (false == HasCostForAttack())
                {
                    //Debug.Log("총알 혹은 스테미너 부족으로 인한 차징 공격 실패");
                    return;
                }
                isFullCharged = false;
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

    // 실제 공격 함수, 무기 상태를 Attack으로 바꾸고 공격 패턴 n사이클 실행.
    public void Attack(float damageIncreaseRate)
    {
        if(HasCostForAttack())
        {
            UpdateWeaponBuff();
            weaponState = WeaponState.Attack;
            if (AttackType.MELEE == attackType && false == PlayerManager.Instance.GetPlayer().IsNotConsumeStamina)
                Stamina.Instance.ConsumeStamina(info.staminaConsumption);
            if (isFullCharged && info.canSpecialPatternWhenFullyCharged)
                StartCoroutine(PatternCycleWhenFullCharged(damageIncreaseRate));
            else
                StartCoroutine(PatternCycle(damageIncreaseRate));
        }
        else
        {
            Debug.Log("총알 혹은 스테미너 부족으로 인한 공격 실패");
            // UI 에니메이션 실행
            Handheld.Vibrate(); // 진동 효과
            ControllerUI.Instance.WeaponSwitchButton.StartShake(2f, 2f, 1, false);
        }
    }


    // 공격 시도 중지, onwer랑 공격 방식에 따라 달라짐.
    // player : 공격 버튼 터치 후 땠을 때, 차징 공격같은 경우 오히려 여기서 공격. 함수 명 바뀔 예정
    //          레이저 같은 경우 공격 중지
    public void StopAttack()
    {
        if (info.touchMode == TouchMode.NORMAL)
        {
            for (int i = 0; i < info.bulletPatterns.Length; i++)
            {
                info.bulletPatterns[i].StopAttack();
            }
        }
        else if (info.touchMode == TouchMode.CHARGE && weaponState == WeaponState.Charge)
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

        if(WeaponType.LASER == info.weaponType)
        {
            Reload();
        }
    }

    // 재장전
    public void Reload()
    {
        // 공격 딜레이에 재 장전
        StartCoroutine(ReloadTime());
    }


    #endregion
    /// <summary> 공격 애니메이션 play </summary>
    public void PlayAttackAnimation()
    {        
        if(AttackAniType.NONE != info.attackAniType)
        {
            animator.SetTrigger(info.attackAniType.ToString());
            //Debug.Log("playerAttakAnimation : " + Time.time + ", " + animator.speed);
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
        if(false == PlayerManager.Instance.GetPlayer().IsNotConsumeAmmo)
        {
            info.ammo -= 1;
            weaponManager.UpdateAmmoView(info);
        }
    }

    /// <summary>
    /// 근거리 무기인지 아닌지 체크
    /// </summary>
    /// <returns></returns>
    public static bool IsMeleeWeapon(WeaponInfo info)
    {
        // SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE, MELEE_SPECIAL
        switch (info.weaponType)
        {
            case WeaponType.SPEAR:
            case WeaponType.CLUB:
            case WeaponType.SPORTING_GOODS:
            case WeaponType.SWORD:
            case WeaponType.CLEANING_TOOL:
            case WeaponType.KNUCKLE:
            case WeaponType.MELEE_SPECIAL:
                return true;
            default:
                return false;
        }
    }

    
    #endregion

    #region SubWeaponFunc
    public void UpdateBehavior()
    {
        AutoMoving();
        AutoAttack();
    }

    private void AutoMoving()
    {
        additinalPos = MathCalculator.VectorRotate(Vector3.right, satelliteAngle) * info.radius;
        satelliteAngle += info.rotatedAnglePerSecond * Time.fixedDeltaTime;
        objTransform.localPosition = additinalPos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, satelliteAngle);
    }

    private void AutoAttack()
    {
        if (SubWeaponAttackType.NON_TARGETING_WITH_MAIN_WEAPON == info.subWeaponAttackType || SubWeaponAttackType.TARGETING_WITH_MAIN_WEAPON == info.subWeaponAttackType ||
            SubWeaponAttackType.AUTO_TARGETING_WITH_MAIN_WEAPON == info.subWeaponAttackType)
            return;
        Attack(1f);
    }
    #endregion

    #region coroutine

    private IEnumerator MuzzleFlash()
    {
        muzzleFlashTransform.position = GetMuzzlePos();
        muzzleFlashObj.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(0.07f);
        muzzleFlashObj.SetActive(false);
    }

    // 공격 패턴 사이클.
    private IEnumerator PatternCycle(float damageIncreaseRate)
    {
        // 총구 위치 보기용 for Debug
        if(OwnerType.PLAYER == ownerType)
        {
            if(DebugSetting.Instance.showsMuzzlePos)
                PlayerManager.Instance.GetPlayer().UpdateMuzzlePosition(GetMuzzlePos(), true);
            else
                PlayerManager.Instance.GetPlayer().UpdateMuzzlePosition(Vector3.zero, false);
        }

        PlayAttackAnimation();
        if (0 < info.castingTime)
        {
            yield return YieldInstructionCache.WaitForSeconds(info.castingTime);
        }

        // 공격 한 번 실행. (pattern cycle n번 실행)
        for (int i = 0; i < info.cycleRepetitionCount; i++)
        {
            if(0 < i)
            {
                PlayAttackAnimation();
                /*if (0 < info.castingTime)
                {
                    yield return YieldInstructionCache.WaitForSeconds(info.castingTime);
                }*/
            }
            // Pattern cycle 실행
            for (int j = 0; j < info.bulletPatterns.Length; j++)
            {
                for (int k = 0; k < info.bulletPatterns[j].GetExeuctionCount(); k++)
                {
                    if (OwnerType.ENEMY == ownerType)
                    {
                        if (false == enemy.GetIsAcitveAttack())
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
                    //공격 사운드 실행
                    if (-1 != info.soundId)
                    {
                        AudioManager.Instance.PlaySound(info.soundId, SOUNDTYPE.WEAPON);
                    }
                    else if ("NONE" != info.soundName && "" != info.soundName)
                    {
                        AudioManager.Instance.PlaySound(info.soundName, SOUNDTYPE.WEAPON);
                    }
                    CameraController.Instance.Shake(info.cameraShakeAmount, info.cameraShakeTime);
                    info.bulletPatterns[j].StartAttack(damageIncreaseRate, ownerType);
                    //info.bulletPatterns[j].IncreaseAdditionalAngle();
                    info.bulletPatterns[j].CalcAdditionalValuePerExecution();
                    if (0 < info.bulletPatterns[j].GetDelay())
                    {
                        yield return YieldInstructionCache.WaitForSeconds(info.bulletPatterns[j].GetDelay());
                    }
                }
            }
        }
        for (int i = 0; i < info.bulletPatterns.Length; i++)
        {
            //info.bulletPatterns[i].InitAdditionalAngle();
            info.bulletPatterns[i].InitAdditionalVariables();
        }

        if (WeaponType.LASER != info.weaponType)
        {
            Reload();
        }
        else
        {
            weaponState = WeaponState.Idle;
        }
    }

    private IEnumerator PatternCycleWhenFullCharged(float damageIncreaseRate)
    {
        // 총구 위치 보기용 for Debug
        if (OwnerType.PLAYER == ownerType)
        {
            if (DebugSetting.Instance.showsMuzzlePos)
                PlayerManager.Instance.GetPlayer().UpdateMuzzlePosition(GetMuzzlePos(), true);
            else
                PlayerManager.Instance.GetPlayer().UpdateMuzzlePosition(Vector3.zero, false);
        }

        PlayAttackAnimation();
        if (0 < info.castingTime)
        {
            yield return YieldInstructionCache.WaitForSeconds(info.castingTime);
        }

        // 공격 한 번 실행. (pattern cycle n번 실행)
        for (int i = 0; i < info.cycleRepetitionCount; i++)
        {
            if (0 < i)
            {
                PlayAttackAnimation();
            }
            // Pattern cycle 실행
            for (int j = 0; j < info.bulletPatternsWhenFullyCharged.Length; j++)
            {
                for (int k = 0; k < info.bulletPatternsWhenFullyCharged[j].GetExeuctionCount(); k++)
                {
                    if (OwnerType.ENEMY == ownerType)
                    {
                        if (false == enemy.GetIsAcitveAttack())
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
                    //공격 사운드 실행
                    if (-1 != info.soundId)
                    {
                        AudioManager.Instance.PlaySound(info.soundId, SOUNDTYPE.WEAPON);
                    }
                    else if ("NONE" != info.soundName && "" != info.soundName)
                    {
                        AudioManager.Instance.PlaySound(info.soundName, SOUNDTYPE.WEAPON);
                    }
                    CameraController.Instance.Shake(info.cameraShakeAmount, info.cameraShakeTime);
                    info.bulletPatternsWhenFullyCharged[j].StartAttack(damageIncreaseRate, ownerType);
                    //info.bulletPatterns[j].IncreaseAdditionalAngle();
                    info.bulletPatternsWhenFullyCharged[j].CalcAdditionalValuePerExecution();
                    if (0 < info.bulletPatternsWhenFullyCharged[j].GetDelay())
                    {
                        yield return YieldInstructionCache.WaitForSeconds(info.bulletPatternsWhenFullyCharged[j].GetDelay());
                    }
                }
            }
        }
        for (int i = 0; i < info.bulletPatternsWhenFullyCharged.Length; i++)
        {
            //info.bulletPatterns[i].InitAdditionalAngle();
            info.bulletPatternsWhenFullyCharged[i].InitAdditionalVariables();
        }

        if (WeaponType.LASER != info.weaponType)
        {
            Reload();
        }
        else
        {
            weaponState = WeaponState.Idle;
        }
    }

    // 재장전 코루틴, cooldown 시간 이후의 WeaponState가 Idle로 됨.
    private IEnumerator ReloadTime()
    {
        weaponState = WeaponState.Reload;
        // WaitForSeconds(0)인 경우 무시하고 진행되는게 아니고 코루틴이 한 프레임 밀리고 실행 됨. 
        // 레이저 cost 처리할 때 0.5배속으로 감소 됨 그래서 if문 하나 걸어둠.
        // info.bulletPatterns[j].GetDelay() 경우도 그래서 0 이상일 때 되게 함.
        if (0 < info.cooldown)
        {
            yield return YieldInstructionCache.WaitForSeconds(info.cooldown);
        }
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
        }
        isFullCharged = true;
        chargedTime = info.chargeTimeMax;
        weaponManager.UpdateChargingUI(chargedTime / info.chargeTimeMax);
    }

    #endregion
}