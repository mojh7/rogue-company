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
    #endregion
    private void Awake()
    {
        privateBlackBoard = new BT.BlackBoard();
        MovingPattern = GetComponent<MovingPattern>();
        AttackPattern = GetComponent<AttackPattern>();
    }
    #region Func
    public void Init(float speed, Character target, AnimationHandler animationHandler, WeaponManager weaponManager, BT.Task task, SkillData[] skillDatas)
    {
        //Components
        MovingPattern.Init(speed, animationHandler);
        this.AttackPattern.Init(skillDatas, weaponManager);
        //BehaviorTree
        if (privateBlackBoard == null)
            privateBlackBoard = new BT.BlackBoard();
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Target"] = target;
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
    public void ChangeTarget(Character target)
    {
        if (null == target)
            return;
        privateBlackBoard["Target"] = target;
        MovingPattern.SwitchTarget(target.transform);
        //TODO : 아마 AI쪽에서 블랙보드 target을 쓸 필요가 있다면 업데이트 해줘야함. 아직은 필요없을 듯
    }
    #endregion
}
