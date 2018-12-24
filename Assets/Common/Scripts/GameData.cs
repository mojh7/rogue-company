using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class GameData
{
    float hp;
    int stamina;
    int m_floor;
    int m_coin;
    int m_key;
    int m_kill;
    float m_time;
    int[] weaponIds;
    int[] weaponAmmos;
    List<int> miscItems;
    Player.PlayerType m_playerType;

    public GameData()
    {
        m_floor = 1;
        m_coin = 0;
        m_key = 0;
        m_kill = 0;
        m_time = 0;
        m_playerType = Player.PlayerType.SOCCER;
        miscItems = new List<int>();
    }
    #region getter
    public float GetHp() { return hp; }
    public int GetStamina() { return stamina; }
    public int GetFloor() { return m_floor; }
    public int GetCoin() { return m_coin; }
    public int GetKey() { return m_key; }
    public int GetKill() { return m_kill; }
    public float GetTime() { return m_time; }
    public int[] GetWeaponIds() { return weaponIds; }
    public int[] GetWeaponAmmos() { return weaponAmmos; }
    public List<int> GetMiscItems() { return miscItems; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region setter
    public void SetHp(float _hp) { hp = _hp; }
    public void SetStamina(int _stamina) { stamina = _stamina; }
    public void SetFloor(int m_floor) { this.m_floor = m_floor; }
    public void SetCoin(int _coin) { m_coin = _coin; }
    public void SetKey(int _Key) { m_key = _Key; }
    public void SetKill(int _kill) { m_kill = _kill; }
    public void SetTime(float _time) { m_time += _time; }
    public void SetWeaponIds(int[] _weaponIds) { weaponIds = _weaponIds; }
    public void SetWeaponAmmos(int[] _weaponAmmos) { weaponAmmos = _weaponAmmos; }
    public void SetMiscItems(List<int> _miscItems) { miscItems = _miscItems; }
    #endregion
}