using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BehaviorTree : MonoBehaviour
    {
        Character character;
        Selector root;
        bool isActive;

        public void Init()
        {
            character = this.GetComponent<Character>();
            root = new Selector(); 
            Selector selector = new Selector();
            CharacterDeadAction characterDead = new CharacterDeadAction(character);
            DistanceLessDecorate distanceLessDecorate6 = new DistanceLessDecorate(character, 6);
            DistanceGreaterDecorate distanceGreaterDecorate2 = new DistanceGreaterDecorate(character, 2);
            DistanceLessDecorate distanceLessDecorate2 = new DistanceLessDecorate(character, 2);
            RoundingTrackAction roundingTrackAction = new RoundingTrackAction(character, 2);
            AStarTrackAtion aStarTrackAtion = new AStarTrackAtion(character);
            RushTrackAtion rushTrackAtion = new RushTrackAtion(character);
            root.AddChild(selector);
                selector.AddChild(characterDead);
                selector.AddChild(distanceLessDecorate6);
                    distanceLessDecorate6.AddChild(distanceGreaterDecorate2);
                        distanceGreaterDecorate2.AddChild(rushTrackAtion);
                selector.AddChild(distanceLessDecorate2);
                    distanceLessDecorate2.AddChild(roundingTrackAction);
                selector.AddChild(aStarTrackAtion);

            isActive = true;
        }
        private void Update()
        {
            if(isActive)
                root.Run();
        }
 
    }
 
    #region baseNode

    abstract class Task
    {
        public abstract bool Run();
    }
    abstract class CompositeTask : Task
    {
        private List<Task> mChildren;

        protected CompositeTask() { mChildren = new List<Task>(); }

        public void AddChild(Task task)
        {
            mChildren.Add(task);
        }
        protected List<Task> GetChildren()
        {
            return mChildren;
        }
    }
    abstract class DecorateTask : Task
    {
        private Task mChildren;

        public void AddChild(Task task)
        {
            mChildren = task;
        }
        protected Task GetChildren()
        {
            return mChildren;
        }
    }
    abstract class Action : Task
    {
        protected Character character;
        protected bool success;
        protected Action(Character character)
        {
            this.character = character;
        }
        public override bool Run()
        {
            return success;
        }
    }
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
    class DistanceLessDecorate : DecorateTask
    {
        Character character;
        Character target;
        float distance;
        public DistanceLessDecorate(Character character, float distance)
        {
            this.character = character;
            this.target = BlackBoard.Instance.data["Player"] as Character;
            this.distance = distance;
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
    class DistanceGreaterDecorate : DecorateTask
    {
        Character character;
        Character target;
        float distance;
        public DistanceGreaterDecorate(Character character, float distance)
        {
            this.character = character;
            this.target = BlackBoard.Instance.data["Player"] as Character;
            this.distance = distance;
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
    class CharacterDeadAction : Action
    {
        public CharacterDeadAction(Character character) : base(character)
        {
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
    class AStarTrackAtion : Action
    {
        MovingPattern movingPattern;

        public AStarTrackAtion(Character character) : base(character)
        {
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.AStarTracker();
        }
        public override bool Run()
        {
            return movingPattern.AStarTracking();
        }
    }
    class RoundingTrackAction : Action
    {
        MovingPattern movingPattern;
        float radius;

        public RoundingTrackAction(Character character, float radius) : base(character)
        {
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RoundingTracker(radius);
            this.radius = radius;
        }
        public override bool Run()
        {
            return movingPattern.RoundingTracking();
        }
    }
    class RushTrackAtion : Action
    {
        MovingPattern movingPattern;

        public RushTrackAtion(Character character) : base(character)
        {
            movingPattern = character.GetComponent<MovingPattern>();
            movingPattern.RushTracker();
        }
        public override bool Run()
        {
            return movingPattern.RushTracking();
        }
    }

    class AttackAction : Action
    {

        public AttackAction(Character character) : base(character)
        {
        }
        public override bool Run()
        {
            return false;
        }
    }
    #endregion

}
