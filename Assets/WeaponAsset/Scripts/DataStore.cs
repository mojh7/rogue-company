using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using UnityEngine.Serialization;
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
    /// <summary>
    /// 원거리 : 권총, 산탄총, 기관총, 저격소총, 레이저, 활
    /// 근거리 : 창, 몽둥이, 스포츠용품, 검, 청소도구, 주먹장착무기
    /// 함정 : 폭탄, 가스탄, 접근발동무기
    /// 특수 : 지팡이, 쓰레기
    /// </summary>
    // END 는 WeaponType 총 갯수를 알리기 위해서 enum 맨 끝에 기입 했음.
    public enum WeaponType
    {
        NULL, PISTOL, SHOTGUN, MACHINEGUN, SNIPER_RIFLE, LASER, BOW,
        SPEAR, CLUB, SPORTING_GOODS, SWORD, CLEANING_TOOL, KNUCKLE,
        BOMB, GAS_SHELL, TRAP,
        WAND, TRASH, OTHER, END
    }

    // PISTOL, SHOTGUN, MACHINEGUN, SNIPLER_RIFLE, LASER, BOW
    public enum AttackAniType { NULL, BLOW, STRIKE, SWING, PUNCH, SHOT }
    public enum AttackType { MELEE, RANGED }
    public enum TouchMode { Normal, Charge }
    public enum BulletType { PROJECTILE, LASER, MELEE, NULL, MINE, EXPLOSION}
    /*---*/

    public enum BulletPropertyType { Collision, Update, Delete }
    public enum CollisionPropertyType { BaseNormal, Laser, Undeleted }
    public enum UpdatePropertyType { StraightMove, AccelerationMotion, Laser, Summon, Homing, MineBomb, FixedOwner }
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
    //[FormerlySerializedAs("tests")] 이거 선언, 이전 이름, 새로운 변수 명 한 번에 해야됨.
    private WeaponInfo[] tempWeaponInfos;
    [SerializeField]
    private WeaponInfo[] enemyWeaponInfos;
    [SerializeField]
    private UsableItemInfo[] clothingItemInfos;
    [SerializeField]
    private UsableItemInfo[] etcItemInfos;
    [SerializeField]
    private UsableItemInfo[] foodItemInfos;
    [SerializeField]
    private UsableItemInfo[] medicalItemInfos;
    [SerializeField]
    private UsableItemInfo[] miscItemInfos;
    [SerializeField]
    private UsableItemInfo[] petItemInfos;

    [SerializeField]
    private EffectInfo[] effectInfos;

    [Header("true하고 실행 시 엑셀 내용으로 무기 초기화")]
    [SerializeField]
    private bool canInputWeaponDatas;
    public List<Dictionary<string, object>> weaponDatas;


    // private List<BulletInfo> initializedBulletInfosAtRuntime;
    // private int initializedBulletInfosLength;

    #endregion

    #region getter

    
    public int GetWeaponInfosLength()
    {
        if (WeaponModeForDebug.TEST == DebugSetting.Instance.weaponModeForDebug)
            return weaponInfos.Length;
        else
            return tempWeaponInfos.Length;
    }
    public int GetTempWeaponInfosLength()
    {
        return tempWeaponInfos.Length;
    }

    public int GetClothingItemInfosLength()
    {
        return clothingItemInfos.Length;
    }

    public int GetEtcItemInfosLength()
    {
        return etcItemInfos.Length;
    }

    public int GetFoodItemInfosLength()
    {
        return foodItemInfos.Length;
    }

    public int GetMedicalItemInfosLength()
    {
        return medicalItemInfos.Length;
    }

    public int GetMiscItemInfosLength()
    {
        return miscItemInfos.Length;
    }

    public int GetPetItemInfosLength()
    {
        return petItemInfos.Length;
    }

    public int GetEnemyWeaponInfosLength()
    {
        return enemyWeaponInfos.Length;
    }

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
                if (WeaponModeForDebug.TEST == DebugSetting.Instance.weaponModeForDebug)
                    return weaponInfos[id];
                else
                    return tempWeaponInfos[id];
            // 구분 만 해놓고 아직 player 이외의 owner weaponDataList 안 만듬, 봐서 bullet, Pattern도 이렇게 처리 할듯
            case CharacterInfo.OwnerType.Enemy:
                return enemyWeaponInfos[id];
            case CharacterInfo.OwnerType.Object:
            default:
                break;
        }
        return null;
    }

    public WeaponInfo GetTempWeaponInfo(int id)
    {
        return tempWeaponInfos[id];

    }

    public EffectInfo GetEffectInfo(int id) { return effectInfos[id]; }

    public UsableItemInfo GetClothingItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, clothingItemInfos.Length);
        return clothingItemInfos[id];
    }
    public UsableItemInfo GetEtcItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, etcItemInfos.Length);
        return etcItemInfos[id];
    }
    public UsableItemInfo GetFoodItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, foodItemInfos.Length);
        return foodItemInfos[id];
    }
    public UsableItemInfo GetMedicalItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, medicalItemInfos.Length);
        return medicalItemInfos[id];
    }
    public UsableItemInfo GetMiscItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, miscItemInfos.Length);
        return miscItemInfos[id];
    }

    public UsableItemInfo GetPetItemInfo(int id)
    {
        if (-1 == id)
            id = Random.Range(0, petItemInfos.Length);
        return petItemInfos[id];
    }
    #endregion


    #region setter
    #endregion


    #region UnityFunction
    void Awake()
    {
        InitWepaonInfo();
        InitMiscItems();
        // initializedBulletInfosAtRuntime = new List<BulletInfo>();
        // initializedBulletInfosLength = 0;
    }

    #endregion
    #region Function

    private void InitMiscItems()
    {
        for (int i = 0; i < miscItemInfos.Length; i++)
            miscItemInfos[i].SetID(i);
    }

    /// <summary> 무기 정보 관련 초기화 </summary>
    public void InitWepaonInfo()
    {
        if(WeaponModeForDebug.TEST == DebugSetting.Instance.weaponModeForDebug)
        {
            for (int i = 0; i < weaponInfos.Length; i++)
                weaponInfos[i].Init();
        }
        else
        {
            for (int i = 0; i < tempWeaponInfos.Length; i++)
                tempWeaponInfos[i].Init();
        }
        
        for (int i = 0; i < enemyWeaponInfos.Length; i++)
        {
            enemyWeaponInfos[i].Init();
        }
        //passiveItems
        
        InputWeaponDatas();
    }

    /*
    public void AddInitialedbulletInfo(BulletInfo info)
    {
        initializedBulletInfosAtRuntime.Add(info);
        initializedBulletInfosLength += 1;
    }
    */

    public void InputWeaponDatas()
    {
        if (false == canInputWeaponDatas)
            return;
        weaponDatas = WeaponDataCSVParser.Read("weaponDatas");
        Debug.Log("CSV 데이터 파싱 후 weapon data 입력");

        WeaponType weaponType;
        AttackAniType attackAniType;
        Rating rating;
        float chargeTimeMax;
        float criticalChance;
        float damage;
        float staminaConsumption;
        float cooldown;
        int ammoCapacity;
        float range;
        float bulletSpeed;
        int size = weaponDatas.Count;
        for (int i = 0; i < size; i++)
        {
            tempWeaponInfos[i].name = (string)weaponDatas[i]["name"];
            Debug.Log(i + ", name : " + (string)weaponDatas[i]["name"]);
        
            weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), (string)weaponDatas[i]["weaponType"]);
            tempWeaponInfos[i].weaponType = weaponType;

            switch (weaponType)
            {
                case WeaponType.PISTOL:
                case WeaponType.SHOTGUN:
                case WeaponType.MACHINEGUN:
                case WeaponType.SNIPER_RIFLE:
                    tempWeaponInfos[i].showsMuzzleFlash = true;
                    break;
                default:
                    tempWeaponInfos[i].showsMuzzleFlash = false;
                    break;
            }

            switch (weaponType)
            {
                case WeaponType.SPEAR:
                case WeaponType.CLUB:
                case WeaponType.SPORTING_GOODS:
                case WeaponType.SWORD:
                case WeaponType.CLEANING_TOOL:
                    tempWeaponInfos[i].addDirVecMagnitude = 1.2f;
                    break;
                case WeaponType.KNUCKLE:
                case WeaponType.SHOTGUN:
                case WeaponType.BOW:
                case WeaponType.WAND:
                case WeaponType.SNIPER_RIFLE:
                    tempWeaponInfos[i].addDirVecMagnitude = 0.5f;
                    break;
                case WeaponType.LASER:
                case WeaponType.MACHINEGUN:
                case WeaponType.TRASH:
                case WeaponType.OTHER:
                    tempWeaponInfos[i].addDirVecMagnitude = 0.3f;
                    break;
                case WeaponType.PISTOL:
                    tempWeaponInfos[i].addDirVecMagnitude = 0.2f;
                    break;
                default:
                    tempWeaponInfos[i].addDirVecMagnitude = 0f;
                    break;
            }

            attackAniType = (AttackAniType)System.Enum.Parse(typeof(AttackAniType), (string)weaponDatas[i]["attackAniType"]);
            tempWeaponInfos[i].attackAniType = attackAniType;
            //Debug.Log(attackAniType);

            rating = (Rating)System.Enum.Parse(typeof(Rating), (string)weaponDatas[i]["rating"]);
            tempWeaponInfos[i].rating = rating;
            //Debug.Log(rating);


            float.TryParse(weaponDatas[i]["chargeTimeMax"].ToString(), out chargeTimeMax);
            tempWeaponInfos[i].chargeTimeMax = chargeTimeMax;
            //Debug.Log(chargeTimeMax);
            if (0 == chargeTimeMax)
                tempWeaponInfos[i].touchMode = TouchMode.Normal;
            else
                tempWeaponInfos[i].touchMode = TouchMode.Charge;

            float.TryParse(weaponDatas[i]["criticalChance"].ToString(), out criticalChance);
            tempWeaponInfos[i].criticalChance = criticalChance;
            //Debug.Log(criticalChance);

            float.TryParse(weaponDatas[i]["damage"].ToString(), out damage);
            tempWeaponInfos[i].damage = damage;
            //Debug.Log(damage);

            float.TryParse(weaponDatas[i]["staminaConsumption"].ToString(), out staminaConsumption);
            tempWeaponInfos[i].staminaConsumption = staminaConsumption;
            //Debug.Log(staminaConsumption);

            float.TryParse(weaponDatas[i]["cooldown"].ToString(), out cooldown);
            tempWeaponInfos[i].cooldown = cooldown;
            //Debug.Log(cooldown);

            int.TryParse(weaponDatas[i]["ammoCapacity"].ToString(), out ammoCapacity);
            tempWeaponInfos[i].ammoCapacity = ammoCapacity;
            //Debug.Log(ammoCapacity);

            float.TryParse(weaponDatas[i]["range"].ToString(), out range);
            tempWeaponInfos[i].range = range;
            //Debug.Log(range);

            float.TryParse(weaponDatas[i]["bulletSpeed"].ToString(), out bulletSpeed);
            tempWeaponInfos[i].bulletMoveSpeed = bulletSpeed;
            //Debug.Log(bulletSpeed);
        }
        
    }
    #endregion
}