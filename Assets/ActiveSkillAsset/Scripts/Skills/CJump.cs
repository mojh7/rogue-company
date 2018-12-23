using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CJump", menuName = "SkillData/CJump")]
public class CJump : SkillData
{
    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run();
    }
    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run();
    }
    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run();
    }

    private BT.State Run()
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }

        Vector2 targetPos = other.GetPosition();
        if (caster)
        {
            caster.GetCharacterComponents().AIController.StopMove();
            caster.GetCharacterComponents().CircleCollider2D.enabled = false;
            ActiveSkillManager.Instance.StartJumpCoroutine(caster, caster.transform.position, targetPos);
        }
        return BT.State.SUCCESS;
    }

}
