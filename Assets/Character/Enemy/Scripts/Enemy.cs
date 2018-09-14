using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// 기존의 상태이상이 적용되고 있을 때 추가 상태이상 적용시
/// 기존 것이 없어지는 종류의 상태 이상들
/// </summary>
public enum AbnormalStatusType { NAG, CLIMBING, GRAVEYARDSHIFT, FREEZE, REACTANCE, STUN, CHARM, END }

public class Enemy : Character
{
    #region variables
    EnemyData enemyData;
    // temp Hp Max 나중에 EnemyData로 옮겨야 될듯? 아니면 그대로 hpMax여기서 쓰던가
    private float hpMax;
    protected bool isBossEnemy;  // 0810 모, 보스 몬스터, 일반 몬스터 구분을 위해 사용
    
    #endregion

    #region abnormalStatusVariables
    private int restrictMovingCount;
    private int restrictAttackingCount;

    private bool isPoisoning;
    private int poisonOverlappingCount;
    private int[] poisonCount;
    private bool isBurning;
    private int burnOverlappingCount;
    private int[] burnCount;
    private bool isDelayingState;
    private int delayStateOverlappingCount;
    private int delayStateCount;

    private float[] climbingTime;
    private bool[] isAbnormalStatuses;
    private int[] abnormalStatusCounts;
    private int[] overlappingCounts;
    private float[] abnormalStatusTime;
    private float[] abnormalStatusDurationTime;
    private Coroutine[] abnormalStatusCoroutines;

    private Coroutine knockBackCheck;
    private Coroutine poisonCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine delayStateCoroutine;

    #endregion

    #region setter
    #endregion

