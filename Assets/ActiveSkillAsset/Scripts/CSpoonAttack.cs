using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSpoonAttack", menuName = "SkillData/CSpoonAttack")]
public class CSpoonAttack : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Run();
    }

    private BT.State Run()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        ActiveSkillManager.Instance.StartCoroutine(SpoonAttack, character, temporary, delay, amount);
        character.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void SpoonAttack(Character user, object temporary, float amount)
    {
        Vector3 dir = (temporary as Character).transform.position - user.transform.position;

        dir.Normalize();

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = user.transform.position + dir;
        gameObject.AddComponent<CollisionSkill>().Init(user, amount, "spoonAttack");
    }

}