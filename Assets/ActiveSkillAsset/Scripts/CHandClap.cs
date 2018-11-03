using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CHandClap", menuName = "SkillData/CHandClap")]
public class CHandClap : SkillData
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
        ActiveSkillManager.Instance.StartCoroutine(HandClap, character, character.transform.position + Vector3.left, 0, amount);
        ActiveSkillManager.Instance.StartCoroutine(HandClap, character, character.transform.position + Vector3.right, delay, amount);
        character.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void HandClap(Character user, object temporary, float amount)
    {
        Vector3 pos = (Vector3)temporary;
        
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(user, amount, "handClap");
    }

}
