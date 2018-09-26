using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CAbnormal", menuName = "SkillData/CAbnormal")]
public class CAbnormal : SkillData
{
    [SerializeField]
    float radius;
    [SerializeField]
    StatusEffectInfo statusEffectInfo;
    [SerializeField]
    string skillName;
    [SerializeField]
    Color color;
    [SerializeField]
    string particleName;

    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Abnormal(temporary, statusEffectInfo);
    }

    private BT.State Abnormal(object position, StatusEffectInfo statusEffectInfo)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        Vector3 pos = Vector3.zero;
        if (position != null)
            pos = (Vector3)position;
        else
            pos = character.transform.position;
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character, delay, amount, radius, statusEffectInfo, skillName, color, particleName);

        return BT.State.SUCCESS;
    }
}
