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

public static class StatusConstants
{
    public static StatusConstant poisonInfo;
    public static StatusConstant burnInfo;
    public static StatusConstant nagInfo;
    public static StatusConstant delayStateInfo;
    public static float graduallyDamageCycle;
    public static int graduallyDamageCountMax;
    public static Vector2[] nagDirVector;
    //public static int damageCount = graduallyDamageCycle;
    static StatusConstants()
    {
        // 독, 화상 데미지 점화식 ( fd / 2 ) ( n + 1 )
        poisonInfo = new StatusConstant(0.05f, 3f, 3);
        burnInfo = new StatusConstant(0.05f, 3f, 3);
        nagInfo = new StatusConstant(0.5f, 4f, 2);
        nagDirVector = new Vector2[8]
        { new Vector2(0, 1), new Vector2(0, -1), new Vector2(0, -1), new Vector2(0, 1),
         new Vector2(-1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-1, 0)};
        delayStateInfo = new StatusConstant(0.5f, 3f, 3);
        graduallyDamageCycle = 0.1f;
        graduallyDamageCountMax = (int)(poisonInfo.effectiveTime / graduallyDamageCycle);
        Debug.Log("상태 이상 공격 횟수 : " + graduallyDamageCountMax);
    }
}
#endregion

public class Enemy : Character
{
    EnemyData enemyData;

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

    private Coroutine poisonCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine nagCoroutine;
    private Coroutine delayStateCoroutine;

    // temp Hp Max 나중에 EnemyData로 옮겨야 될듯? 아니면 그대로 hpMax여기서 쓰던가
    private float hpMax;
    #region setter
    #endregion

    #region getter
    public float GetHP() { return hp; }
    #endregion

    #region UnityFunc
    private void Awake()
    {
        rgbody = GetComponent<Rigidbody2D>();
        buffManager = GetComponent<BuffManager>();
        isKnockBack = false;
    }

    private void Update()
    {
        AutoAim();
        weaponManager.AttackButtonDown();
        renderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
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
    #region Func
    //0603 이유성 적 데이터로 적만들기 (애니메이션 아직 보류)
    //public void Init(EnemyData enemyData)
    //{
    //    pState = CharacterInfo.State.ALIVE;
    //    hp = enemyData.HP;
    //    moveSpeed = enemyData.Speed;
    //    animator = enemyData.Animator;
    //    weaponManager = GetComponentInChildren<WeaponManager>();
    //    weaponManager.Init(this, OwnerType.Enemy);
    //    weaponManager.EquipWeapon(enemyData.WeaponInfo);
    //}
    public void Init(Sprite _sprite)
    {
        sprite = _sprite;
        pState = CharacterInfo.State.ALIVE;
        renderer.sprite = sprite;
        renderer.color = new Color(1, 1, 1);
        scaleVector = transform.localScale;

        hpMax = 7;
        hp = hpMax;

        InitStatusEffects();

        // 0630 Enemy용 buffManager 초기화
        buffManager.Init();
        buffManager.SetOwner(this);
        // 0526 임시용
        weaponManager = GetComponentInChildren<WeaponManager>();
        weaponManager.Init(this, CharacterInfo.OwnerType.Enemy);
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
    }
    protected override void Die()
    {
        pState = CharacterInfo.State.DIE;
        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if (null != poisonCoroutine)
        {
            StopCoroutine(poisonCoroutine);
        }
        if (null != burnCoroutine)
        {
            StopCoroutine(burnCoroutine);
        }
        if (null != nagCoroutine)
        {
            StopCoroutine(nagCoroutine);
        }
        if (null != delayStateCoroutine)
        {
            StopCoroutine(delayStateCoroutine);
        }
        
        EnemyManager.Instance.DeleteEnemy(this);
        RoomManager.Instance.DieMonster();
        gameObject.SetActive(false);
        DropItem();
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
        StopCoroutine(CoroutineAttacked());
        StartCoroutine(CoroutineAttacked());
        if (hp <= 0)
            Die();
        return damage;
    }

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
            isKnockBack = true;

        // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
        rgbody.velocity = Vector3.zero;

        // bullet과 충돌 Object 위치 차이 기반의 넉백  
        if (positionBasedKnockBack)
        {
            rgbody.AddForce(knockBack * ((Vector2)transform.position - bulletPos).normalized);
        }
        // bullet 방향 기반의 넉백
        else
        {
            rgbody.AddForce(knockBack * _dir);
        }

        StopCoroutine(KnockBackCheck());
        StartCoroutine(KnockBackCheck());
        StopCoroutine(CoroutineAttacked());
        StartCoroutine(CoroutineAttacked());

        if (hp <= 0)
            Die();
        return damage;
    }

