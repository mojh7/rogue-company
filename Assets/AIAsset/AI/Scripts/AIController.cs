using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    BT.BehaviorTree behaviorTree;
    BT.BlackBoard privateBlackBoard;
    BT.Task root;

    private void Awake()
    {
        privateBlackBoard = new BT.BlackBoard();
    }

    public void Init(BT.Task task)
    {
        if(privateBlackBoard == null)
            privateBlackBoard = new BT.BlackBoard();
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Target"] = PlayerManager.Instance.GetPlayer();
        root = task.Clone();
        behaviorTree = new BT.BehaviorTree(privateBlackBoard, root);
        behaviorTree.Run();
    }

}
