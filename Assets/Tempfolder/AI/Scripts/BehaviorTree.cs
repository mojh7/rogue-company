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

    abstract class Task : ScriptableObject
    {
        public Root RootTask;

        protected virtual void SetRoot(Root rootTask)
        {
            this.RootTask = rootTask;
        }

        public virtual BlackBoard BlackBoard
        {
            get
            {
                return RootTask.BlackBoard;
            }
        }

        public virtual Clock Clock
        {
            get
            {
                return RootTask.Clock;
            }
        }

        public abstract bool Run();

        public virtual void AddChild(Task task)
        {
            task.SetRoot(this.RootTask);
        }
    }
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
        protected List<Task> GetChildren()
        {
            return mChildren;
        }
    }
    abstract class DecorateTask : Task
    {
        [SerializeField]
        private Task mChildren;

        public override void AddChild(Task task)
        {
            base.AddChild(task);
            mChildren = task;
        }
        protected Task GetChildren()
        {
            return mChildren;
        }
    }
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
    class Root : DecorateTask
    {
        private BlackBoard blackboard;
        public override BlackBoard BlackBoard
        {
            get
            {
                return blackboard;
            }
        }
        private Clock clock;
        public override Clock Clock
        {
            get
            {
                return clock;
            }
        }
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
    [CreateAssetMenu(fileName = "Task", menuName = "Task/Service")]
    class Service : CompositeTask
    {
        float frequency;
        private float randomVariation;

        public Service(float frequency)
        {
            this.frequency = frequency;
            this.randomVariation = frequency * 0.05f;

        }

        public override bool Run()
        {
            Clock.AddTimer(frequency, randomVariation, -1, Run);

            foreach (var task in GetChildren())
            {
                task.Run();
            }
            return true;
        }
    }
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
    [CreateAssetMenu(fileName = "Task", menuName = "Task/Sequence")]
    class Sequence : CompositeTask
    {
        public override bool Run()
        {
            foreach (var task in GetChildren())
            {
                if (!task.Run())
                    return true;
            }
            return false;
        }
    }
    #endregion

    #region Decoator
    [CreateAssetMenu(fileName = "Task", menuName = "Task/DistanceLessDecorate")]
    class DistanceLessDecorate : DecorateTask
    {
        Character character;
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
        Character character;
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
