using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스케줄링과 공용 데이터 저장소를 위한 오브젝트.
/// </summary>
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
        if(GetInstance().clock == null)
            GetInstance().clock = new Clock();
        return GetInstance().clock;
    }
    /// <summary>
    /// 공통의 데이터 저장소
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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

    public static void DestroyClock()
    {
        GetInstance().clock = null;
    }
    /// <summary>
    /// 일정 시간마다 스케줄러를 실행.
    /// </summary>
    void Update()
    {
        if(clock != null)
            clock.Update(Time.deltaTime);
    }
}