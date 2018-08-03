using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 가지고 있는 자식들을 순회하다가 자식 노드 중 하나라도 실패할 경우 순회를 중단하고 false를 반환함.
/// list 순회이므로 자식 노드의 삽입 순서가 실행 우선순위가 됩니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/Sequence")]
public class Sequence : CompositeTask
{
    public override bool Run()
    {
        foreach (var task in GetChildren())
        {
            if (!task.Run())
                return false;
        }
        return true;
    }
    public override Task Clone()
    {
        Sequence parent = new Sequence();
        if (GetChildren() != null)
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
        return parent;
    }
}