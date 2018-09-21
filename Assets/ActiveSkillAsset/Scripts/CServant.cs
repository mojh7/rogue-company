using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return SpawnServant(character);
    }

    private BT.State SpawnServant(Character user)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        EnemyData[] enemyDatas = (user as Enemy).GetServants();
        if (enemyDatas.Length <= 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        for (int i = 0; i < amount; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            ActiveSkillManager.Instance.StartCoroutine(SpawnServant, character, enemyDatas[idx], randDelay, amount);
        }
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void SpawnServant(Character user, object servantData, float amount)
    {
        EnemyManager.Instance.Generate(RoomManager.Instance.SpawnedServant(), servantData as EnemyData, user);
    }
}
