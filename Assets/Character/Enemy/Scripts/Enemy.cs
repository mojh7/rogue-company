using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 기존의 상태이상이 적용되고 있을 때 추가 상태이상 적용시
/// 기존 것이 없어지는 종류의 상태 이상들
/// </summary>
public enum AbnormalStatusType {FREEZE, STUN, CHARM, END }

public class Enemy : Character
{
    #region variables
    float bodyblowTime = 0;
    EnemyData enemyData;
    protected bool isBossEnemy;  // 0810 모, 보스 몬스터, 일반 몬스터 구분을 위해 사용
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
        isAbnormalStatuses = new bool[(int)AbnormalStatusType.END];
        abnormalStatusCounts = new int[(int)AbnormalStatusType.END];
        overlappingCounts = new int[(int)AbnormalStatusType.END];
        abnormalStatusCoroutines = new Coroutine[(int)AbnormalStatusType.END];
        abnormalStatusTime = new float[(int)AbnormalStatusType.END];
        abnormalStatusDurationTime = new float[(int)AbnormalStatusType.END];
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
    //0603 이유성 적 데이터로 적만들기 (애니메이션 아직 보류)
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
        ownerType = CharacterInfo.OwnerType.Enemy;
        damageImmune = CharacterInfo.DamageImmune.NONE;
        abnormalImmune = CharacterInfo.AbnormalImmune.NONE;

        originalautoAimType = CharacterInfo.AutoAimType.AUTO;
        autoAimType = originalautoAimType;

        isActiveAttack = true;
        isActiveMove = true;

        DeactivateAbnormalComponents();

        isBossEnemy = false;
        hp = enemyData.HP;
        maxHP = hp;
        moveSpeed = enemyData.Speed;
        scaleVector = Vector3.one * enemyData.Size;
        sprite = enemyData.Sprite;
        buffManager.Init();
        buffManager.SetOwner(this);
        Components.CircleCollider2D.enabled = true;
        enemyLayer = UtilityClass.GetEnemyLayer(this);
        // weaponManager.init이 buff init 보다 뒤에 와야됨.
        weaponManager.Init(this, enemyData);
        animationHandler.Init(this, enemyData.AnimatorController);
        aiController.Init(moveSpeed, animationHandler, weaponManager, enemyData.Task, enemyData.SkillDatas);
        InitStatusEffects();
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
        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if (null != knockBackCheck)
            StopCoroutine(KnockBackCheck());
        if (null != poisonCoroutine)
            StopCoroutine(poisonCoroutine);
        if (null != burnCoroutine)
            StopCoroutine(burnCoroutine);
        if (null != delayStateCoroutine)
            StopCoroutine(delayStateCoroutine);

        for (int i = 0; i < (int)AbnormalStatusType.END; i++)
        {
            if (null != abnormalStatusCoroutines[i])
                StopCoroutine(abnormalStatusCoroutines[i]);
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

    // 0813 모, 체력이 깎이는 다양한 경우에서 hp감소를 공통된 방식으로 처리하기 위해 함수화하여 처리
    private void ReduceHp(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            Die();
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
            damage *= BulletConstants.CRITICAL_DAMAGE + transferredInfo.criticalDamageIncrement;
            //Debug.Log("crt dmg inc : " + (BulletConstants.CRITICAL_DAMAGE + transferredInfo.criticalDamageIncrement));
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
            damage *= 2f;
        }

        if (knockBack > 0)
            KnockBack(knockBack, _dir, bulletPos, positionBasedKnockBack);

        ReduceHp(damage);
        AttackedEffect();
        return damage;
    }

    public override void ApplyItemEffect()
    {
        // 개발 중
    }

