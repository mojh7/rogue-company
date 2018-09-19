using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandClap", menuName = "SkillData/CHandClap")]
public class CHandClap : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return HandClap(character, temporary, idx, delay, amount);
    }

    private BT.State HandClap(Character user, object unneeded, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        ActiveSkillManager.Instance.StartCoroutine(HandClap, user, user.transform.position + Vector3.left, 0, amount);
        ActiveSkillManager.Instance.StartCoroutine(HandClap, user, user.transform.position + Vector3.right, delay, amount);
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void HandClap(Character user, object unneeded, float amount)
    {
        Vector3 pos = (Vector3)unneeded;

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(user, amount, "handClap");
    }

}
