using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

[CreateAssetMenu(fileName = "CSpoonAttack", menuName = "SkillData/CSpoonAttack")]
public class CSpoonAttack : SkillData
{
    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run(Vector3.zero);
    }
    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run(Vector3.zero);
    }
    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run(other.transform.position - caster.transform.position);
    }

    private BT.State Run(Vector3 dir)
    {
        if (delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        caster.isCasting = true;
        SpoonAttack(caster, dir, amount);
        caster.isCasting = false;
        return BT.State.SUCCESS;
    }

    private void SpoonAttack(Character caster, Vector2 dir, float amount)
    {
        dir.Normalize();

        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = caster.transform.position + new Vector3(dir.x, dir.y, 0);
        CollisionSkillObject skillObject = gameObject.AddComponent<CollisionSkillObject>();
        if (other)
            skillObject.Init(other);
        skillObject.Init(ref caster, this, time);
        skillObject.Init("spoonAttack");
    }

}