using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Weapon 화면에 그려질, 외형의 관한 것을 관리하는 클래스
 * [현재]
 * 1. scale x, y
 * 2. sprite
 * ------------------
 * [예정]
 * 1. view 관련된 것들 추가할게 있으면 더 추가될 예정.
 * 
 * ------------------
 * [미정]
 * 1. Animation 관리
 *  - Weapon class에서 하기
 *  - View class에서 하기
 *  - 따로 WeaponAnimation class 만들어서 하기
 * 2. rotation 관리도 이쪽에서 아마도 할 듯.
 */

// 지금은 WeaponView지만 나중에 상위 view 클래스 만들고 상속 하거나 해서
// Bullet, Effect, item 이런 쪽도? 써먹게끔 할 수도.

public class WeaponView {

    private Transform transform;
    private Vector3 scaleVecter = new Vector3(1f, 1f, 1f);
    private SpriteRenderer spriteRenderer;

    public WeaponView(Transform transform, SpriteRenderer spriteRenderer)
    {
        this.transform = transform;
        this.spriteRenderer = spriteRenderer;
    }

    public void Init(Sprite sprite, float scaleX, float scaleY)
    {
        this.spriteRenderer.sprite = sprite;
        SetScale(scaleX, scaleY);
    }
    // Overloading scale 설정. (x, y 같은 값 설정)
    public void SetScale(float scale)
    {
        // 1.5 => weaponManager scalce 크기
        scale = 1.5f + (scale - 1.0f);
        scaleVecter.x = scale;
        scaleVecter.y = scale;
        transform.localScale = scaleVecter;
    }

    // Overloading scale 설정. (x, y 다른 값 설정)
    public void SetScale(float scaleX, float scaleY)
    {
        // 1.5 => weaponManager scalce 크기
        scaleVecter.x = 1.5f + (scaleX - 1.0f);
        scaleVecter.y = 1.5f + (scaleY - 1.0f);
        transform.localScale = scaleVecter;
    }

    // Sprite Renderer안의 sprite 설정.
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
