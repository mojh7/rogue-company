using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WeaponAsset;
// BulletProperty.cs

[System.Serializable]
public abstract class BulletProperty
{
    protected GameObject bulletObj;
    protected Bullet bullet;
    protected Transform bulletTransform;
    protected DelDestroyBullet delDestroyBullet;
    protected DelCollisionBullet delCollisionBullet;

    /// <summary>
    /// bullet class에 정보를 받아와서 속성에 맞는 초기화
    /// </summary>
    /// <param name="bullet"></param>
    public virtual void Init(Bullet bullet)
    {
        this.bullet = bullet;
        this.bulletObj = bullet.gameObject;
        this.bulletTransform = bullet.objTransform;
        delDestroyBullet = bullet.DestroyBullet;
    }
    //protected WeaponState.Owner owner;
}