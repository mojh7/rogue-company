using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterInfo
{
    public enum OwnerType
    {
        Player, Enemy, Pet, Object
    }
    public enum State
    {
        NOTSPAWNED, DIE, ALIVE
    }
    public enum DamageImmune
    {
        NONE, DAMAGE
    }
    // 생각 중, 
    public enum AbnormalImmune
    {
        NONE
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
    public float moveSpeed;     // Character move Speed
    public float hp; // protected인데 debug용으로 어디서든 접근되게 public으로 했고 현재 hpUI에서 접근
    protected float maxHP;
    protected CharacterInfo.DamageImmune damageImmune;
    protected CharacterInfo.AutoAimType autoAimType;
    protected CharacterInfo.AutoAimType originalautoAimType;
    protected CharacterInfo.State pState;
    protected CharacterInfo.OwnerType ownerType;
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
    #endregion
    #region variables
    public bool isCasting;
    protected bool isActiveAI;
    protected bool isActiveMoveAI;
    protected bool isActiveAttackAI;

    protected bool isEvade = false;
    protected bool isKnockBack = false;
    [SerializeField]
    protected Sprite sprite;

    protected bool isAutoAiming;    // 오토에임 적용 유무
    protected Vector3 directionVector;
    protected float directionDegree;  // 바라보는 각도(총구 방향)

    protected bool isRightDirection;    // character 방향이 우측이냐(true) 아니냐(flase = 좌측)

    /// <summary> owner 좌/우 바라볼 때 spriteObject scale 조절에 쓰일 player scale, 우측 (1, 1, 1), 좌측 : (-1, 1, 1) </summary>
    protected Vector3 scaleVector;
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
        return transform.position;
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

    public bool GetIsAcitveAttackAI()
    {
        return isActiveAttackAI;
    }
    public bool GetIsAcitveMoveAI()
    {
        return isActiveMoveAI;
    }
    #endregion
    #region Func
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
            StartCoroutine(ColorChange(Color.red));
    }

    IEnumerator ColorChange(Color color)
    {
        spriteRenderer.color = Color.red;
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
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

        isActiveAI = true;
        isCasting = false;
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

    /// <summary> 상태 이상 효과 적용 </summary>
    public virtual void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        // Enemy랑 Player랑 효과 다르게 받아야 될 게 생길 듯
    }


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

    protected abstract bool IsAbnormal();

    public abstract bool Evade();

    public bool IsEvade()
    {
        return isEvade;
    }
}


