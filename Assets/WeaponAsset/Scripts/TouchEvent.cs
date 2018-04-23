using System.Collections;
using UnityEngine;

// 현재 상황에서는 안 쓸 것 같다.


public abstract class WeaponTouchEvent {

    public Weapon weapon;

    public void Init(Weapon weapon) { this.weapon = weapon; }

    public abstract void MouseDown(Transform position, Vector2 dest);

    public abstract void MouseUp(Transform position, Vector2 dest);
}

class NormalMode : WeaponTouchEvent
{
    // 공격 시작
    public override void MouseDown(Transform position, Vector2 dest)
    {
        
    }
    // 공격 중지
    public override void MouseUp(Transform position, Vector2 dest) { }
}

class ChargeMode : WeaponTouchEvent
{
    // 차징
    public override void MouseDown(Transform position, Vector2 dest)
    {
        
    }

    // 차징된 세기 만큼 공격 실행
    public override void MouseUp(Transform position, Vector2 dest)
    {
        
    }
}