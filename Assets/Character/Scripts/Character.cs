 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterInfo
{
    public enum SpawnType
    {
        NORMAL, SERVANT
    }
    public enum OwnerType
    {
        Player, Enemy, Pet, Object
    }
        
    public enum State
    {
        DIE, ALIVE
    }

    public enum DamageImmune
    {
        NONE, ALL
    }
    // 생각 중, 
    public enum AbnormalImmune
    {
        NONE, ALL
    }

    public enum AutoAimType
    {
        AUTO, SEMIAUTO, REACTANCE, MANUAL
    }
}

struct RaycasthitEnemy
{
    public int index;
    public float distance;
}

// 무기 매니저를 착용 하고 쓸 수 있는 owner들 (player, character, object)에서 써야 될 함수 변수들에 대한 걸 따로 묶어서 인터페이스화 해서 쓸 예정
// 그래야 character는 palyer, enemy에만 적용 하는건데 무기 착용 object에 대한 처리가 애매해짐.

public abstract class Character : MonoBehaviour
{
    #region Status
    protected float totalSpeed;
    protected float battleSpeed;
    public float moveSpeed;     // Character move Speed
    public float hp; // protected인데 debug용으로 어디서든 접근되게 public으로 했고 현재 hpUI에서 접근
    protected float maxHP;
    protected CharacterInfo.DamageImmune damageImmune;
    protected CharacterInfo.AbnormalImmune abnormalImmune;
    protected CharacterInfo.AutoAimType autoAimType;
    protected CharacterInfo.AutoAimType originalautoAimType;
    protected CharacterInfo.State pState;
    protected CharacterInfo.OwnerType ownerType;
    protected CharacterInfo.SpawnType spawnType;    
    #endregion
    #region componets
    protected CharacterComponents Components;
    protected AbnormalComponents abnormalComponents;
    protected WeaponManager weaponManager;
    protected SpriteRenderer spriteRenderer;
    protected Transform spriteTransform;
    protected CircleCollider2D interactiveCollider2D;
    protected AnimationHandler animationHandler;
    protected BuffManager buffManager;
    protected Rigidbody2D rgbody;
    protected AIController aiController;
    protected Transform shadowTransform;
    protected Transform bodyTransform;
    #endregion
    #region variables
    public bool isCasting;
    protected bool isActiveAI;
    protected bool isActiveMove;
    protected bool isActiveAttack;

    protected float evadeCoolTime;
    protected bool canEvade = true;
    protected bool isBattle = false;
    protected bool isEvade = false;
    [SerializeField]
    protected Sprite sprite;

    protected bool isAutoAiming;    // 오토에임 적용 유무
    protected Vector3 directionVector;
    protected float directionDegree;  // 바라보는 각도(총구 방향)

    protected bool isRightDirection;    // character 방향이 우측이냐(true) 아니냐(flase = 좌측)
    protected Color redColor = Color.red;
    protected Color burnColor = new Color(1, 0, 0);
    protected Color freezeColor = new Color(.7f, .7f, 1);
    protected Color poisonColor = new Color(.7f, 1, .7f);
    protected Color baseColor;

    protected LayerMask enemyLayer;
    /// <summary> owner 좌/우 바라볼 때 spriteObject scale 조절에 쓰일 player scale, 우측 (1, 1, 1), 좌측 : (-1, 1, 1) </summary>
    protected Vector3 scaleVector;
    #endregion
    // TODO : Enemy에서 다른 owner에서도 적용하는 것들은 옮겨 와야됨~
    #region abnormalStatusVariables
    protected int restrictMovingCount;
    protected int restrictAttackingCount;

    protected bool isPoisoning;
    protected int poisonOverlappingCount;
    protected int[] poisonCount;
    protected bool isBurning;
    protected int burnOverlappingCount;
    protected int[] burnCount;
    protected bool isDelayingState;
    protected int delayStateOverlappingCount;
    protected int delayStateCount;

