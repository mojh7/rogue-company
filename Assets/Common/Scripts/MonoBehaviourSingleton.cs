using UnityEngine;
using System.Collections;

public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T objs = FindObjectOfType(typeof(T)) as T;
                _instance = objs;
                if (_instance == null)
                {
                    GameObject obj = new GameObject
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
