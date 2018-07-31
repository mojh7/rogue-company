using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkil : MonoBehaviour {
    protected float damage;
    protected LayerMask enemyLayer;

    protected bool isAvailable;
    protected void DestroyAndDeactive()
    {
        Destroy(this);
        this.gameObject.SetActive(false);
    }
    private void EndAnimation()
    {
        DestroyAndDeactive();
    }
}

public class CollisionSkill : ActiveSkil
{
    public void Init(Character character, float damage, string skillName)
    {
        GetComponent<Animator>().SetTrigger(skillName);
        isAvailable = true;
        this.damage = damage;
        this.enemyLayer = UtilityClass.GetEnemyLayer(character);
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
