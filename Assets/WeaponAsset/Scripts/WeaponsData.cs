using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

public class WeaponsData : MonoBehaviourSingleton<WeaponsData>
{
    #region variables
    [SerializeField]
    private WeaponInfo[] testWeaponInfos;
    [SerializeField]
    //[FormerlySerializedAs("tempWeaponInfos")], UnityEngine.Serialization에 있음.
    //이거 선언, 이전 이름, 새로운 변수 명 한 번에 해야됨.
    private WeaponInfo[] mainWeaponInfos;
    //[FormerlySerializedAs("밑에 이전 변수명")] 이전 변수명과 밑에 에는 새로운 변수 명 쳐줘야 안의 값 그대로 이동함. 직렬화 되어있어야 했던가?
    [SerializeField]
    private WeaponInfo[] shapeSampleWeaponInfos;
    [SerializeField]
    private WeaponInfo[] testBossWeaponInfos;
    [SerializeField]
    private WeaponInfo[] enemyWeaponInfos;
    [SerializeField]
    private WeaponInfo[] testBasicWeaponInfos;
    private int ratingLength;

    [SerializeField]
    // 런타임 때 결정되는 정보들.
    private List<WeaponInfo>[] weaponInfoByRating;

    [Header("true하고 실행 시 엑셀 내용으로 무기 초기화")]
    [SerializeField]
    private bool canInputWeaponData;
    public List<Dictionary<string, object>> csvWeaponData;
    #endregion

    #region getter
    public int GetWeaponInfosLength()
    {
        switch (DebugSetting.Instance.weaponModeForDebug)
        {
            case WeaponModeForDebug.TEST:
                return testWeaponInfos.Length;
            case WeaponModeForDebug.MAIN:
                return mainWeaponInfos.Length;
            case WeaponModeForDebug.SHAPE_SAMPLE:
                return shapeSampleWeaponInfos.Length;
            case WeaponModeForDebug.TEST_BOSS:
                return testBossWeaponInfos.Length;
            case WeaponModeForDebug.TEST_BASIC_ENEMY:
                return testBasicWeaponInfos.Length;
            default:
                break;
        }
        return 0;
    }

    public int GetEnemyWeaponInfosLength()
    {
        return enemyWeaponInfos.Length;
    }

    public int GetWeaponInfoByRatingLength(int rating)
    {
        return weaponInfoByRating[rating].Count;
    }

    /// <summary>
    /// Player Weapon 중 해당 Rating에서 랜덤하게 무기 정보 얻기 
    /// </summary>
    /// <param name="rating"></param>
    /// <returns></returns>
    public WeaponInfo GetWeaponInfo(Rating rating)
    {
        if (Rating.NORATING == rating)
            return null;
        int ratingIndex = (int)(rating - 1);
        if (0 >= weaponInfoByRating[ratingIndex].Count)
            return null;
        int weaponIndex = Random.Range(0, weaponInfoByRating[ratingIndex].Count);
        return weaponInfoByRating[ratingIndex][weaponIndex];
    }

    /// <summary>
    /// Owner에 따른 Weapon Data 반환, ownerType 기본 값 Player
    /// </summary>
    /// <param name="id"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public WeaponInfo GetWeaponInfo(int id, CharacterInfo.OwnerType ownerType)
    {
        // player용 switch 안에 switch 못해서 따로 떼어놓음.
        if (CharacterInfo.OwnerType.PLAYER == ownerType)
        {
            switch (DebugSetting.Instance.weaponModeForDebug)
            {
                case WeaponModeForDebug.TEST:
                    return testWeaponInfos[id];
                case WeaponModeForDebug.MAIN:
                    return mainWeaponInfos[id];
                case WeaponModeForDebug.SHAPE_SAMPLE:
                    return shapeSampleWeaponInfos[id];
                case WeaponModeForDebug.TEST_BOSS:
                    return testBossWeaponInfos[id];
                case WeaponModeForDebug.TEST_BASIC_ENEMY:
                    return testBasicWeaponInfos[id];
                default:
                    break;
            }
        }
        switch (ownerType)
        {
            case CharacterInfo.OwnerType.ENEMY:
                return enemyWeaponInfos[id];
            case CharacterInfo.OwnerType.OBJECT:
            default:
                break;
        }
        return null;
    }
    #endregion

    #region unityFunc
    void Awake()
    {
        InitWepaonInfo();
    }
    #endregion

