using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public CircleCollider2D interactiveCollider2D;

    public float moveSpeed;     // 플레이어 이동속도

    private float playerScale;      // player 크기
    private Vector3 scaleVector;    // player scale 우측 (1, 1, 1), 좌측 : (-1, 1, 1) 
    private float directionDegree;  // 바라보는 각도(총구 방향)
    private bool rightDirection;    // player 방향이 우측이냐(true) 아니냐(flase = 좌측)

    // Update is called once per frame
    void Update () {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
            Interact();
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    void Interact()
    {
        float bestDistance = interactiveCollider2D.radius*10;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, interactiveCollider2D.radius);

        for(int i=0;i< collider2D.Length; i++)
        {
            if (null == collider2D[i].GetComponent<CustomObject>())
                continue;
            if (!collider2D[i].GetComponent<CustomObject>().isAvailable)
                continue;
            float distance = Vector2.Distance(transform.position, collider2D[i].transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = collider2D[i];
            }
        }

        if (null == bestCollider)
            return;
        bestCollider.GetComponent<CustomObject>().Active();
    }
}

