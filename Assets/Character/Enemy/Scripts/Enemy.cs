using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region sturct
public struct StatusConstant
{
    public float value;         // 각 상태이상 별로 세기, 값
    public float effectiveTime; // 효과 유지 시간
    public int overlapCountMax; // 중첩 횟수 최대치
    public StatusConstant(float value, float effectiveTime, int overlapCountMax)
    {
        this.value = value;
        this.effectiveTime = effectiveTime;
        this.overlapCountMax = overlapCountMax;
    }
}
#endregion

public enum AbnormalStatusType { STUN, FREEZE, ROOT, FEAR, CHARM, END }

public enum EnemyAutoAimType { AUTO, SEMIAUTO, RANDOM }

public class Enemy : Character
{
    #region variables
    
    private EnemyAutoAimType enemyAutoAimType;

    

    // temp Hp Max 나중에 EnemyData로 옮겨야 될듯? 아니면 그대로 hpMax여기서 쓰던가
    private float hpMax;
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
    private bool isNagging;
    private int nagOverlappingCount;
    private int nagCount;
    private bool isDelayingState;
    private int delayStateOverlappingCount;
    private int delayStateCount;

    private float[] abnormalStatusTime;
    private float[] abnormalStatusDurationTime;

    private Coroutine knockBackCheck;
    private Coroutine poisonCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine stunCoroutine;
    private Coroutine freezeCoroutine;
    private Coroutine rootCoroutine;
    private Coroutine fearCoroutine;
    private Coroutine charmCoroutine;
    private Coroutine nagCoroutine;
    private Coroutine delayStateCoroutine;
    #endregion
    #region setter
    #endregion

    #region getter
    public float GetHP() { return hp; }

    public CircleCollider2D GetCircleCollider2D()
    {
        return Components.CircleCollider2D;
    }
    #endregion

    #region UnityFunc
    private void Awake()
    {
        abnormalStatusTime = new float[(int)AbnormalStatusType.END];
        abnormalStatusDurationTime = new float[(int)AbnormalStatusType.END];
    }

