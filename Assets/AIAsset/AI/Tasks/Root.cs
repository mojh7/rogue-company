using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// 무조건 루트에 있어야함. 데이터 저장을 위한 Blackboard와 스케쥴링을 위한 Clock 변수가 담겨있습니다.
/// </summary>
public class Root : DecorateTask
{
    /// <summary>
    /// 데이터 저장소
    /// </summary>
    private BlackBoard blackboard;
    public new BlackBoard BlackBoard
    {
        get
        {
            return blackboard;
        }
    }
    private Clock clock;
    public new Clock Clock
    {
        get
        {
            return clock;
        }
    }
    /// <summary>
    /// 데이터 저장소 및 스케줄러 생성
    /// </summary>
    /// <param name="blackboard">데이터 저장소</param>
    public void Init(BlackBoard blackboard)
    {
        this.blackboard = blackboard;
        this.clock = UnityContext.GetClock();
        RootTask = this;
        GetChildren().Init(this);
    }
    public override bool Run()
    {
        GetChildren().Run();
        return true;
    }
    public override Task Clone()
    {
        Root parent = ScriptableObject.CreateInstance<Root>();
        parent.AddChild(GetChildren().Clone());

        return parent;
    }
}
