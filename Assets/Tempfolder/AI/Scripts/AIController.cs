using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    BT.BehaviorTree behaviorTree;
    BT.BlackBoard privateBlackBoard;
    private void Awake()
    {
        privateBlackBoard = new BT.BlackBoard();
    }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Target"] = PlayerManager.Instance.GetPlayer();

        behaviorTree = new BT.BehaviorTree(privateBlackBoard);
        behaviorTree.Start();
    }
}
