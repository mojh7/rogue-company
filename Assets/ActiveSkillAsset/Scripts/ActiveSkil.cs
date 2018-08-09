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

    protected bool isActvie;
    protected bool isAvailable;
    protected System.Action<Character, object, float> action;

    protected void DestroyAndDeactive()
    {
        isActvie = false;
        Destroy(this);
        this.gameObject.SetActive(false);
    }

    public void LapseAnimation()
    {
        action.Invoke(character, temporary, amount);
    }

    public void EndAnimation()
    {
        DestroyAndDeactive();
    }

    protected void Init()
    {
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        UtilityClass.Invoke(this, DestroyAndDeactive, 100);
    }
}

public class CollisionSkill : ActiveSkil
{
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAvailable)
            return;
        if (UtilityClass.CheckLayer(collision.gameObject.layer, enemyLayer))
        {
            isAvailable = false;
            Character character = collision.GetComponent<Character>();
            character.Attacked(Vector2.zero, transform.position, damage, 0, 0);
        }
    }   
}