    #region func
    /// <summary> 무기 정보 관련 초기화 </summary>
    public void InitWepaonInfo()
    {
        ratingLength = (int)Rating.E;
        weaponInfoByRating = new List<WeaponInfo>[ratingLength];
        for (int i = 0; i < weaponInfoByRating.Length; i++)
        {
            weaponInfoByRating[i] = new List<WeaponInfo>();
        }

        switch (DebugSetting.Instance.weaponModeForDebug)
        {
            case WeaponModeForDebug.TEST:
                for (int i = 0; i < testWeaponInfos.Length; i++)
                    testWeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.MAIN:
                for (int i = 0; i < mainWeaponInfos.Length; i++)
                {
                    mainWeaponInfos[i].Init();
                    mainWeaponInfos[i].SetWeaponId(i);
                    InsertWeaponInfoByRating(mainWeaponInfos[i]);
                }
                break;
            case WeaponModeForDebug.SHAPE_SAMPLE:
                for (int i = 0; i < shapeSampleWeaponInfos.Length; i++)
                    shapeSampleWeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TEST_BOSS:
                for (int i = 0; i < testBossWeaponInfos.Length; i++)
                    testBossWeaponInfos[i].Init();
                break;
            case WeaponModeForDebug.TEST_BASIC_ENEMY:
                for (int i = 0; i < testBasicWeaponInfos.Length; i++)
                    testBasicWeaponInfos[i].Init();
                break;
            default:
                break;
        }

        for (int i = 0; i < enemyWeaponInfos.Length; i++)
        {
            enemyWeaponInfos[i].Init();
        }
        InputWeaponData();
    }

    private void InsertWeaponInfoByRating(WeaponInfo weaponInfo)
    {
        if (Rating.NORATING == weaponInfo.rating)
        {
            return;
        }
        int rating = (int)weaponInfo.rating - 1;
        // notRating 일 때 넣지 않음.
        weaponInfoByRating[rating].Add(weaponInfo);
    }

    public void InputWeaponData()
    {
        if (WeaponModeForDebug.MAIN != DebugSetting.Instance.weaponModeForDebug)
            return;

        if (false == canInputWeaponData)
            return;
        csvWeaponData = WeaponDataCSVParser.Read("weaponData");
        //Debug.Log("CSV 데이터 파싱 후 weapon data 입력");

        #region parsing variables
        WeaponType weaponType;
        AttackAniType attackAniType;
        Rating rating;
        float chargeTimeMax;
        float criticalChance;
        float damage;
        int staminaConsumption;
        float cooldown;
        int ammoCapacity;
        float range;
        float bulletSpeed;
        float scaleX;
        float scaleY;
        float castingTime;
        float addDirVec;
        float addVerticalVec;
        #endregion


        int size = csvWeaponData.Count;
        for (int i = 0; i < size; i++)
        {
            mainWeaponInfos[i].weaponName = (string)csvWeaponData[i]["name"];
            //Debug.Log(i + ", name : " + (string)csvWeaponData[i]["name"]);

            weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), (string)csvWeaponData[i]["weaponType"]);
            mainWeaponInfos[i].weaponType = weaponType;

            attackAniType = (AttackAniType)System.Enum.Parse(typeof(AttackAniType), (string)csvWeaponData[i]["attackAniType"]);
            mainWeaponInfos[i].attackAniType = attackAniType;
            //Debug.Log(attackAniType);

            rating = (Rating)System.Enum.Parse(typeof(Rating), (string)csvWeaponData[i]["rating"]);
            mainWeaponInfos[i].rating = rating;
            //Debug.Log(rating);

            float.TryParse(csvWeaponData[i]["chargeTimeMax"].ToString(), out chargeTimeMax);
            mainWeaponInfos[i].chargeTimeMax = chargeTimeMax;
            //Debug.Log(chargeTimeMax);
            if (0 == chargeTimeMax)
                mainWeaponInfos[i].touchMode = TouchMode.NORMAL;
            else
                mainWeaponInfos[i].touchMode = TouchMode.CHARGE;

            float.TryParse(csvWeaponData[i]["criticalChance"].ToString(), out criticalChance);
            mainWeaponInfos[i].criticalChance = criticalChance;
            //Debug.Log(criticalChance);

            float.TryParse(csvWeaponData[i]["damage"].ToString(), out damage);
            mainWeaponInfos[i].damage = damage;
            //Debug.Log(damage);

            int.TryParse(csvWeaponData[i]["staminaConsumption"].ToString(), out staminaConsumption);
            mainWeaponInfos[i].staminaConsumption = staminaConsumption;
            //Debug.Log(staminaConsumption);

            float.TryParse(csvWeaponData[i]["cooldown"].ToString(), out cooldown);
            mainWeaponInfos[i].cooldown = cooldown;
            //Debug.Log(cooldown);

            int.TryParse(csvWeaponData[i]["ammoCapacity"].ToString(), out ammoCapacity);
            mainWeaponInfos[i].ammoCapacity = ammoCapacity;
            mainWeaponInfos[i].ammo = ammoCapacity;
            //Debug.Log(ammoCapacity);

