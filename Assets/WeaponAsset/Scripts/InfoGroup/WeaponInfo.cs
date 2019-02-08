using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

public enum BulletPatternType { MultiDirPattern, RowPattern, LaserPattern }

[System.Serializable]
public struct BulletPatternEditInfo
{
    public BulletPatternInfo patternInfo;
    public int executionCount;      // 한 사이클에서의 실행 횟수
    public float delay;             // 사이클 내에서의 delay
    public bool isFixedOwnerDir;
    public bool isFixedOwnerPos;

    public BulletPatternEditInfo(BulletPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos)
    {
        this.patternInfo = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.isFixedOwnerDir = isFixedOwnerDir;
        this.isFixedOwnerPos = isFixedOwnerPos;
    }
}

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "WeaponData/OwnerPlayer/WeaponInfo", order = 0)]
public class WeaponInfo : ScriptableObject
{
    [SerializeField]
    private int inGameId;

    [SerializeField]
    [TextArea(3, 100)] public string description;

    protected CharacterInfo.OwnerType ownerType;

    [Header("기본 스펙")]
    // 기본 스펙
    public string weaponName;           // 무기 이름
    public Rating rating;
    public Sprite sprite;               // 무기 sprtie
    public float scaleX;                // 가로 크기 scale x
    public float scaleY;                // 세로 크기 scale y

    public AttackAniType attackAniType; // 무기 공격 애니메이션
    public TouchMode touchMode;         // 무기 공격 타입?? 일반 공격, 차징 공격
    public WeaponType weaponType;       // 무기 종류

    [Header("일단 임시로 ammoCapacity 값 -1 => 탄창 무한대")]
    public int ammoCapacity;            // 탄약 량
    public int ammo;                    // 현재 탄약
    public float chargeTimeMax;         // 차징 최대 시간, 0 = 차징 X
    public float bulletMoveSpeed;       // 총알 이동속도
    public float range;                 // 사정 거리
    public float damage;                // 공격력
    public float criticalChance;        // 크리티컬 확률 : 치명타가 뜰 확률
    public float cooldown;              // 쿨타임
    public float castingTime;           // 캐스팅 시간, 시전 시간
    public int staminaConsumption;    // 근접 무기 1회 공격시 스테미너 소모량

    public bool showsMuzzleFlash;       // 총구 화염 show on / off
    public MuzzleFlashType muzzleFlashType;
    [HideInInspector]
    public string muzzleFlashName;
    [Tooltip("soundId : 테스트용, soundName : 실제 사용 sound Name(soundId -1 값이어야 soundName 사용")]
    public int soundId;                 // 공격시 효과음 id, 테스트용
    public string soundName;            // 공격시 효과음 name
    public int SoundPlayCountMax;       // 효과음 재생 횟수
    public float cameraShakeAmount;     // 카메라 흔들림 양
    public float cameraShakeTime;       // 카메라 흔들림 시간

    [Tooltip("총알 발사시 초기 position이 중심에서 멀어지는 정도")]
    public float addDirVecMagnitude;    // onwer가 바라보는 방향의 벡터의 크기 값, bullet 초기 위치 = owner position + owner 방향 벡터 * addDirVecMagnitude
    [Tooltip("총알 발사시 바라보는 방향에 +90도인 수직 방향으로 멀어지는 정도")]
    public float additionalVerticalPos;

    [Header("공격 한 번에 n번 Cycle 실행")]
    public int cycleRepetitionCount = 1;
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
        soundId = -1;
        cycleRepetitionCount = 1;
    }

    public void SetWeaponId(int id) { inGameId = id; }
    public int GetWeaponId() { return inGameId; }

    public WeaponInfo Clone()
    {
        WeaponInfo clonedInfo = CreateInstance<WeaponInfo>();

        clonedInfo.ownerType = ownerType;

        clonedInfo.weaponName = weaponName;
        clonedInfo.rating = rating;
        clonedInfo.sprite = sprite;
        clonedInfo.scaleX = scaleX;
        clonedInfo.scaleY = scaleY;

        clonedInfo.attackAniType = attackAniType;
        clonedInfo.touchMode = touchMode;
        clonedInfo.weaponType = weaponType;

        clonedInfo.ammoCapacity = ammoCapacity;
        clonedInfo.ammo = ammo;
        clonedInfo.chargeTimeMax = chargeTimeMax;
        clonedInfo.bulletMoveSpeed = bulletMoveSpeed;
        clonedInfo.range = range;
        clonedInfo.damage = damage;
        clonedInfo.criticalChance = criticalChance;
        clonedInfo.cooldown = cooldown;
        clonedInfo.castingTime = castingTime;
        clonedInfo.staminaConsumption = staminaConsumption;

        clonedInfo.showsMuzzleFlash = showsMuzzleFlash;
        clonedInfo.muzzleFlashType = muzzleFlashType;
        clonedInfo.muzzleFlashName = muzzleFlashName;
        clonedInfo.soundId = soundId;
        clonedInfo.soundName = soundName;
        clonedInfo.cameraShakeAmount = cameraShakeAmount;
        clonedInfo.cameraShakeTime = cameraShakeTime;
        clonedInfo.addDirVecMagnitude = addDirVecMagnitude;
        clonedInfo.additionalVerticalPos = additionalVerticalPos;

        clonedInfo.cycleRepetitionCount = cycleRepetitionCount; 
        clonedInfo.bulletPatterns = new List<BulletPattern>();
        clonedInfo.bulletPatternsLength = bulletPatternsLength;

        for (int i = 0; i < clonedInfo.bulletPatternsLength; i++)
        {
            clonedInfo.bulletPatterns.Add(bulletPatterns[i].Clone());
        }

        return clonedInfo;
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
        // bulletPatternEditInfo를 토대로 실제 원래의 bulletPatterns 만들기, 각각 info마다 다운캐스팅으로 매개변수 넣어줌
        muzzleFlashName = muzzleFlashType.ToString();

        bulletPatternsLength = bulletPatternEditInfos.Length;
        bulletPatterns = new List<BulletPattern>();
        for(int i = 0; i < bulletPatternsLength; i++)
        {
            bulletPatterns.Add(BulletPatternInfo.CreatePatternInfo(bulletPatternEditInfos[i], ownerType).Clone());
        }
    }

}