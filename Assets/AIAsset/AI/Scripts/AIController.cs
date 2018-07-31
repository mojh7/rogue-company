using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    #region behaviorTree
    BT.BehaviorTree behaviorTree;
    BT.BlackBoard privateBlackBoard;

    #endregion
    #region parameter
    public MovingPattern MovingPattern
    {
        get;
        private set;
    }
    public AttackPattern AttackPattern
    {
        get;
        private set;
    }
    public AnimationHandler AnimationHandler
    {
        get;
        private set;
    }
    #endregion

    private void Awake()
    {
        privateBlackBoard = new BT.BlackBoard();
        MovingPattern = GetComponent<MovingPattern>();
        AttackPattern = GetComponent<AttackPattern>();
    }

    #region Func
    public void Init(float speed, AnimationHandler animationHandler, BT.Task task, SkillData[] skillDatas)
    {
        //Components
        MovingPattern.Init(speed);
        this.AnimationHandler = animationHandler;
        this.AttackPattern.Init(skillDatas);
        //BehaviorTree
        if (privateBlackBoard == null)
            privateBlackBoard = new BT.BlackBoard();
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Animation"] = this.AnimationHandler;
        privateBlackBoard["Target"] = PlayerManager.Instance.GetPlayer();
        behaviorTree = new BT.BehaviorTree(privateBlackBoard, task.Clone());

        behaviorTree.Run();
    }
    public void PlayMove()
    {
        MovingPattern.Play();
    }
    public void StopMove()
    {
        MovingPattern.Stop();
    }
    public void PlayAttack()
    {
        AttackPattern.Play();
    }
    public void StopAttack()
    {
        AttackPattern.Stop();
    }
    #endregion
}
