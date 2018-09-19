using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CJump", menuName = "SkillData/CJump")]
public class CJump : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Jump(character, temporary, idx, delay, amount);
    }

    public BT.State Jump(Character user, object victim, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        ActiveSkillManager.Instance.StartCoroutine(Jump, user, victim, delay, amount);
        return BT.State.SUCCESS;
    }

    private void Jump(Character user, object victim, float amount)
    {
        Vector2 targetPos = ((victim as Character).transform).position;
        if (user)
        {
            user.GetCharacterComponents().AIController.StopMove();
            user.GetCharacterComponents().CircleCollider2D.enabled = false;
            ActiveSkillManager.Instance.StartJumpCoroutine(user, user.transform.position, targetPos);
        }
    }

}
