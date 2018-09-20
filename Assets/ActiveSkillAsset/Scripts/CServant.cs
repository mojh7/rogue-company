using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    public override BT.State Run(Character character, object parameter, int idx)
    {
        base.Run(character, temporary, idx);

        return SpawnServant(character, parameter);
    }

    private BT.State SpawnServant(Character user, object servantData)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        EnemyData[] enemyDatas = servantData as EnemyData[];
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