    private void Update()
    {
        AutoAim();
        weaponManager.AttackButtonDown();
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
    public void Init(EnemyData enemyData)
    {
        base.Init();
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.Enemy;

        isActiveAttackAI = true;
        isActiveMoveAI = true;

        hp = enemyData.HP;
        moveSpeed = enemyData.Speed;
        sprite = enemyData.Sprite;
        buffManager.Init();
        buffManager.SetOwner(this);
        weaponManager.Init(this, CharacterInfo.OwnerType.Enemy);
        for (int i = 0; i < enemyData.WeaponInfo.Count; i++)
        {
            //Debug.Log(enemyData.WeaponInfo[i].name + ", " + name);
            weaponManager.EquipWeapon(enemyData.WeaponInfo[i]);
        }
        animationHandler.Init(enemyData.AnimatorController);
        aiController.Init(moveSpeed, animationHandler, weaponManager, enemyData.Task, enemyData.SkillDatas);
        InitStatusEffects();
        scaleVector = new Vector3(1f, 1f, 1f);
    }
    /// <summary> 상태 이상 효과 관련 초기화 </summary>
    private void InitStatusEffects()
    {
        isPoisoning = false;
        poisonOverlappingCount = 0;
        poisonCount = new int[3];
        isBurning = false;
        burnOverlappingCount = 0;
        burnCount = new int[3];
        isNagging = false;
        nagCount = 0;
        isDelayingState = false;
        delayStateCount = 0;

        restrictMovingCount = 0;
        restrictAttackingCount = 0;
        for (int i = 0; i < (int)AbnormalStatusType.END; i++)
        {
            abnormalStatusTime[i] = 0;
            abnormalStatusDurationTime[i] = 0;
        }
    }
    #endregion

    #region Func


    protected override void Die()
    {
        pState = CharacterInfo.State.DIE;
        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if(null != knockBackCheck)
        {
            StopCoroutine(KnockBackCheck());
        }
        if (null != poisonCoroutine)
        {
            StopCoroutine(poisonCoroutine);
        }
        if (null != burnCoroutine)
        {
            StopCoroutine(burnCoroutine);
        }
        if (null != stunCoroutine)
        {
            StopCoroutine(stunCoroutine);
        }
        if (null != freezeCoroutine)
        {
            StopCoroutine(freezeCoroutine);
        }
        if (null != rootCoroutine)
        {
            StopCoroutine(rootCoroutine);
        }
        if (null != fearCoroutine)
        {
            StopCoroutine(fearCoroutine);
        }
        if (null != charmCoroutine)
        {
            StopCoroutine(charmCoroutine);
        }
        if (null != nagCoroutine)
        {
            StopCoroutine(nagCoroutine);
        }
        if (null != delayStateCoroutine)
        {
            StopCoroutine(delayStateCoroutine);
        }

        DropItem();
        Stamina.Instance.StaminaPlus();
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
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
    /// <summary>총알에서 전달된 정보로 공격 처리</summary>
    public override float Attacked(TransferBulletInfo transferredInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        float damage = transferredInfo.damage;
        // 크리티컬 공격
        if(criticalCheck < transferredInfo.criticalChance)
        {
            damage *= 2f;
        }
        hp -= damage;

        if (hp <= 0)
            Die();
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
        hp -= damage;

        if (knockBack > 0)
            KnockBack(knockBack, _dir, bulletPos, positionBasedKnockBack);

        if (hp <= 0)
            Die();
        return damage;
    }

    public override void ApplyItemEffect(CharacterTargetEffect itemUseEffect)
    {
        // 개발 중
    }

    

    // TODO : 0802 모장현, enemy aim 조절 타입에 따라서 알고리즘 변경

    // 0526 땜빵
    public void AutoAim()
    {
        switch(enemyAutoAimType)
        {
            case EnemyAutoAimType.AUTO:
                break;
            case EnemyAutoAimType.SEMIAUTO:
                break;
            case EnemyAutoAimType.RANDOM:
                break;
            default:
                break;
        }
        directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - transform.position).normalized;
        directionDegree = directionVector.GetDegFromVector();
    }

    #endregion

    #region abnormalState
    /// <summary> 이동 방해 상태 이상 갯수 증가 및 이동 AI OFF Check </summary>
    private void AddRetrictsMovingCount()
    {
        restrictMovingCount += 1;
        if (restrictMovingCount > 0)
        {
            isActiveMoveAI = false;
            aiController.StopMove();
            // Debug.Log(name + " Move AI Off");
        }
    }
    /// <summary> 이동 방해 상태 이상 갯수 감소 및 이동 AI ON Check </summary>
    private void SubRetrictsMovingCount()
    {
        restrictMovingCount -= 1;
        if (0 == restrictMovingCount)
        {
            isActiveMoveAI = true;
            aiController.PlayMove();
            // Debug.Log(name + " Move AI ON");
        }
    }

    /// <summary> 공격 방해 상태 이상 갯수 증가 및 공격 AI OFF Check </summary>
    private void AddRetrictsAttackingCount()
    {
        restrictAttackingCount += 1;
        if (restrictAttackingCount > 0)
        {
            isActiveMoveAI = false;
            aiController.StopAttack();
            //Debug.Log(name + " Attack AI Off");
        }
    }
    /// <summary> 공격 방해 상태 이상 갯수 감소 및 공격 AI ON Check </summary>
    private void SubRetrictsAttackingCount()
    {
        restrictMovingCount -= 1;
        if (0 == restrictMovingCount)
        {
            isActiveMoveAI = true;
            aiController.PlayAttack();
            //Debug.Log(name + " Attack AI ON");
        }
    }


    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return;
        if (null == statusEffectInfo) return;

        // 독
        if (true == statusEffectInfo.canPoison)
            Poison();
        // 화상
        if (true == statusEffectInfo.canBurn)
            Burn();

        // 스턴
        if (0 != statusEffectInfo.stun)
            Stun(statusEffectInfo.stun);

        // 빙결
        if (0 != statusEffectInfo.freeze)
            Freeze(statusEffectInfo.freeze);

        // 속박
        if (0 != statusEffectInfo.root)
            Root(statusEffectInfo.root);
        // 공포
        if (0 != statusEffectInfo.fear)
            Fear(statusEffectInfo.fear);
        // 매혹
        if (0 != statusEffectInfo.charm)
            Charm(statusEffectInfo.charm);

        // 넉백 : + 밀기, - 당기기
        if (0 != statusEffectInfo.knockBack)
        {
            KnockBack(statusEffectInfo.knockBack, statusEffectInfo.BulletDir, statusEffectInfo.BulletPos, statusEffectInfo.positionBasedKnockBack);
        }

        // 잔소리
        if (true == statusEffectInfo.canNag)
            Nag();
        // 이동 지연
        if (true == statusEffectInfo.canDelayState)
            DelayState();
    }
    private void Poison()
    {
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
    private void Burn()
    {
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

    private void Stun(float effectiveTime)
    {
        // 기존에 걸려있는 스턴이 없을 때
        if (null == stunCoroutine)
        {
            stunCoroutine = StartCoroutine(StunCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 있을 때
        else
        {
            abnormalStatusDurationTime[(int)AbnormalStatusType.STUN] = abnormalStatusTime[(int)AbnormalStatusType.STUN] + effectiveTime;
        }
    }
    private void Freeze(float effectiveTime)
    {
        // 기존에 걸려있는 빙결이 없을 때
        if (null == freezeCoroutine)
        {
            freezeCoroutine = StartCoroutine(FreezeCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 빙결이 있을 때
        else
        {
            abnormalStatusDurationTime[(int)AbnormalStatusType.FREEZE] = abnormalStatusTime[(int)AbnormalStatusType.FREEZE] + effectiveTime;
        }
    }
    private void Root(float effectiveTime)
    {
        // TODO : 이동 AI OFF 후 속박 시간만큼 뒤에 정상화
        // 기존에 걸려있는 속박이 없을 때
        if (null == rootCoroutine)
        {
            rootCoroutine = StartCoroutine(RootCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 있을 때
        else
        {
            abnormalStatusDurationTime[(int)AbnormalStatusType.ROOT] = abnormalStatusTime[(int)AbnormalStatusType.ROOT] + effectiveTime;
        }
    }
    private void Fear(float effectiveTime)
    {
        // TODO : 공격, 이동 AI OFF 후 시전자에서 멀어지게 움직이게 하고 공포 시간만큼 뒤에 정상화
        // 기존에 걸려있는 공포가 없을 때
        if (null == fearCoroutine)
        {
            fearCoroutine = StartCoroutine(FearCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 있을 때
        else
        {
            abnormalStatusDurationTime[(int)AbnormalStatusType.FEAR] = abnormalStatusTime[(int)AbnormalStatusType.FEAR] + effectiveTime;
        }
    }
    private void Charm(float effectiveTime)
    {
        // TODO : 공격, 이동 AI OFF 후 시전자에서 가까워지게 움직이게 하고 매혹 시간만큼 뒤에 정상화
        // 기존에 걸려있는 매혹이 없을 때
        if (null == charmCoroutine)
        {
            charmCoroutine = StartCoroutine(CharmCoroutine(effectiveTime));
        }
        // 걸려있는 매혹이 있을 때
        else
        {
            abnormalStatusDurationTime[(int)AbnormalStatusType.CHARM] = abnormalStatusTime[(int)AbnormalStatusType.CHARM] + effectiveTime;
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

    // 이동 translate, velocity, addForce ??
    protected override void Nag()
    {
        rgbody.velocity = Vector3.zero;
        DebugX.Log(name + " Nag 시도, count = " + nagCount + ", " + StatusConstants.Instance.NagInfo.overlapCountMax);
        if (nagCount >= StatusConstants.Instance.NagInfo.overlapCountMax)
        {
            DebugX.Log("중첩 횟수 제한으로 인한 return");
            return;
        }
        nagCount += 1;
        nagOverlappingCount += 1;
        if (false == isNagging)
        {
            nagCoroutine = StartCoroutine(NagCoroutine());
        }
        DebugX.Log(gameObject.name + " 잔소리 적용");
    }

    ///<summary>이동 지연</summary>
    protected override void DelayState()
    {
        if (delayStateCount >= StatusConstants.Instance.DelayStateInfo.overlapCountMax)
            return;
        DebugX.Log(gameObject.name + " 이동지연 적용");
        delayStateCount += 1;
        delayStateOverlappingCount += 1;
        if (false == isDelayingState)
        {
            delayStateCoroutine = StartCoroutine(DelayStateCoroutine());
        }
    }
    #endregion

    #region coroutine
    IEnumerator PoisonCoroutine()
    {
        isPoisoning = true;
        Components.PoisonEffect.SetActive(true);
        while (poisonOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);
            hp = hpMax * (StatusConstants.Instance.PoisonInfo.value * 0.5f) * (poisonOverlappingCount + 1);
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
        Components.PoisonEffect.SetActive(false);
        isPoisoning = false;
    }

    IEnumerator BurnCoroutine()
    {
        isBurning = true;
        Components.BurnEffect.SetActive(true);
        while (burnOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.GraduallyDamageCycle);
            hp = hpMax * (StatusConstants.Instance.BurnInfo.value * 0.5f) * (burnOverlappingCount + 1);
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
        Components.BurnEffect.SetActive(false);
        isBurning = false;
    }

    IEnumerator StunCoroutine(float effectiveTime)
    {
        Components.StunEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        int type = (int)AbnormalStatusType.STUN;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;
        Debug.Log("스턴 시작");
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        Components.StunEffect.SetActive(false);
        SubRetrictsMovingCount();
        SubRetrictsAttackingCount();
    }

    IEnumerator FreezeCoroutine(float effectiveTime)
    {
        Components.FreezeEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        animationHandler.Idle();
        int type = (int)AbnormalStatusType.FREEZE;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;
        Debug.Log("빙결 시작");
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        Components.FreezeEffect.SetActive(false);
        SubRetrictsMovingCount();
        SubRetrictsAttackingCount();
    }

    IEnumerator RootCoroutine(float effectiveTime)
    {
        AddRetrictsMovingCount();
        // 이동 애니메이션 off
        // 속박 이펙트
        int type = (int)AbnormalStatusType.ROOT;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;

        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        SubRetrictsMovingCount();
    }
    IEnumerator FearCoroutine(float effectiveTime)
    {
        Components.FearEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        // 이동 애니메이션 on
        // 공포 이펙트
        // 공포 처리 : 시전자와 멀어지는 방향으로 걸어서 이동하기
        int type = (int)AbnormalStatusType.FEAR;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;
        Vector3 dirVec;
        Debug.Log("공포 시작");
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            dirVec = PlayerManager.Instance.GetPlayerPosition() - transform.position;
            if(dirVec.magnitude < 5f)
            {
                transform.Translate(moveSpeed * 0.5f * (-dirVec).normalized * Time.fixedDeltaTime);
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        Components.FearEffect.SetActive(false);
        SubRetrictsMovingCount();
        SubRetrictsAttackingCount();
    }
    IEnumerator CharmCoroutine(float effectiveTime)
    {
        Components.CharmEffect.SetActive(true);
        AddRetrictsMovingCount();
        AddRetrictsAttackingCount();
        // 이동 애니메이션 on
        // 매혹 이펙트
        // 매혹 처리 : 시전자에게 다가가는 방향으로 걸어서 이동하기
        int type = (int)AbnormalStatusType.CHARM;
        abnormalStatusTime[type] = 0;
        abnormalStatusDurationTime[type] = effectiveTime;
        Debug.Log(name + "매혹 시작");
        while (abnormalStatusTime[type] <= abnormalStatusDurationTime[type])
        {
            // Debug.Log("매혹 : " + abnormalStatusTime[type]);
            abnormalStatusTime[type] += Time.fixedDeltaTime;
            transform.Translate(moveSpeed * 0.5f * (PlayerManager.Instance.GetPlayerPosition() - transform.position).normalized * Time.fixedDeltaTime);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }

        Components.CharmEffect.SetActive(false);
        SubRetrictsMovingCount();
        SubRetrictsAttackingCount();
    }

    // 해야 됨, 이동 방식 뭘로하지 addForce Translate ????
    // 이런거 할 때 ai 잠시 멈춰야 될 것 같은데
    IEnumerator NagCoroutine()
    {
        isNagging = true;
        rgbody.velocity = Vector2.zero;
        AddRetrictsMovingCount();
        Debug.Log("상태이상 잔소리 시작, " + StatusConstants.Instance);
        while (nagOverlappingCount > 0)
        {
            for(int i = 0; i < 8; i++)
            {
                rgbody.velocity = 3f * StatusConstants.Instance.NagDirVector[i];
                yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.NagInfo.value);
                isActiveAI = false;
            }
            nagOverlappingCount -= 1;
        }
        DebugX.Log("상태이상 잔소리 끝");
        SubRetrictsMovingCount();
        nagCount = 0;
        isNagging = false;
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
            buffManager.RegisterItemEffect(characterTargetEffect, StatusConstants.Instance.DelayStateInfo.effectiveTime);
            buffManager.RegisterItemEffect(weaponTargetEffect, StatusConstants.Instance.DelayStateInfo.effectiveTime);
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.Instance.DelayStateInfo.effectiveTime);
            delayStateOverlappingCount -= 1;
        }
        delayStateCount = 0;
        isDelayingState = false;
    }

    private IEnumerator KnockBackCheck()
    {
        while (true)
        {
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 0.3f)
            {
                SubRetrictsMovingCount();
                knockBackCheck = null;
            }
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    #endregion
}

public class BossEnemy : Enemy
{

    public override float Attacked(TransferBulletInfo transferBulletInfo)
    {
        float damage = base.Attacked(transferBulletInfo);
        UIManager.Instance.bossHPUI.DecreaseHp(damage);
        return damage;
    }

    protected override void Die()
    {
        pState = CharacterInfo.State.DIE;
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        gameObject.SetActive(false);
        DropItem();
        Destroy(this);
    }
}