            float.TryParse(csvWeaponData[i]["range"].ToString(), out range);
            mainWeaponInfos[i].range = range;
            //Debug.Log(range);

            float.TryParse(csvWeaponData[i]["bulletSpeed"].ToString(), out bulletSpeed);
            mainWeaponInfos[i].bulletMoveSpeed = bulletSpeed;
            //Debug.Log(bulletSpeed);

            mainWeaponInfos[i].soundName = csvWeaponData[i]["soundName"].ToString();

            if ("TRUE" == csvWeaponData[i]["showsMuzzleFlash"].ToString())
                mainWeaponInfos[i].showsMuzzleFlash = true;
            else
                mainWeaponInfos[i].showsMuzzleFlash = false;

            float.TryParse(csvWeaponData[i]["scaleX"].ToString(), out scaleX);
            mainWeaponInfos[i].scaleX = scaleX;

            float.TryParse(csvWeaponData[i]["scaleY"].ToString(), out scaleY);
            mainWeaponInfos[i].scaleY = scaleY;

            float.TryParse(csvWeaponData[i]["castingTime"].ToString(), out castingTime);
            mainWeaponInfos[i].castingTime = castingTime;

            float.TryParse(csvWeaponData[i]["addDirVec"].ToString(), out addDirVec);
            mainWeaponInfos[i].addDirVecMagnitude = addDirVec;

            float.TryParse(csvWeaponData[i]["addVerticalVec"].ToString(), out addVerticalVec);
            mainWeaponInfos[i].additionalVerticalPos = addVerticalVec;


            switch (weaponType)
            {
                case WeaponType.PISTOL:
                case WeaponType.SHOTGUN:
                case WeaponType.MACHINEGUN:
                case WeaponType.SNIPER_RIFLE:
                    mainWeaponInfos[i].showsMuzzleFlash = true;
                    break;
                case WeaponType.LASER:
                    //mainWeaponInfos[i].cooldown = 0f;
                    mainWeaponInfos[i].cameraShakeAmount = 0f;
                    mainWeaponInfos[i].cameraShakeTime = 0f;
                    break;
                default:
                    mainWeaponInfos[i].showsMuzzleFlash = false;
                    break;
            }

            switch (weaponType)
            {
                case WeaponType.PISTOL:
                case WeaponType.SHOTGUN:
                case WeaponType.MACHINEGUN:
                case WeaponType.SNIPER_RIFLE:
                case WeaponType.BOW:
                case WeaponType.WAND:
                case WeaponType.RANGED_SPECIAL:
                    mainWeaponInfos[i].cameraShakeAmount = 0.1f;
                    mainWeaponInfos[i].cameraShakeTime = 0.1f;
                    break;
                case WeaponType.SPEAR:
                case WeaponType.CLUB:
                case WeaponType.SPORTING_GOODS:
                case WeaponType.SWORD:
                case WeaponType.CLEANING_TOOL:
                case WeaponType.KNUCKLE:
                    mainWeaponInfos[i].cameraShakeAmount = 0.1f;
                    mainWeaponInfos[i].cameraShakeTime = 0.04f;
                    break;
                case WeaponType.BOMB:
                    mainWeaponInfos[i].cameraShakeAmount = 0f;
                    mainWeaponInfos[i].cameraShakeTime = 0f;
                    break;
                default:
                    break;
            }

            switch (weaponType)
            {
                case WeaponType.SPEAR:
                case WeaponType.CLUB:
                case WeaponType.SPORTING_GOODS:
                case WeaponType.SWORD:
                case WeaponType.CLEANING_TOOL:
                    if (addDirVec == 0)
                        mainWeaponInfos[i].addDirVecMagnitude = 1.2f;
                    break;
                case WeaponType.KNUCKLE:
                case WeaponType.SHOTGUN:
                case WeaponType.BOW:
                case WeaponType.WAND:
                case WeaponType.SNIPER_RIFLE:
                    if (addDirVec == 0)
                        mainWeaponInfos[i].addDirVecMagnitude = 0.5f;
                    break;
                case WeaponType.LASER:
                case WeaponType.MACHINEGUN:
                case WeaponType.RANGED_SPECIAL:
                    if (addDirVec == 0)
                        mainWeaponInfos[i].addDirVecMagnitude = 0.3f;
                    break;
                default:
                    break;
            }

            //시전 시간
            switch (mainWeaponInfos[i].attackAniType)
            {
                case AttackAniType.STRIKE:
                    mainWeaponInfos[i].castingTime = 0.3f;
                    break;
                case AttackAniType.BLOW:
                    mainWeaponInfos[i].castingTime = 0.2f;
                    break;
                case AttackAniType.SWING:
                    mainWeaponInfos[i].castingTime = 0.3f;
                    break;
                case AttackAniType.SHOT:
                    break;
                default:
                    break;
            }
        }

    }
    #endregion
}
