using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class GameData
{
    float hp;
    int hungry;
    int stamina;
    int m_floor;
    int m_coin;
    int m_kill;
    float m_time;
    int[] weaponIds;
    int[] weaponAmmos;
    Player.PlayerType m_playerType;

    public GameData()
    {
        m_floor = 1;
        m_coin = 0;
        m_kill = 0;
        m_time = 0;
        m_playerType = Player.PlayerType.SOCCER;
    }
    #region getter
    public float GetHp() { return hp; }
    public int GetStamina() { return stamina; }
    public int GetFloor() { return m_floor; }
    public int GetCoin() { return m_coin; }
    public int GetKill() { return m_kill; }
    public float GetTime() { return m_time; }
    public int[] GetWeaponIds() { return weaponIds; }
    public int[] GetWeaponAmmos() { return weaponAmmos; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region setter
    public void SetHp(float _hp) { hp = _hp; }
    public void SetStamina(int _stamina) { stamina = _stamina; }
    public void SetFloor() { m_floor++; }
    public void SetCoin(int _coin) { m_coin = _coin; }
    public void SetKill(int _kill) { m_kill = _kill; }
    public void SetTime(float _time) { m_time = _time; }
    public void SetWeaponIds(int[] _weaponIds) { weaponIds = _weaponIds; }
    public void SetWeaponAmmos(int[] _weaponAmmos) { weaponAmmos = _weaponAmmos; }
    #endregion
}