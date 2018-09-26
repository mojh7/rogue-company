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

        return ThrowingImporter();
    }

    private BT.State ThrowingImporter()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;

        Character enemy = temporary as Character;
        Vector3 destPos = Vector2.zero;
        if (null == enemy)
        {
            destPos = character.transform.position + character.GetDirVector() * speed;
        }
        else
        {
            destPos = enemy.transform.position;
        }

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = new Vector2(character.transform.position.x, character.transform.position.y);
        gameObject.AddComponent<ThrowingSkill>().Init(character, destPos, idx, skillName, skillData, speed, acceleration);
        if (null != enemy)
        {
            character.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<ThrowingSkill>().LapseAnimation);
            character.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<ThrowingSkill>().EndAnimation);
            character.GetCharacterComponents().AnimationHandler.Skill(idx);
        }
        else
        {
            gameObject.GetComponent<ThrowingSkill>().LapseAnimation();
        }
        return BT.State.SUCCESS;
    }

}
