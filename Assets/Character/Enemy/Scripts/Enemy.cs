using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Character
{
    #region variables
    float bodyblowTime = 0;
    EnemyData enemyData;
    public bool tutorial;
    public int price
    {
        private set;
        get;
    }
    #endregion

    #region setter
    public void SetTutorial(bool tutorial)
    {
        this.tutorial = tutorial;
    }
    #endregion

    #region getter
    public CircleCollider2D GetCircleCollider2D()
    {
        return Components.CircleCollider2D;
    }
    public BoxCollider2D GetHitBox()
    {
        return Components.HitBox;
    }
    public EnemyData[] GetServants()
    {
        return enemyData.ServantDatas;
    }
    #endregion

    #region UnityFunc
    private void Awake()
    {
        int attackTypeAbnormalStatusTypeLength = (int)AttackTypeAbnormalStatusType.END;
        isAttackTypeAbnormalStatuses = new bool[attackTypeAbnormalStatusTypeLength];
        effectAppliedCount = new int[attackTypeAbnormalStatusTypeLength];
        attackTypeAbnormalStatusCoroutines = new Coroutine[attackTypeAbnormalStatusTypeLength];

        int controlTypeAbnormalStatusTypeLength = (int)ControlTypeAbnormalStatusType.END;
        isControlTypeAbnormalStatuses = new bool[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusCoroutines = new Coroutine[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusTime = new float[controlTypeAbnormalStatusTypeLength];
        controlTypeAbnormalStatusesDurationMax = new float[controlTypeAbnormalStatusTypeLength];
        servants = new List<Character>();
    }

    private void Update()
    {
        bodyblowTime -= Time.deltaTime;
        SetAim();
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(bodyTransform.position.y * 100);
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = -Mathf.Abs(scaleVector.x);
            bodyTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = Mathf.Abs(scaleVector.x);
            bodyTransform.localScale = scaleVector;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (bodyblowTime >= 0)
            return;
        if(UtilityClass.CheckLayer(collision.gameObject.layer,enemyLayer))
        {
            bodyblowTime = 3;
            collision.GetComponent<Character>().Attacked((collision.transform.position - this.bodyTransform.position).normalized, this.bodyTransform.position, 1, 1);
        }
    }
    #endregion

    #region initalization
    // 0603 이유성 적 데이터로 적만들기 (애니메이션 아직 보류)
    // 독, 화상 데미지 표시를 위해 보스 몬스터와 일반 몬스터 구분위해 init virtual 함. 0810 모
    public virtual void Init(EnemyData enemyData)
    {
        base.Init();
        shadowTransform.localPosition = Vector3.zero;
        this.enemyData = enemyData;
        baseColor = enemyData.Color;
        spriteRenderer.color = baseColor;
        this.price = enemyData.Price;
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.ENEMY;
        damageImmune = CharacterInfo.DamageImmune.NONE;
        abnormalImmune = CharacterInfo.AbnormalImmune.NONE;

        aimType = CharacterInfo.AimType.AUTO;

        isActiveAttack = true;
        isActiveMove = true;

        DeactivateAbnormalComponents();

        hp = enemyData.HP;
        hpMax = hp;
        poisonDamagePerUnit = AbnormalStatusConstants.ENEMY_TARGET_POISON_INFO.FIXED_DAMAGE_PER_UNIT + 
            hpMax * AbnormalStatusConstants.ENEMY_TARGET_POISON_INFO.PERCENT_DAMAGE_PER_UNIT;
        burnDamagePerUnit = AbnormalStatusConstants.ENEMY_TARGET_BURN_INFO.FIXED_DAMAGE_PER_UNIT +
            hpMax * AbnormalStatusConstants.ENEMY_TARGET_BURN_INFO.PERCENT_DAMAGE_PER_UNIT;

        moveSpeed = enemyData.Speed;
        scaleVector = Vector3.one * enemyData.Size;
        sprite = enemyData.Sprite;
        buffManager.Init();
        buffManager.SetOwner(this);
        Components.CircleCollider2D.enabled = true;
        enemyLayer = UtilityClass.GetEnemyLayer(this);
        weaponManager.Init(this, enemyData);
        animationHandler.Init(this, enemyData.AnimatorController);
        aiController.Init(moveSpeed, PlayerManager.Instance.GetPlayer(), animationHandler, weaponManager, enemyData.Task, enemyData.SkillDatas);
        InitStatusEffects();

        if(enemyData.StaticType)
        {
            rgbody.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rgbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
    #endregion

    #region Func

    public override bool Evade()
    {
        return false;
    }

    protected override void Die()
    {
        if (tutorial)
        {
            TutorialManager.Instance.StartShake(2, 2, 1);
            TutorialManager.Instance.SetPortal();
            gameObject.SetActive(false);
            return;
        }
        pState = CharacterInfo.State.DIE;
        if (null != checkingknockBackEnded)
            StopCoroutine(CheckDashEnded());
        // 실행 중인 상태이상 관련 코루틴이 있으면 코루틴 멈춤
        for (int i = 0; i < (int)AttackTypeAbnormalStatusType.END; i++)
        {
            if (null != attackTypeAbnormalStatusCoroutines[i])
                StopCoroutine(attackTypeAbnormalStatusCoroutines[i]);
        }
        for (int i = 0; i < (int)ControlTypeAbnormalStatusType.END; i++)
        {
            if (null != controlTypeAbnormalStatusCoroutines[i])
                StopCoroutine(controlTypeAbnormalStatusCoroutines[i]);
        }

        if (spawnType == CharacterInfo.SpawnType.NORMAL)
        {
            PlayerManager.Instance.GetPlayer().AddKilledEnemyCount();
            Stamina.Instance.RecoverStamina();
            GameDataManager.Instance.SetKill();
            EnemyManager.Instance.DeleteEnemy(this);
            RoomManager.Instance.DieMonster();
        }
        weaponManager.RemoveAllWeapons();
        DropItem();
        ParticleManager.Instance.PlayParticle("Pixel", spriteTransform.position);
        gameObject.SetActive(false);

        Destroy(this);
    }

    protected void DropItem()
    {
        int count = EconomySystem.Instance.DropCoin(this.price);
        for(int i=0;i< count;i++)
        {
            ItemManager.Instance.CreateItem(ItemManager.Instance.DropCoin(), bodyTransform.position, new Vector2(Random.Range(-1f, 1f), Random.Range(3, 8)));
        }
    }

    public void Heal(float hp)
    {
        RecoveryHp(hp);
    }

    /// <summary>총알에서 전달된 정보로 공격 처리</summary>
    public override float Attacked(TransferBulletInfo transferredInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        float damage = transferredInfo.damage;
        // 크리티컬 공격
        if (criticalCheck < transferredInfo.criticalChance)
        {
            damage *= Bullet.CRITICAL_DAMAGE + transferredInfo.criticalDamageIncrement;
        }

        ReduceHp(damage);
        AttackedEffect();
        return damage;
    }

    /// <summary> 각종 오브젝트들에 의한 공격 및 넉백 </summary>
    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        // 크리티컬 공격
        if (criticalCheck < criticalChance)
        {
            damage *= Bullet.CRITICAL_DAMAGE;
        }

        if (knockBack > 0)
            KnockBack(knockBack, _dir, bulletPos, positionBasedKnockBack);

        ReduceHp(damage);
        AttackedEffect();
        return damage;
    }

    protected override bool IsControlTypeAbnormal()
    {
        return isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.STUN] || isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.FREEZE] ||
            isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.CHARM];
    }

    // TODO : 0802 모장현, enemy aim 조절 타입에 따라서 알고리즘 변경

    // 0526 땜빵
    public override void SetAim()
    {
        switch (aimType)
        {
            case CharacterInfo.AimType.AUTO:
                directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - bodyTransform.position).normalized;
                directionDegree = directionVector.GetDegFromVector();
                break;
            //case CharacterInfo.aimType.REACTANCE:
            //    directionVector = (bodyTransform.position - PlayerManager.Instance.GetPlayer().GetPosition()).normalized;
            //    directionDegree = directionVector.GetDegFromVector();
            //    break;
            case CharacterInfo.AimType.SEMIAUTO:
                break;
            case CharacterInfo.AimType.MANUAL:
                directionVector = rgbody.velocity;
                directionDegree = directionVector.GetDegFromVector();
                break;
            default:
                break;
        }
    }

    public override void SelfDestruction()
    {
        Attacked(Vector2.zero, bodyTransform.position, hpMax * 0.5f, 0);
    }
    #endregion

    #region itemEffect
    public override void ApplyItemEffect()
    {
        CharacterTargetEffect itemUseEffect = buffManager.CharacterTargetEffectTotal;
        //playerData.StaminaMax = Mathf.RoundToInt(originPlayerData.StaminaMax * itemUseEffect.staminaMaxRatio);
        //if (playerData.MoveSpeed != originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
        //{
        //    if (playerData.MoveSpeed > originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
        //    {
        //        ParticleManager.Instance.PlayParticle("SpeedDown", this.bodyTransform.position);
        //    }
        //    else
        //    {
        //        ParticleManager.Instance.PlayParticle("SpeedUp", this.bodyTransform.position);
        //    }
        //}
        //playerData.MoveSpeed = originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement;
        //IsNotConsumeStamina = itemUseEffect.isNotConsumeStamina;
        //IsNotConsumeAmmo = itemUseEffect.isNotConsumeAmmo;
        //damageImmune = itemUseEffect.isImmuneDamage;
        //abnormalImmune = itemUseEffect.isImmuneAbnormal;

        //if (itemUseEffect.hpMaxRatio > 0)
        //    hpMax = originPlayerData.HpMax * itemUseEffect.hpMaxRatio;
        //if (itemUseEffect.skillGage > 0)
        //    skillGageMultiple = itemUseEffect.skillGage;

        //if (itemUseEffect.charScale > 0 && itemUseEffect.charScale <= 2f)
        //    ScaleChange(itemUseEffect.charScale);

        //stamina.SetStaminaMax(playerData.StaminaMax);
        //stamina.RecoverStamina(0);
        //playerHPUi.SetHpMax(hpMax);
        //playerHPUi.ChangeHp(hp);
    }
    public override void ApplyConsumableItem(CharacterTargetEffect itemUseEffect)
    {
        if (0 != itemUseEffect.recoveryHp)
        {
            RecoveryHp(itemUseEffect.recoveryHp);
        }
        //if (0 != itemUseEffect.recoveryStamina)
        //{

        //}
    }

    #endregion

    #region abnormalStatusFunc

    /*
    protected override void StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType controlTypeAbnormalStatusType)
    {
        int type = (int)controlTypeAbnormalStatusType;
        if (false == isControlTypeAbnormalStatuses[type])
            return;
        isControlTypeAbnormalStatuses[type] = false;

        if (null != controlTypeAbnormalStatusCoroutines[type])
            StopCoroutine(controlTypeAbnormalStatusCoroutines[type]);
        controlTypeAbnormalStatusCoroutines[type] = null;

        switch (controlTypeAbnormalStatusType)
        {
            case ControlTypeAbnormalStatusType.FREEZE:
                abnormalComponents.FreezeEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case ControlTypeAbnormalStatusType.STUN:
                abnormalComponents.StunEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case ControlTypeAbnormalStatusType.CHARM:
                abnormalComponents.CharmEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            default:
                break;
        }
    }*/

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

    // 여러 상태이상, 단일 상태이상 중첩 시 공격, 이동 제한을 한 곳에서 관리하기 위해서
    /// <summary> 이동 방해 상태 이상 갯수 증가 및 이동 AI OFF Check </summary>
    protected override void AddRetrictsMovingCount()
    {
        restrictMovingCount += 1;
        if (1 <= restrictMovingCount)
        {
            isActiveMove = false;
            aiController.StopMove();
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
            aiController.PlayMove();
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 증가 및 공격 AI OFF Check </summary>
    protected override void AddRetrictsAttackingCount()
    {
        restrictAttackingCount += 1;
        if (1 == restrictAttackingCount)
        {
            isActiveAttack = false;
            aiController.StopAttack();
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 감소 및 공격 AI ON Check </summary>
    protected override void SubRetrictsAttackingCount()
    {
        restrictAttackingCount -= 1;
        if (0 == restrictAttackingCount)
        {
            isActiveAttack = true;
            aiController.PlayAttack();
        }
    }
    #endregion

    #region coroutine
    protected override IEnumerator PoisonCoroutine()
    {
        int type = (int)AttackTypeAbnormalStatusType.POISON;
        isAttackTypeAbnormalStatuses[type] = true;
        abnormalComponents.PoisonEffect.SetActive(true);
        effectAppliedCount[type] = AbnormalStatusConstants.ENEMY_TARGET_POISON_INFO.EFFECT_APPLIED_COUNT_MAX;
        while (effectAppliedCount[type] > 0)
        {
            effectAppliedCount[type] -= 1;
            ColorChange(POISON_COLOR);
            ReduceHp(poisonDamagePerUnit);
            yield return YieldInstructionCache.WaitForSeconds(AbnormalStatusConstants.ENEMY_TARGET_POISON_INFO.TIME_PER_APPLIED_UNIT);
        }
        
        ColorChange(baseColor);
        StopAttackTypeAbnormalStatus(AttackTypeAbnormalStatusType.POISON);
    }

    protected override IEnumerator BurnCoroutine()
    {
        int type = (int)AttackTypeAbnormalStatusType.BURN;
        isAttackTypeAbnormalStatuses[type] = true;
        abnormalComponents.BurnEffect.SetActive(true);

        effectAppliedCount[type] = AbnormalStatusConstants.ENEMY_TARGET_BURN_INFO.EFFECT_APPLIED_COUNT_MAX;
        while (effectAppliedCount[type] > 0)
        {
            effectAppliedCount[type] -= 1;
            ColorChange(BURN_COLOR);
            ReduceHp(burnDamagePerUnit);
            yield return YieldInstructionCache.WaitForSeconds(AbnormalStatusConstants.ENEMY_TARGET_BURN_INFO.TIME_PER_APPLIED_UNIT);
        }

        ColorChange(baseColor);
        StopAttackTypeAbnormalStatus(AttackTypeAbnormalStatusType.BURN);
    }

    protected override IEnumerator FreezeCoroutine(float effectiveTime)
    {
        int type = (int)ControlTypeAbnormalStatusType.FREEZE;
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        abnormalComponents.FreezeEffect.SetActive(true);
        isControlTypeAbnormalStatuses[type] = true;
        controlTypeAbnormalStatusTime[type] = 0;
        controlTypeAbnormalStatusesDurationMax[type] = effectiveTime;

        while (controlTypeAbnormalStatusTime[type] <= controlTypeAbnormalStatusesDurationMax[type])
        {
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
            controlTypeAbnormalStatusTime[type] += Time.fixedDeltaTime;
            bodyTransform.Translate(moveSpeed * AbnormalStatusConstants.CHARM_MOVE_SPEED_RATE * (PlayerManager.Instance.GetPlayerPosition() - bodyTransform.position).normalized * Time.fixedDeltaTime);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.CHARM);
    }
    #endregion
}

public class BossEnemy : Enemy
{
    public override void Init(EnemyData enemyData)
    {
        base.Init(enemyData);
        abnormalImmune = CharacterInfo.AbnormalImmune.ALL;
        UIManager.Instance.bossHPUI.Notify();
    }
    public override float Attacked(TransferBulletInfo transferBulletInfo)
    {
        float damage = base.Attacked(transferBulletInfo);
        AttackedEffect();
        return damage;
    }
    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        base.Attacked(_dir, bulletPos, damage, knockBack, criticalChance, positionBasedKnockBack);
        AttackedEffect();
        return damage;
    }

    protected override void Die()
    {
        DeleteServant();
        ParticleManager.Instance.PlayParticle("Pixel", spriteTransform.position);

        pState = CharacterInfo.State.DIE;
        GameDataManager.Instance.SetKill();
        UIManager.Instance.bossHPUI.Toggle();
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        gameObject.SetActive(false);
        DropItem();
        Destroy(this);
    }

    public override void ApplyConsumableItem(CharacterTargetEffect itemUseEffect)
    {
        base.ApplyConsumableItem(itemUseEffect);
        UIManager.Instance.bossHPUI.Notify();
    }
    public override void ApplyItemEffect()
    {
        base.ApplyItemEffect();
        UIManager.Instance.bossHPUI.Notify();
    }

    protected override void RecoveryHp(float amount)
    {
        base.RecoveryHp(amount);
        UIManager.Instance.bossHPUI.Notify();
    }

    protected override void ReduceHp(float amount)
    {
        base.ReduceHp(amount);
        UIManager.Instance.bossHPUI.Notify();
    }
}
