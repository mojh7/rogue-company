using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CThrowingImporter", menuName = "SkillData/CThrowingImporter")]
public class CThrowingImporter : SkillData
{
    [SerializeField]
    SkillData skillData;
    [SerializeField]
    float speed, acceleration;
    [SerializeField]
    string skillName;

    public override State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return ThrowingImporter(character);
    }

    private BT.State ThrowingImporter(Character user)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = new Vector2(user.transform.position.x, user.transform.position.y);
        gameObject.AddComponent<ThrowingSkill>().Init(user, (temporary as Character).transform.position, idx, skillName, skillData, speed, acceleration);

        user.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<ThrowingSkill>().LapseAnimation);
        user.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<ThrowingSkill>().EndAnimation);
        user.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

}
