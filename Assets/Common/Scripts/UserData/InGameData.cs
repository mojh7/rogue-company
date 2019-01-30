using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class InGameData
{
    private float hp;
    private int stamina;
    private int floor;
    private int coin;
    private int key;
    private int kill;
    private float time;
    private int[] weaponIds;
    private int[] weaponAmmos;
    private List<int> miscItems;
    private Player.PlayerType m_playerType;

    public InGameData()
    {
        floor = 1;
        coin = 0;
        key = 0;
        kill = 0;
        time = 0;
        m_playerType = Player.PlayerType.SOCCER;
        miscItems = new List<int>();
    }
    #region getter
    public float GetHp() { return hp; }
    public int GetStamina() { return stamina; }
    public int GetFloor() { return floor; }
    public int GetCoin() { return coin; }
    public int GetKey() { return key; }
    public int GetKill() { return kill; }
    public float GetTime() { return time; }
    public int[] GetWeaponIds() { return weaponIds; }
    public int[] GetWeaponAmmos() { return weaponAmmos; }
    public List<int> GetMiscItems() { return miscItems; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region setter
    public void SetHp(float _hp) { hp = _hp; }
    public void SetStamina(int _stamina) { stamina = _stamina; }
    public void SetFloor(int m_floor) { this.floor = m_floor; }
    public void SetCoin(int _coin) { coin = _coin; }
    public void SetKey(int _Key) { key = _Key; }
    public void SetKill(int _kill) { kill = _kill; }
    public void SetTime(float _time) { time += _time; }
    public void SetWeaponIds(int[] _weaponIds) { weaponIds = _weaponIds; }
    public void SetWeaponAmmos(int[] _weaponAmmos) { weaponAmmos = _weaponAmmos; }
    public void SetMiscItems(List<int> _miscItems) { miscItems = _miscItems; }
    #endregion
}
