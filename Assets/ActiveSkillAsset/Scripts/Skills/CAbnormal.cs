using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CAbnormal", menuName = "SkillData/CAbnormal")]
public class CAbnormal : SkillData
{
    [SerializeField]
    StatusEffectInfo statusEffectInfo;
    [SerializeField]
    string particleName;

    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run(statusEffectInfo, customObject.objectPosition);
    }

    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run(statusEffectInfo, caster.GetPosition());
    }

    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);

        return Run(statusEffectInfo, caster.GetPosition());
    }

    private BT.State Run(StatusEffectInfo statusEffectInfo,Vector3 pos)
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        if (mPos != ActiveSkillManager.nullVector)
            pos = mPos;
        gameObject.transform.position = pos;
        CollisionSkillObject skillObject = gameObject.AddComponent<CollisionSkillObject>();
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Set(statusEffectInfo);
        ParticleManager.Instance.PlayParticle(particleName, pos, radius);

        return BT.State.SUCCESS;
    }

}
