using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CFlash", menuName = "SkillData/CFlash")]
public class CFlash : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Flash(character, RoomManager.Instance.GetNearestAvailableArea((temporary as Character).transform.position));
    }

    private BT.State Flash(Character user, object position)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = user.transform.position;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, position, amount, Flash);

        user.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().LapseAnimation);
        user.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
        user.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

    private void Flash(Character user, object position, float amount)
    {
        user.transform.position = (Vector2)position;
    }

}
