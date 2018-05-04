 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponData;
using BulletData;
/*
 * Weapon, Bullet, Effect 고유 정보 저장소
 * 
 * 나중에 에디터랑 정보 저장(xml, json 등등) 이런거 고려해서
 * 일단 임시로 따로 빼놨음 언제든지 구조랑 내용 데이터 다루는
 * 방식 바뀔 예정.
 * 
 * 
 */
namespace DelegateCollection
{
    public delegate float DelGetDirDegree();    // 총구 방향 각도
    public delegate Vector3 DelGetPosition();   // owner position이지만 일단 player position 용도로만 사용.
}

// 무기 관련 정보
namespace WeaponData
{
    public enum WeaponType { Blow, Strike, Swing, Gun, ShotGun, Laser }
    public enum AttackAniType { Blow, Strike, Shot }
    public enum TouchMode { Normal, Charge }

    public struct WeaponInfo
    { 
        // 기본 스펙
        public string name;                 // 무기 이름
        public int spriteId;                // 무기 sprtie id
        public AttackAniType attackAniType;
        public TouchMode touchMode;
        public WeaponType weaponType;

        public float chargeTime;            // 차징 시간, 0 = 차징 X, 0 초과 = 1회 차징당 시간
        public int ammoCapacity;            // 탄약 량
        public int ammo;                    // 현재 탄약
        public float scaleX;                // 가로 크기 scale x
        public float scaleY;                // 세로 크기 scale y
        public float bulletMoveSpeed;       // 총알 이동속도
        public float range;                 // 사정 거리
        public float damage;                // 공격력
        public float criticalRate;          // 크리티컬 확률 : 치명타가 뜰 확률
        public float knockBack;             // 넉백, 값이 0 이면 넉백 X
        public float cooldown;              // 쿨타임

        //public int chargeCountMax;          // 차징 횟수 최대치 일단 5로 잡고 테스트
        // 풀 차징시에만 공격되니 없앨 예정 public float damagePerCharging;     // 차징 1회 당 데미지 증가량
        public float addDirVecMagnitude;    // onwer가 바라보는 방향의 벡터의 크기 값, bullet 초기 위치 = owner position + owner 방향 벡터 * addDirVecMagnitude
        
        // 총알 패턴 정보
        public BulletPattern[] originalBulletPatterns; // 패턴 종류, 해당 패턴 id
        // 이름, spriteId, 탄약 량, 현재 탄약, 공격력, 크리티컬 확률, 넉백, 쿨타임, 차징 total 시간, 차징 횟수, 차징 당 데미지 증가량
        public WeaponInfo(string name, int spriteId, AttackAniType attackAniType, TouchMode touchMode, WeaponType weaponType, float chargeTime, int ammoCapacity, int ammo, float scaleX, float scaleY,
            float bulletMoveSpeed, float range, float damage, float criticalRate, float knockBack, float cooldown, float addDirVecMagnitude,
            BulletPattern[] originalBulletPatterns) 
            //, int chargeCountMax = 5)
        {
            this.name = name;
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
            this.originalBulletPatterns = originalBulletPatterns;

            //this.chargeCountMax = chargeCountMax;
        }
    }

    public struct WeaponBuffInfo
    {
        // 단순히 능력치 증가 감소
        #region ability
        public float cooldownIncreaseRate;         // 쿨타임 증가율
        public float damageIncreaseRate;           // 공격력 증가율
        public float criticalRateIncreaseRate;     // 치명타 확률 증가율      
        public float knockBackIncreaseRate;        // 넉백 증가율
        public float ammoCapacityIncreaseRate;     // 탄창 Maximum 증가율, int형으로 갯수로 해야 될 수도

        public float bulletScaleIncreaseRate;      // 총알 크기 증가율
        public float bulletRangeIncreaseRate;      // 총알 사정 거리 증가율
        public float bulletSpeedIncreaseRate;      // 총알 속력 증가율

        public float chargeTimeIncreaseRate;       // 차징 시간 증가율
        public float chargeDamageIncreaseRate;     // 차징 공격 데미지 증가율

