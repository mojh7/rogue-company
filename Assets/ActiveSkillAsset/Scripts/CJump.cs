using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CJump", menuName = "SkillData/CJump")]
public class CJump : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Jump();
    }

    private BT.State Jump()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        ActiveSkillManager.Instance.StartCoroutine(Jump, character, temporary, delay, amount);
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
