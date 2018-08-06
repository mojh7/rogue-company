using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkil : MonoBehaviour {
    protected float damage;
    protected LayerMask enemyLayer;
    protected Animator animator;
    protected CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected bool isActvie;
    protected bool isAvailable;
    protected void DestroyAndDeactive()
    {
        isActvie = false;
        Destroy(this);
        this.gameObject.SetActive(false);
    }
    private void EndAnimation()
    {
        DestroyAndDeactive();
    }

    protected void Init()
    {
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}

public class CollisionSkill : ActiveSkil
{
    public void Init(Character character, float damage)
    {
        Init();
        isAvailable = true;
        isActvie = true;
        this.damage = damage;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        StartCoroutine(CoroutineUpdate());
    }

    public void Init(Character character, float damage, string skillName)
    {
        Init();
        animator.SetTrigger(skillName);
        isAvailable = true;
        isActvie = true;
        this.damage = damage;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);

        StartCoroutine(CoroutineUpdate());
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

    IEnumerator CoroutineUpdate()
    {
        while(isActvie)
        {
            circleCollider.radius = spriteRenderer.sprite.bounds.size.x * 0.5f;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }
}
