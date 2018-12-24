using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CRangeEffect", menuName = "SkillData/CRangeEffect")]
public class CRangeEffect : SkillData
{
    public enum EffectType { NONE , REMOVE, REFLECT }
    [SerializeField]
    StatusEffectInfo statusEffectInfo;
    [SerializeField]
    EffectType effectType;
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
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();

        if (mPos != ActiveSkillManager.nullVector)
            pos = mPos;
        gameObject.transform.position = pos;
        CollisionSkillObject skillObject = gameObject.AddComponent<CollisionSkillObject>();
        skillObject.Set(effectType);
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Set(statusEffectInfo);
        ParticleManager.Instance.PlayParticle(particleName, pos, radius);

        return BT.State.SUCCESS;
    }
}
