using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour {
    CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Init(Sprite sprite)
    {
        circleCollider.offset = new Vector2(0, sprite.bounds.min.y + circleCollider.radius * .5f);
    }
}