        // 임시
        public int bulletCountIncreaseCount;      // 다방향 패턴 총알 갯수 증가
        #endregion

        // 속성 추가 삭제
        #region property
        // 추가 속성
        public List<CollisionProperty> addCollisionProperties;
        public List<UpdateProperty> addUpdateProperties;
        public List<DeleteProperty> addDeleteProperties;
        // 추가 속성 길이
        public int addCollisionPropertiesLength;
        public int addUpdatePropertiesLength;
        public int addDeletePropertiesLength;
        // buffManager에서 buffList에 속성 추가할 때에 첫 번째 index
        public int addedCollisionPropertyFirstIndex;
        public int addedUpdatePropertyFirstIndex;
        public int addedDeletePropertyFirstIndex;
        #endregion
    }
}   // namespace weaponData

namespace BulletData
{
    // 총알 삭제 함수 델리게이트
    public delegate void DelDestroyBullet();
    // 총알 충돌 함수 델리게이트
    public delegate void DelCollisionBullet(Collider2D coll);

    //public enum PatternType { MultiDir, Laser };

    // 다방향 패턴 정보
    public struct MultiDirPatternInfo
    {
        public int bulletCount;     // 총알 갯수
        public float initAngle;     // 초기 각도, -initAngle 으로 계산됨, 시계 방향
        public float deltaAngle;    // 총알간의 각도 차이
        public float randomAngle;   // 각 총알에서 random값으로 보정이 필요할 때.
        public float speed;         // 총알 속도
        public float range;         // 사정 거리
        public float damage;        // 총알 한 발 당 데미지
        public int bulletId;        // 총알 Id
        public int bulletSpriteId;  // 총알 sprite Id;
        public MultiDirPatternInfo(int bulletCount, float initAngle, float deltaAngle, float randomAngle, float speed = 0, float range = 0)
        {
            this.bulletCount = bulletCount;
            this.initAngle = initAngle;
            this.deltaAngle = deltaAngle;
            this.randomAngle = randomAngle;
            this.speed = speed;
            this.range = range;
            this.damage = 0;
            this.bulletId = 0;
            this.bulletSpriteId = 0;
        }
    }

    // 일렬로 총알 생성 패턴 정보
    public struct RowPatternInfo
    {
        public int bulletCount;     // 총알 갯수
        public float initPos;       // 초기 위치, 오른쪽 방향(0도)기준으로 시계방향 위치에서 반시계 방향 위치로 생성 -initPos 으로 계산됨
        public float deltaPos;      // 총알간의 위치 차이
        public float randomAngle;   // 총알 random값으로 각도 보정
        public float speed;         // 총알 속도
        public float range;         // 사정 거리
        public float damage;        // 총알 한 발 당 데미지
        public int bulletId;        // 총알 Id
        public int bulletSpriteId;  // 총알 sprite Id;
        public RowPatternInfo(int bulletCount, float initPos, float deltaPos, float randomAngle, float speed = 0, float range = 0)
        {
            this.bulletCount = bulletCount;
            this.initPos = initPos;
            this.deltaPos = deltaPos;
            this.randomAngle = randomAngle;
            this.speed = speed;
            this.range = range;
            this.damage = 0;
            this.bulletId = 0;
            this.bulletSpriteId = 0;
        }
    }

    // 레이저 패턴 정보
    // 레이저 폭, 스프라이트나, 메터리얼, 텍스쳐 등 이미지 설정 변수 있어야 됨.
    public struct LaserPatternInfo
    {
        public int bulletId;        // 총알 Id, laserUpdate와 laserDelete 속성을 꼭 가진 총알 만
        public int damage;          // 공격력

        // 주로 어떤 레이저, 레이저 색깔이나 모양 형태? 이런거 위주로 정보 담을 예정

        public LaserPatternInfo(int bulletId = 0, int damage = 0)
        {
            this.bulletId = bulletId;
            this.damage = damage;
        }
    }

    //public struct DeleteAfterSummonProperty

    // 총알 정보
    public struct BulletInfo
    {
        public int spriteId;
        public int damage;
        public float speed;
        public float range;
        public float scaleX;
        public float scaleY;
        
