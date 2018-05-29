using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviourSingleton<GameDataManager> {

    int m_floor;
    Player.PlayerType m_playerType;
    int m_coin;
    GameData gameData;

    #region setter
    public void SetGameData(GameData _gameData) { gameData = _gameData; }
    public void SetCoin() { m_coin++; UIManager.Instance.SetCoinText(m_coin); }
    public void SetFloor() { m_floor++; }
    public void SetPlayerType(Player.PlayerType _playerType) { m_playerType = _playerType; }
    #endregion
    #region getter
    public GameData GetGameData() { return gameData; } 
    public int GetCoin() { return m_coin; }
    public int GetFloor() { return m_floor; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region Func
    public void ResetData()
    {
        if (gameData != null)
        {
            gameData = null;
        }
    }

    void SaveData(string _name, GameData _gameData)
    {
        string path = "Data/";

    }
    #endregion
    private void Awake()
    {
        ResetData();
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

}

public class GameData : ScriptableObject
{
    int m_floor;
    int m_coin;
    Player.PlayerType m_playerType;
    Random.State randomSeed;

    public GameData()
    {
        m_floor = 1;
        m_coin = 0;
        m_playerType = Player.PlayerType.SOCCER;
        randomSeed = new Random.State();
    }
    #region getter
    public int GetFloor() { return m_floor; }
    public int GetCoin() { return m_coin; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    public Random.State GetRandomSeed() { return randomSeed; }
    #endregion
    #region setter
    public void SetRandomSeed(Random.State _state) { randomSeed = _state; }
    #endregion
}