    protected float[] climbingTime;
    protected bool[] isAbnormalStatuses;
    protected int[] abnormalStatusCounts;
    protected int[] overlappingCounts;
    protected float[] abnormalStatusTime;
    protected float[] abnormalStatusDurationTime;
    protected Coroutine[] abnormalStatusCoroutines;

    protected Coroutine poisonCoroutine;
    protected Coroutine burnCoroutine;

    protected Coroutine knockBackCheck;
    protected Coroutine delayStateCoroutine;
    #endregion
    #region dataStruct
    protected List<Character> servants;
    #endregion
    #region getter
    public float GetHP()
    {
        return hp;
    }
    public float GetMaxHP()
    {
        return maxHP;
    }
    public float GetPercentHP()
    {
        return (hp / maxHP) * 100;
    }
    public CharacterComponents GetCharacterComponents()
    {
        return Components;
    }
    public AbnormalComponents GetAbnormalComponents()
    {
        return abnormalComponents;
    }
    public bool GetAIAct()
    {
        return isActiveAI;
    }
    public virtual bool GetRightDirection()
    {
        return isRightDirection;
    }
    public virtual Vector3 GetDirVector()
    {
        return directionVector;
    }
    public virtual float GetDirDegree()
    {
        return directionDegree;
    }
    public virtual Vector3 GetPosition()
    {
        return bodyTransform.position;
    }
    public virtual WeaponManager GetWeaponManager()
    {
        return weaponManager;
    }
    public BuffManager GetBuffManager()
    {
        return buffManager;
    }
    public CharacterInfo.OwnerType GetOwnerType()
    {
        return ownerType;
    }

    public bool GetIsAcitveAttack()
    {
        return isActiveAttack;
    }
    public bool GetIsAcitveMove()
    {
        return isActiveMove;
    }
    #endregion
    #region Func
    public void SpawnServant(Character character)
    {
        character.SetSpawnType(CharacterInfo.SpawnType.SERVANT);
        servants.Add(character);
    }
    
    public void DeleteServant()
    {
        for(int i=0;i< servants.Count;i++)
        {
            if(servants[i] != null)
                servants[i].Die();
        }
    }

    public bool IsDie()
    {
        if (CharacterInfo.State.DIE == pState)
        {
            return true;
        }
        return false;
    }

    protected void AttackedEffect()
    {
        if(gameObject.activeSelf)
            StartCoroutine(CoroutineColorChange(redColor, 0.1f));
    }

    protected IEnumerator CoroutineColorChange(Color color,float seconds)
    {
        spriteRenderer.color = color;
        yield return YieldInstructionCache.WaitForSeconds(seconds);
        spriteRenderer.color = baseColor;
    }

    protected void ColorChange(Color color)
    {
        spriteRenderer.color = color;
    }
    #endregion
    public virtual void Init()
    {
        Components = GetComponent<CharacterComponents>();
        abnormalComponents = GetComponent<AbnormalComponents>();
        Components.Init();
        weaponManager = Components.WeaponManager;
        spriteRenderer = Components.SpriteRenderer;
        spriteTransform = Components.SpriteTransform;
        interactiveCollider2D = Components.InteractiveCollider2D;
        animationHandler = Components.AnimationHandler;
        buffManager = Components.BuffManager;
        rgbody = Components.Rigidbody2D;
        aiController = Components.AIController;
        shadowTransform = Components.ShadowTransform;
        bodyTransform = GetComponent<Transform>();

        isActiveAI = true;
        isCasting = false;
        spawnType = CharacterInfo.SpawnType.NORMAL;
    }
    public virtual void ActiveSkill()
    {
        //TODO : 만약에 Enemy를 조종하게 될 경우 Enemy Class에 재정의 필요
    }
    public virtual CustomObject Interact()
    {   
        //TODO : 만약에 Enemy를 조종하게 될 경우 Enemy Class에 재정의 필요
        return null;
    }
    public abstract void SetAim();
    /*--abstract--*/
    protected abstract void Die();
    public abstract float Attacked(TransferBulletInfo info);

    // item Character 대상 효과 적용
    public abstract void ApplyItemEffect();

