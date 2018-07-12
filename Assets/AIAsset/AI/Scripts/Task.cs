using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    #region baseNode
    /// <summary>
    /// 행동 트리 기본 추상 노드
    /// </summary>
    [SerializeField]
    public abstract class Task : ScriptableObject
    {
        public Root RootTask;
        protected Character character;

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
            //task.SetRoot(this.RootTask);
        }
        /// <summary>
        /// RootTask 설정 및 Blackboard에서 데이터 인출
        /// </summary>
        /// <param name="task">부모 Task</param>
        public virtual void Init(Task task)
        {
            RootTask = task.RootTask;
        }
        public abstract Task Clone();
    }
    /// <summary>
    /// 추상 합성 노드로 자식 노드를 List형태로 갖고있습니다.
    /// </summary>
    public abstract class CompositeTask : Task
    {
        [SerializeField]
        private List<Task> mChildren = new List<Task>();

        public override void AddChild(Task task)
        {
            base.AddChild(task);
            mChildren.Add(task);
        }
        public override void Init(Task task)
        {
            base.Init(task);
            for (int i = 0; i < mChildren.Count; i++)
            {
                mChildren[i].Init(this);
            }
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
        public override void Init(Task task)
        {
            base.Init(task);
            mChildren.Init(this);
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
        public override Task Clone()
        {
            Selector parent = new Selector();
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
            return parent;
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
        public override Task Clone()
        {
            Sequence parent = new Sequence();
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
            return parent;
        }
    }
    /// <summary>
    /// 일정 주파수마다 실행되는 노드 한번 실행할 때마다 Clock에 저장,업데이트가 됩니다.
    /// </summary>
    public class Service : CompositeTask
    {
        [SerializeField]
        float frequency;

        public Task Set(float frequency)
        {
            this.frequency = frequency;
            return this;
        }
        /// <summary>
        /// Clcok 스케줄러에 Run함수를 저장하여 주파수마다 실행되도록 합니다.
        /// </summary>
        /// <returns></returns>
        public override bool Run()
        {
            Clock.AddTimer(frequency, -1, Run);

            foreach (var task in GetChildren())
            {
                task.Run();
            }
            return true;
        }
        public override Task Clone()
        {
            Service parent = new Service();
            parent.Set(frequency);
            for (int i = 0; i < GetChildren().Count; i++)
            {
                parent.AddChild(GetChildren()[i].Clone());
            }
            return parent;
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
        /// <summary>
        /// 데이터 저장소 및 스케줄러 생성
        /// </summary>
        /// <param name="blackboard">데이터 저장소</param>
        public void Init(BlackBoard blackboard)
        {
            this.blackboard = blackboard;
            this.clock = UnityContext.GetClock();
            RootTask = this;
            GetChildren().Init(this);
        }
        public override bool Run()
        {
            GetChildren().Run();
            return true;
        }
        public override Task Clone()
        {
            Root parent = new Root();
            parent.AddChild(GetChildren().Clone());

            return parent;
        }
    }
    /// <summary>
    /// 조건에 따라 자식의 수행 여부를 정하는 추상 조건 노드입니다.
    /// </summary>
    public abstract class ConditionDecorate : DecorateTask
    {
        [SerializeField]
        protected BehaviorCondition condition;

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
        Character target;
        [SerializeField]
        float distance;
        public Task Set(BehaviorCondition condition, float distance)
        {
            this.condition = condition;
            this.distance = distance;
            return this;
        }
        public override void Init(Task task)
        {
            base.Init(task);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
        }
        public override bool Run()
        {
            if (Check(Vector2.Distance(character.transform.position, target.transform.position), distance))
            {
                return GetChildren().Run();
            }
            else
            {
                return false;
            }
        }
        public override Task Clone()
        {
            DistanceDecorate parent = new DistanceDecorate();
            parent.Set(condition, distance);
            parent.AddChild(GetChildren().Clone());

            return parent;
        }
    }
    #endregion

    #region Action
    /// <summary>
    /// 캐릭터 사망시 행동을 담은 노드입니다.
    /// </summary>
    public class CharacterDeadAction : ActionTask
    {
        public override void Init(Task task)
        {
            base.Init(task);
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
        public override Task Clone()
        {
            CharacterDeadAction parent = new CharacterDeadAction();

            return parent;
        }
    }
    /// <summary>
    /// 기본 A* 추적 행동을 담은 노드입니다.
    /// </summary>
    public class AStarTrackAtion : ActionTask
    {
        MovingPattern movingPattern;
        Character target;

        public override void Init(Task task)
        {
            base.Init(task);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.AStarTracker(target.transform);
        }
        public override bool Run()
        {
            return movingPattern.AStarTracking();
        }
        public override Task Clone()
        {
            AStarTrackAtion parent = new AStarTrackAtion();

            return parent;
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

        public Task Set(float radius)
        {
            this.radius = radius;
            return this;
        }
        public override void Init(Task task)
        {
            base.Init(task);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RoundingTracker(target.transform, radius);
        }
        public override bool Run()
        {
            return movingPattern.RoundingTracking();
        }
        public override Task Clone()
        {
            RoundingTrackAction parent = new RoundingTrackAction();
            parent.Set(radius);
            return parent;
        }
    }
    /// <summary>
    /// 기본 A* 돌진 행동을 담은 노드입니다.
    /// </summary>
    public class RushTrackAtion : ActionTask
    {
        MovingPattern movingPattern;
        Character target;

        public override void Init(Task task)
        {
            base.Init(task);
            this.character = RootTask.BlackBoard["Character"] as Character;
            this.target = RootTask.BlackBoard["Target"] as Character;
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RushTracker(target.transform);
        }
        public override bool Run()
        {
            return movingPattern.RushTracking();
        }
        public override Task Clone()
        {
            RushTrackAtion parent = new RushTrackAtion();

            return parent;
        }
    }
    /// <summary>
    /// 기본 공격 행동을 담은 노드입니다.
    /// </summary>
    public class AttackAction : ActionTask
    {
        public override bool Run()
        {
            return false;
        }
        public override Task Clone()
        {
            AttackAction parent = new AttackAction();

            return parent;
        }
    }
    #endregion
}