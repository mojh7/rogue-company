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
        PLAYER, ENEMY, OBJECT, PET
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

    public enum AimType
    {
        AUTO, SEMIAUTO, MANUAL
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
    #region constants
    protected static readonly Color RED_COLOR = Color.red;
    protected static readonly Color BURN_COLOR = new Color(1, 0, 0);
    protected static readonly Color FREEZE_COLOR = new Color(.7f, .7f, 1);
    protected static readonly Color POISON_COLOR = new Color(.7f, 1, .7f);
    #endregion
    #region Status
    protected float totalSpeed;
    protected float battleSpeed;
    public float moveSpeed;     // Character move Speed
    protected float hp;
    protected float hpMax;
    protected CharacterInfo.DamageImmune damageImmune;
    protected CharacterInfo.AbnormalImmune abnormalImmune;
    protected CharacterInfo.AimType aimType;
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

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return spriteRenderer;
        }
    }
    #endregion
    #region variables
    public bool isCasting;
    protected bool isActiveAI;
    protected bool isActiveMove;
    protected bool isActiveAttack;
    protected bool isDash;

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

    protected Color baseColor;

    protected LayerMask enemyLayer;
    /// <summary> owner 좌/우 바라볼 때 spriteObject scale 조절에 쓰일 player scale, 우측 (1, 1, 1), 좌측 : (-1, 1, 1) </summary>
    protected Vector3 scaleVector;
    #endregion
    // TODO : Enemy에서 다른 owner에서도 적용하는 것들은 옮겨 와야됨~
    #region abnormalStatusVariables
    protected int restrictMovingCount;
    protected int restrictAttackingCount;

    protected float poisonDamagePerUnit;
    protected float burnDamagePerUnit;

    protected bool[] isAttackTypeAbnormalStatuses;
    protected int[] effectAppliedCount;
    protected Coroutine[] attackTypeAbnormalStatusCoroutines;

    protected bool[] isControlTypeAbnormalStatuses;
    protected float[] controlTypeAbnormalStatusTime;
    protected float[] controlTypeAbnormalStatusesDurationMax;
    protected Coroutine[] controlTypeAbnormalStatusCoroutines;

    protected Coroutine checkingknockBackEnded;
    protected Coroutine checkingDashEnded;
    #endregion

    #region dataStruct
    protected List<Character> servants;
    #endregion

    #region getter
    public float HP
    {
        get
        {
            return hp;
        }
    }
    public float HPMax
    {
        get
        {
            return hpMax;
        }
    }
    public float PercentHP
    {
        get
        {
            return (hp / hpMax) * 100;
        }
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
    public virtual float GetDirDegree()
    {
        return directionDegree;
    }
    public virtual Vector3 GetDirVector()
    {
        return directionVector;
    }
    public virtual Vector3 GetPosition()
    {
        return bodyTransform.position;
    }
    public Transform GetbodyTransform()
    {
        return bodyTransform;
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

    #region func
    public void SpawnServant(Character character)
    {
        character.SetSpawnType(CharacterInfo.SpawnType.SERVANT);
        servants.Add(character);
    }

    public void DeleteServant()
    {
        for (int i = 0; i < servants.Count; i++)
        {
            if (servants[i] != null)
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
        if (gameObject.activeSelf)
            StartCoroutine(CoroutineColorChange(RED_COLOR, 0.1f));
    }

    protected IEnumerator CoroutineColorChange(Color color, float seconds)
    {
        spriteRenderer.color = color;
        yield return YieldInstructionCache.WaitForSeconds(seconds);
        spriteRenderer.color = baseColor;
    }

    protected void ColorChange(Color color)
    {
        spriteRenderer.color = color;
    }

    protected virtual void RecoveryHp(float amount)
    {
        if (amount <= 0)
            return;
        hp += amount;
        if (hp >= hpMax)
            hp = hpMax;
    }

    protected virtual void ReduceHp(float amount)
    {
        if (amount <= 0)
            return;
        hp -= amount;
        if (hp <= 0)
            Die();
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
        isDash = false;
        spawnType = CharacterInfo.SpawnType.NORMAL;
        DeactivateAbnormalComponents();
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
    public abstract void ApplyConsumableItem(CharacterTargetEffect itemUseEffect);
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

    protected void StopAttackTypeAbnormalStatus(AttackTypeAbnormalStatusType attackTypeAbnormalStatusType)
    {
        int type = (int)attackTypeAbnormalStatusType;
        if (false == isAttackTypeAbnormalStatuses[type])
            return;
        isAttackTypeAbnormalStatuses[type] = false;

        if (null != attackTypeAbnormalStatusCoroutines[type])
            StopCoroutine(attackTypeAbnormalStatusCoroutines[type]);
        attackTypeAbnormalStatusCoroutines[type] = null;

        switch (attackTypeAbnormalStatusType)
        {
            case AttackTypeAbnormalStatusType.POISON:
                abnormalComponents.PoisonEffect.SetActive(false);
                break;
            case AttackTypeAbnormalStatusType.BURN:
                abnormalComponents.BurnEffect.SetActive(false);
                break;
            default:
                break;
        }
    }

    protected void StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType controlTypeAbnormalStatusType)
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
    }

    //protected abstract void StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType controlTypeAbnormalStatusType);

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
        if (0 < statusEffectInfo.freeze)
            Freeze(statusEffectInfo.freeze, statusEffectInfo.freezeChance);
        if (0 < statusEffectInfo.stun)
            Stun(statusEffectInfo.stun, statusEffectInfo.stunChance);
        if (0 < statusEffectInfo.charm)
            Charm(statusEffectInfo.charm, statusEffectInfo.charmChance);
    }

    protected abstract void UpdateEffectAppliedCount(AttackTypeAbnormalStatusType attackTypeAbnormalStatusType);

    private void Poison(float chance)
    {
        int type = (int)AttackTypeAbnormalStatusType.POISON;
        if (false == AbnormalChance(chance))
            return;

        if (false == isAttackTypeAbnormalStatuses[type])
        {
            attackTypeAbnormalStatusCoroutines[type] = StartCoroutine(PoisonCoroutine());
        }
        else
        {
            UpdateEffectAppliedCount(AttackTypeAbnormalStatusType.POISON);
        }
    }

    private void Burn(float chance)
    {
        if (IsDie())
        {
            Debug.Log("죽음");
            return;
        }
        int type = (int)AttackTypeAbnormalStatusType.BURN;
        if (false == AbnormalChance(chance))
            return;

        if (false == isAttackTypeAbnormalStatuses[type])
        {
            attackTypeAbnormalStatusCoroutines[type] = StartCoroutine(BurnCoroutine());
        }
        else
        {
            UpdateEffectAppliedCount(AttackTypeAbnormalStatusType.BURN);
        }
    }

    private void Freeze(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)ControlTypeAbnormalStatusType.FREEZE;
        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.CHARM);
        // 기존에 걸려있는 빙결이 없을 때
        if (null == controlTypeAbnormalStatusCoroutines[type])
        {
            controlTypeAbnormalStatusCoroutines[type] = StartCoroutine(FreezeCoroutine(effectiveTime));
        }
        // 걸려있는 스턴이 빙결이 있을 때
        else
        {
            controlTypeAbnormalStatusesDurationMax[type] = controlTypeAbnormalStatusTime[type] + effectiveTime;
        }
    }

    private void Stun(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)ControlTypeAbnormalStatusType.STUN;
        StopControlTypeAbnormalStatus(ControlTypeAbnormalStatusType.CHARM);
        // 기존에 걸려있는 기절이 없을 때
        if (null == controlTypeAbnormalStatusCoroutines[type])
        {
            controlTypeAbnormalStatusCoroutines[type] = StartCoroutine(StunCoroutine(effectiveTime));
        }
        // 걸려있는 기절이 있을 때
        else
        {
            controlTypeAbnormalStatusesDurationMax[type] = controlTypeAbnormalStatusTime[type] + effectiveTime;
        }
    }

    private void Charm(float effectiveTime, float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        int type = (int)ControlTypeAbnormalStatusType.CHARM;
        if (isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.STUN] || isControlTypeAbnormalStatuses[(int)ControlTypeAbnormalStatusType.FREEZE])
            return;
        // 기존에 걸려있는 매혹이 없을 때
        if (null == controlTypeAbnormalStatusCoroutines[type])
        {
            controlTypeAbnormalStatusCoroutines[type] = StartCoroutine(CharmCoroutine(effectiveTime));
        }
        // 걸려있는 매혹이 있을 때
        else
        {
            controlTypeAbnormalStatusesDurationMax[type] = controlTypeAbnormalStatusTime[type] + effectiveTime;
        }
    }

    public void KnockBack(float knockBack, Vector2 bulletDir, Vector2 bulletPos, bool positionBasedKnockBack)
    {
        // 기본 상태에서 넉백
        if (null == checkingknockBackEnded)
        {
            AddRetrictsMovingCount();
            checkingknockBackEnded = StartCoroutine(CheckKnockbackEnded());
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

    #region AbnormalCoroutine
    protected abstract IEnumerator PoisonCoroutine();
    protected abstract IEnumerator BurnCoroutine();
    protected abstract IEnumerator FreezeCoroutine(float effectiveTime);
    protected abstract IEnumerator StunCoroutine(float effectiveTime);
    protected abstract IEnumerator CharmCoroutine(float effectiveTime);
    protected IEnumerator CheckDashEnded(float distance)
    {
        float dashDistanceTotal = 0;
        while (true)
        {
            //Debug.Log(rgbody.velocity + " | " + rgbody.velocity.magnitude + " | " + dashDistanceTotal);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            dashDistanceTotal += rgbody.velocity.magnitude;
            if (rgbody.velocity.magnitude < 0.2f || dashDistanceTotal >= distance)
            {
                rgbody.velocity = Vector2.zero;
                checkingDashEnded = null;
                isDash = false;
                break;
            }
        }
    }
    protected IEnumerator CheckKnockbackEnded()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (rgbody.velocity.magnitude < 0.2f)
            {
                SubRetrictsMovingCount();
                checkingknockBackEnded = null;
                break;
            }
        }
    }
    #endregion

    /// <summary>총알 외의 충돌로 인한 공격과 넉백 처리</summary>
    public abstract float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false);

    public void SetAimType(CharacterInfo.AimType aimType)
    {
        this.aimType = aimType;
    }

    public void SetSpawnType(CharacterInfo.SpawnType spawnType)
    {
        this.spawnType = spawnType;
    }

    protected abstract bool IsControlTypeAbnormal();

    public void Dash(float dashSpeed, float distance)
    {
        if(isDash)
        {
            StopCoroutine(checkingDashEnded);
            checkingDashEnded = StartCoroutine(CheckDashEnded(distance));
        }

        if (null == checkingDashEnded)
        {
            isDash = true;
            checkingDashEnded = StartCoroutine(CheckDashEnded(distance));
        }

        rgbody.velocity = Vector3.zero;
        rgbody.AddForce(dashSpeed * GetDirVector());
    }

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
        restrictMovingCount = 0;
        restrictAttackingCount = 0;

        for (int i = 0; i < (int)AttackTypeAbnormalStatusType.END; i++)
        {
            isAttackTypeAbnormalStatuses[i] = false;
            attackTypeAbnormalStatusCoroutines[i] = null;
        }

        for (int i = 0; i < (int)ControlTypeAbnormalStatusType.END; i++)
        {
            isControlTypeAbnormalStatuses[i] = false;
            controlTypeAbnormalStatusCoroutines[i] = null;
            controlTypeAbnormalStatusTime[i] = 0;
            controlTypeAbnormalStatusesDurationMax[i] = 0;
        }
    }

    public abstract void SelfDestruction();
}