    public override void ApplyItemEffect(CharacterTargetEffect itemUseEffect)
    {

    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return;

        if (null == statusEffectInfo) return;
        // 독
        if(true == statusEffectInfo.canPoison)
        {
            Poison();
        }

        // 화상
        if(true == statusEffectInfo.canBurn)
        {
            Burn();
        }

        // 넉백 : + 밀기, - 당기기
        if (0 != statusEffectInfo.knockBack)
        {
            isKnockBack = true;

            // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
            rgbody.velocity = Vector3.zero;

            // bullet과 충돌 Object 위치 차이 기반의 넉백  
            if (statusEffectInfo.positionBasedKnockBack)
            {
                rgbody.AddForce(statusEffectInfo.knockBack * ((Vector2)transform.position - statusEffectInfo.bulletPos).normalized);
            }
            // bullet 방향 기반의 넉백
            else
            {
                rgbody.AddForce(statusEffectInfo.knockBack * statusEffectInfo.bulletDir);
            }

            StopCoroutine(KnockBackCheck());
            StartCoroutine(KnockBackCheck());
        }

        if (true == statusEffectInfo.canNag)
        {
            Nag();
        }

        if (true == statusEffectInfo.canDelayState)
        {
            DelayState();
        }
    }
    public void Poison()
    {
        if (poisonOverlappingCount >= StatusConstants.poisonInfo.overlapCountMax)
            return;
        poisonOverlappingCount += 1;
        for (int i = 0; i < StatusConstants.poisonInfo.overlapCountMax; i++)
        {
            if (0 == poisonCount[i])
            {
                poisonCount[i] = StatusConstants.graduallyDamageCountMax;
                break;
            }
        }
        if (false == isPoisoning)
        {
            poisonCoroutine = StartCoroutine(PoisonCoroutine());
        }
    }
    public void Burn()
    {
        if (burnOverlappingCount >= StatusConstants.burnInfo.overlapCountMax)
            return;
        burnOverlappingCount += 1;
        for (int i = 0; i < StatusConstants.burnInfo.overlapCountMax; i++)
        {
            if (0 == burnCount[i])
            {
                burnCount[i] = StatusConstants.graduallyDamageCountMax;
                break;
            }
        }
        if (false == isBurning)
        {
            burnCoroutine = StartCoroutine(BurnCoroutine());
        }
    }

    // 이동 translate, velocity, addForce ??
    public override void Nag()
    {
        if (nagCount >= StatusConstants.nagInfo.overlapCountMax)
            return;

        nagCount += 1;
        nagOverlappingCount += 1;
        if (false == isNagging)
        {
            nagCoroutine = StartCoroutine(NagCoroutine());
        }
    }

    //
    public override void DelayState()
    {
        if (delayStateCount >= StatusConstants.delayStateInfo.overlapCountMax)
            return;

        delayStateCount += 1;
        delayStateOverlappingCount += 1;
        if (false == isDelayingState)
        {
            delayStateCoroutine = StartCoroutine(DelayStateCoroutine());
        }
    }

    private bool CanApplyPoison()
    {
        for(int i = 0; i < StatusConstants.poisonInfo.overlapCountMax; i++)
        {
            if(poisonCount[i] > 0)
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator PoisonCoroutine()
    {
        isPoisoning = true;
        while (poisonOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.graduallyDamageCycle);
            hp = hpMax * (StatusConstants.poisonInfo.value * 0.5f) * (poisonOverlappingCount + 1);
            for (int i = 0; i < StatusConstants.poisonInfo.overlapCountMax; i++)
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
        isPoisoning = false;
    }

    IEnumerator BurnCoroutine()
    {
        isBurning = true;
        while (burnOverlappingCount > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.graduallyDamageCycle);
            hp = hpMax * (StatusConstants.burnInfo.value * 0.5f) * (burnOverlappingCount + 1);
            for (int i = 0; i < StatusConstants.burnInfo.overlapCountMax; i++)
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
        isBurning = false;
    }

    // 해야 됨, 이동 방식 뭘로하지 addForce Translate ????
    // 이런거 할 때 ai 잠시 멈춰야 될 것 같은데
    IEnumerator NagCoroutine()
    {
        isNagging = true;
        while(nagOverlappingCount > 0)
        {
            for(int i = 0; i < 8; i++)
            {
                rgbody.AddForce(50f * StatusConstants.nagDirVector[i]);
                yield return YieldInstructionCache.WaitForSeconds(StatusConstants.nagInfo.value);
            }
            nagOverlappingCount -= 1;
        }
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
            characterTargetEffect.moveSpeedIncrease = -StatusConstants.delayStateInfo.value;
            weaponTargetEffect.bulletSpeedIncrease = -StatusConstants.delayStateInfo.value;
            buffManager.RegisterItemEffect(characterTargetEffect, StatusConstants.delayStateInfo.effectiveTime);
            buffManager.RegisterItemEffect(weaponTargetEffect, StatusConstants.delayStateInfo.effectiveTime);
            yield return YieldInstructionCache.WaitForSeconds(StatusConstants.delayStateInfo.effectiveTime);
            delayStateOverlappingCount -= 1;
        }
        delayStateCount = 0;
        isDelayingState = false;
    }
    
    // 0526 땜빵
    public void AutoAim()
    {
        directionVector = (PlayerManager.Instance.GetPlayer().GetPosition() - transform.position).normalized;
        directionDegree = directionVector.GetDegFromVector();
    }
    #endregion
}
