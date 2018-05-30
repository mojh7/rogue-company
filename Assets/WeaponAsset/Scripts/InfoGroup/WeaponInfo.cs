using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

public enum BulletPatternType { MultiDirPattern, RowPattern, LaserPattern }

[System.Serializable]
public struct BulletPatternEditInfo
{
    public BulletPatternType type;  // 패턴 타입
    public int id;                  // 해당 패턴의 id
    public int executionCount;      // 한 사이클에서의 실행 횟수
    public float delay;             // 사이클 내에서의 delay

    public BulletPatternEditInfo(BulletPatternType type, int id, int executionCount, float delay, Sprite bulletSprite)
    {
        this.type = type;
        this.id = id;
        this.executionCount = executionCount;
        this.delay = delay;
    }
}

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "GameData/WeaponInfo", order = 0)]
public class WeaponInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    [Header("Owner 꼭 설정 해주세요")]
    [SerializeField] protected OwnerType ownerType;

    [Header("기본 스펙")]
    // 기본 스펙
    public string weaponName;           // 무기 이름
    public Sprite sprite;               // 무기 sprtie
    public float scaleX;                // 가로 크기 scale x
    public float scaleY;                // 세로 크기 scale y

    public AttackAniType attackAniType; // 무기 공격 애니메이션
    public TouchMode touchMode;         // 무기 공격 타입?? 일반 공격, 차징 공격
    public WeaponType weaponType;       // 무기 종류

    [Header("일단 임시로 ammoCapacity 값 -1 => 탄창 무한대")]
    public int ammoCapacity;            // 탄약 량
    public int ammo;                    // 현재 탄약
    public float bulletMoveSpeed;       // 총알 이동속도
    public float range;                 // 사정 거리
    public float damage;                // 공격력
    public float criticalRate;          // 크리티컬 확률 : 치명타가 뜰 확률
    public float knockBack;             // 넉백, 값이 0 이면 넉백 X
    public float cooldown;              // 쿨타임
    public float chargeTime;            // 차징 시간, 0 = 차징 X, 0 초과 = 1회 차징당 시간

    public int soundId;                 // 공격시 효과음 id

    [Tooltip("총알 발사시 초기 position이 중심에서 멀어지는 정도")]
    public float addDirVecMagnitude;    // onwer가 바라보는 방향의 벡터의 크기 값, bullet 초기 위치 = owner position + owner 방향 벡터 * addDirVecMagnitude

    // edit용 총알 패턴 정보
    public BulletPatternEditInfo[] bulletPatternEditInfos; // 패턴 종류, 해당 패턴 id
    // 총알 패턴 정보
    public List<BulletPattern> bulletPatterns; // 패턴 종류, 해당 패턴 id
    [HideInInspector]
    public int bulletPatternsLength;

    public WeaponInfo()
    {
        scaleX = 1.0f;
        scaleY = 1.0f;

        ammoCapacity = -1;
    }

    
    /*
    public WeaponInfo(WeaponInfo info)
    {
        Debug.Log(name);
        weaponName = info.weaponName;
        sprite = info.sprite;
        scaleX = info.scaleX;
        scaleY = info.scaleY;

        attackAniType = info.attackAniType;
        touchMode = info.touchMode;
        weaponType = info.weaponType;

        ammoCapacity = info.ammoCapacity;
        ammo = info.ammo;
        bulletMoveSpeed = info.bulletMoveSpeed;
        range = info.range;
        damage = info.damage;
        criticalRate = info.criticalRate;
        knockBack = info.knockBack;
        cooldown = info.cooldown;
        chargeTime = info.chargeTime;
        addDirVecMagnitude = info.addDirVecMagnitude;

        bulletPatterns = new List<BulletPattern>();
        bulletPatternsLength = info.bulletPatternsLength;

        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            bulletPatterns.Add(info.bulletPatterns[i].Clone());
        }
        
    }*/
    /*
    public WeaponInfo(string weaponName, int spriteId, AttackAniType attackAniType, TouchMode touchMode, WeaponType weaponType, float chargeTime, int ammoCapacity, int ammo, float scaleX, float scaleY,
        float bulletMoveSpeed, float range, float damage, float criticalRate, float knockBack, float cooldown, float addDirVecMagnitude,
        BulletPattern[] bulletPatterns)
    {
        this.name = weaponName;
        this.spriteId = spriteId;
        this.attackAniType = attackAniType;
        this.touchMode = touchMode;
        this.weaponType = weaponType;
        this.chargeTime = chargeTime;

        this.ammoCapacity = ammoCapacity;
        this.ammo = ammo;

        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.bulletMoveSpeed = bulletMoveSpeed;
        this.range = range;
        this.damage = damage;
        this.criticalRate = criticalRate;
        this.knockBack = knockBack;
        this.cooldown = cooldown;
        this.addDirVecMagnitude = addDirVecMagnitude;
        this.bulletPatterns = bulletPatterns;
    }*/


    public WeaponInfo Clone()
    {
        WeaponInfo info = CreateInstance<WeaponInfo>();

        info.ownerType = ownerType;

        info.weaponName = weaponName;
        info.sprite = sprite;
        info.scaleX = scaleX;
        info.scaleY = scaleY;

        info.attackAniType = attackAniType;
        info.touchMode = touchMode;
        info.weaponType = weaponType;

        info.ammoCapacity = ammoCapacity;
        info.ammo = ammo;
        info.bulletMoveSpeed = bulletMoveSpeed;
        info.range = range;
        info.damage = damage;
        info.criticalRate = criticalRate;
        info.knockBack = knockBack;
        info.cooldown = cooldown;
        info.chargeTime = chargeTime;
        info.addDirVecMagnitude = addDirVecMagnitude;
        info.soundId = soundId;

        info.bulletPatterns = new List<BulletPattern>();
        info.bulletPatternsLength = bulletPatternsLength;

        for (int i = 0; i < info.bulletPatternsLength; i++)
        {
            info.bulletPatterns.Add(bulletPatterns[i].Clone());
        }

        return info;
    }

    public void Init()
    {
        /*
        if(name == "WeaponInfo11")
        {
            Debug.Log(name);
            name = "zzz123123";
            Debug.Log(name);
        }
        */
        // bulletPatternEditInfo를 토대로 실제 원래의 bulletPatterns 만들기
        bulletPatternsLength = bulletPatternEditInfos.Length;
        bulletPatterns = new List<BulletPattern>();
        for(int i = 0; i < bulletPatternsLength; i++)
        {
            switch(bulletPatternEditInfos[i].type)
            {
                case BulletPatternType.MultiDirPattern :
                    bulletPatterns.Add(new MultiDirPattern(bulletPatternEditInfos[i].id, bulletPatternEditInfos[i].executionCount, bulletPatternEditInfos[i].delay, ownerType));
                    break;
                case BulletPatternType.RowPattern:
                    bulletPatterns.Add(new RowPattern(bulletPatternEditInfos[i].id, bulletPatternEditInfos[i].executionCount, bulletPatternEditInfos[i].delay, ownerType));
                    break;
                case BulletPatternType.LaserPattern:
                    bulletPatterns.Add(new LaserPattern(bulletPatternEditInfos[i].id, ownerType));
                    break;
                default:
                    break;
            }
        }
    }

}