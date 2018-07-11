using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    BT.BehaviorTree behaviorTree;
    BT.BlackBoard privateBlackBoard;
    [SerializeField]
    BT.Task root;

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
        if(privateBlackBoard == null)
            privateBlackBoard = new BT.BlackBoard();
        privateBlackBoard["Character"] = this.GetComponent<Character>();
        privateBlackBoard["Target"] = PlayerManager.Instance.GetPlayer();
        root = root.Clone();
        behaviorTree = new BT.BehaviorTree(privateBlackBoard, root);

        behaviorTree.Run();
    }
}
