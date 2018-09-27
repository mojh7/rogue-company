using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CBuff", menuName = "SkillData/CBuff")]
public class CBuff : SkillData
{
    [SerializeField]
    ItemUseEffect buffEffectInfo;
    [SerializeField]
    string skillName;
    [SerializeField]
    Color color;
    [SerializeField]
    string particleName;

    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Run();
    }

    private BT.State Run()
    {
        character.GetBuffManager().RegisterItemEffect(buffEffectInfo, BuffManager.EffectApplyType.BUFF, -1, delay);
        ParticleManager.Instance.PlayParticle(particleName, character.transform.position);
        return BT.State.SUCCESS;
    }
}
