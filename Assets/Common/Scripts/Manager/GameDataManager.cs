using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// TODO : 데이터 어느정도 구분 해놓기, 하는 중

/* UserSettingData : 셋팅 정보
 * 배경음악 볼륨
 * 효과음 볼륨
 * Aim type
 * 
 * ----------------
 * UserGlobalData : 유저 전역 정보
 * 골드
 * 캐릭터 해금(보유) 여부
 * 도감 코텐츠 해금 여부(콘텐츠 처음 접하면 열리게 or 아무런 조건 없이 모든 정보 오픈)
 * 업적 관련
 * 
 * UserInDungeonData : 던전 내에서의 정보
 * 던전 코인
 * 던전 키
 * 무기 정보(id, 탄약)
 * 패시브 아이템 
 * 자동 저장 가능 여부 (ex : 소울나이트 처럼 스테이지 별 1회 까지만 자동 저장)
 * Hp
 * Stamina
 * 몇 스테이지
 * 
 */


public class GameDataManager : MonoBehaviourSingleton<GameDataManager>
{
    


    #region variables
    #endregion

    public enum UserDataType { USER, INGAME, SETTING }
    private string[] dataPath;

    private int floor;
    private Player.PlayerType playerType;
    private int coin;
    private int key;

    private UserData userData;
    private GameSettingData gameSettingData;
    private InGameData inGameData;

    private string dataName;
    private PlayerData playerData;

    // 0810 주윤아 플레이 타임
    private int kill;
    private float time;

    private int[] weaponIds;
    private int[] weaponAmmos;
    private List<int> miscItems;

    [SerializeField]
    private PlayerData[] playerDatas;

