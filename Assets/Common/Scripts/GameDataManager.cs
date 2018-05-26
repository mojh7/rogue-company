using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviourSingleton<GameDataManager> {

    int m_floor;
    Player.PlayerType m_playerType;

    #region setter
    void InitFloor() { m_floor = 0; }
    public void SetFloor() { m_floor++; }
    public void SetPlayerType(Player.PlayerType _playerType) { m_playerType = _playerType; }
    #endregion
    #region getter
    public int GetFloor() { return m_floor; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region Func
    public void ResetData()
    {
        InitFloor();
        m_playerType = Player.PlayerType.SOCCER;
    }
    #endregion


    private void Start()
    {
        DontDestroyOnLoad(this);
    }

}
