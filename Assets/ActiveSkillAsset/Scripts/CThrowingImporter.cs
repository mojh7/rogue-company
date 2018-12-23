using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CThrowingImporter", menuName = "SkillData/CThrowingImporter")]
public class CThrowingImporter : SkillData
{
    [SerializeField]
    List<SkillData> skillData;
    [SerializeField]
    float speed, acceleration;
    [SerializeField]
    string animName;

    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run(customObject.objectPosition, customObject.objectPosition + Random.insideUnitSphere * speed);
    }
    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run(caster.GetPosition(), caster.GetPosition() + caster.GetDirVector() * speed);
    }
    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run(caster.GetPosition(), other.GetPosition());
    }

    private BT.State Run(Vector3 srcPos, Vector3 destPos)
    {
        if (delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = srcPos;
        ProjectileSkillObject skillObject = gameObject.AddComponent<ProjectileSkillObject>();
        skillObject.SetSkillData(null, skillData);
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Set(animName, speed, acceleration, destPos - srcPos);
        return BT.State.SUCCESS;
    }

}
