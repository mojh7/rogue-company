using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BehaviorTree : MonoBehaviour
    {
        public Character character;
        public Character target;
        Selector root;
        private void Start()
        {
            root = new Selector();
            CharacterDead characterDead = new CharacterDead(character);
            SeekTarget seekTarget = new SeekTarget(character, target);
            root.AddChild(characterDead);
            root.AddChild(seekTarget);
        }

        public void Active()
        {
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

        public void Init(Task task)
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


    class CharacterDead : Action
    {
        public CharacterDead(Character character) : base(character)
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
    class SeekTarget : Action
    {
        Character target;
        public SeekTarget(Character character,Character target) : base(character)
        {
            this.target = target;
        }
        public override bool Run( )
        {
            character.GetComponent<TempChar>().Move(target.transform.position);
            return true;
        }
    }

}
