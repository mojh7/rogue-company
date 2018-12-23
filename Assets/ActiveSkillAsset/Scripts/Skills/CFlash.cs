using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;
[CreateAssetMenu(fileName = "CFlash", menuName = "SkillData/CFlash")]
public class CFlash : SkillData
{
    public override State Run(CustomObject customObject, Vector3 pos)
    {
        base.Run(customObject, pos);
        return Run(RoomManager.Instance.GetCurrentRoomAvailableArea());
    }

    public override State Run(Character caster, Vector3 pos)
    {
        base.Run(caster, pos);
        return Run(RoomManager.Instance.GetNearestAvailableArea(caster.GetPosition() + caster.GetDirVector()));
    }

    public override State Run(Character caster, Character other, Vector3 pos)
    {
        base.Run(caster, other, pos);
        return Run(RoomManager.Instance.GetNearestAvailableArea(other.GetPosition()));
    }

    private BT.State Run(Vector3 destPos)
    {
        if (!(caster || other || customObject) || amount < 0)
        {
            return BT.State.FAILURE;
        }
        if (mPos != ActiveSkillManager.nullVector)
            destPos = mPos;

        caster.transform.position = destPos;
        return BT.State.SUCCESS;
    }
}
