using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    [SerializeField]
    int servantIdx;

    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run();
    }
    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run();
    }
    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run();
    }

    private BT.State Run()
    {
        if (!(caster || other || customObject) || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        EnemyData[] enemyDatas = (caster as Enemy).GetServants();
        if (enemyDatas.Length <= 0)
        {
            return BT.State.FAILURE;
        }
        for (int i = 0; i < amount; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            SpawnServant(caster, enemyDatas[servantIdx]);
        }
        return BT.State.SUCCESS;
    }

    private void SpawnServant(Character user, EnemyData servantData)
    {
        EnemyManager.Instance.Generate(RoomManager.Instance.SpawnedServant(), servantData, user);
    }
}
