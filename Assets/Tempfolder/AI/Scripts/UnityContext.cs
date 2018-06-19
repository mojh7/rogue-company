using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityContext : MonoBehaviour
{
    private static UnityContext instance = null;

    private static UnityContext GetInstance()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "~Context";
            instance = (UnityContext)gameObject.AddComponent(typeof(UnityContext));
            gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
        }
        return instance;
    }

    public static Clock GetClock()
    {
        return GetInstance().clock;
    }

    public static BT.BlackBoard GetSharedBlackboard(string key)
    {
        UnityContext context = GetInstance();
        if (!context.blackboards.ContainsKey(key))
        {
            context.blackboards.Add(key, new BT.BlackBoard());
        }
        return context.blackboards[key];
    }

    private Dictionary<string, BT.BlackBoard> blackboards = new Dictionary<string, BT.BlackBoard>();

    private Clock clock = new Clock();

    void Update()
    {
        clock.Update(Time.deltaTime);
    }
}