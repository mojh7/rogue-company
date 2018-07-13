using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    #region BehaviorTree
    BT.BehaviorTree behaviorTree;
    BT.BlackBoard privateBlackBoard;
    BT.Task root;
    #endregion
    #region Components
    MovingPattern movingPattern;
    AnimationHandler animationHandler;
    #endregion


    private void Awake()
    {
        privateBlackBoard = new BT.BlackBoard();
        movingPattern = GetComponent<MovingPattern>();
    }

    public void Init(float speed, AnimationHandler animationHandler, BT.Task task)
    {
        //Components
        movingPattern.Init(speed);
        this.animationHandler = animationHandler;

        //BehaviorTree
        if (privateBlackBoard == null)
            privateBlackBoard = new BT.BlackBoard();
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Animation"] = this.animationHandler;
        privateBlackBoard["Target"] = PlayerManager.Instance.GetPlayer();
        root = task.Clone();
        behaviorTree = new BT.BehaviorTree(privateBlackBoard, root);
        behaviorTree.Run();
    }
}
