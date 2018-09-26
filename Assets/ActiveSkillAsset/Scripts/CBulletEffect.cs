using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CBulletEffect", menuName = "SkillData/CBulletEffect")]
public class CBulletEffect : SkillData
{
    public enum EffectType { REMOVE, REFLECT }
    [SerializeField]
    float radius;
    [SerializeField]
    EffectType effectType;

    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Run();
    }

    private BT.State Run()
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        if ((Vector3)temporary == null)
            gameObject.transform.position = character.transform.position;
        else
            gameObject.transform.position = (Vector3)temporary;

        gameObject.AddComponent<CollisionSkill>().InitBullet(character as Character, delay, amount, (float)radius, effectType);

        return BT.State.SUCCESS;
    }
}
