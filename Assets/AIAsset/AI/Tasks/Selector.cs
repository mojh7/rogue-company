using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 가지고 있는 자식들을 순회하다가 자식 노드가 성공적으로 수행될 경우 순회를 중단하고 true를 반환함.
/// list 순회이므로 자식 노드의 삽입 순서가 실행 우선순위가 됩니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/Selector")]
public class Selector : CompositeTask
{
    public override State Run()
    {
        foreach (var task in GetChildren())
        {
            State state = task.Run();

            if (state == State.SUCCESS || state == State.CONTINUE)
            {
                return state;
            }
        }
        return State.FAILURE;
    }
    public override Task Clone()
    {
        Selector parent = ScriptableObject.CreateInstance<Selector>();
        if (GetChildren() != null)
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
        return parent;
    }
}