using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameDataManager : MonoBehaviourSingleton<GameDataManager>
{
    int m_floor;
    Player.PlayerType m_playerType;
    int m_coin;
    int m_key;

    GameData gameData;
    string dataPath;
    string dataName;
    PlayerData playerData;

    // 0810 주윤아 플레이 타임
    int m_kill;
    float m_time;

    private int[] m_weaponIds;
    private int[] m_weaponAmmos;
    private List<int> miscItems;

    [SerializeField]
    private PlayerData[] playerDatas;

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
    public int GetKey() { return m_key; }
    public int GetCoin() { return m_coin; }
    public int GetFloor() { return m_floor; }
    public int GetKill() { return m_kill; }
    public float GetTime() { return m_time; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
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
    public int[] GetWeaponIds() { return m_weaponIds; }
    public int[] GetWeaponAmmos() { return m_weaponAmmos; }
    public List<int> GetMiscItems() { return miscItems; }
    #endregion

    #region setter
    public void SetKey() { m_key++; ShowUI(); }
    public void SetCoin() { m_coin++; ShowUI(); }
    public void SetFloor() { m_floor++; }
    public void SetKill() { m_kill++; }
    public void SetTime(float _time) { m_time += _time; }
    public void SetPlayerType(Player.PlayerType _playerType) { m_playerType = _playerType; }
    public void SetMiscItems(List<int> _miscItems) { miscItems = _miscItems; }
    #endregion

    #region Func
    public void UseKey()
    {
        if (m_key > 0)
            m_key--;
        ShowUI();
    }
    public void ReduceCoin(int value)
    {
        if (value <= 0)
            return;
        m_coin -= value;
        ShowUI();
    }
    void ShowUI()
    {
        UIManager.Instance.SetCoinText(m_coin);
        UIManager.Instance.SetKeyText(m_key);
    }
  
    public void Savedata()
    {
        if (GameStateManager.Instance.GetMode() == GameStateManager.GameMode.NORMAL)
            dataName = "/save.bin";
        else
            dataName = "/saveRush.bin";
#if (UNITY_EDITOR)
        dataPath = Application.dataPath + dataName;
#else
        dataPath = Application.persistentDataPath + dataName;
#endif
        if (gameData == null)
            gameData = new GameData();
        gameData.SetFloor(m_floor);
        gameData.SetCoin(m_coin);
        gameData.SetKey(m_key);
        gameData.SetKill(m_kill);
        gameData.SetTime(TimeController.Instance.GetPlayTime);

        gameData.SetWeaponIds(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponIds());
        gameData.SetWeaponAmmos(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponAmmos());
 
        gameData.SetHp(PlayerManager.Instance.GetPlayer().PlayerData.Hp);
        gameData.SetStamina(PlayerManager.Instance.GetPlayer().PlayerData.Stamina);
        gameData.SetMiscItems(PlayerBuffManager.Instance.BuffManager.PassiveIds);
        BinarySerialize(gameData);
    }
    public bool LoadData()
    {
        if (GameStateManager.Instance.GetMode() == GameStateManager.GameMode.NORMAL)
            dataName = "/save.bin";
        else
            dataName = "/saveRush.bin";
#if (UNITY_EDITOR)
        dataPath = Application.dataPath + dataName;
#else
        dataPath = Application.persistentDataPath + dataName;
#endif
        if (gameData != null)
            gameData = null;
        if (File.Exists(dataPath))
        {
            gameData = BinaryDeserialize();
            m_floor = gameData.GetFloor();
            m_coin = gameData.GetCoin();
            m_key = gameData.GetKey();
            m_playerType = gameData.GetPlayerType();
            m_kill = gameData.GetKill();
            m_time = gameData.GetTime();
            m_weaponIds = gameData.GetWeaponIds();
            m_weaponAmmos = gameData.GetWeaponAmmos();
            playerData = playerDatas[(int)m_playerType].Clone();
            playerData.Hp = gameData.GetHp();
            playerData.Stamina = gameData.GetStamina();
            miscItems = gameData.GetMiscItems();
            return true;
        }
        ResetData();
        return false;
    }
    public void ResetData()
    {
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }
        if (gameData != null)
        {
            gameData = null;
        }
        m_floor = 0;
        m_coin = 0;
        m_key = 0;
        m_kill = 0;
        m_time = 0;
    }
    void BinarySerialize(GameData _gameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(dataPath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();
    }
    GameData BinaryDeserialize()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(dataPath, FileMode.Open);
            GameData gamedata = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return gamedata;
        }
        else
            return null;
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #endregion
}

