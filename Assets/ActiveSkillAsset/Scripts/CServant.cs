using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return SpawnServant();
    }

    private BT.State SpawnServant()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        EnemyData[] enemyDatas = (character as Enemy).GetServants();
        if (enemyDatas.Length <= 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        for (int i = 0; i < amount; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            ActiveSkillManager.Instance.StartCoroutine(SpawnServant, character, enemyDatas[idx], randDelay, amount);
        }
        character.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void SpawnServant(Character user, object servantData, float amount)
    {
        EnemyManager.Instance.Generate(RoomManager.Instance.SpawnedServant(), servantData as EnemyData, user);
    }
}