    #region unityFunc
    private void Awake()
    {
        DontDestroyOnLoad(this);

        #region constants
#if (UNITY_EDITOR)
        string basePath = Application.dataPath;
#else
        string basePath = Application.persistentDataPath + dataName;
#endif
        // enum 순서대로
        dataPath = new string[] { basePath + "/UserData.bin", basePath + "/InGameData.bin", basePath + "/GameSettingData.bin" };
    #endregion
}
#endregion


#region getter
public bool isFirst
    {
        get
        {
            if (PlayerPrefs.GetInt("First") == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        set
        {
            PlayerPrefs.SetInt("First", 1);
        }
    }
    public int GetKey() { return key; }
    public int GetCoin() { return coin; }
    public int GetFloor() { return floor; }
    public int GetKill() { return kill; }
    public float GetTime() { return time; }
    public Player.PlayerType GetPlayerType() { return playerType; }
    public PlayerData GetPlayerData() { return playerData; }
    public PlayerData GetPlayerData(Player.PlayerType playerType)
    {
        if(playerDatas == null)
        {
            playerDatas = PlayerManager.Instance.playerDatas;
        }
        return playerDatas[(int)playerType].Clone();
    }
    // 0531 모장현
    public int[] GetWeaponIds() { return weaponIds; }
    public int[] GetWeaponAmmos() { return weaponAmmos; }
    public List<int> GetMiscItems() { return miscItems; }
    #endregion

    #region setter
    public void SetKey() { key++; ShowUI(); }
    public void SetCoin() { coin++; ShowUI(); }
    public void SetFloor() { floor++; }
    public void SetKill() { kill++; }
    public void SetTime(float _time) { time += _time; }
    public void SetPlayerType(Player.PlayerType _playerType) { playerType = _playerType; }
    public void SetMiscItems(List<int> _miscItems) { miscItems = _miscItems; }
    #endregion

    #region Func
    public void UseKey()
    {
        if (key > 0)
            key--;
        ShowUI();
    }
    public void ReduceCoin(int value)
    {
        if (value <= 0)
            return;
        coin -= value;
        ShowUI();
    }
    void ShowUI()
    {
        switch(GameStateManager.Instance.GetGameScene())
        {
            case GameStateManager.GameScene.LOBBY:
                // show lobby ui
                break;
            case GameStateManager.GameScene.IN_GAME :
            case GameStateManager.GameScene.BOSS_RUSH :
                UIManager.Instance.SetCoinText(coin);
                UIManager.Instance.SetKeyText(key);
                break;
            default:
                break;
        }
    }
  
    
    
    public void ResetData(UserDataType userDataType)
    {
        switch(userDataType)
        {
            case UserDataType.USER:
                break;
            case UserDataType.INGAME:
                if (File.Exists(dataPath[(int)UserDataType.INGAME]))
                {
                    File.Delete(dataPath[(int)UserDataType.INGAME]);
                }
                if (inGameData != null)
                {
                    inGameData = null;
                }
                floor = 0;
                coin = 0;
                key = 0;
                kill = 0;
                time = 0;
                break;
            case UserDataType.SETTING:
                break;
            default:
                break;
        }
    }


    #endregion

    #region serialize / deserialize

    private void BinarySerialize(UserData userData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(dataPath[(int)UserDataType.USER], FileMode.Create);
        binaryFormatter.Serialize(fileStream, this.userData);
        fileStream.Close();
    }
    private void BinarySerialize(InGameData inGameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(dataPath[(int)UserDataType.INGAME], FileMode.Create);
        binaryFormatter.Serialize(fileStream, this.inGameData);
        fileStream.Close();
    }
    private void BinarySerialize(GameSettingData gameSettingData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(dataPath[(int)UserDataType.SETTING], FileMode.Create);
        binaryFormatter.Serialize(fileStream, this.gameSettingData);
        fileStream.Close();
    }

    private UserData UserDataDeserialize()
    {
        if (File.Exists(dataPath[(int)UserDataType.USER]))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(dataPath[(int)UserDataType.USER], FileMode.Open);
            UserData userData = (UserData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return userData;
        }
        else
            return null;
    }

    private InGameData InGameDataDeserialize()
    {
        if (File.Exists(dataPath[(int)UserDataType.INGAME]))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(dataPath[(int)UserDataType.INGAME], FileMode.Open);
            InGameData inGameData = (InGameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return inGameData;
        }
        else
            return null;
    }

    private GameSettingData GameSettingDataDeserialize()
    {
        if (File.Exists(dataPath[(int)UserDataType.SETTING]))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(dataPath[(int)UserDataType.SETTING], FileMode.Open);
            GameSettingData gameSettingData = (GameSettingData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return gameSettingData;
        }
        else
            return null;
    }
    #endregion



    #region save / load Data

    public void Savedata(UserDataType userDataType)
    {
        switch (userDataType)
        {
            case UserDataType.USER:
                break;
            case UserDataType.SETTING:
                break;
            case UserDataType.INGAME:
                if (inGameData == null)
                    inGameData = new InGameData();
                inGameData.SetFloor(floor);
                inGameData.SetCoin(coin);
                inGameData.SetKey(key);
                inGameData.SetKill(kill);
                inGameData.SetTime(TimeController.Instance.GetPlayTime);

                inGameData.SetWeaponIds(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponIds());
                inGameData.SetWeaponAmmos(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponAmmos());

                inGameData.SetHp(PlayerManager.Instance.GetPlayer().PlayerData.Hp);
                inGameData.SetStamina(PlayerManager.Instance.GetPlayer().PlayerData.Stamina);
                inGameData.SetMiscItems(PlayerBuffManager.Instance.BuffManager.PassiveIds);
                BinarySerialize(inGameData);
                break;
        }

    }

    public bool LoadData(UserDataType userDataType)
    {
        switch (userDataType)
        {
            case UserDataType.USER:
                break;
            case UserDataType.SETTING:
                break;
            case UserDataType.INGAME:
                if (inGameData != null)
                    inGameData = null;
                if (File.Exists(dataPath[(int)userDataType]))
                {
                    inGameData = InGameDataDeserialize();
                    floor = inGameData.GetFloor();
                    coin = inGameData.GetCoin();
                    key = inGameData.GetKey();
                    playerType = inGameData.GetPlayerType();
                    kill = inGameData.GetKill();
                    time = inGameData.GetTime();
                    weaponIds = inGameData.GetWeaponIds();
                    weaponAmmos = inGameData.GetWeaponAmmos();
                    playerData = playerDatas[(int)playerType].Clone();
                    playerData.Hp = inGameData.GetHp();
                    playerData.Stamina = inGameData.GetStamina();
                    miscItems = inGameData.GetMiscItems();
                    return true;
                }
                ResetData(UserDataType.INGAME);
                break;
            default:
                break;
        }
        return false;
    }

    #endregion
}

