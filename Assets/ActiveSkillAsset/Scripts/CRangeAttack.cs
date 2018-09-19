using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CRangeAttack", menuName = "SkillData/CRangeAttack")]
public class CRangeAttack : SkillData
{
    [SerializeField]
    float radius;

    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return RangeAttack(character, radius, idx, delay, amount);
    }

    public BT.State RangeAttack(Character user, object radius, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = user.transform.position;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, amount, (float)radius);
        gameObject.GetComponent<CollisionSkill>().SetAvailableFalse();
        user.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().SetAvailableTrue);
        user.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
        user.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

}
