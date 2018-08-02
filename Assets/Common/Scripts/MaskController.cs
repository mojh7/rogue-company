using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskController : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();
    }

    private void Update()
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }
}
