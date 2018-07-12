using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BT
{
    public class BehaviorTree 
    {
        readonly BlackBoard blackBoard;
        Root root;

        public BehaviorTree(BlackBoard blackBoard, Task root)
        {
            this.blackBoard = blackBoard;

            this.root = root as Root;
            this.root.Init(blackBoard);
        }

        public void Run()
        {
            root.Run();
        }
    }
}
