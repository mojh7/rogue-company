﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToInGameSence : MonoBehaviour {

    void Update () {
        // 활성화 되어 클릭 당하면 실행
        if (Input.GetKeyDown(KeyCode.V))
        {
            SceneManager.LoadScene("InGameScene");
        }
	}
}

/*public class VendingMachine : RandomSpriteObject
{
    int value;
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        isAnimate = true;
        objectType = ObjectType.VENDINMACHINE;
        value = 5;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            if (GameDataManager.Instance.GetCoin() >= value)
            {
                GameDataManager.Instance.ReduceCoin(value);
                Item item = ObjectPoolManager.Instance.CreateUsableItem(UsableItemType.FOOD);
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                ItemManager.Instance.CreateItem(item, pos);
            }
            return true;
        }
        return base.Active();
    }
}*/
