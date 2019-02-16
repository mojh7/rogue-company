using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CRandomDropImporter", menuName = "SkillData/CRandomDropImporter")]
public class CRandomDropImporter : SkillData
{
    static string alert = "alert";
    [Space(30)]

    [SerializeField]
    float speed, acceleration;
    [SerializeField]
    string animName;
    [SerializeField]
    StatusEffectInfo statusEffectInfo;
    [SerializeField]
    List<SkillData> skillData;
    [Header("오브젝트에 붙어서 호출되는 파티클")]
    [SerializeField]
    string attachedParticleName;
    [Header("계속해서 글로벌 호출되는 파티클")]
    [SerializeField]
    string particleName;
    [SerializeField]
    float particleTerm;
    [SerializeField]
    bool isDestroy;

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

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        Vector3 destPos = RoomManager.Instance.GetCurrentRoomAvailableArea();
        Vector3 srcPos = destPos + new Vector3(Random.Range(-1, 2), 2, 0);

        GameObject ground = ResourceManager.Instance.skillPool.GetPooledObject();
        CollisionSkillObject groundSkill = ground.AddComponent<CollisionSkillObject>();
        groundSkill.Init(ref caster, this, time);
        groundSkill.SetAnim(alert);

        ground.transform.position = destPos;
        gameObject.transform.position = srcPos;
        ProjectileSkillObject skillObject = gameObject.AddComponent<ProjectileSkillObject>();
        skillObject.SetSkillData(null, skillData);
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Set(statusEffectInfo);
        skillObject.Set(animName, speed, acceleration, destPos - srcPos);
        skillObject.SetReachBoom(destPos);
        skillObject.Set(attachedParticleName);
        skillObject.Set(particleName, particleTerm);
        skillObject.Set(isDestroy, true);
        return BT.State.SUCCESS;
    }
}
