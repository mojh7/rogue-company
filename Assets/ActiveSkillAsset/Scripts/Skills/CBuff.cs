using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CBuff", menuName = "SkillData/CBuff")]
public class CBuff : SkillData
{
    [SerializeField]
    ItemUseEffect buffEffectInfo;
    [SerializeField]
    string particleName;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        return base.Run(customObject, pos, ref lapsedTime);
    }

    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        base.Run(caster, pos, ref lapsedTime);
        return Run();
    }

    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        base.Run(caster, other, pos, ref lapsedTime);
        return Run();
    }
 
    private BT.State Run()
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }
        caster.GetBuffManager().RegisterItemEffect(buffEffectInfo, BuffManager.EffectApplyType.BUFF, -1, time);
        ParticleManager.Instance.PlayParticle(particleName, caster.transform.position);
        return BT.State.SUCCESS;
    }
}
