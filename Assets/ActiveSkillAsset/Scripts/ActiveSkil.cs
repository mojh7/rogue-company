using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkil : MonoBehaviour {
    protected float damage;
    protected LayerMask enemyLayer;
    protected Animator animator;
    protected CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected Character character;
    protected object temporary;
    protected float amount;
    protected float radius;
    protected int idx;

    protected bool isActvie;
    protected bool isAvailable;
    protected System.Action<Character, object, float> action;

    public virtual void SetAvailableTrue()
    {
        isAvailable = true;
    }
    public virtual void SetAvailableFalse()
    {
        isAvailable = false;
    }
    protected void DestroyAndDeactive()
    {
        isActvie = false;
        Destroy(this);
        this.gameObject.SetActive(false);
    }

    public virtual void LapseAnimation()
    {
        action.Invoke(character, temporary, amount);
    }

    public virtual void EndAnimation()
    {
        DestroyAndDeactive();
    }

    protected virtual void Init()
    {
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.color = Color.white;
        transform.localScale = new Vector3(1, 1, 1);
        animator.SetTrigger("default");

        DestroySelf();
    }

    protected IEnumerator ColliderUpdate()
    {
        while (isActvie)
        {
            if(spriteRenderer.sprite)
                circleCollider.radius = spriteRenderer.sprite.bounds.size.x * 0.5f;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }

    void DestroySelf()
    {
        UtilityClass.Invoke(this, DestroyAndDeactive, 30);
    }
}

public class CollisionSkill : ActiveSkil
{
    protected StatusEffectInfo statusEffectInfo;
    protected SkillData skillData;
    protected CBulletEffect.EffectType effectType;

    public void Init(Character character, object temporary, float amount, System.Action<Character, object, float> action)
    {
        Init();

        this.character = character;
        this.temporary = temporary;
        this.amount = amount;
        this.action = action;
        isAvailable = true;
        isActvie = true;
        circleCollider.radius = 0;
    } // CFlash - 스킬 함수 전달 시전자 애니메이션으로 종료 선언

    public void Init(Character character, float damage, float radius)
    {
        Init();

        isAvailable = true;
        isActvie = true;
        this.character = character;
        this.damage = damage;
        this.radius = radius;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        circleCollider.radius = radius;
    } //CRangeAttack with animation  - 시전자 애니메이션으로 종료 선언

    public void Init(Character character, float time, float damage, float radius)
    {
        Init();

        isAvailable = true;
        isActvie = true;
        this.character = character;
        this.damage = damage;
        this.radius = radius;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        circleCollider.radius = radius;
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    } // CRangeAttack - 애니메이션 없는 객체

    public void Init(Character character, float time, float damage, float radius, SkillData skillData)
    {
        Init();

        isAvailable = true;
        isActvie = true;
        this.character = character;
        this.damage = damage;
        this.radius = radius;
        this.skillData = skillData;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        circleCollider.radius = radius;
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    } // CRangeAttack - 애니메이션 없는 객체

    public void Init(Character character, float damage, string skillName)
    {
        Init();

        animator.SetTrigger(skillName);
        isAvailable = true;
        isActvie = true;
        this.character = character;
        this.damage = damage;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        StartCoroutine(ColliderUpdate());
    } // HandUp, HandClap - 자체 애니메이션 객체

    public void Init(Character character, float time, float damage, float radius, StatusEffectInfo statusEffectInfo, string skillName, Color color, string particleName)
    {
        Init();
        this.character = character;
        this.radius = radius;
        this.statusEffectInfo = statusEffectInfo;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);
        this.isAvailable = true;
        transform.localScale = new Vector3(1, 1, 1) * radius;
        spriteRenderer.sortingLayerName = "Background";
        circleCollider.radius = 0.3f;
        if(skillName.Length > 0)
            animator.SetTrigger(skillName);
        spriteRenderer.color = color;
        ParticleManager.Instance.PlayParticle(particleName, this.transform.position, 0.3f * radius, time);
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    } // CAbnormal - 자체 애니메이션 객체

    public void InitBullet(Character character, float time, float damage, float radius,CBulletEffect.EffectType effectType)
    {
        Init();

        isAvailable = true;
        isActvie = true;
        this.character = character;
        this.damage = damage;
        this.radius = radius;
        this.effectType = effectType;
        this.enemyLayer = UtilityClass.GetEnemyBulletLayer(character);

        circleCollider.radius = radius;
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    } // CRangeAttack - 애니메이션 없는 객체

    public override void SetAvailableFalse()
    {
        base.SetAvailableFalse();
        radius = circleCollider.radius;
        circleCollider.radius = 0;
    }

    public override void SetAvailableTrue()
    {
        base.SetAvailableTrue();
        circleCollider.radius = radius;
        radius = 0;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            Character triggeredCharacter = collision.GetComponent<Character>();
            if (triggeredCharacter)
            {
                triggeredCharacter.Attacked(Vector2.zero, transform.position, damage, 0, 0);
                triggeredCharacter.ApplyStatusEffect(statusEffectInfo);
                if (skillData)
                    skillData.Run(this.character, this.transform.position, this.idx);
            }
            else
            {
                Bullet bullet = collision.transform.parent.GetComponent<Bullet>();
                if (!bullet)
                    return;
                switch (effectType)
                {
                    case CBulletEffect.EffectType.REMOVE:
                        bullet.DestroyBullet();
                        break;
                    case CBulletEffect.EffectType.REFLECT:
                        bullet.SetOwnerType(UtilityClass.GetOnwerTypeLayer(character));
                        bullet.RotateDirection(180);
                        break;
                    default:
                        break;
                }

            }
        }
    }   
}