    #region Abnormal
    /// <summary> 상태 이상 효과 적용 </summary>
    protected bool AbnormalChance(float appliedChance)
    {
        float chance = Random.Range(0, 1f);
        if (chance < appliedChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected abstract void AddRetrictsMovingCount();
    /// <summary> 이동 방해 상태 이상 갯수 감소 및 이동 AI ON Check </summary>
    protected abstract void SubRetrictsMovingCount();
    /// <summary> 공격 방해 상태 이상 갯수 증가 및 공격 AI OFF Check </summary>
    protected abstract void AddRetrictsAttackingCount();
    /// <summary> 공격 방해 상태 이상 갯수 감소 및 공격 AI ON Check </summary>
    protected abstract void SubRetrictsAttackingCount();


    protected abstract void StopAbnormalStatus(AbnormalStatusType abnormalStatusType);

    public void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState || null == statusEffectInfo || CharacterInfo.AbnormalImmune.ALL == abnormalImmune)
            return;

        if (0 != statusEffectInfo.knockBack)
            KnockBack(statusEffectInfo.knockBack, statusEffectInfo.BulletDir, statusEffectInfo.BulletPos, statusEffectInfo.positionBasedKnockBack);

        if (true == statusEffectInfo.canPoison)
            Poison(statusEffectInfo.posionChance);
        if (true == statusEffectInfo.canBurn)
            Burn(statusEffectInfo.burnChance);
        if (true == statusEffectInfo.canFreeze)
            Freeze(statusEffectInfo.freezeChance);
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

    private void Freeze(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.FREEZE;
        StopAbnormalStatus(AbnormalStatusType.CHARM);
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

    private void Stun(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)AbnormalStatusType.STUN;
        StopAbnormalStatus(AbnormalStatusType.CHARM);
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
            rgbody.AddForce(knockBack * ((Vector2)bodyTransform.position - bulletPos).normalized);
        }
        // bullet 방향 기반의 넉백
        else
        {
            rgbody.AddForce(knockBack * bulletDir);
        }
    }
    #endregion
    #region AbnormalCo
    protected abstract IEnumerator PoisonCoroutine();
    protected abstract IEnumerator BurnCoroutine();
    protected abstract IEnumerator DelayStateCoroutine();
    protected abstract IEnumerator FreezeCoroutine();
    protected abstract IEnumerator StunCoroutine(float effectiveTime);
    protected abstract IEnumerator CharmCoroutine(float effectiveTime);
    protected abstract IEnumerator KnockBackCheck();
    #endregion


    /// <summary>총알 외의 충돌로 인한 공격과 넉백 처리</summary>
    public abstract float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false);

    public void SetAutoAim()
    {
        if (!IsAbnormal())
        {
            autoAimType = CharacterInfo.AutoAimType.AUTO;
        }
        originalautoAimType = CharacterInfo.AutoAimType.AUTO;
    }

    public void SetSemiAutoAim()
    {
        if (!IsAbnormal())
        {
            autoAimType = CharacterInfo.AutoAimType.SEMIAUTO;
        }
        originalautoAimType = CharacterInfo.AutoAimType.SEMIAUTO;
    }

    public void SetManualAim()
    {
        if(!IsAbnormal())
        {
            autoAimType = CharacterInfo.AutoAimType.MANUAL;
        }
        originalautoAimType = CharacterInfo.AutoAimType.MANUAL;
    }

    public void SetSpawnType(CharacterInfo.SpawnType spawnType)
    {
        this.spawnType = spawnType;
    }

    protected abstract bool IsAbnormal();

    public abstract bool Evade();

    public bool IsEvade()
    {
        return isEvade;
    }

    protected void DeactivateAbnormalComponents()
    {
        abnormalComponents.BurnEffect.SetActive(false);
        abnormalComponents.PoisonEffect.SetActive(false);

        abnormalComponents.FreezeEffect.SetActive(false);

        abnormalComponents.StunEffect.SetActive(false);
        abnormalComponents.CharmEffect.SetActive(false);
    }

    protected void InitStatusEffects()
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

    public abstract void SelfDestruction();
}


