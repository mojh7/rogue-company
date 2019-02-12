using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
class UserData
{
    private int gold;
    private Dictionary<string, bool> characterUnLockState;

    public UserData()
    {
        gold = 0;
        characterUnLockState = new Dictionary<string, bool>();
        characterUnLockState.Add(Player.PlayerType.SOCCER.ToString(), true);
        for (int i = 1; i < (int)Player.PlayerType.END; i++)
        {
            characterUnLockState.Add(((Player.PlayerType)i).ToString(), false);
        }
    }

    #region getter
    public int GetGold() { return gold; }

    public Dictionary<string, bool> GetCharacterUnLockState()
    {
        return characterUnLockState;
    }
    #endregion

    #region setter
    public void SetGold(int gold) { this.gold = gold; }

    public void SetCharacterUnLockState(Dictionary<string, bool> unlockState)
    {
        characterUnLockState = unlockState;
    }
    #endregion
}

