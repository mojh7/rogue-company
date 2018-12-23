using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CRangeEffect", menuName = "SkillData/CRangeEffect")]
public class CRangeEffect : SkillData
{
    public enum EffectType { REMOVE, REFLECT, NONE }
    [SerializeField]
    EffectType effectType;
    [SerializeField]
    string particleName;

    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run(customObject.objectPosition);
    }

    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run(caster.GetPosition());
    }

    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run(caster.GetPosition());
    }

    private BT.State Run(Vector3 pos)
    {
        if (delay < 0 || amount < 0)
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
        ParticleManager.Instance.PlayParticle(particleName, pos, radius);

        return BT.State.SUCCESS;
    }
}
