using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CRangeAttack", menuName = "SkillData/CRangeAttack")]
public class CRangeAttack : SkillData
{
    [SerializeField]
    float radius;
    [SerializeField]
    bool hasAnimation;

    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return RangeAttack();
    }

    private BT.State RangeAttack()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        if((Vector3)temporary == null)
            gameObject.transform.position = character.transform.position;
        else
            gameObject.transform.position = (Vector3)temporary;

        if (hasAnimation)
        {
            gameObject.AddComponent<CollisionSkill>().Init(character as Character, amount, (float)radius);

            gameObject.GetComponent<CollisionSkill>().SetAvailableFalse();
            character.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().SetAvailableTrue);
            character.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
            character.GetCharacterComponents().AnimationHandler.Skill(idx);
        }
        else
        {
            gameObject.AddComponent<CollisionSkill>().Init(character as Character, delay, amount, (float)radius);
        }
        return BT.State.SUCCESS;
    }

}
