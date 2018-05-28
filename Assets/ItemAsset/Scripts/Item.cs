using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    public abstract void Active();
}

public class Coin : Item
{
    bool isActive = false;

    public override void Active()
    {
        if (!isActive)
        {
            Debug.Log("Coin");
            GameDataManager.Instance.SetCoin();
            isActive = !isActive;
        }
    }
}