        // 속성 정보
        public CollisionProperty[] originalCollisionProperties; // 충돌 속성 원본
        public UpdateProperty[] originalUpdateProperties;       // update 속성 원본
        public DeleteProperty[] originalDeleteProperties;      // 삭제 속성 원본
        public BulletInfo(CollisionProperty[] originalCollisionProperties, UpdateProperty[] originalUpdateProperties, DeleteProperty[] originalDeleteProperties, float speed = 0, float range = 0)
        {
            this.scaleX = 1f;// scaleX;
            this.scaleY = 1f;// scaleY;
            this.originalCollisionProperties = originalCollisionProperties;
            this.originalUpdateProperties = originalUpdateProperties;
            this.originalDeleteProperties = originalDeleteProperties;

            this.speed = speed;
            this.range = range;
            this.spriteId = 0;// spriteId;
            this.damage = 0;// damage;
        }
    }
} // namespace BulletData


// 각종 데이터 실제로 저장해서 모아놓은 클래스.
public class DataStore : MonoBehaviour
{
    #region variables
    [SerializeField]
    private Sprite[] weaponSpriteList;
    [SerializeField]
    private Sprite[] bulletSpriteList;

    private static DataStore instance;
    private List<WeaponInfo> weaponInfo;
    private List<MultiDirPatternInfo> multiDirPatternInfo;
    private List<RowPatternInfo> RowPatternInfo;
    private List<LaserPatternInfo> laserPatternInfo;
    private List<BulletInfo> bulletInfo;
    #endregion

    #region getter
    public static DataStore Instance { get { return instance; } }

    public Sprite GetWeaponSprite(int id) { return weaponSpriteList[id]; }
    public Sprite GetBulletSprite(int id) { return bulletSpriteList[id]; }
    public WeaponInfo GetWeaponInfo(int id) { return weaponInfo[id]; }
    public MultiDirPatternInfo GetMultiDirPatternInfo(int id) { return multiDirPatternInfo[id]; }
    public RowPatternInfo GetRowPatternInfo(int id) { return RowPatternInfo[id]; }
    public LaserPatternInfo GetLaserPatternInfo(int id) { return laserPatternInfo[id]; }
    public BulletInfo GetBulletInfo(int id) { return bulletInfo[id]; }
    #endregion

    #region setter
    #endregion

