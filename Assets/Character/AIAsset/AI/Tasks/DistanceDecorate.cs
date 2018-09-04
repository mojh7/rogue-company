using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 거리 조건에 따라 자식의 수행 여부를 정하는 조건 노드입니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/DistanceDecorate")]
public class DistanceDecorate : ConditionDecorate
{
    Character target;
    [SerializeField]
    float distance;
    private RaycastHit2D hit;
    private LayerMask layer;
    private bool observe;
    private float obeserveTime;
    private float elapsedTime = 0;
    public float Value
    {
        get
        {
            return distance;
        }
    }
    public Task Set(BehaviorCondition condition, float distance)
    {
        this.condition = condition;
        this.distance = distance;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        this.target = RootTask.BlackBoard["Target"] as Character;
        this.obeserveTime = 1f;
        this.elapsedTime = 0;
        layer = 1 << LayerMask.NameToLayer("TransparentFX");
        observe = false;
    }
    public override State Run()
    {
        if (Check(Vector2.Distance(character.transform.position, target.transform.position), distance))
        {
            hit = Physics2D.Raycast(character.transform.position, target.transform.position - character.transform.position, distance, layer);
            if (hit.collider)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= obeserveTime)
                {
                    observe = false;
                    return State.FAILURE;
                }
                return State.SUCCESS;
            }
            else
            {
                elapsedTime = 0;

                if (!observe)
                {
                    ParticleManager.Instance.PlayParticle("ExclamationMark", character.transform.position + Vector3.up);
                }
                observe = true;
                return GetChildren().Run();
            }
        }
        else
        {
            observe = false;
            return State.FAILURE;
        }
    }
    public override Task Clone()
    {
        DistanceDecorate parent = ScriptableObject.CreateInstance<DistanceDecorate>();
        parent.Set(condition, distance);
        if (GetChildren() != null)
            parent.AddChild(GetChildren().Clone());

        return parent;
    }
}