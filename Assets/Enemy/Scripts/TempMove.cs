using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMove : MonoBehaviour {

    Enemy m_enemy;
    Player m_player;
    float m_speed = 2;
    Rigidbody2D rg;
    Vector3 scale = new Vector3(1, 1, 1);
    float size;
    Vector2 direction;

    //추적
    public void Chase()
    {
        direction = (m_player.GetPosition() - transform.position).normalized;
        if(direction.x > 0)
        {
            scale.x = -size;
            transform.localScale = scale;
        }
        else
        {
            scale.x = size;
            transform.localScale = scale;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
    private void Start()
    {
        m_enemy = GetComponent<Enemy>();
        m_player = PlayerManager.Instance.GetPlayer();
        m_speed = m_enemy.moveSpeed;
        rg = GetComponent<Rigidbody2D>();
        scale = transform.localScale;
        size = scale.x;
    }

    private void Update()
    {
        Chase();
    }
    private void FixedUpdate()
    {
        rg.velocity = direction * m_speed;
    }
}
