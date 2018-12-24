using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillObject : MonoBehaviour
{
    protected LayerMask enemyLayer, enemyBulletLayer;
    protected CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected List<SkillData> preSkillData, postSkillData;

    protected Character caster, other;
    protected CustomObject customObject;

    protected bool isActive;
    protected bool isAvailable;
    #region skillDataParameter
    protected float radius;
    protected float amount;
    protected string animName;
    #endregion

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    protected void Init(SkillData skillData)
    {
        isActive = true;
        isAvailable = true;
        radius = skillData.Radius;
        amount = skillData.Amount;
        circleCollider.radius = radius;
    }
    public void Init(string aniName)
    {
        animator.SetTrigger(aniName);
    }
    public void Init(Character other)
    {
        this.other = other;
    }
    public void Init(ref CustomObject customObject, SkillData skillData, float time)
    {
        Init(skillData);
        this.customObject = customObject;
        this.enemyLayer = UtilityClass.GetEnemyLayer(null);
        this.enemyBulletLayer = UtilityClass.GetEnemyBulletLayer(null);
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    }
    public void Init(ref Character caster, SkillData skillData, float time)
    {
        Init(skillData);
        this.caster = caster;
        this.enemyLayer = UtilityClass.GetEnemyLayer(caster);
        this.enemyBulletLayer = UtilityClass.GetEnemyBulletLayer(caster);
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    }
    public void SetSkillData(List<SkillData> preSkillData, List<SkillData> postSkillData)
    {
        this.preSkillData = preSkillData;
        this.postSkillData = postSkillData;
    }
    protected void DestroyAndDeactive()
    {
        isAvailable = false;
        Destroy(this);
        this.gameObject.SetActive(false);
    }
}

public class CollisionSkillObject : SkillObject
{
    protected StatusEffectInfo statusEffectInfo;
    protected CRangeEffect.EffectType effectType;

    public void Set(StatusEffectInfo statusEffectInfo)
    {
        this.statusEffectInfo = statusEffectInfo;
    }
    public void Set(CRangeEffect.EffectType effectType)
    {
        this.effectType = effectType;
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            Character triggeredCharacter = collision.GetComponent<Character>();
            triggeredCharacter.Attacked(Vector2.zero, transform.position, amount, 0, 0);
            triggeredCharacter.ApplyStatusEffect(statusEffectInfo);
        }
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyBulletLayer))
        {
            Bullet bullet = collision.transform.parent.GetComponent<Bullet>();
            if (!bullet)
                return;
            switch (effectType)
            {
                case CRangeEffect.EffectType.REMOVE:
                    bullet.DestroyBullet();
                    break;
                case CRangeEffect.EffectType.REFLECT:
                    bullet.SetOwnerType(UtilityClass.GetOnwerTypeLayer(caster));
                    bullet.RotateDirection(180);
                    break;
                case CRangeEffect.EffectType.NONE:
                default:
                    break;
            }
        }
    }
}

public class ProjectileSkillObject : SkillObject
{
    float speed, acceleration;
    Vector3 direction;

    public void Set(string animName, float speed, float acceleration, Vector3 direction)
    {
        this.speed = speed;
        this.acceleration = acceleration;
        this.direction = direction.normalized;
        animator.SetTrigger(animName);

        StartCoroutine(CoroutineThrow());
    }

    IEnumerator CoroutineThrow()
    {
        float elapsedDist = 0;
        float elapsedTime = 0;
        while (speed > 0)
        {
            transform.localPosition = transform.localPosition + direction * speed * Time.deltaTime;
            elapsedDist += speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            speed += acceleration * elapsedTime * Time.deltaTime;
            if (!isActive)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

        if (postSkillData != null && postSkillData.Count > 0)
        {
            animator.SetTrigger("default");
            float lapsedTime = 9999;
            foreach (SkillData item in postSkillData)
            {
                if (other)
                    item.Run(caster, other, transform.position, ref lapsedTime);
                else if (caster)
                    item.Run(caster, transform.position, ref lapsedTime);
                if (customObject)
                    item.Run(customObject, transform.position, ref lapsedTime);
            }
        }

        DestroyAndDeactive();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer) ||
            UtilityClass.CheckLayer(collision.gameObject.layer, 14, 1))
        {
            StopCoroutine(CoroutineThrow());
            isAvailable = false;
            animator.SetTrigger("default");
            float lapsedTime = 9999;
            foreach (SkillData item in postSkillData)
            {
                if (other)
                    item.Run(caster, other, transform.position, ref lapsedTime);
                else if (caster)
                    item.Run(caster, transform.position, ref lapsedTime);
                if (customObject)
                    item.Run(customObject, transform.position, ref lapsedTime);
            }

            DestroyAndDeactive();
        }
    }
}