    protected override bool IsAbnormal()
    {
        return isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE] ||
            isAbnormalStatuses[(int)AbnormalStatusType.CHARM];
    }

    // TODO : 0802 모장현, enemy aim 조절 타입에 따라서 알고리즘 변경

    // 0526 땜빵
    public override void SetAim()
    {
        switch (autoAimType)
        {
            case CharacterInfo.AutoAimType.AUTO:
                directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - bodyTransform.position).normalized;
                directionDegree = directionVector.GetDegFromVector();
                break;
            case CharacterInfo.AutoAimType.REACTANCE:
                directionVector = (bodyTransform.position - PlayerManager.Instance.GetPlayer().GetPosition()).normalized;
                directionDegree = directionVector.GetDegFromVector();
                break;
            case CharacterInfo.AutoAimType.SEMIAUTO:
                break;
            case CharacterInfo.AutoAimType.MANUAL:
                directionVector = rgbody.velocity;
                directionDegree = directionVector.GetDegFromVector();
                break;
            default:
                break;
        }
    }

    public override void SelfDestruction()
    {
        Attacked(Vector2.zero, bodyTransform.position, maxHP * 0.5f, 0);
    }
    #endregion

    #region abnormalStatus
    protected override void StopAbnormalStatus(AbnormalStatusType abnormalStatusType)
    {
        int type = (int)abnormalStatusType;
        if (false == isAbnormalStatuses[type])
            return;
        isAbnormalStatuses[type] = false;
        if (null != abnormalStatusCoroutines[type])
            StopCoroutine(abnormalStatusCoroutines[type]);
        abnormalStatusCoroutines[type] = null;
        switch (abnormalStatusType)
        {
            case AbnormalStatusType.FREEZE:
                abnormalComponents.FreezeEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case AbnormalStatusType.STUN:
                abnormalComponents.StunEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case AbnormalStatusType.CHARM:
                abnormalComponents.CharmEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
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
            //Debug.Log(name + ", 이동 off, " + restrictMovingCount);
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
            //Debug.Log(name + ", 이동 on, " + restrictMovingCount);
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
        isPoisoning = true;
        abnormalComponents.PoisonEffect.SetActive(true);
        float damage = 0;
        while (poisonOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);
            ColorChange(poisonColor);
            if (1 == poisonOverlappingCount)
                damage = maxHP * StatusConstants.Instance.PoisonInfo.value;
            else
                damage = maxHP * StatusConstants.Instance.PoisonInfo.value * 1.5f;
            damage += 0.1f * (poisonOverlappingCount - 1);
            damage *= StatusConstants.Instance.GraduallyDamagePerUnit;
            hp -= damage;
            if(isBossEnemy)
            {
                UIManager.Instance.bossHPUI.DecreaseHp(damage);
            }
            for (int i = 0; i < StatusConstants.Instance.PoisonInfo.overlapCountMax; i++)
            {
                if (poisonCount[i] > 0)
                {
                    poisonCount[i] -= 1;
                    if (0 == poisonCount[i])
                    {
                        poisonOverlappingCount -= 1;
                    }
                }
            }
        }
        abnormalComponents.PoisonEffect.SetActive(false);
        isPoisoning = false;
        ColorChange(baseColor);
    }

    protected override IEnumerator BurnCoroutine()
    {
        isBurning = true;
        abnormalComponents.BurnEffect.SetActive(true);
        float damage = 0;
        while (burnOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);
            ColorChange(burnColor);

            if (1 == burnOverlappingCount)
                damage = maxHP * StatusConstants.Instance.BurnInfo.value;
            else
                damage = maxHP * StatusConstants.Instance.BurnInfo.value * 1.5f;
            damage += 0.1f * (burnOverlappingCount - 1);
            damage *= StatusConstants.Instance.GraduallyDamagePerUnit;
            hp -= damage;
            if (isBossEnemy)
            {
                UIManager.Instance.bossHPUI.DecreaseHp(damage);
            }
            for (int i = 0; i < StatusConstants.Instance.BurnInfo.overlapCountMax; i++)
            {
                if (burnCount[i] > 0)
                {
                    burnCount[i] -= 1;
                    if (0 == burnCount[i])
                    {
                        burnOverlappingCount -= 1;
                    }
                }
            }
        }
        abnormalComponents.BurnEffect.SetActive(false);
        isBurning = false;
        ColorChange(baseColor);
    }

    protected override IEnumerator FreezeCoroutine()
    {
        int type = (int)AbnormalStatusType.FREEZE;
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        abnormalComponents.FreezeEffect.SetActive(true);
        isAbnormalStatuses[type] = true;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = StatusConstants.Instance.FreezeInfo.effectiveTime;

        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            ColorChange(freezeColor);

            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.FREEZE);
        ColorChange(baseColor);
    }

    protected override IEnumerator StunCoroutine(float effectiveTime)
    {
        int type = (int)AbnormalStatusType.STUN;
        abnormalComponents.StunEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        isAbnormalStatuses[type] = true;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.STUN);
    }

    protected override IEnumerator CharmCoroutine(float effectiveTime)
    {
        int type = (int)AbnormalStatusType.CHARM;
        abnormalComponents.CharmEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        isAbnormalStatuses[type] = true;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;

        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            bodyTransform.Translate(moveSpeed * 0.5f * (PlayerManager.Instance.GetPlayerPosition() - bodyTransform.position).normalized * Time.fixedDeltaTime);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.CHARM);
    }

    protected override IEnumerator KnockBackCheck()
    {
        while (true)
        {
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 1f)
            {
                SubRetrictsMovingCount();
                knockBackCheck = null;
                break;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    #endregion
}

public class BossEnemy : Enemy
{
    public override void Init(EnemyData enemyData)
    {
        base.Init(enemyData);
        abnormalImmune = CharacterInfo.AbnormalImmune.ALL;
        isBossEnemy = true;
    }
    public override float Attacked(TransferBulletInfo transferBulletInfo)
    {
        float damage = base.Attacked(transferBulletInfo);
        UIManager.Instance.bossHPUI.DecreaseHp(damage);
        AttackedEffect();
        return damage;
    }

    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        base.Attacked(_dir, bulletPos, damage, knockBack, criticalChance, positionBasedKnockBack);
        UIManager.Instance.bossHPUI.DecreaseHp(damage);
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
}
