using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BT
{
    class TaskNode : ScriptableObject
    {
        public Rect windowRect;
        public string windowTitle = "";

        private TaskNode input1;
        private Rect input1Rect;

        private TaskNode input2;
        private Rect input2Rect;
        public List<TaskNode> childrens;
        public TaskNode parent;

        BehaviorCondition behaviorCondition;
        float value;

        private TaskType taskType;
        private ECompositeTask compositeTask;
        private EDecorateTask decorateTask;
        private EActionTask actionTask;

        public TaskNode()
        {
            windowTitle = "Task";
            childrens = new List<TaskNode>();
        }

        public TaskNode(bool isRoot)
        {
            windowTitle = "Root";
            childrens = new List<TaskNode>();
            taskType = TaskType.DecorateTask;
            decorateTask = EDecorateTask.Root;
        }

        public TaskNode ClickedOnInput(Vector2 pos)
        {
            TaskNode retVal = null;

            pos.x -= windowRect.x;
            pos.y -= windowRect.y;

            if (input1Rect.Contains(pos))
            {
                retVal = input1;
                input1 = null;
            }
            else if (input2Rect.Contains(pos))
            {
                retVal = input2;
                input2 = null;
            }

            return retVal;
        }

        public void DrawCurves()
        {
            if (input1)
            {
                Rect rect = windowRect;
                rect.x += input1Rect.x;
                rect.y += input1Rect.y + input1Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(input1.windowRect, rect);
            }

            if (input2)
            {
                Rect rect = windowRect;
                rect.x += input2Rect.x;
                rect.y += input2Rect.y + input2Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(input2.windowRect, rect);
            }
        }

        public void DrawWindow()
        {
            Event e = Event.current;
            taskType = (TaskType)EditorGUILayout.EnumPopup("Task Type", taskType);
            switch (taskType)
            {
                case TaskType.CompositeTask:
                    compositeTask = (ECompositeTask)EditorGUILayout.EnumPopup("CompositeTask", compositeTask);
                    break;
                case TaskType.DecorateTask:
                    decorateTask = (EDecorateTask)EditorGUILayout.EnumPopup("DecorateTask", decorateTask);      
                    break;
                case TaskType.ActionTask:
                    actionTask = (EActionTask)EditorGUILayout.EnumPopup("ActionTask", actionTask);
                    break;
            }
            if(taskType == TaskType.DecorateTask && decorateTask != EDecorateTask.Root)
            {
                if (decorateTask == EDecorateTask.DistanceDecorate)
                {
                    behaviorCondition = (BehaviorCondition)EditorGUILayout.EnumPopup("BehaviorCondition", behaviorCondition);
                }
                value = EditorGUILayout.FloatField("Value", value);
            }

            if (e.type == EventType.Repaint)
            {
                input1Rect = GUILayoutUtility.GetLastRect();

            }

            if (e.type == EventType.Repaint)
            {
                input2Rect = GUILayoutUtility.GetLastRect();

            }
        }

        public void NodeDeleted(TaskNode node)
        {
            if (node.Equals(input1))
            {
                input1 = null;
            }

            if (node.Equals(input2))
            {
                input2 = null;
            }
        }

        public void SetInput(TaskNode input, Vector2 clickPos)
        {
            clickPos.x -= windowRect.x;
            clickPos.y -= windowRect.y;

            if (input1Rect.Contains(clickPos))
            {
                input1 = input;
                input.childrens.Add(this);
                parent = input;
            }
            else if (input2Rect.Contains(clickPos))
            {
                input2 = input;
                input.childrens.Add(this);
                parent = input;
            }
        }

        public Task CreateBehaviorNode()
        {
            switch (taskType)
            {
                case TaskType.CompositeTask:
                    switch (compositeTask)
                    {
                        case ECompositeTask.Selector:
                            return new Selector();
                        case ECompositeTask.Sequence:
                            return new Sequence();
                    }
                    break;
                case TaskType.DecorateTask:
                    switch (decorateTask)
                    {
                        case EDecorateTask.Root:
                            return new Root();
                        case EDecorateTask.Service:
                            return new Service(value);
                        case EDecorateTask.DistanceDecorate:
                            return new DistanceDecorate(BehaviorCondition.EQUAL, value);
                    }
                    break;
                case TaskType.ActionTask:
                    switch (actionTask)
                    {
                        case EActionTask.CharacterDeadAction:
                            return new CharacterDeadAction();
                        case EActionTask.AStarTrackAtion:
                            return new AStarTrackAtion();
                        case EActionTask.RoundingTrackAction:
                            return new RoundingTrackAction(value);
                        case EActionTask.RushTrackAtion:
                            return new RushTrackAtion();
                        case EActionTask.AttackAction:
                            return new AttackAction();
                    }
                    break;
            }
            return null;
        }

    }
}
