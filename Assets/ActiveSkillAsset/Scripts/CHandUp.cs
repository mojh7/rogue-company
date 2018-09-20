    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HandUp", menuName = "SkillData/CHandUp")]
public class CHandUp : SkillData
{
    [SerializeField]
    float radius;
    [SerializeField]
    float num;

    public override BT.State Run(Character character,object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return HandUp(character);
    }

    private BT.State HandUp(Character user)
    {
        if (!user || delay < 0 || amount < 0 || num < 1)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        for (int i = 0; i < num; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            ActiveSkillManager.Instance.StartCoroutine(HandUp, user, radius, randDelay, amount);
        }
        user.isCasting = false;
        return BT.State.SUCCESS;
    }


    private void HandUp(Character user, object radius, float amount)
    {
        if (!user)
            return;
        float rad = (float)radius;
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * rad;
        GameObject gameObject = ResourceManager.Instance.objectPool.GetPooledObject();
        gameObject.transform.position = new Vector2(user.transform.position.x + randPos.x, user.transform.position.y + randPos.y);
        gameObject.AddComponent<Alert>();
        gameObject.GetComponent<Alert>().Init(HandUpPart, user, amount, 1, null);
        gameObject.GetComponent<Alert>().Active();
    }

    private void HandUpPart(Vector3 pos, object user, float amount, Character character)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, amount, "handUp");
    }
}
