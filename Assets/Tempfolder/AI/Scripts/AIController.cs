using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    BT.BehaviorTree behaviorTree;

    private void Update()
    {
        behaviorTree.Active();
    }
}