public class ThrowingSkill : ActiveSkil
{
    SkillData importedSkill;
    Vector3 src, dest, direction;
    float dist;
    float speed, acceleration;
    string skillName;
    bool isActive;

    public override void LapseAnimation()
    {
        Throwing();
    }

    public override void EndAnimation()
    {
    }

    public void Init(Character character, object temporary, float radius, int idx, string skillName, SkillData skillData, float speed, float acceleration)
    {
        Init();
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);
        this.character = character;
        this.temporary = temporary;
        this.idx = idx;
        this.skillName = skillName;
        this.importedSkill = skillData;
        this.src = character.transform.position;
        this.dest = (Vector3)temporary;
        this.direction = (dest - src).normalized;
        this.dist = Vector2.Distance(dest, src);
        this.isAvailable = true;
        this.speed = speed;
        this.acceleration = acceleration;
        circleCollider.radius = radius;
        this.isActive = false;
    }

    private void Throwing()
    {
        animator.SetTrigger(skillName);
        if(this)
            StartCoroutine(CoroutineThrow());
    }

    IEnumerator CoroutineThrow()
    {
        float elapsedDist = 0;
        float elapsedTime = 0;
        while (elapsedDist <= dist && speed > 0)
        {
            transform.localPosition = transform.localPosition + direction * speed * Time.deltaTime;
            elapsedDist += speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            speed += acceleration * elapsedTime * Time.deltaTime;
            if (isActive)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

        if(!isActive)
        {
            animator.SetTrigger("default");
            ParticleManager.Instance.PlayParticle("SpikyImpact", transform.position, this.importedSkill.radius * .3f);
            this.importedSkill.Run(this.character, this.transform.position, this.idx);
        }
        DestroyAndDeactive();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer) || 
            UtilityClass.CheckLayer(collision.gameObject.layer, 14, 1))
        {
            StopCoroutine(CoroutineThrow());
            isActive = true;
            isAvailable = false;
            animator.SetTrigger("default");
            ParticleManager.Instance.PlayParticle("SpikyImpact", transform.position, this.importedSkill.radius * .3f);
            this.importedSkill.Run(this.character, this.transform.position, this.idx);                                          
            DestroyAndDeactive();
        }
    }
}