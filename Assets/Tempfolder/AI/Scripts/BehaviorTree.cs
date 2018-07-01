using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BT
{
    public class BehaviorTree 
    {
        BlackBoard blackBoard;
        Root root;
        public BehaviorTree(BlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
            Init();
        }

        void Init()
        {
            root = new Root(blackBoard);
            Service service = new Service(0.05f);

            Selector selector = new Selector();
            CharacterDeadAction characterDead = new CharacterDeadAction();
            DistanceLessDecorate distanceLessDecorate6 = new DistanceLessDecorate(6);
            DistanceGreaterDecorate distanceGreaterDecorate2 = new DistanceGreaterDecorate(2);
            DistanceLessDecorate distanceLessDecorate2 = new DistanceLessDecorate(2);
            RoundingTrackAction roundingTrackAction = new RoundingTrackAction(2);
            AStarTrackAtion aStarTrackAtion = new AStarTrackAtion();
            RushTrackAtion rushTrackAtion = new RushTrackAtion();
            root.AddChild(service);
            service.AddChild(selector);
                selector.AddChild(characterDead);
                selector.AddChild(distanceLessDecorate6);
                    distanceLessDecorate6.AddChild(distanceGreaterDecorate2);
                        distanceGreaterDecorate2.AddChild(rushTrackAtion);
                selector.AddChild(distanceLessDecorate2);
                    distanceLessDecorate2.AddChild(roundingTrackAction);
                selector.AddChild(aStarTrackAtion);
        }

        public void Start()
        {
            root.Run();
        }
    }

    //class BehaviorNodeData : ScriptableObject
    //{
    //    List<Task> tasks;

    //    public Task Building()
    //    {
    //        if (tasks.Count == 0)
    //            return null;
    //        Task root = tasks[0];

    //        for(int i = 0; i < tasks.Count; i++)
    //        {
    //            Task task = tasks[i];
    //            BehaivorSwitch.Instance.Switch(root,task);
    //        }
    //        return tasks[0];
    //    }

    //    public class BehaivorSwitch
    //    {
    //        private static BehaivorSwitch instance = null;
    //        static readonly object padlock = new object();
    //        Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();
    //        BehaivorSwitch Case<T>(Action<T> action) { matches.Add(typeof(T), (x) => action((T)x)); return this; }
    //        void Switch(object x) { matches[x.GetType()](x); }

    //        BehaivorSwitch()
    //        {

    //        }

    //        public static BehaivorSwitch Instance
    //        {
    //            get
    //            {
    //                if (instance == null)
    //                {
    //                    lock (padlock)
    //                    {
    //                        if (instance == null)
    //                        {
    //                            instance = new BehaivorSwitch();
    //                        }
    //                    }
    //                }
    //                return instance;
    //            }
    //        }

    //        public Task Switch(Task parent, Task child)
    //        {
    //            var caseSwitch = new BehaivorSwitch()
    //                .Case((CompositeTask x) => parent.AddChild(child))
    //                .Case((DecorateTask x) => parent.AddChild(child))
    //                .Case((ActionTask x) => parent.AddChild(child));

    //            caseSwitch.Switch(parent, child);
    //            return parent;
    //        }
    //    }
    //}

    #region baseNode
    /// <summary>
    /// 행동 트리 기본 추상 노드
    /// </summary>
    abstract class Task : ScriptableObject
    {
        public Root RootTask;
        protected Character character;

        /// <summary>
        /// 부모 노드 설정과 함께 부모 노드의 blackboard에서 데이터를 불러와 생성합니다.
        /// </summary>
        /// <param name="rootTask">루트 노드</param>
        protected virtual void SetRoot(Root rootTask)
        {
            this.RootTask = rootTask;
        }
        /// <summary>
        /// 데이터 저장소 프로퍼티
        /// </summary>
        public BlackBoard BlackBoard
        {
            get
            {
                return RootTask.BlackBoard;
            }
        }
        /// <summary>
        /// 스케쥴러 프로퍼티
        /// </summary>
        public Clock Clock
        {
            get
            {
                return RootTask.Clock;
            }
        }
        /// <summary>
        /// 노드 실행 함수
        /// </summary>
        /// <returns></returns>
        public abstract bool Run();
        /// <summary>
        /// 자식 추가 함수
        /// </summary>
        /// <param name="task">자식 노드</param>
        public virtual void AddChild(Task task)
        {
            task.SetRoot(this.RootTask);
        }
    }
    /// <summary>
    /// 추상 합성 노드로 자식 노드를 List형태로 갖고있습니다.
    /// </summary>
    abstract class CompositeTask : Task
    {
        [SerializeField]
        private List<Task> mChildren;

        protected CompositeTask() { mChildren = new List<Task>(); }

        public override void AddChild(Task task)
        {
            base.AddChild(task);
            mChildren.Add(task);
        }
        /// <summary>
        /// 자식 리스트 반환 함수
        /// </summary>
        /// <returns>자식 노드 리스트 반환</returns>
        protected List<Task> GetChildren()
        {
            return mChildren;
        }
    }
    /// <summary>
    /// 추상 데코레이터 노드로 하나의 자식을 갖고 있습니다.
    /// </summary>
    abstract class DecorateTask : Task
    {
        [SerializeField]
        private Task mChildren;

        public override void AddChild(Task task)
        {
            base.AddChild(task);
            mChildren = task;
        }
        /// <summary>
        /// 자식 반환 함수
        /// </summary>
        /// <returns>자식 노드 반환</returns>
        protected Task GetChildren()
        {
            return mChildren;
        }
    }
    /// <summary>
    /// 추상 액션 노드로 실질적인 행동(공격, 이동, 능력 증가, 패턴, 사망)을 수행하게됩니다.
    /// </summary>
    abstract class ActionTask : Task
    {
        protected Character character;
        protected bool success;
        protected ActionTask()
        {
        }
        public override bool Run()
        {
            return success;
        }
    }
    /// <summary>
    /// 무조건 루트에 있어야함. 데이터 저장을 위한 Blackboard와 스케쥴링을 위한 Clock 변수가 담겨있습니다.
    /// </summary>
    class Root : DecorateTask
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
        public Root(BlackBoard blackboard)
        {
            this.blackboard = blackboard;
            this.clock = UnityContext.GetClock();
            SetRoot(this);
        }
        public override bool Run()
        {
            GetChildren().Run();
            return true;
        }
    }
    /// <summary>
    /// 일정 주파수마다 실행되는 노드 한번 실행할 때마다 Clock에 저장,업데이트가 됩니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/Service")]
    class Service : CompositeTask
    {
        float frequency;

        public Service(float frequency)
        {
            this.frequency = frequency;
        }
        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
        }
        /// <summary>
        /// Clcok 스케줄러에 Run함수를 저장하여 주파수마다 실행되도록 합니다.
        /// </summary>
        /// <returns></returns>
        public override bool Run()
        {
            if (character == null)
                return false;
            Clock.AddTimer(frequency, -1, Run);

            foreach (var task in GetChildren())
            {
                task.Run();
            }
            return true;
        }
    }
    /// <summary>
    /// 가지고 있는 자식들을 순회하다가 자식 노드가 성공적으로 수행될 경우 순회를 중단하고 true를 반환함.
    /// list 순회이므로 자식 노드의 삽입 순서가 실행 우선순위가 됩니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/Selector")]
    class Selector : CompositeTask
    {
        public override bool Run()
        {
            foreach (var task in GetChildren())
            {
                if (task.Run())
                    return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 가지고 있는 자식들을 순회하다가 자식 노드 중 하나라도 실패할 경우 순회를 중단하고 false를 반환함.
    /// list 순회이므로 자식 노드의 삽입 순서가 실행 우선순위가 됩니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/Sequence")]
    class Sequence : CompositeTask
    {
        public override bool Run()
        {
            foreach (var task in GetChildren())
            {
                if (!task.Run())
                    return false;
            }
            return true;
        }
    }
    #endregion

    #region Decoator
    /// <summary>
    /// 조건에 따라 자식의 수행 여부를 정하는 조건 노드입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/DistanceLessDecorate")]
    class DistanceLessDecorate : DecorateTask
    {
        [SerializeField]
        Character target;
        [SerializeField]
        float distance;
        public DistanceLessDecorate(float distance)
        {
            this.distance = distance;
        }
        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
        }
        public override bool Run()
        {
            if (Vector2.Distance(character.transform.position, target.transform.position) <= distance)
            {
                return GetChildren().Run();
            }
            else
            {
                return false;
            }
        }
    }
    [CreateAssetMenu(fileName = "Task", menuName = "Task/DistanceGreaterDecorate")]
    class DistanceGreaterDecorate : DecorateTask
    {
        [SerializeField]
        Character target;
        [SerializeField]
        float distance;
        public DistanceGreaterDecorate(float distance)
        {
            this.distance = distance;
        }
        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
        }
        public override bool Run()
        {
            if (Vector2.Distance(character.transform.position, target.transform.position) > distance)
            {
                return GetChildren().Run();
            }
            else
            {
                return false;
            }
        }
    }
    #endregion

    #region Action
    /// <summary>
    /// 캐릭터 사망시 행동을 담은 노드입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/CharacterDeadAction")]
    class CharacterDeadAction : ActionTask
    {
        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
        }
        public override bool Run()
        {
            if (character.IsDie())
            {
                success = true;
            }
            else
            {
                success = false;
            }
            return base.Run();
        }

    }
    /// <summary>
    /// 기본 A* 추적 행동을 담은 노드입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/AStarTrackAtion")]
    class AStarTrackAtion : ActionTask
    {
        MovingPattern movingPattern;
        Character target;

        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.AStarTracker(target.transform);
        }
        public override bool Run()
        {
            return movingPattern.AStarTracking();
        }
    }
    /// <summary>
    /// 기본 회전 추적 행동을 담은 노드입니다. 
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/RoundingTrackAction")]
    class RoundingTrackAction : ActionTask
    {
        MovingPattern movingPattern;
        Character target;

        [SerializeField]
        float radius;

        public RoundingTrackAction(float radius)
        { 
            this.radius = radius;
        }
        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RoundingTracker(target.transform, radius);
        }
        public override bool Run()
        {
            return movingPattern.RoundingTracking();
        }
    }
    /// <summary>
    /// 기본 A* 돌진 행동을 담은 노드입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/RushTrackAtion")]
    class RushTrackAtion : ActionTask
    {
        MovingPattern movingPattern;
        Character target;

        protected override void SetRoot(Root rootTask)
        {
            base.SetRoot(rootTask);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RushTracker(target.transform);
        }
        public override bool Run()
        {
            return movingPattern.RushTracking();
        }
    }
    /// <summary>
    /// 기본 공격 행동을 담은 노드입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Task", menuName = "Task/AttackAction")]
    class AttackAction : ActionTask
    {

        public AttackAction() 
        {
        }
        public override bool Run()
        {
            return false;
        }
    }
    #endregion

}
