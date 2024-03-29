﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;

public class Player : Character
{
    #region constants
    // playerType enum 순서 대로 한글 이름, 190131 기준 SOCCER, MUSIC, FISH, ARMY 
    public static readonly string[] PLAYER_TYPE_NAME = new string[] { "축구", "음악", "낚시", "군인" };
    #endregion

    #region components
    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스
    [SerializeField]
    private PlayerHpbarUI playerHPUi;
    private Stamina stamina;
    #endregion

    #region variables
    private const string RECOVERY_HP = "RecoveryHp";

    // END 끝 지점 알려고 만든 것
    public enum PlayerType { SOCCER, MUSIC, FISH, ARMY, END }
    private Transform objTransform;
    private RaycastHit2D hit;
    private List<RaycasthitEnemy> raycastHitEnemies;
    private RaycasthitEnemy raycasthitEnemyInfo;
    private int layerMask;  // autoAim을 위한 layerMask
    private int killedEnemyCount;
    private PlayerData playerData;
    private PlayerData originPlayerData;    // 아이템 효과 적용시 기준이 되는 정보

    private float skillGageMultiple;

    private float floorSpeed;
    private int shieldCount;

    [SerializeField]
    private GameObject muzzlePosObj;
    [SerializeField]
    private Transform muzzlePosTransform;

    private SkillData skillData;
    private int[] startingWeaponInfos;
    private float prevHpMaxRatio;
    #endregion

