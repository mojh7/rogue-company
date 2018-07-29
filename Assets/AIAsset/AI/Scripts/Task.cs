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
        public virtual bool SubRun()
        {
            return true;
        }
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
        protected AnimationHandler animationHandler;
        public override void Init(Task task)
        {
            base.Init(task);
            animationHandler = RootTask.BlackBoard["Animation"] as AnimationHandler;
        }
        public override bool Run()
        {
            return success;
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
    #endregion
}