    #region UnityFunction
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        InitBulletInfo();
        InitWepaonInfo();
    }
    #endregion
    #region Function

    // 무기 정보 관련 초기화
    public void InitWepaonInfo()
    {
        weaponInfo = new List<WeaponInfo>
        {
            // 지금은 테스트 용도로 하나하나씩 써놓았지만 에디터 만들고 나면 xml이나 json 불러다가 쓸 예정 
            // 권총
            new WeaponInfo("권총", 1, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1f, 1f, 10f, 7f, 3f, 0.1f, 2/7f, 0.2f, 0.5f, new BulletPattern[]{ new MultiDirPattern(0, 1, 1, 1, 0) })
            // 닌자 수리검
            ,new WeaponInfo("닌자 수리검", 2, AttackAniType.Shot, TouchMode.Normal, WeaponType.Gun, 0f, 170, 170, 1f, 1f, 5f, 5f, 7, 0.15f, 0.3f, 1f, 0.5f, new BulletPattern[]{ new MultiDirPattern(1, 1, 2, 1, 0) })
            // 산탄총
            ,new WeaponInfo("산탄총", 3, AttackAniType.Shot, TouchMode.Charge, WeaponType.ShotGun, 1.0f, 180, 180, 1f, 1f, 8f, 3f, 8, 0.2f, 0f, 0.5f, 0.5f, new BulletPattern[]{ new MultiDirPattern(2, 1, 1, 1, 0) })
            // 레이저건
            ,new WeaponInfo("레이저건", 4, AttackAniType.Shot, TouchMode.Normal, WeaponType.Laser, 0f, -1, 0, 1f, 1f, 0f, 100f, 6, 0.15f, 0f, 0f, 0.7f, new BulletPattern[]{ new LaserPattern(0, 2) })

            // 테스트 총 1 : 총알에서 총알 생성
            ,new WeaponInfo("테스트1", 5, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1, 1f, 6f, 8f, 3f, 0.1f, 2/7f, 0.4f, 0.5f, new BulletPattern[]{ new MultiDirPattern(0, 3, 4, 1, 0) })
            // 테스트 총 2 : row 패턴 적용
            ,new WeaponInfo("테스트2", 6, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1, 1f, 6f, 8f, 3f, 0.1f, 2/7f, 0.4f, 0.5f, new BulletPattern[]{ new RowPattern(0, 1, 6, 1, 0) })
            // 테스트 총 3 : row 패턴 적용 2
            ,new WeaponInfo("테스트3", 7, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1, 1f, 6f, 8f, 3f, 0.1f, 2/7f, 0.6f, 0.5f, new BulletPattern[]{ new RowPattern(1, 1, 7, 3, 0.1f) })
            // 테스트 총 4 : multi 패턴
            ,new WeaponInfo("테스트4", 8, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1, 1f, 6f, 8f, 3f, 0.1f, 2/7f, 0.8f, 0.5f, new BulletPattern[]{ new MultiDirPattern(5, 1, 1, 15, 0.05f) })
            // 테스트 총 5 : multi 패턴
            ,new WeaponInfo("테스트5", 9, AttackAniType.Shot , TouchMode.Normal, WeaponType.Gun, 0f, 100, 100, 1, 1f, 6f, 8f, 3f, 0.1f, 2/7f, 0.6f, 0.5f, new BulletPattern[]{ new MultiDirPattern(3, 1, 1, 1, 0) })
        };
    }

    // 총알 정보 관련 초기화 
    public void InitBulletInfo()
    {
        multiDirPatternInfo = new List<MultiDirPatternInfo>()
        {
            /* 총알 갯수, 초기 각도, 총알간의 각도 차이, 총알 마다 각도 보정 random값(-vaule~vaule)
             */
            new MultiDirPatternInfo(1, 0, 0, 5f),
            new MultiDirPatternInfo(8, 0f, 45f, 0f),
            new MultiDirPatternInfo(6, 75f, 30f, 0f),
            new MultiDirPatternInfo(8, 0f, 0f, 60f),
            new MultiDirPatternInfo(2, 90f, 180f, 0f, 5f, 3f),
            new MultiDirPatternInfo(1, 0, 0, 30f),
        };

        RowPatternInfo = new List<RowPatternInfo>()
        {
            /* 총알 갯수, 초기 위치, 총알간의 위치 차이
             */
            new RowPatternInfo(2, 0.1f, 0.2f, 7f),
            new RowPatternInfo(3, 0.5f, 0.5f, 1f)
        };

        laserPatternInfo = new List<LaserPatternInfo>()
        {
            new LaserPatternInfo()
        };



        bulletInfo = new List<BulletInfo>()
        {
            // 총알 정보에는 딱 충돌, update, 삭제 속성 중에서 어떤 것을 가질지에 대한 정보만 담을 것 같고
            // 상세 수치는 이 총알을 참조하게 되는 bulletPattern이나 weapon쪽에 상세 수치(range, damage, speed 등등)를 이어 받아서 사용하려고 함.
            new BulletInfo(),   // 빈 속성
            // 일반 총알
            new BulletInfo(new CollisionProperty[]{new BaseNormalCollisionProperty() }, new UpdateProperty[]{new StraightMoveProperty() }, new DeleteProperty[]{new BaseDeleteProperty() }),
            // 레이저 총알   
            new BulletInfo(new CollisionProperty[]{new LaserCollisionProperty() }, new UpdateProperty[]{new LaserUpdateProperty() }, new DeleteProperty[]{new LaserDeleteProperty() }),    
            // 소환속성 포함 총알
            new BulletInfo(new CollisionProperty[]{new BaseNormalCollisionProperty() }, new UpdateProperty[]{new StraightMoveProperty(), new SummonProperty(new MultiDirPattern(4, 1, 5, 1, 0), 10) }, new DeleteProperty[]{new BaseDeleteProperty() }),
        };
    }
    #endregion
}