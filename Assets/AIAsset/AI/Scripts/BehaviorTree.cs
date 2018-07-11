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
            DistanceDecorate distanceLessDecorate6 = new DistanceDecorate(BehaviorCondition.LESS, 6);
            DistanceDecorate distanceGreaterDecorate2 = new DistanceDecorate(BehaviorCondition.GREATER, 2);
            DistanceDecorate distanceLessDecorate2 = new DistanceDecorate(BehaviorCondition.LESS, 2);
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

    #region baseNode
    /// <summary>
    /// 행동 트리 기본 추상 노드
    /// </summary>
    public  abstract class Task : ScriptableObject
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
    public abstract class CompositeTask : Task
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
    public abstract class DecorateTask : Task
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
    public abstract class ActionTask : Task
    {
        protected bool success;
        protected ActionTask()
        {
        }
        public override bool Run()
        {
            return success;
        }
    }
    #endregion

    #region Composite
    /// <summary>
    /// 가지고 있는 자식들을 순회하다가 자식 노드가 성공적으로 수행될 경우 순회를 중단하고 true를 반환함.
    /// list 순회이므로 자식 노드의 삽입 순서가 실행 우선순위가 됩니다.
    /// </summary>
    public class Selector : CompositeTask
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
    public class Sequence : CompositeTask
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
        public Root()
        {
            this.clock = UnityContext.GetClock();
            SetRoot(this);
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
    public class Service : CompositeTask
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
    /// 조건에 따라 자식의 수행 여부를 정하는 추상 조건 노드입니다.
    /// </summary>
    public abstract class ConditionDecorate : DecorateTask
    {
        BehaviorCondition condition;

        public ConditionDecorate(BehaviorCondition condition)
        {
            this.condition = condition;
        }
        /// <summary>
        /// 조건 체크
        /// </summary>
        /// <param name="a">값</param>
        /// <param name="b">비교 기준</param>
        /// <returns></returns>
        protected bool Check(float a, float b)
        {
            switch (condition)
            {
                case BehaviorCondition.LESS:
                    if (a < b)
                        return true;
                    break;
                case BehaviorCondition.GREATER:
                    if (a > b)
                        return true;
                    break;
                case BehaviorCondition.EQUAL:
                    if (a == b)
                        return true;
                    break;
                case BehaviorCondition.LESSOREQUAL:
                    if (a <= b)
                        return true;
                    break;
                case BehaviorCondition.GREATEROREUAQL:
                    if (a >= b)
                        return true;
                    break;
                default:
                    break;
            }

            return false;
        }
    }
    /// <summary>
    /// 거리 조건에 따라 자식의 수행 여부를 정하는 조건 노드입니다.
    /// </summary>
    public class DistanceDecorate : ConditionDecorate
    {
        [SerializeField]
        Character target;
        [SerializeField]
        float distance;
        public DistanceDecorate(BehaviorCondition condition, float distance) : base(condition)
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
            if(Check(Vector2.Distance(character.transform.position, target.transform.position),distance))
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
    public class CharacterDeadAction : ActionTask
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
    public class AStarTrackAtion : ActionTask
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
    public class RoundingTrackAction : ActionTask
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
    public class RushTrackAtion : ActionTask
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
    public class AttackAction : ActionTask
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
