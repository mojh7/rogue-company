using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour {
    public bool frame;
    public UsableItemInfo itemInfo1, itemInfo2, itemInfo3;
#if UNITY_EDITOR
    float deltaTime = 0.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UsableItem usableItem = ObjectPoolManager.Instance.CreateUsableItem();
            usableItem.Init(itemInfo1);
            ItemManager.Instance.CreateItem(usableItem, PlayerManager.Instance.GetPlayerPosition());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UsableItem usableItem = ObjectPoolManager.Instance.CreateUsableItem();
            usableItem.Init(itemInfo2);
            ItemManager.Instance.CreateItem(usableItem, PlayerManager.Instance.GetPlayerPosition());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UsableItem usableItem = ObjectPoolManager.Instance.CreateUsableItem();
            usableItem.Init(itemInfo3);
            ItemManager.Instance.CreateItem(usableItem, PlayerManager.Instance.GetPlayerPosition());
        }
        if (!frame)
            return;
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!frame)
            return;
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
#endif
}
