using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 일정 주파수마다 실행되는 노드 한번 실행할 때마다 Clock에 저장,업데이트가 됩니다.
/// </summary>
[CreateAssetMenu(menuName = "Task/Service")]
public class Service  : CompositeTask
{
    [SerializeField]
    float frequency;
    bool isRun;
    public float Value
    {
        get
        {
            return frequency;
        }
    }

    public Task Set(float frequency)
    {
        this.frequency = frequency;
        return this;
    }
    public override void Init(Task task)
    {
        base.Init(task);
        this.character = RootTask.BlackBoard["Character"] as Character;
        isRun = false;
    }
    /// <summary>
    /// Clcok 스케줄러에 Run함수를 저장하여 주파수마다 실행되도록 합니다.
    /// </summary>
    /// <returns></returns>
    public override State Run()
    {
        if (character == null)
        {
            Clock.RemoveTimer(Run);
            return State.FAILURE;
        }
        if (!isRun)
        {
            Clock.AddTimer(frequency, -1, Run);
            isRun = true;
        }
        foreach (var task in GetChildren())
        {
            task.Run();
        }
        return State.SUCCESS;
    }
    public override Task Clone()
    {
        Service parent = ScriptableObject.CreateInstance<Service>();
        parent.Set(Probability);
        parent.Set(frequency);
        if (GetChildren() != null)
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
        return parent;
    }
}