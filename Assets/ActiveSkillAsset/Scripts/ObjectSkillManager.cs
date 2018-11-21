using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSkillManager : MonoBehaviourSingleton<ObjectSkillManager>
{

    Dictionary<string, SkillData> poolsDictionary;
    [SerializeField]
    SkillData[] activeSkils;

    private void Awake()
    {
        poolsDictionary = new Dictionary<string, SkillData>();

        for (int i = 0; i < activeSkils.Length; i++)
        {
            if (!poolsDictionary.ContainsKey(activeSkils[i].name))
            {
                poolsDictionary[activeSkils[i].name] = activeSkils[i];
            }
        }
    }

    public SkillData GetSkillData(string str)
    {
        if (!poolsDictionary.ContainsKey(str))
            return null;
        return poolsDictionary[str];
    }
}
