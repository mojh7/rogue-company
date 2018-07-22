using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowClass : MonoBehaviour {

    Transform Transform;
    SpriteRenderer parentRenderer;
    SpriteRenderer spriteRenderer;
    Sprite sprite;
    float size;
    float offset;
    float sin10;

    private void Awake()
    {
        Transform = GetComponent<Transform>();
        Transform.eulerAngles = new Vector3(60, 0, 0);
        Transform.localScale = new Vector3(3, 3, 1);
        parentRenderer = Transform.parent.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        sprite = parentRenderer.sprite;
        offset = sprite.bounds.min.y;
        Transform.localPosition = new Vector2(0, offset);
    }

    //private void Awake()
    //{
    //    Transform = GetComponent<Transform>();
    //    Transform.eulerAngles = new Vector3(80, 0, 0);
    //    Transform.localScale = new Vector3(1, 1, 1);
    //    parentRenderer = Transform.parent.GetComponent<SpriteRenderer>();
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //    sin10 = Mathf.Sin(10 * Mathf.Deg2Rad);
    //}

    //private void Update()
    //{
    //    SetPosition();
    //}

    //void SetPosition()
    //{
    //    sprite = parentRenderer.sprite;
    //    spriteRenderer.sprite = sprite;
    //    size = sprite.bounds.size.y;
    //    offset = size * sin10;
    //    Transform.localPosition = new Vector2(0, -(size - offset) * 0.5f);
    //}
}
