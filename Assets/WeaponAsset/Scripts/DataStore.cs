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

    public enum WeaponState { Idle, Attack, Reload, Charge, Switch, PickAndDrop }
    /// <summary>원거리 : 권총, 산탄총, 기관총, 저격소총, 레이저, 활<br>123</br>
    /// 근거리 : 창, 몽둥이, 스포츠용품, 검, 청소도구, 주먹장착무기
    /// 함정 : 폭탄, 가스탄, 접근발동무기
    /// 특수 : 지팡이, 쓰레기
    /// </summary>
    public enum WeaponType
    {
        NULL, PISTOL, SHOTGUN, MACHINEGUN, SNIPLER_RIFLE, LASER, BOW,
        SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        BOMB, GAS_SHELL, TRAP,
        WAND, TRASH
    }

    // PISTOL, SHOTGUN, MACHINEGUN, SNIPLER_RIFLE, LASER, BOW
    public enum AttackAniType { NULL, BLOW, STRIKE, SWING, PUNCH, SHOT }
    public enum TouchMode { Normal, Charge }
    public enum BulletType { projectile, Laser, Melee}
    /*---*/

    public enum BulletPropertyType { Collision, Update, Delete }
    public enum CollisionPropertyType { BaseNormal, Laser, Undeleted }
    public enum UpdatePropertyType { StraightMove, AccelerationMotion, Laser, Summon, Homing }
    public enum DeletePropertyType { BaseDelete, Laser, SummonBullet, SummonPattern }

    /*---*/

    public enum ColiderType { Box, Circle }

    public enum BulletAnimationType
    {
        NotPlaySpriteAnimation,
        BashAfterImage,
        PowerBullet,
        Wind,
        BashAfterImage2,
        Explosion0,
    }

    /*---*/


    // 총알 삭제 함수 델리게이트
    public delegate void DelDestroyBullet();
    // 총알 충돌 함수 델리게이트
    public delegate void DelCollisionBullet(Collider2D coll);
}


// 각종 데이터 실제로 저장해서 모아놓은 클래스.
public class DataStore : MonoBehaviourSingleton<DataStore>
{
    #region variables
    [SerializeField]
    private WeaponInfo[] weaponInfos;
    [SerializeField]
    private WeaponInfo[] enemyWeaponInfos;
    [SerializeField]
    private UsableItemInfo[] passiveItemInfos;

    [SerializeField]
    private EffectInfo[] effectInfos;

    
    // private List<BulletInfo> initializedBulletInfosAtRuntime;
    // private int initializedBulletInfosLength;

    #endregion

    #region getter

    /// <summary> 0529 Player 무기 디버그용, 현재 weaponInfos에 있는 무기 전체 길이를 반환 </summary>
    public int GetWeaponInfosLength()
    {
        return weaponInfos.Length;
    }
    public int GetPassiveItemInfosLength()
    {
        return passiveItemInfos.Length;
    }

    // TODO : Player외의 owner에서 데이터 저장할 때 좀 더 좋은 구조로 개선해야됨.(asset 폴더 내에 저장시 현재 셋팅이 player 위주로 되어있음)

    /// <summary>
    /// Owner에 따른 Weapon Data 반환, ownerType 기본 값 Player
    /// </summary>
    /// <param name="id"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public WeaponInfo GetWeaponInfo(int id, CharacterInfo.OwnerType ownerType = CharacterInfo.OwnerType.Player)
    {
        switch(ownerType)
        {
            case CharacterInfo.OwnerType.Player:
                return weaponInfos[id];
            // 구분 만 해놓고 아직 player 이외의 owner weaponDataList 안 만듬, 봐서 bullet, Pattern도 이렇게 처리 할듯
            case CharacterInfo.OwnerType.Enemy:
                return enemyWeaponInfos[id];
            case CharacterInfo.OwnerType.Object:
            default:
                break;
        }
        return null;
    }

    public EffectInfo GetEffectInfo(int id) { return effectInfos[id]; }

    public UsableItemInfo GetPassiveItemInfo(int id)
    {
        return passiveItemInfos[id];
    }
    #endregion


    #region setter
    #endregion


    #region UnityFunction
    void Awake()
    {
        InitWepaonInfo();
        // initializedBulletInfosAtRuntime = new List<BulletInfo>();
        // initializedBulletInfosLength = 0;
    }

    #endregion
    #region Function

    /// <summary> 무기 정보 관련 초기화 </summary>
    public void InitWepaonInfo()
    {
        for(int i = 0; i < weaponInfos.Length; i++)
        {
            weaponInfos[i].Init();
        }
        for (int i = 0; i < enemyWeaponInfos.Length; i++)
        {
            enemyWeaponInfos[i].Init();
        }
        //passiveItems
    }

    /*
    public void AddInitialedbulletInfo(BulletInfo info)
    {
        initializedBulletInfosAtRuntime.Add(info);
        initializedBulletInfosLength += 1;
    }
    */
    #endregion

    
    /*
    private void OnApplicationQuit()
    {
        DebugX.Log("DataStore OnApplicationQuit");
        if(null != initializedBulletInfosAtRuntime)
        {
            for(int i = 0; i < initializedBulletInfosLength; i++)
            {
                initializedBulletInfosAtRuntime[i].SetIsInitializable(true);
            }
            initializedBulletInfosAtRuntime = null;
            DebugX.Log("DataStore OnApplicationQuit BulletInfo Init 실행 여부 on");
        }
    }

    public void Dispose()
    {
        DebugX.Log("DataStore Dispose");
        if (null != initializedBulletInfosAtRuntime)
        {
            for (int i = 0; i < initializedBulletInfosLength; i++)
            {
                initializedBulletInfosAtRuntime[i].SetIsInitializable(true);
            }
            initializedBulletInfosAtRuntime = null;
            DebugX.Log("DataStore Dispose BulletInfo Init 실행 여부 on");
        }
    }*/
}