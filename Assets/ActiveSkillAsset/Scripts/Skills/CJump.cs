using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CJump", menuName = "SkillData/CJump")]
public class CJump : SkillData
{
    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run();
    }
    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run();
    }
    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
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
