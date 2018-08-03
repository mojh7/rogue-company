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
    Vector3 scale;
    //private void Awake()
    //{
    //    Transform = GetComponent<Transform>();
    //    Transform.eulerAngles = new Vector3(0, 0, 0);
    //    Transform.localScale = new Vector3(1, 1, 1);
    //    parentRenderer = Transform.parent.GetComponent<SpriteRenderer>();
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //}
    //private void Update()
    //{
    //    SetPosition();
    //}

    //void SetPosition()
    //{
    //    spriteRenderer.sprite = parentRenderer.sprite;
    //}

    private void Awake()
    {
        Transform = GetComponent<Transform>();
        Transform.eulerAngles = new Vector3(0, 0, 0);
        Transform.localScale = new Vector3(1, 1, 1);
        scale = new Vector3(1, 1, 0);
        parentRenderer = Transform.parent.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0, 0, 0,0.2f);
    }

    private void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        sprite = parentRenderer.sprite;
        if (sprite == null)
            return;
        offset = sprite.bounds.min.y;
        size = sprite.bounds.size.x;
        Transform.localPosition = new Vector2(0, offset);
        Transform.localScale = scale * size * 2.5f;
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
