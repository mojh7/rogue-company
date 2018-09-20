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

    protected void Init()
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
    StatusEffectInfo statusEffectInfo;

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
    }

    public void Init(Character character, float damage, float radius)
    {
        Init();
        isAvailable = true;
        isActvie = true;
        this.damage = damage;
        this.radius = radius;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        circleCollider.radius = radius;
    }

    public void Init(Character character, float damage, string skillName)
    {
        Init();
        animator.SetTrigger(skillName);
        isAvailable = true;
        isActvie = true;
        this.damage = damage;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        StartCoroutine(ColliderUpdate());
    }

    public void Init(Character character, float time, float radius, StatusEffectInfo statusEffectInfo, string skillName, Color color, string particleName)
    {
        Init();
        this.radius = radius;
        this.statusEffectInfo = statusEffectInfo;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);
        this.isAvailable = true;
        transform.localScale = new Vector3(1, 1, 1) * radius;
        spriteRenderer.sortingLayerName = "Background";
        circleCollider.radius = 0.3f;
        animator.SetTrigger(skillName);
        spriteRenderer.color = color;
        ParticleManager.Instance.PlayParticle(particleName, this.transform.position, 0.3f * radius, time);
        UtilityClass.Invoke(this, DestroyAndDeactive, time);
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            isAvailable = false;
            Character character = collision.GetComponent<Character>();
            if (statusEffectInfo == null)
                character.Attacked(Vector2.zero, transform.position, damage, 0, 0);
            character.ApplyStatusEffect(statusEffectInfo);
        }
    }   
}

public class ThrowingSkill : ActiveSkil
{
    SkillData importedSkill;
    Vector3 src, dest, direction;
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

    public void Init(Character character, object temporary, int idx, string skillName, SkillData skillData, float speed, float acceleration)
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

        this.speed = speed;
        this.acceleration = acceleration;

        this.isActive = false;
    }

    private void Throwing()
    {
        animator.SetTrigger(skillName);
        StartCoroutine(CoroutineThrow());
    }

    IEnumerator CoroutineThrow()
    {
        float dist = Vector2.Distance(dest, src);
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
            this.importedSkill.Run(this.character, this.transform.position, this.idx);
        }
        DestroyAndDeactive();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            StopCoroutine(CoroutineThrow());
            isActive = true;
            isAvailable = false;
            animator.SetTrigger("default");
            this.importedSkill.Run(this.character, this.transform.position, this.idx);
            DestroyAndDeactive();
        }
    }
}