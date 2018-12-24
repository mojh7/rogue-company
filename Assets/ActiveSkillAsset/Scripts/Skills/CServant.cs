using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CServant", menuName = "SkillData/CServant")]
public class CServant : SkillData
{
    [SerializeField]
    int servantIdx;

    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run();
    }
    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run();
    }
    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
        return Run();
    }

    private BT.State Run()
    {
        if (!(caster || other || customObject) || amount < 0)
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
            SpawnServant(caster, enemyDatas[servantIdx]);
        }
        return BT.State.SUCCESS;
    }

    private void SpawnServant(Character user, EnemyData servantData)
    {
        EnemyManager.Instance.Generate(RoomManager.Instance.SpawnedServant(), servantData, user);
    }
}
