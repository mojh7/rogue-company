 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
/*
 * Weapon, Bullet, Effect 고유 정보 저장소
 * 
 * 나중에 에디터랑 정보 저장(xml, json 등등) 이런거 고려해서
 * 일단 임시로 따로 빼놨음 언제든지 구조랑 내용 데이터 다루는
 * 방식 바뀔 예정.
 * 
 * 
 */
namespace WeaponAsset
{
    public delegate float DelGetDirDegree();    // 총구 방향 각도
    public delegate Vector3 DelGetPosition();   // owner position이지만 일단 player position 용도로만 사용.


    public enum WeaponType { Blow, Strike, Swing, Gun, ShotGun, Laser }
    public enum AttackAniType { Blow, Strike, Shot }
    public enum TouchMode { Normal, Charge }

    /*---*/

    public enum ColiderType { Box, Circle }

    public enum BulletAnimationType
    {
        NotPlaySpriteAnimation = 0,
        BashAfterImage = 1,
        PowerBullet = 2,
        Wind = 3
    }

    // 총알 삭제 함수 델리게이트
    public delegate void DelDestroyBullet();
    // 총알 충돌 함수 델리게이트
    public delegate void DelCollisionBullet(Collider2D coll);

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
}


// 각종 데이터 실제로 저장해서 모아놓은 클래스.
public class DataStore : MonoBehaviourSingleton<DataStore>
{
    #region variables
    [SerializeField]
    private WeaponInfo[] weaponInfos;
    [SerializeField]
    private MultiDirPatternInfo[] multiDirPatternInfos;
    [SerializeField]
    private RowPatternInfo[] rowPatternInfos;
    [SerializeField]
    private LaserPatternInfo[] laserPatternInfos;
    [SerializeField]
    private BulletInfo[] bulletInfos;
    [SerializeField]
    private EffectInfo[] effectInfos;

    #endregion

    #region getter

    public WeaponInfo GetWeaponInfo(int id) { return weaponInfos[id].Clone(); }
    public MultiDirPatternInfo GetMultiDirPatternInfo(int id) { return multiDirPatternInfos[id]; }
    public RowPatternInfo GetRowPatternInfo(int id) { return rowPatternInfos[id]; }
    public LaserPatternInfo GetLaserPatternInfo(int id) { return laserPatternInfos[id]; }
    public BulletInfo GetBulletInfo(int id) { return bulletInfos[id].Clone(); }
    public EffectInfo GetEffectInfo(int id) { return effectInfos[id]; }
    #endregion


    #region setter
    #endregion


    #region UnityFunction
    void Awake()
    {
        InitBulletInfo();
        InitWepaonInfo();
    }

    #endregion
    #region Function

    // 무기 정보 관련 초기화
    public void InitWepaonInfo()
    {
        for(int i = 0; i < weaponInfos.Length; i++)
        {
            weaponInfos[i].Init();
        }
    }

    // 총알 정보 관련 초기화 
    public void InitBulletInfo()
    {
        for (int i = 0; i < bulletInfos.Length; i++)
        {
            bulletInfos[i].Init();
        }
    }
    #endregion
}