    #region property
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
        set
        {
            playerData = value;
        }
    }
    public int KilledEnemyCount
    {
        get
        {
            return killedEnemyCount;
        }
    }
    public float CharScale
    {
        get;
        private set;
    }
    public int ShieldCount
    {
        get { return shieldCount; }
        private set { shieldCount = value; }
    }

    public bool IsNotConsumeStamina
    {
        get; private set;
    }
    public bool IsNotConsumeAmmo
    {
        get; private set;
    }

    #endregion

    #region getter
    public float GetPlayerScaleRatio()
    {
        return objTransform.localScale.x;
    }


    public PlayerController PlayerController
    {
        get
        {
            return controller;
        }
    }
    #endregion

    #region setter
    public void SetInFloor()
    {
        floorSpeed = .5f;
    }
    public void SetInRoom()
    {
        floorSpeed = 0;
    }
    #endregion

    #region UnityFunc
    void Awake()
    {
        objTransform = GetComponent<Transform>();
        scaleVector = new Vector3(1f, 1f, 1f);
        isRightDirection = true;
        raycastHitEnemies = new List<RaycasthitEnemy>();
        raycasthitEnemyInfo = new RaycasthitEnemy();
        layerMask = 1 << LayerMask.NameToLayer("TransparentFX");
        floorSpeed = 0;

        int attackTypeAbnormalStatusTypeLength = (int)AttackTypeAbnormalStatusType.END;
        isAttackTypeAbnormalStatuses = new bool[attackTypeAbnormalStatusTypeLength];
        effectAppliedCount = new int[attackTypeAbnormalStatusTypeLength];
        attackTypeAbnormalStatusCoroutines = new Coroutine[attackTypeAbnormalStatusTypeLength];

        int controlTypeAbnormalStatusTypeLength = (int)ControlTypeAbnormalStatusType.END;
        isControlTypeAbnormalStatuses = new bool[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusCoroutines = new Coroutine[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusTime = new float[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusesDurationMax = new float[controlTypeAbnormalStatusTypeLength];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(false == e && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("무기 장착");
            e = true;
            // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
            weaponManager.Init(this, OwnerType.PLAYER);
        }
        */

        // 총구 방향(각도)에 따른 player 우측 혹은 좌측 바라 볼 때 반전되어야 할 object(sprite는 여기서, weaponManager는 스스로 함) scale 조정
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            ActiveSkill();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Dash(750f, 50f);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            KnockBack(250f, GetDirVector(), Vector2.zero, false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Evade();
        ChargeSkill();
#endif

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(bodyTransform.position.y * 100);
        if (isEvade)
            return;
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = 1f;
            spriteTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -1f;
            spriteTransform.localScale = scaleVector;
        }
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region initialzation

    void InitilizeController()
    {
        ControllerUI.Instance.SetPlayer(this, ref controller);
    }

    public override void Init()
    {
        base.Init();
        CameraController.Instance.AttachObject(this.transform); // get Camera
        baseColor = Color.white;
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.PLAYER;
        damageImmune = CharacterInfo.DamageImmune.NONE;
        abnormalImmune = CharacterInfo.AbnormalImmune.NONE;

        animationHandler.Init(this, PlayerManager.Instance.runtimeAnimator);

        IsNotConsumeStamina = false;
        IsNotConsumeAmmo = false;
        isActiveMove = true;
        isActiveAttack = true;

        directionVector = new Vector3(1, 0, 0);
        shieldCount = 0;
        evadeCoolTime = 0.05f;
        battleSpeed = 0.5f;
        prevHpMaxRatio = 1f;
        InitilizeController();

        playerHPUi = GameObject.Find("HPbar").GetComponent<PlayerHpbarUI>();
        buffManager = PlayerBuffManager.Instance.BuffManager;
        buffManager.SetOwner(this);

        stamina = Stamina.Instance;
        stamina.SetPlayer(this);
        
        TimeController.Instance.PlayStart();

        
        poisonDamagePerUnit = AbnormalStatusConstants.PLAYER_TARGET_POISON_INFO.FIXED_DAMAGE_PER_UNIT;
        burnDamagePerUnit = AbnormalStatusConstants.PLAYER_TARGET_BURN_INFO.FIXED_DAMAGE_PER_UNIT;
        InitStatusEffects();
        //Debug.Log("hpMax : " + hpMax);
    }

    public void InitWithPlayerData(PlayerData playerData)
    {
        skillGageMultiple = 1;
        hp = playerData.Hp;
        hpMax = playerData.HpMax;
        playerHPUi.Init(this);

        //Debug.Log("InitPlayerData hp, hpmax : " + playerData.Hp +", " + playerData.HpMax);
        this.playerData = playerData;
        originPlayerData = playerData.Clone();
        ApplyItemEffect();
        stamina.SetStaminaBar(playerData.StaminaMax);
        playerData.SkillGauge = 100;
        //Debug.Log("loadsGameData : " + GameStateManager.Instance.GetLoadsGameData());
        if (GameStateManager.Instance.GetLoadsGameData())
            PlayerBuffManager.Instance.LoadMiscItemDatas();

        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this, OwnerType.PLAYER, PlayerData.StartingWeaponInfos);
    }
    #endregion

    #region func
    private void ScaleChange(float scale)
    {
        CharScale = scale;
        if (scale != this.bodyTransform.localScale.x)
        {
            ParticleManager.Instance.PlayParticle("Smoke", this.bodyTransform.position);
        }
        this.bodyTransform.localScale = Vector3.one * scale;
        CameraController.Instance.transform.localScale = Vector3.one / scale;
    }

    public void UpdateMuzzlePosition(Vector3 pos, bool visible)
    {
        muzzlePosObj.SetActive(visible);
        muzzlePosTransform.position = pos;
    }

    public override bool Evade()
    {
        if (!isActiveMove)
            return false;
        if (isEvade || !canEvade)
            return false;
        controller.AttackJoyStickUp();
        canEvade = false;
        isEvade = true;
        gameObject.layer = 0;
        directionVector = controller.GetMoveRecentNormalInputVector();
        directionVector.Normalize();
        directionDegree = directionVector.GetDegFromVector();
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = 1f;
            spriteTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -1f;
            spriteTransform.localScale = scaleVector;
        }
        ParticleManager.Instance.PlayParticle("WalkSmoke", bodyTransform.position, spriteRenderer.transform.localScale);
        StartCoroutine(Roll(directionVector));
        return true;
    }

    protected override void Die()
    {
        GameDataManager.Instance.SetTime(TimeController.Instance.GetPlayTime);
        UIManager.Instance.GameOverUI();
        GameStateManager.Instance.GameOver();
    }

    ///// <summary>
    ///// 쉴드가 있을시 데미지 상관 없이 공격(공격 타입 상관 X) 방어, 아직 미사용
    ///// </summary>
    //public bool DefendAttack()
    //{
    //    if (0 >= shieldCount)
    //        return false;

    //    shieldCount -= 1;
    //    // 버프매니저 쪽으로 쉴드 버프 없애는 명령 보내기
    //    return true;
    //}
    private void ChargeSkill()
    {
        playerData.SkillGauge += Time.deltaTime;
        controller.ActiveSkill(playerData.SkillGauge, playerData.SkillData.CoolTime);
    }

    public override float Attacked(TransferBulletInfo transferredBulletInfo)
    {
        if(damageImmune == CharacterInfo.DamageImmune.ALL)
        {
            ParticleManager.Instance.PlayParticle("Guard", interactiveCollider2D.transform.position);
            return 0;
        }
        if (CharacterInfo.State.ALIVE != pState || isEvade)
            return 0;
        ReduceHp(transferredBulletInfo.damage);
        AttackedAction(1);
        AttackedEffect();
        return transferredBulletInfo.damage;
    }

    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        if (damageImmune == CharacterInfo.DamageImmune.ALL)
        {
            ParticleManager.Instance.PlayParticle("Guard", interactiveCollider2D.transform.position);
            return 0;
        }
        if (CharacterInfo.State.ALIVE != pState || isEvade)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        // 크리티컬 공격
        ReduceHp(damage);
        AttackedAction(damage);

        if (knockBack > 0)
            KnockBack(knockBack, _dir, bulletPos, positionBasedKnockBack);

        AttackedEffect();
        return damage;
    }

    public override void ActiveSkill()
    {
        float lapsedTime = playerData.SkillGauge;
        playerData.SkillData.Run(this, ActiveSkillManager.nullVector, ref lapsedTime);
        playerData.SkillGauge = lapsedTime;
        controller.ActiveSkill(lapsedTime, playerData.SkillData.CoolTime);
    }

    public override void SetAim()
    {
        switch (aimType)
        {
            default:
            //case CharacterInfo.AutoAimType.REACTANCE:
            case CharacterInfo.AimType.AUTO:
                AutoAim();
                break;
            case CharacterInfo.AimType.SEMIAUTO:
                SemiAutoAim();
                break;
            case CharacterInfo.AimType.MANUAL:
                ManualAim();
                break;
        }
    }

    public override CustomObject Interact()
    {
        float bestDistance = interactiveCollider2D.radius;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(bodyTransform.position, interactiveCollider2D.radius, (1 << 1) | (1 << 9));

        for (int i = 0; i < collider2D.Length; i++)
        {
            if (!collider2D[i].GetComponent<CustomObject>().GetAvailable())
                continue;
            float distance = Vector2.Distance(bodyTransform.position, collider2D[i].transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = collider2D[i];
            }
        }

        if (null == bestCollider)
            return null;

        return bestCollider.GetComponent<CustomObject>();
    }

    public override void SelfDestruction()
    {
        Attacked(Vector2.zero, bodyTransform.position, hp * 0.5f, 0);
    }

    public bool AttackAble()
    {
        if (pState == CharacterInfo.State.ALIVE && !isEvade)
            return true;
        else return false;
    }

    public void AddKilledEnemyCount()
    {   
        playerData.SkillGauge += (int)(10 * skillGageMultiple);
        if (playerData.SkillGauge >= 100)
            playerData.SkillGauge = 100;
        controller.ActiveSkill(playerData.SkillGauge, 100);
        if (buffManager.CharacterTargetEffectTotal.canDrainHp)
        {
            killedEnemyCount += 1;
            if (killedEnemyCount == 7)
            {
                RecoveryHp(1f);
                killedEnemyCount = 0;
            }
        }
    }

    public bool ConsumeStamina(int staminaConsumption)
    {
        if (0 <= playerData.Stamina)
            return false;
        playerData.Stamina -= staminaConsumption;
        if (0 <= playerData.Stamina)
            playerData.Stamina = 0;

        return true;
    }

    protected override void RecoveryHp(float damage)
    {
        base.RecoveryHp(damage);
        ParticleManager.Instance.PlayParticle(RECOVERY_HP, this.bodyTransform.position, 1f, bodyTransform);
        playerHPUi.Notify();
    }

    protected override void ReduceHp(float damage)
    {
        hp -= damage;
        playerHPUi.Notify();
        if (hp <= 0)
            Die();
    }

    private void AttackedAction(float power)
    {
        CameraController.Instance.Shake(0.2f, 0.2f);
        LayerController.Instance.FlashAttackedLayer(0.2f);
    }
    private void AutoAim()
    {
        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        if (0 == enemyTotal)
        {
            directionVector = controller.GetMoveAttackInputVector();
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            return;
        }
        else
        {
            List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;
            List<BoxCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
            raycastHitEnemies.Clear();
            int raycasthitEnemyNum = 0;
            float minDistance = 10000f;
            int proximateEnemyIndex = -1;

            Vector3 enemyPos = new Vector3(0, 0, 0);
            for (int i = 0; i < enemyTotal; i++)
            {
                raycasthitEnemyInfo.index = i;
                enemyPos = enemyColliderList[i].transform.position + new Vector3(enemyColliderList[i].offset.x + enemyColliderList[i].offset.y, 0);
                raycasthitEnemyInfo.distance = Vector2.Distance(enemyPos, objTransform.position);
                hit = Physics2D.Raycast(objTransform.position, enemyPos - objTransform.position, raycasthitEnemyInfo.distance, layerMask);
                if (hit.collider == null)
                {
                    raycastHitEnemies.Add(raycasthitEnemyInfo);
                    raycasthitEnemyNum += 1;
                }
            }

            if (raycasthitEnemyNum == 0)
            {
                isBattle = false;
                directionVector = controller.GetMoveAttackInputVector();
                directionDegree = directionVector.GetDegFromVector();
                return;
            }

            for (int j = 0; j < raycasthitEnemyNum; j++)
            {
                if (raycastHitEnemies[j].distance <= minDistance)
                {
                    minDistance = raycastHitEnemies[j].distance;
                    proximateEnemyIndex = j;
                }
            }

            BoxCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionVector.z = 0;
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            isBattle = true;
        }
    }
    private void SemiAutoAim()
    {
        Vector2 enemyVector;

        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        if (0 == enemyTotal)
        {
            directionVector = controller.GetMoveAttackInputVector();
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            isBattle = false;
            return;
        }
        else
        {
            directionVector = controller.GetAttackRecentNormalInputVector();
            directionVector.Normalize();
            List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;
            List<BoxCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
            raycastHitEnemies.Clear();
            int raycasthitEnemyNum = 0;
            float minDistance = 10000f;
            int proximateEnemyIndex = -1;

            Vector3 enemyPos;
            for (int i = 0; i < enemyTotal; i++)
            {
                raycasthitEnemyInfo.index = i;
                enemyPos = enemyColliderList[i].transform.position;
                raycasthitEnemyInfo.distance = Vector2.Distance(enemyPos, objTransform.position);
                enemyVector = enemyPos - objTransform.position;
                hit = Physics2D.Raycast(objTransform.position, enemyVector, raycasthitEnemyInfo.distance, layerMask);
                if (hit.collider == null && Vector2.Dot(directionVector, enemyVector.normalized) >.25f)
                {
                    raycastHitEnemies.Add(raycasthitEnemyInfo);
                    raycasthitEnemyNum += 1;
                }
            }

            if (raycasthitEnemyNum == 0)
            {
                directionDegree = directionVector.GetDegFromVector();
                return;
            }

            for (int j = 0; j < raycasthitEnemyNum; j++)
            {
                if (raycastHitEnemies[j].distance <= minDistance)
                {
                    minDistance = raycastHitEnemies[j].distance;
                    proximateEnemyIndex = j;
                }
            }
            BoxCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
        }
        isBattle = true;
    }
    private void ManualAim()
    {
        directionVector = controller.GetMoveAttackInputVector();
        directionVector.Normalize();
        directionDegree = directionVector.GetDegFromVector();
        isBattle = true;
    }
    private void Move()
    {
        if (isEvade || !isActiveMove || isDash)
            return;

        if(isBattle)
        {
            totalSpeed = playerData.MoveSpeed + floorSpeed - battleSpeed;
        }
        else
        {
            totalSpeed = playerData.MoveSpeed + floorSpeed;
        }
        if(rgbody)
            rgbody.MovePosition(objTransform.position 
            + controller.GetMoveInputVector() * (totalSpeed) * Time.fixedDeltaTime);

        if (controller.GetMoveInputVector().sqrMagnitude > 0.1f)
        {
            animationHandler.Walk();
        }
        else
        {
            animationHandler.Idle();
        }
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
        {
            bodyTransform.Translate(Vector2.up * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bodyTransform.Translate(Vector2.down * playerData.MoveSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            bodyTransform.Translate(Vector2.right * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            bodyTransform.Translate(Vector2.left * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
#endif
    }
    private void EndEvade()
    {
        isEvade = false;
        gameObject.layer = 16;
        weaponManager.RevealWeapon();
    }
    #endregion

    #region itemEffect
    public override void ApplyItemEffect()
    {
        CharacterTargetEffect itemUseEffect = buffManager.CharacterTargetEffectTotal;
        playerData.StaminaMax = Mathf.RoundToInt(originPlayerData.StaminaMax * itemUseEffect.staminaMaxRatio);
        if(playerData.MoveSpeed != originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
        {
            if(playerData.MoveSpeed > originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
            {
                ParticleManager.Instance.PlayParticle("SpeedDown", this.bodyTransform.position);
            }
            else
            {
                ParticleManager.Instance.PlayParticle("SpeedUp", this.bodyTransform.position);
            }
        }
        playerData.MoveSpeed = originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement;
        IsNotConsumeStamina = itemUseEffect.isNotConsumeStamina;
        IsNotConsumeAmmo = itemUseEffect.isNotConsumeAmmo;
        damageImmune = itemUseEffect.isImmuneDamage;
        abnormalImmune = itemUseEffect.isImmuneAbnormal;
        if(damageImmune == CharacterInfo.DamageImmune.ALL)
            ShaderController.ChangeBoundary(spriteRenderer.material, ShaderController.ShaderData.EFFECTAMOUNT, 1);
        else
            ShaderController.ChangeBoundary(spriteRenderer.material, ShaderController.ShaderData.EFFECTAMOUNT, 0);
        if(abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            ShaderController.ChangeBoundary(spriteRenderer.material, ShaderController.ShaderData.CONTRAST, .5f);
        else
            ShaderController.ChangeBoundary(spriteRenderer.material, ShaderController.ShaderData.CONTRAST, 2);

        if (itemUseEffect.hpMaxRatio > 0)
        {
            // hpMax 상승
            if(prevHpMaxRatio < itemUseEffect.hpMaxRatio)
            {
                // 5 / 10 => 6 / 12, 10 / 10 => 12 / 12 비율 따라가면서 회복
                float hpPersent = hp / hpMax;
                hpMax = originPlayerData.HpMax * itemUseEffect.hpMaxRatio;
                hp = (int)(hpMax * hpPersent);
                prevHpMaxRatio = itemUseEffect.hpMaxRatio;
            }
            // hpMax 감소
            else if (prevHpMaxRatio > itemUseEffect.hpMaxRatio)
            {
                // 20 / 20 => 16 / 16, 12 / 20 => 12 / 16
                hpMax = originPlayerData.HpMax * itemUseEffect.hpMaxRatio;
                if(hp >= hpMax)
                {
                    hp = hpMax;
                }
                prevHpMaxRatio = itemUseEffect.hpMaxRatio;
            }
        }
        if (itemUseEffect.skillGage > 0)
            skillGageMultiple = itemUseEffect.skillGage;

        ScaleChange(itemUseEffect.charScale);

        stamina.SetStaminaMax(playerData.StaminaMax);
        stamina.RecoverStamina(0);
        playerHPUi.SetHpMax();
        playerHPUi.Notify();
    }

    // total을 안 거치고 바로 효과 적용하기 위해 구분함, 소모형 아이템 용 함수
    public override void ApplyConsumableItem(CharacterTargetEffect itemUseEffect)
    {
        if (0 != itemUseEffect.recoveryHp)
        {
            RecoveryHp(itemUseEffect.recoveryHp);
        }
        if (0 != itemUseEffect.recoveryStamina)
        {
            stamina.RecoverStamina(playerData.Stamina);
        }
    }
    #endregion

    #region abnormalStatusFunc
    protected override void UpdateEffectAppliedCount(AttackTypeAbnormalStatusType attackTypeAbnormalStatusType)
    {
        int type = (int)attackTypeAbnormalStatusType;
        switch (attackTypeAbnormalStatusType)
        {
            case AttackTypeAbnormalStatusType.POISON:
                effectAppliedCount[type] = AbnormalStatusConstants.ENEMY_TARGET_POISON_INFO.EFFECT_APPLIED_COUNT_MAX;
                break;
            case AttackTypeAbnormalStatusType.BURN:
                effectAppliedCount[type] = AbnormalStatusConstants.ENEMY_TARGET_BURN_INFO.EFFECT_APPLIED_COUNT_MAX;
                break;
            default:
                break;
        }
    }

    protected override bool IsControlTypeAbnormal()
    {
        return isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.STUN] || isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.FREEZE] ||
            isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.CHARM];
    }

    // 여러 상태이상, 단일 상태이상 중첩 시 공격, 이동 제한을 한 곳에서 관리하기 위해서
    /// <summary> 이동 방해 상태 이상 갯수 증가 및 이동 AI OFF Check </summary>
    protected override void AddRetrictsMovingCount()
    {
        restrictMovingCount += 1;
        if (1 <= restrictMovingCount)
        {
            isActiveMove = false;
        }
    }
    /// <summary> 이동 방해 상태 이상 갯수 감소 및 이동 AI ON Check </summary>
    protected override void SubRetrictsMovingCount()
    {
        restrictMovingCount -= 1;
        if (0 >= restrictMovingCount)
        {
            restrictMovingCount = 0;
            isActiveMove = true;
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 증가 및 공격 AI OFF Check </summary>
    protected override void AddRetrictsAttackingCount()
    {
        restrictAttackingCount += 1;
        if (1 >= restrictAttackingCount)
        {
            isActiveAttack = false;
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 감소 및 공격 AI ON Check </summary>
    protected override void SubRetrictsAttackingCount()
    {
        restrictAttackingCount -= 1;
        if (0 <= restrictAttackingCount)
        {
            isActiveAttack = true;
        }
    }

    #endregion

    #region coroutine
    protected override IEnumerator PoisonCoroutine()
    {
        int type = (int)AttackTypeAbnormalStatusType.POISON;
        isAttackTypeAbnormalStatuses[type] = true;
        abnormalComponents.PoisonEffect.SetActive(true);
        effectAppliedCount[type] = AbnormalStatusConstants.PLAYER_TARGET_POISON_INFO.EFFECT_APPLIED_COUNT_MAX;
        while (effectAppliedCount[type] > 0)
        {
            if (abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            {
                effectAppliedCount[type] = 0;
                break;
            }
            effectAppliedCount[type] -= 1;
            ColorChange(POISON_COLOR);
            ReduceHp(poisonDamagePerUnit);
            yield return YieldInstructionCache.WaitForSeconds(AbnormalStatusConstants.PLAYER_TARGET_POISON_INFO.TIME_PER_APPLIED_UNIT);
        }

        ColorChange(baseColor);
        StopAttackTypeAbnormalStatus(AttackTypeAbnormalStatusType.POISON);
    }

    protected override IEnumerator BurnCoroutine()
    {
        int type = (int)AttackTypeAbnormalStatusType.BURN;
        isAttackTypeAbnormalStatuses[type] = true;
        abnormalComponents.BurnEffect.SetActive(true);
        effectAppliedCount[type] = AbnormalStatusConstants.PLAYER_TARGET_BURN_INFO.EFFECT_APPLIED_COUNT_MAX;
        while (effectAppliedCount[type] > 0)
        {
            if (abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            {
                effectAppliedCount[type] = 0;
                break;
            }
            effectAppliedCount[type] -= 1;
            ColorChange(BURN_COLOR);
            ReduceHp(burnDamagePerUnit);
            yield return YieldInstructionCache.WaitForSeconds(AbnormalStatusConstants.PLAYER_TARGET_BURN_INFO.TIME_PER_APPLIED_UNIT);
        }
     
        ColorChange(baseColor);
        StopAttackTypeAbnormalStatus(AttackTypeAbnormalStatusType.BURN);
    }

    protected override IEnumerator FreezeCoroutine(float effectiveTime)
    {
        int type = (int)ControlTypeAbnormalStatusType.FREEZE;
        animationHandler.Idle();
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        abnormalComponents.FreezeEffect.SetActive(true);
        isControlTypeAbnormalStatuses[type] = true;
        controlTypeAbnormalStatusTime[type] = 0;
        controlTypeAbnormalStatusesDurationMax[type] = effectiveTime;

        while (controlTypeAbnormalStatusTime[type] <= controlTypeAbnormalStatusesDurationMax[type])
        {
            if (abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            {
                controlTypeAbnormalStatusesDurationMax[type] = 0;
                break;
            }
            ColorChange(FREEZE_COLOR);

            controlTypeAbnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        ColorChange(baseColor);
        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.FREEZE);

    }

    protected override IEnumerator StunCoroutine(float effectiveTime)
    {
        int type = (int)ControlTypeAbnormalStatusType.STUN;
        abnormalComponents.StunEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        isControlTypeAbnormalStatuses[type] = true;
        controlTypeAbnormalStatusTime[type] = 0;
        controlTypeAbnormalStatusesDurationMax[type] = effectiveTime;
        while (controlTypeAbnormalStatusTime[type] <= controlTypeAbnormalStatusesDurationMax[type])
        {
            if (abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            {
                controlTypeAbnormalStatusesDurationMax[type] = 0;
                break;
            }
            controlTypeAbnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.STUN);
    }

    protected override IEnumerator CharmCoroutine(float effectiveTime)
    {
        int type = (int)ControlTypeAbnormalStatusType.CHARM;
        abnormalComponents.CharmEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        isControlTypeAbnormalStatuses[type] = true;
        controlTypeAbnormalStatusTime[type] = 0;
        controlTypeAbnormalStatusesDurationMax[type] = effectiveTime;

        while (controlTypeAbnormalStatusTime[type] <= controlTypeAbnormalStatusesDurationMax[type])
        {
            if (abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            {
                controlTypeAbnormalStatusesDurationMax[type] = 0;
                break;
            }
            controlTypeAbnormalStatusTime[type] += Time.fixedDeltaTime;
            bodyTransform.Translate(moveSpeed * AbnormalStatusConstants.CHARM_MOVE_SPEED_RATE * (PlayerManager.Instance.GetPlayerPosition() - bodyTransform.position).normalized * Time.fixedDeltaTime);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.CHARM);
    }

    private IEnumerator Roll(Vector3 dir)
    {
        animationHandler.Skill(0);
        weaponManager.HideWeapon();
        totalSpeed = playerData.MoveSpeed + floorSpeed;

        float doubling = 2f;
        float animTime = .63f;
         while (animTime > 0)
        {
            animTime -= Time.fixedDeltaTime;

            yield return YieldInstructionCache.WaitForFixedUpdate;

            doubling -= Time.fixedDeltaTime * 3;
            if (doubling <= 1f)
                doubling = 1f;
            rgbody.MovePosition(objTransform.position + dir * (totalSpeed) * Time.fixedDeltaTime * doubling);
        }
        animationHandler.Skill(-1);
        EndEvade();
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        yield return YieldInstructionCache.WaitForSeconds(evadeCoolTime);

        canEvade = true;
    }

    #endregion
}

public class PlayerController
{
    #region components
    private Joystick moveJoyStick;
    private AttackJoyStick attackJoyStick;
    private ActiveSkillButton activeSkillButton;
    #endregion

    public PlayerController(Joystick moveJoyStick, AttackJoyStick attackJoyStick,ActiveSkillButton activeSkillButton)
    {
        this.moveJoyStick = moveJoyStick;
        this.attackJoyStick = attackJoyStick;
        this.activeSkillButton = activeSkillButton;
        attackJoyStick.StartRoutine();
    }
    public Vector2 GetMoveAttackInputVector()
    {
        if(attackJoyStick.GetButtonDown())
        {
            return attackJoyStick.GetRecentNormalInputVector();
        }
        return moveJoyStick.GetRecentNormalInputVector();
    }
    #region move
    /// <summary>
    /// 조이스틱이 현재 바라보는 방향의 벡터  
    /// </summary> 
    public Vector3 GetMoveInputVector()
    {
        float h = moveJoyStick.GetHorizontalValue();
        float v = moveJoyStick.GetVerticalValue();

        return new Vector3(h, v, 0).normalized;
    }

    /// <summary>
    /// 입력한 조이스틱의 가장 최근 Input vector의 normal vector 반환 
    /// </summary>
    public Vector3 GetMoveRecentNormalInputVector()
    {
        return moveJoyStick.GetRecentNormalInputVector();
    }
    #endregion
    #region attack
    /// <summary>
    /// 조이스틱이 현재 바라보는 방향의 벡터  
    /// </summary> 
    public Vector3 GetAttackInputVector()
    {
        float h = attackJoyStick.GetHorizontalValue();
        float v = attackJoyStick.GetVerticalValue();

        return new Vector3(h, v, 0).normalized; 
    }

    /// <summary>
    /// 입력한 조이스틱의 가장 최근 Input vector의 normal vector 반환 
    /// </summary>
    public Vector3 GetAttackRecentNormalInputVector()
    {
        return attackJoyStick.GetRecentNormalInputVector();
    }

    public void AttackJoyStickUp()
    {
        attackJoyStick.OnPointerUp(null);
    }
    #endregion
    #region skill
    public void ActiveSkill(float sum, float total)
    {
        if (!PlayerManager.Instance.GetPlayer().GetIsAcitveAttack())
            return;
        activeSkillButton.ChargeFill(sum / total);
    }
    #endregion
}