    #region getter
    public CircleCollider2D GetCircleCollider2D()
    {
        return Components.CircleCollider2D;
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
        SetAim();
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = -Mathf.Abs(scaleVector.x);
            transform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = Mathf.Abs(scaleVector.x);
            transform.localScale = scaleVector;
        }
    }
    #endregion

    #region initalization
    //0603 이유성 적 데이터로 적만들기 (애니메이션 아직 보류)
    // 독, 화상 데미지 표시를 위해 보스 몬스터와 일반 몬스터 구분위해 init virtual 함. 0810 모
    public virtual void Init(EnemyData enemyData)
    {
        base.Init();
        this.enemyData = enemyData;
        spriteRenderer.color = Color.white;

        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.Enemy;
        damageImmune = CharacterInfo.DamageImmune.NONE;

        originalautoAimType = CharacterInfo.AutoAimType.AUTO;
        autoAimType = originalautoAimType;

        isActiveAttackAI = true;
        isActiveMoveAI = true;

        abnormalComponents.BurnEffect.SetActive(false);
        abnormalComponents.PoisonEffect.SetActive(false);

        abnormalComponents.NagEffect.SetActive(false);
        abnormalComponents.ClibmingEffect.SetActive(false);
        abnormalComponents.GraveyardShiftEffect.SetActive(false);
        abnormalComponents.FreezeEffect.SetActive(false);
        abnormalComponents.ReactanceEffect.SetActive(false);

        abnormalComponents.StunEffect.SetActive(false);
        abnormalComponents.CharmEffect.SetActive(false);

        isBossEnemy = false;
        hpMax = enemyData.HP;
        hp = enemyData.HP;
        maxHP = hp;
        moveSpeed = enemyData.Speed;
        scaleVector = Vector3.one * enemyData.Size;
        sprite = enemyData.Sprite;
        buffManager.Init();
        buffManager.SetOwner(this);
        Components.CircleCollider2D.enabled = true;

        // weaponManager.init이 buff init 보다 뒤에 와야됨.
        weaponManager.Init(this, enemyData);
        animationHandler.Init(this, enemyData.AnimatorController);
        aiController.Init(moveSpeed, animationHandler, weaponManager, enemyData.Task, enemyData.SkillDatas);
        InitStatusEffects();
    }
    /// <summary> 상태 이상 효과 관련 초기화 </summary>
    private void InitStatusEffects()
    {
        isPoisoning = false;
        poisonOverlappingCount = 0;
        poisonCount = new int[StatusConstants.Instance.PoisonInfo.overlapCountMax];
        isBurning = false;
        burnOverlappingCount = 0;
        burnCount = new int[StatusConstants.Instance.BurnInfo.overlapCountMax];
        isDelayingState = false;
        delayStateCount = 0;

        climbingTime = new float[StatusConstants.Instance.ClimbingInfo.overlapCountMax];

        restrictMovingCount = 0;
        restrictAttackingCount = 0;
        for (int i = 0; i < (int)AbnormalStatusType.END; i++)
        {
            isAbnormalStatuses[i] = false;
            abnormalStatusCounts[i] = 0;
            overlappingCounts[i] = 0;
            abnormalStatusCoroutines[i] = null;
            abnormalStatusTime[i] = 0;
            abnormalStatusDurationTime[i] = 0;
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
        ParticleManager.Instance.PlayParticle("Pixel", spriteTransform.position, sprite);
        gameObject.SetActive(false);
        Destroy(this);
    }

    protected void DropItem()
    {
        GameObject coin = new GameObject();
        coin.AddComponent<SpriteRenderer>().sprite = ItemManager.Instance.coinSprite;
        coin.AddComponent<Coin>();
        ItemManager.Instance.CreateItem(coin.GetComponent<Coin>(), transform.position);
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
            damage *= 2f;
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
        return damage;
    }

    public override void ApplyItemEffect()
    {
        // 개발 중
    }

    protected override bool IsAbnormal()
    {
        return isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE] ||
            isAbnormalStatuses[(int)AbnormalStatusType.CHARM] || isAbnormalStatuses[(int)AbnormalStatusType.CLIMBING] ||
            isAbnormalStatuses[(int)AbnormalStatusType.NAG] || isAbnormalStatuses[(int)AbnormalStatusType.REACTANCE];
    }

    // TODO : 0802 모장현, enemy aim 조절 타입에 따라서 알고리즘 변경

    // 0526 땜빵
    public override void SetAim()
    {
        switch (autoAimType)
        {
            case CharacterInfo.AutoAimType.AUTO:
                directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - transform.position).normalized;
                directionDegree = directionVector.GetDegFromVector();
                break;
            case CharacterInfo.AutoAimType.REACTANCE:
                directionVector = (transform.position - PlayerManager.Instance.GetPlayer().GetPosition()).normalized;
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
    #endregion

    #region abnormalState

    // 여러 상태이상, 단일 상태이상 중첩 시 공격, 이동 제한을 한 곳에서 관리하기 위해서
    /// <summary> 이동 방해 상태 이상 갯수 증가 및 이동 AI OFF Check </summary>
    private void AddRetrictsMovingCount()
    {
        restrictMovingCount += 1;
        if (1 <= restrictMovingCount)
        {
            isActiveMoveAI = false;
            aiController.StopMove();
            //Debug.Log(name + ", 이동 off, " + restrictMovingCount);
        }
    }
    /// <summary> 이동 방해 상태 이상 갯수 감소 및 이동 AI ON Check </summary>
    private void SubRetrictsMovingCount()
    {
        restrictMovingCount -= 1;
        if (0 >= restrictMovingCount)
        {
            restrictMovingCount = 0;
            isActiveMoveAI = true;
            aiController.PlayMove();
            //Debug.Log(name + ", 이동 on, " + restrictMovingCount);
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 증가 및 공격 AI OFF Check </summary>
    private void AddRetrictsAttackingCount()
    {
        restrictAttackingCount += 1;
        if (1 == restrictAttackingCount)
        {
            isActiveAttackAI = false;
            aiController.StopAttack();
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 감소 및 공격 AI ON Check </summary>
    private void SubRetrictsAttackingCount()
    {
        restrictAttackingCount -= 1;
        if (0 == restrictAttackingCount)
        {
            isActiveAttackAI = true;
            aiController.PlayAttack();
        }
    }


    private bool AbnormalChance(float appliedChance)
    {
        float chance = Random.Range(0, 1f);
        if(chance < appliedChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return;
        if (null == statusEffectInfo) return;

        if (0 != statusEffectInfo.knockBack)
            KnockBack(statusEffectInfo.knockBack, statusEffectInfo.BulletDir, statusEffectInfo.BulletPos, statusEffectInfo.positionBasedKnockBack);

        if (isBossEnemy)
            return;

        if (true == statusEffectInfo.canPoison)
            Poison(statusEffectInfo.posionChance);
        if (true == statusEffectInfo.canBurn)
            Burn(statusEffectInfo.burnChance);
        if (true == statusEffectInfo.canDelayState)
            DelayState(statusEffectInfo.delayStateChance);

        if (true == statusEffectInfo.canNag)
            Nag(statusEffectInfo.nagChance);
        if (true == statusEffectInfo.canClimb)
            Climbing(statusEffectInfo.climbChance);
        if (true == statusEffectInfo.graveyardShift)
            GraveyardShift(statusEffectInfo.graveyardShiftChance);
        if (true == statusEffectInfo.canFreeze)
            Freeze(statusEffectInfo.freezeChance);
        if (true == statusEffectInfo.reactance)
            Reactance(statusEffectInfo.reactanceChance);

        if (0 != statusEffectInfo.stun)
            Stun(statusEffectInfo.stun, statusEffectInfo.stunChance);
        if (0 != statusEffectInfo.charm)
            Charm(statusEffectInfo.charm, statusEffectInfo.charmChance);
    }
    private void Poison(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        if (poisonOverlappingCount >= StatusConstants.Instance.PoisonInfo.overlapCountMax)
            return;
        poisonOverlappingCount += 1;
        for (int i = 0; i < StatusConstants.Instance.PoisonInfo.overlapCountMax; i++)
        {
            if (0 == poisonCount[i])
            {
                poisonCount[i] = StatusConstants.Instance.GraduallyDamageCountMax;
                break;
            }
        }
        if (false == isPoisoning)
        {
            poisonCoroutine = StartCoroutine(PoisonCoroutine());
        }
    }

    private void Burn(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        if (burnOverlappingCount >= StatusConstants.Instance.BurnInfo.overlapCountMax)
            return;
        burnOverlappingCount += 1;
        for (int i = 0; i < StatusConstants.Instance.BurnInfo.overlapCountMax; i++)
        {
            if (0 == burnCount[i])
            {
                burnCount[i] = StatusConstants.Instance.GraduallyDamageCountMax;
                break;
            }
        }
        if (false == isBurning)
        {
            burnCoroutine = StartCoroutine(BurnCoroutine());
        }
    }
    // TODO : 다른 상태이상이 걸려있는 상태에서 상태이상이 걸릴 때 우선 순위 처리 고민 및 해야됨.

    private void DelayState(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        if (delayStateCount >= StatusConstants.Instance.DelayStateInfo.overlapCountMax)
            return;
        Debug.Log(gameObject.name + " 이동지연 적용");
        delayStateCount += 1;
        delayStateOverlappingCount += 1;
        if (false == isDelayingState)
        {
            delayStateCoroutine = StartCoroutine(DelayStateCoroutine());
        }
    }

    private void Nag(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.NAG;

        if (isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE] ||
             isAbnormalStatuses[(int)AbnormalStatusType.CHARM] || isAbnormalStatuses[(int)AbnormalStatusType.CLIMBING])
            return;
        StopAbnormalStatus(AbnormalStatusType.REACTANCE);

        if (abnormalStatusCounts[type] >= StatusConstants.Instance.NagInfo.overlapCountMax)
        {
            return;
        }
        abnormalStatusCounts[type] += 1;
        overlappingCounts[type] += 1;
        if (false == isAbnormalStatuses[type])
            abnormalStatusCoroutines[type] = StartCoroutine(NagCoroutine());
        else
            abnormalStatusDurationTime[type] += StatusConstants.Instance.NagInfo.effectiveTime;
    }

    private void Climbing(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.CLIMBING;
        if (isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE] ||
             isAbnormalStatuses[(int)AbnormalStatusType.CHARM])
            return;
        StopAbnormalStatus(AbnormalStatusType.NAG);
        StopAbnormalStatus(AbnormalStatusType.REACTANCE);

        if (overlappingCounts[type] >= StatusConstants.Instance.ClimbingInfo.overlapCountMax)
            return;
        overlappingCounts[type] += 1;
        for (int i = 0; i < StatusConstants.Instance.ClimbingInfo.overlapCountMax; i++)
        {
            if (0 <= climbingTime[i])
            {
                climbingTime[i] = StatusConstants.Instance.ClimbingInfo.effectiveTime;
                break;
            }
        }
        if (false == isAbnormalStatuses[type])
        {
            abnormalStatusCoroutines[type] = StartCoroutine(ClimbingCoroutine());
        }
    }

    private void GraveyardShift(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.GRAVEYARDSHIFT;
        if (abnormalStatusCounts[type] >= StatusConstants.Instance.GraveyardShiftInfo.overlapCountMax)
        {
            Debug.Log("철야근무 중첩 횟수 제한으로 인한 return");
            return;
        }
        abnormalStatusCounts[type] += 1;
        if (false == isAbnormalStatuses[type])
            abnormalStatusCoroutines[type] = StartCoroutine(GraveyardShiftCoroutine());
        else
            abnormalStatusDurationTime[type] += StatusConstants.Instance.GraveyardShiftInfo.effectiveTime;
        Debug.Log(gameObject.name + " 철야근무 적용");
    }
    private void Freeze(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.FREEZE;
        StopAbnormalStatus(AbnormalStatusType.CHARM);
        StopAbnormalStatus(AbnormalStatusType.NAG);
        StopAbnormalStatus(AbnormalStatusType.REACTANCE);
        StopAbnormalStatus(AbnormalStatusType.CLIMBING);
        // 기존에 걸려있는 빙결이 없을 때
        if (null == abnormalStatusCoroutines[type])
        {
            abnormalStatusCoroutines[type] = StartCoroutine(FreezeCoroutine());
        }
        // 걸려있는 스턴이 빙결이 있을 때
        else
        {
            abnormalStatusDurationTime[type] = abnormalStatusTime[type] + StatusConstants.Instance.FreezeInfo.effectiveTime;
        }
    }

    private void Reactance(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.REACTANCE;
        if (isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE] || isAbnormalStatuses[(int)AbnormalStatusType.CHARM] ||
            isAbnormalStatuses[(int)AbnormalStatusType.NAG] || isAbnormalStatuses[(int)AbnormalStatusType.CLIMBING])
            return;

        // 기존에 적용 되어있는 청개구리가 없을 때
        if (null == abnormalStatusCoroutines[type])
        {
            abnormalStatusCoroutines[type] = StartCoroutine(ReactanceCoroutine());
        }
        // 적용 되어있는 청개구리가 있을 때
        else
        {
            abnormalStatusDurationTime[type] = abnormalStatusTime[type] + StatusConstants.Instance.ReactanceInfo.effectiveTime;
        }
    }


    private void Stun(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.STUN;
        StopAbnormalStatus(AbnormalStatusType.CHARM);
        StopAbnormalStatus(AbnormalStatusType.NAG);
        StopAbnormalStatus(AbnormalStatusType.REACTANCE);
        StopAbnormalStatus(AbnormalStatusType.CLIMBING);
        // 기존에 걸려있는 스턴이 없을 때
        if (null == abnormalStatusCoroutines[type])
        {
            abnormalStatusCoroutines[type] = StartCoroutine(StunCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 있을 때
        else
        {
            abnormalStatusDurationTime[type] = abnormalStatusTime[type] + effectiveTime;
        }
    }
    private void Charm(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.CHARM;
        if (isAbnormalStatuses[(int)AbnormalStatusType.STUN] || isAbnormalStatuses[(int)AbnormalStatusType.FREEZE])
            return;
        StopAbnormalStatus(AbnormalStatusType.NAG);
        StopAbnormalStatus(AbnormalStatusType.REACTANCE);
        StopAbnormalStatus(AbnormalStatusType.CLIMBING);
        // 기존에 걸려있는 매혹이 없을 때
        if (null == abnormalStatusCoroutines[type])
        {
            abnormalStatusCoroutines[type] = StartCoroutine(CharmCoroutine(effectiveTime));
        }
        // 걸려있는 매혹이 있을 때
        else
        {
            abnormalStatusDurationTime[type] = abnormalStatusTime[type] + effectiveTime;
        }
    }



    public void KnockBack(float knockBack, Vector2 bulletDir, Vector2 bulletPos, bool positionBasedKnockBack)
    {
        //Debug.Log(name + ", " + knockBackCheck);
        // 기본 상태에서 넉백
        if (null == knockBackCheck)
        {
            AddRetrictsMovingCount();
            knockBackCheck = StartCoroutine(KnockBackCheck());
        }

        rgbody.velocity = Vector3.zero;

        // bullet과 충돌 Object 위치 차이 기반의 넉백  
        if (positionBasedKnockBack)
        {
            rgbody.AddForce(knockBack * ((Vector2)transform.position - bulletPos).normalized);
        }
        // bullet 방향 기반의 넉백
        else
        {
            rgbody.AddForce(knockBack * bulletDir);
        }
    }


    private void StopAbnormalStatus(AbnormalStatusType abnormalStatusType)
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
            case AbnormalStatusType.NAG:
                abnormalComponents.NagEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case AbnormalStatusType.CLIMBING:
                for (int i = 0; i < StatusConstants.Instance.ClimbingInfo.overlapCountMax; i++)
                    climbingTime[i] = 0;
                overlappingCounts[type] = 0;
                abnormalComponents.ClibmingEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case AbnormalStatusType.GRAVEYARDSHIFT:
                abnormalStatusCounts[type] = 0;
                abnormalComponents.GraveyardShiftEffect.SetActive(false);
                SubRetrictsMovingCount();
                break;
            case AbnormalStatusType.FREEZE:
                abnormalComponents.FreezeEffect.SetActive(false);
                SubRetrictsMovingCount();
                SubRetrictsAttackingCount();
                break;
            case AbnormalStatusType.REACTANCE:
                abnormalComponents.ReactanceEffect.SetActive(false);
                SubRetrictsMovingCount();
                autoAimType = originalautoAimType;
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

    #endregion

    #region coroutine
    IEnumerator PoisonCoroutine()
    {
        isPoisoning = true;
        abnormalComponents.PoisonEffect.SetActive(true);
        float damage = 0;
        while (poisonOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);

            if (1 == poisonOverlappingCount)
                damage = hpMax * StatusConstants.Instance.PoisonInfo.value;
            else
                damage = hpMax * StatusConstants.Instance.PoisonInfo.value * 1.5f;
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
    }

    IEnumerator BurnCoroutine()
    {
        isBurning = true;
        abnormalComponents.BurnEffect.SetActive(true);
        float damage = 0;
        while (burnOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);

            if (1 == burnOverlappingCount)
                damage = hpMax * StatusConstants.Instance.BurnInfo.value;
            else
                damage = hpMax * StatusConstants.Instance.BurnInfo.value * 1.5f;
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
    }

    IEnumerator DelayStateCoroutine()
    {
        isDelayingState = true;
        while (delayStateOverlappingCount > 0)
        {
            CharacterTargetEffect characterTargetEffect = new CharacterTargetEffect();
            WeaponTargetEffect weaponTargetEffect = new WeaponTargetEffect();
            characterTargetEffect.moveSpeedIncrement = -StatusConstants.Instance.DelayStateInfo.value;
            weaponTargetEffect.bulletSpeedIncrement = -StatusConstants.Instance.DelayStateInfo.value;
            buffManager.RegisterItemEffect(characterTargetEffect, BuffManager.EffectApplyType.BUFF, -1, StatusConstants.Instance.DelayStateInfo.effectiveTime);
            buffManager.RegisterItemEffect(weaponTargetEffect, BuffManager.EffectApplyType.BUFF, -1, StatusConstants.Instance.DelayStateInfo.effectiveTime);
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.DelayStateInfo.effectiveTime);
            delayStateOverlappingCount -= 1;
        }
        delayStateCount = 0;
        isDelayingState = false;
    }


    IEnumerator NagCoroutine()
    {
        int type = (int)AbnormalStatusType.NAG;
        isAbnormalStatuses[type] = true;
        abnormalComponents.NagEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = StatusConstants.Instance.NagInfo.effectiveTime;
        Vector3 dirVec;
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            dirVec = PlayerManager.Instance.GetPlayerPosition() - transform.position;
            if (dirVec.magnitude < StatusConstants.Instance.NagInfo.range)
            {
                transform.Translate(moveSpeed * 0.5f * (-dirVec).normalized * Time.fixedDeltaTime);
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.NAG);
    }

    IEnumerator ClimbingCoroutine()
    {
        int type = (int)AbnormalStatusType.CLIMBING;
        isAbnormalStatuses[type] = true;
        abnormalComponents.ClibmingEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = StatusConstants.Instance.ClimbingInfo.effectiveTime;

        float speedRate;
        Vector3 dirVec;
        while (overlappingCounts[type] > 0)
        {
            if (1 == overlappingCounts[type])
                speedRate = 0.45f;
            else
                speedRate = 0.3f;
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            dirVec = PlayerManager.Instance.GetPlayerPosition() - transform.position;
            if (dirVec.magnitude < StatusConstants.Instance.ClimbingInfo.range)
            {
                transform.Translate(moveSpeed * speedRate * dirVec.normalized * Time.fixedDeltaTime);
            }
            else
                break;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            for (int i = 0; i < StatusConstants.Instance.ClimbingInfo.overlapCountMax; i++)
            {
                if (climbingTime[i] > 0)
                {
                    climbingTime[i] -= Time.fixedDeltaTime;
                    if (climbingTime[i] <= 0)
                    {
                        climbingTime[i] = 0;
                        overlappingCounts[type] -= 1;
                    }
                }
            }
        }

        StopAbnormalStatus(AbnormalStatusType.CLIMBING);
    }

    IEnumerator GraveyardShiftCoroutine()
    {
        int type = (int)AbnormalStatusType.GRAVEYARDSHIFT;
        isAbnormalStatuses[type] = true;
        AddRetrictsMovingCount();
        animationHandler.Idle();
        abnormalComponents.GraveyardShiftEffect.SetActive(true);
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = StatusConstants.Instance.GraveyardShiftInfo.effectiveTime;

        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.GRAVEYARDSHIFT);
    }

    IEnumerator FreezeCoroutine()
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
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.FREEZE);
    }

    IEnumerator ReactanceCoroutine()
    {
        int type = (int)AbnormalStatusType.REACTANCE;
        AddRetrictsMovingCount();
        animationHandler.Idle();
        abnormalComponents.ReactanceEffect.SetActive(true);
        isAbnormalStatuses[type] = true;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = StatusConstants.Instance.ReactanceInfo.effectiveTime;
        autoAimType = CharacterInfo.AutoAimType.REACTANCE;
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.REACTANCE);
    }

    IEnumerator StunCoroutine(float effectiveTime)
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

    IEnumerator CharmCoroutine(float effectiveTime)
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
            transform.Translate(moveSpeed * 0.5f * (PlayerManager.Instance.GetPlayerPosition() - transform.position).normalized * Time.fixedDeltaTime);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        StopAbnormalStatus(AbnormalStatusType.CHARM);
    }

    private IEnumerator KnockBackCheck()
    {
        while (true)
        {
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 1f)
            {
                //Debug.Log("z : " + restrictMovingCount);
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
        isBossEnemy = true;
    }
    public override float Attacked(TransferBulletInfo transferBulletInfo)
    {
        float damage = base.Attacked(transferBulletInfo);
        UIManager.Instance.bossHPUI.DecreaseHp(damage);
        AttackedEffect();
        return damage;
    }

    protected override void Die()
    {
        DeleteServant();
        ParticleManager.Instance.PlayParticle("Pixel", spriteTransform.position, sprite);

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
