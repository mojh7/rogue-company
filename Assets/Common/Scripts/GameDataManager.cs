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
    int m_card;

    GameData gameData;
    string dataPath;
    PlayerData playerData;

    // 0810 주윤아 플레이 타임
    int m_kill;
    float m_time;

    // 0531 모장현
    private int[] m_weaponIds;
    private int[] m_weaponAmmos;


    [SerializeField]
    private PlayerData[] playerDatas;

    #region setter
    public void SetCard() { m_card++; ShowUI(); }
    public void SetKey() { m_key++; ShowUI(); }
    public void SetCoin() { m_coin++; ShowUI(); }
    public void SetFloor() { m_floor++; }
    public void SetKill() { m_kill++; }
    public void SetTime(float _time) { m_time = _time; }
    public void SetPlayerType(Player.PlayerType _playerType) { m_playerType = _playerType; }
    #endregion

    #region getter
    public int GetCard() { return m_card; }
    public int GetKey() { return m_key; }
    public int GetCoin() { return m_coin; }
    public int GetFloor() { return m_floor; }
    public int GetKill() { return m_kill; }
    public float GetTime() { return m_time; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    public PlayerData GetPlayerData() { return playerData; }
    public PlayerData GetPlayerData(Player.PlayerType playerType)
    {
        // temp
        if(playerDatas == null)
        {
            playerDatas = PlayerManager.Instance.playerDatas;
        }
        //temp
        return playerDatas[(int)playerType].Clone();
    }
    // 0531 모장현
    public int[] GetWeaponIds() { return m_weaponIds; }
    public int[] GetWeaponAmmos() { return m_weaponAmmos; }
    #endregion

    #region Func
    public void UseCard()
    {
        if (m_card > 0)
            m_card--;
    }
    public void UseKey()
    {
        if (m_key <= 0)
            return;
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
        if (gameData == null)
            gameData = new GameData();
        gameData.SetFloor();
        gameData.SetCoin(m_coin);
        gameData.SetKill(m_kill);
        gameData.SetTime(m_time);

        // 0531 모장현
        gameData.SetWeaponIds(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponIds());
        gameData.SetWeaponAmmos(PlayerManager.Instance.GetPlayer().GetWeaponManager().GetWeaponAmmos());
        // 0611 모장현
        // gameData.SetPlayerData(PlayerManager.Instance.GetPlayer().PlayerData);
        gameData.SetHp(PlayerManager.Instance.GetPlayer().PlayerData.Hp);
        gameData.SetStamina(PlayerManager.Instance.GetPlayer().PlayerData.Stamina);
        Debug.Log("save hp : " + gameData.GetHp());

        BinarySerialize(gameData);
    }
    public bool LoadData()
    {
        if (File.Exists(dataPath))
        {
            gameData = BinaryDeserialize();
            m_floor = gameData.GetFloor();
            m_coin = gameData.GetCoin();
            m_playerType = gameData.GetPlayerType();
            m_kill = gameData.GetKill();
            m_time = gameData.GetTime();
            // 0531 모장현
            m_weaponIds = gameData.GetWeaponIds();
            m_weaponAmmos = gameData.GetWeaponAmmos();
            //Debug.Log(gameData.GetWeaponIds()[0]);
            // 0611 모장현
            // this.playerData = gameData.GetPlayerData();
            playerData = playerDatas[(int)m_playerType].Clone();
            playerData.Hp = gameData.GetHp();
            playerData.Stamina = gameData.GetStamina();

            return true;
        }
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
            m_floor = 1;
            m_coin = 0;
            m_kill = 0;
            m_time = 0;
            m_playerType = Player.PlayerType.SOCCER;
            playerData.Hp = playerDatas[0].Hp;
            playerData.Stamina = playerDatas[0].Stamina;

            playerData = null;
        }
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
    #endregion

    private void Start()
    {
#if (UNITY_EDITOR)
        dataPath = Application.dataPath + "/save.bin";
#else
        dataPath = Application.persistentDataPath + "/save.bin";
#endif
        DontDestroyOnLoad(this);
    }


}

