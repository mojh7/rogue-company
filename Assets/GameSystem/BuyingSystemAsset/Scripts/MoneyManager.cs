using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviourSingleton<MoneyManager>
{
    private int globalGold;

    private void Awake()
    {
        GameDataManager.Instance.LoadData(GameDataManager.UserDataType.USER);
        globalGold = GameDataManager.Instance.GetGold();
    }

    
}
