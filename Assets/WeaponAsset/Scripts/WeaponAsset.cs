using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponAsset
{
    public delegate float DelGetDirDegree();    // 총구 방향 각도
    public delegate Vector3 DelGetPosition();   // owner position이지만 일단 player position 용도로만 사용.

    public enum WeaponState { Idle, Attack, Reload, Charge, Switch, PickAndDrop }
    /// <summary>
    /// 190206 이전 17개
    /// 원거리 : 권총, 산탄총, 기관총, 저격소총, 레이저, 활, 지팡이, 원거리 특수
    /// 근거리 기반 : 창, 몽둥이, 스포츠용품, 검, 청소도구, 주먹장착무기, 근거리 특수
    /// 폭발형? : 폭탄, 접근발동무기
    /// 
    /// 190206 이후 16개
    /// 권총, 산탄총, 기관총, 저격 소총, 레이저, 활, 지팡이, 원거리 특수
    /// 창, 몽둥이, 스포츠용품, 검, 청소도구, 주먹장착무기, 근거리 특수
    /// 폭발형 무기
    /// </summary>
    // END 는 WeaponType 총 갯수를 알리기 위해서 enum 맨 끝에 기입 했음.
    public enum WeaponType
    {
        NULL = 0, PISTOL, SHOTGUN, MACHINEGUN, SNIPER_RIFLE, LASER, BOW,
        SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        BOMB = 13,
        WAND = 15, MELEE_SPECIAL, RANGED_SPECIAL, END
    }

    // PISTOL, SHOTGUN, MACHINEGUN, SNIPLER_RIFLE, LASER, BOW
    public enum AttackAniType { None, Blow, Strike, Swing, Punch, Shot }
    public enum AttackType { MELEE, RANGED, TRAP }
    public enum TouchMode { Normal, Charge }
    public enum BulletType { PROJECTILE, LASER, MELEE, NULL, MINE, EXPLOSION }
    public enum BulletPresetType
    {
        None, YELLOW_CIRCLE, RED_CIRCLE, SKYBLUE_BASH, BLUE_BASH, RED_BASH, ORANGE_BASH,
        BLUE_CIRCLE, SKYBLUE_CIRCLE, PINK_CIRCLE, VIOLET_CIRCLE, EMPTY,
        YELLOW_BULLET, BLUE_BULLET, PINK_BULLET, VIOLET_BULLET, RED_BULLET, GREEN_BULLET,
        YELLOW_BEAM, BLUE_BEAM, PINK_BEAM, VIOLET_BEAM, RED_BEAM, GREEN_BEAM, GREEN_CIRCLE,
        YELLOW_BULLET2, SKYBLUE_BULLET2, BLUE_BULLET2, PINK_BULLET2, VIOLET_BULLET2, RED_BULLET2, GREEN_BULLET2,
        BLUE_THICK_DONUT, BLUE_THIN_DONUT, BLUE_SQUARE, BLUE_RHOMBUS, BLUE_TRIANGLE, BLUE_X_SHAPE,
        GREEN_THICK_DONUT, GREEN_THIN_DONUT, GREEN_SQUARE, GREEN_RHOMBUS, GREEN_TRIANGLE, GREEN_X_SHAPE,
        PURPLE_THICK_DONUT, PURPLE_THIN_DONUT, PURPLE_SQUARE, PURPLE_RHOMBUS, PURPLE_TRIANGLE, PURPLE_X_SHAPE,
        RED_THICK_DONUT, RED_THIN_DONUT, RED_SQUARE, RED_RHOMBUS, RED_TRIANGLE, RED_X_SHAPE,
        YELLOW_THICK_DONUT, YELLOW_THIN_DONUT, YELLOW_SQUARE, YELLOW_RHOMBUS, YELLOW_TRIANGLE, YELLOW_X_SHAPE,
    };
    /*---*/
    public enum BulletParticleType { NONE, BASIC }
    public enum ImpactParticleType { NONE, BASIC }
    public enum TrailParticleType { NONE, BASIC }
    public enum MuzzleFlashType
    {
        NONE, BASIC, MUZZLE_FLASH_FROST, MUZZLE_FLASH_FIRE_BALL_BLUE, MUZZLE_FLASH_FIRE_BALL_GREEN, MUZZLE_FLASH_FIRE_BALL_PURPLE, MUZZLE_FLASH_FIRE_BALL_RED, MUZZLE_FLASH_FIRE_BALL_YELLOW,
        MUZZLE_FLASH_SPIKY_YELLOW
    }

    public enum BulletPropertyType { Collision, Update, Delete }
    public enum CollisionPropertyType { BaseNormal, Laser, Undeleted }
    public enum UpdatePropertyType { StraightMove, AccelerationMotion, Laser, Summon, Homing, MineBomb, FixedOwner, Spiral, Rotation, Child, TRIGONOMETRIC }
    public enum DeletePropertyType { BaseDelete, Laser, SummonBullet, SummonPattern }
    public enum BehaviorPropertyType { SpeedControl, Rotate }

    /*---*/

    public enum ColliderType { NONE, Beam, AUTO_SIZE_BOX, AUTO_SIZE_CIRCLE, MANUAL_SIZE_BOX, MANUAL_SIZE_CIRCLE, MANUAL_SIZE_RHOMBUS, MANUAL_SIZE_TRIANGLE }

    public enum BulletAnimationType
    {
        NotPlaySpriteAnimation,
        BashAfterImage,
        PowerBullet,
        Wind,
        BashAfterImage2,
        Explosion0,
        BashSkyBlue,
        BashBlue,
        BashRed,
        BashOrange,
        PaperShot
    }

    public enum BulletImpactType
    {
        NONE, BasicImpactRed, BasicImpactYellow, BasicImpactGreen, BasicImpactBlue, BasicImpactPurple, BasicImpactPink
    }

    /*---*/


    // 총알 삭제 함수 델리게이트
    public delegate void DelDestroyBullet();
    // 총알 충돌 함수 델리게이트
    public delegate void DelCollisionBullet(Collider2D coll);
}
