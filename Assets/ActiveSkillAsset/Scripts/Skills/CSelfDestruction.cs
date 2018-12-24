using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CSelfDestruction", menuName = "SkillData/CSelfDestruction")]
public class CSelfDestruction : SkillData
{
    [SerializeField]
    List<SkillData> skillData;
    [SerializeField]
    string particleName;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(customObject.objectPosition);
    }

    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(caster.GetPosition());
    }

    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(caster.GetPosition());
    }

    private BT.State Run(Vector3 pos)
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }
        float lapsedTime = 9999;
        if (mPos != ActiveSkillManager.nullVector)
            pos = mPos;

        if(caster)
        {
            caster.SelfDestruction();
        }
        else if(customObject)
        {
            customObject.Delete();
        }

        foreach (SkillData item in skillData)
        {
            if (other)
                item.Run(caster, other, mPos,ref lapsedTime);
            else if (caster)
                item.Run(caster, mPos, ref lapsedTime);
            if (customObject)
                item.Run(customObject, mPos, ref lapsedTime);
        }
        ParticleManager.Instance.PlayParticle(particleName, pos, radius);

        return BT.State.SUCCESS;
    }
}