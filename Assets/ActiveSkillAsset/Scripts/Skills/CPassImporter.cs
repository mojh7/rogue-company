using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CPassImporter", menuName = "SkillData/CPassImporter")]
public class CPassImporter : SkillData
{
    [Space(30)]
    [SerializeField]
    CharacterInfo.OwnerType target;
    [SerializeField]
    float speed, acceleration;
    [SerializeField]
    string animName;
    [SerializeField]
    List<SkillData> skillData;
    [Header("오브젝트에 붙어서 호출되는 파티클")]
    [SerializeField]
    string attachedParticleName;
    [Header("계속해서 글로벌 호출되는 파티클")]
    [SerializeField]
    string particleName;
    [SerializeField]
    float particleTerm;
    [SerializeField]
    bool isDestroy;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(customObject.objectPosition, customObject.objectPosition + Random.insideUnitSphere * speed);
    }
    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(caster.GetPosition(), caster.GetPosition() + caster.GetDirVector() * speed);
    }
    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(caster.GetPosition(), other.GetPosition());
    }

    private BT.State Run(Vector3 srcPos, Vector3 destPos)
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = srcPos;
        PassSkillObject skillObject = gameObject.AddComponent<PassSkillObject>();
        skillObject.SetSkillData(null, skillData);
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Set(target);
        skillObject.Set(animName, speed, acceleration, destPos - srcPos);
        skillObject.Set(attachedParticleName);
        skillObject.Set(particleName, particleTerm);
        skillObject.Set(isDestroy, false);

        return BT.State.SUCCESS;
    }

}
