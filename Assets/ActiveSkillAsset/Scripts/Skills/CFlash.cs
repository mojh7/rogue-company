using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;
[CreateAssetMenu(fileName = "CFlash", menuName = "SkillData/CFlash")]
public class CFlash : SkillData
{
    public override State Run(CustomObject customObject, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(customObject, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(RoomManager.Instance.GetCurrentRoomAvailableArea());
    }

    public override State Run(Character caster, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(RoomManager.Instance.GetNearestAvailableArea(caster.GetPosition() + caster.GetDirVector()));
    }

    public override State Run(Character caster, Character other, Vector3 pos, ref float lapsedTime)
    {
        if (State.FAILURE == base.Run(caster, other, pos, ref lapsedTime))
            return State.FAILURE;
        return Run(RoomManager.Instance.GetNearestAvailableArea(other.GetPosition() + Random.insideUnitSphere * 1));
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
