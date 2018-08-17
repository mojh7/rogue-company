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

        public List<TaskNode> childrens;
        private string inputTitle = "";
        private Rect inputRect;
        private TaskNode input;
        public List<TaskNode> parents;

        BehaviorCondition behaviorCondition;
        float value1;
        float value2;

        public TaskType taskType;
        public ECompositeTask compositeTask;
        public EDecorateTask decorateTask;
        public EActionTask actionTask;

        public TaskNode()
        {
            childrens = new List<TaskNode>();
            parents = new List<TaskNode>();
        }

        public TaskNode(Task task)
        {
            childrens = new List<TaskNode>();
            parents = new List<TaskNode>();
            System.Type type = task.GetType();
            inputTitle = task.name;
            switch (type.ToString())
            {
                case "Service":
                    taskType = TaskType.CompositeTask;
                    compositeTask = ECompositeTask.Service;
                    value1 = (task as Service).Value;
                    break;
                case "Selector":
                    taskType = TaskType.CompositeTask;
                    compositeTask = ECompositeTask.Selector;
                    break;
                case "Sequence":
                    taskType = TaskType.CompositeTask;
                    compositeTask = ECompositeTask.Sequence;
                    break;
                case "SubSelector":
                    taskType = TaskType.CompositeTask;
                    compositeTask = ECompositeTask.SubSelector;
                    break;
                case "Root":
                    taskType = TaskType.DecorateTask;
                    decorateTask = EDecorateTask.Root;
                    break;
                case "Bool":
                    taskType = TaskType.DecorateTask;
                    decorateTask = EDecorateTask.Bool;
                    break;
                case "DistanceDecorate":
                    taskType = TaskType.DecorateTask;
                    decorateTask = EDecorateTask.DistanceDecorate;
                    value1 = (task as DistanceDecorate).Value;
                    behaviorCondition = (task as DistanceDecorate).Condition;
                    break;
                case "TimeDecorate":
                    taskType = TaskType.DecorateTask;
                    decorateTask = EDecorateTask.TimeDecorate;
                    value1 = (task as TimeDecorate).Value;
                    behaviorCondition = (task as TimeDecorate).Condition;
                    break;
                case "HealthDecorate":
                    taskType = TaskType.DecorateTask;
                    decorateTask = EDecorateTask.HealthDecorate;
                    value1 = (task as HealthDecorate).Value;
                    behaviorCondition = (task as HealthDecorate).Condition;
                    break;
                case "CharacterDeadAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.CharacterDeadAction;
                    break;
                case "AStarTrackAtion":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.AStarTrackAtion;
                    break;
                case "RoundingTrackAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.RoundingTrackAction;
                    break;
                case "RushTrackAtion":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.RushTrackAtion;
                    break;
                case "SkillAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.SkillAction;
                    value1 = (task as SkillAction).Value;
                    break;
                case "StopAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.StopAction;
                    break;
                case "RunawayAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.RunawayAction;
                    break;
                case "ShotAction":
                    taskType = TaskType.ActionTask;
                    actionTask = EActionTask.ShotAction;
                    value1 = (task as ShotAction).Value;
                    break;
            }
        }

        public TaskNode(bool isRoot)
        {
            childrens = new List<TaskNode>();
            taskType = TaskType.DecorateTask;
            decorateTask = EDecorateTask.Root;
        }

        public TaskNode ClickedOnInput(Vector2 pos)
        {
            TaskNode retVal = null;

            pos.x -= windowRect.x;
            pos.y -= windowRect.y;

            if (inputRect.Contains(pos))
            {
                retVal = input;
                input = null;
                if(parents.Contains(retVal))
                {
                    parents.Remove(retVal);
                }
            }

            return retVal;
        }

        public void DrawCurves()
        {
            //if (input)
            //{
            //    Rect rect = windowRect;
            //    rect.x += inputRect.x;
            //    rect.y += inputRect.y + inputRect.height / 2;
            //    rect.width = 1;
            //    rect.height = 1;

            //    NodeEditor.DrawNodeCurve(input.windowRect, rect);
            //}
            if (parents == null)
                return;
            for(int i=0;i<parents.Count;i++)
            {
                Rect rect = windowRect;
                rect.x += inputRect.x;
                rect.y += inputRect.y + inputRect.height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(parents[i].windowRect, rect);
            }

        }

        public void DrawWindow()
        {
            Event e = Event.current;
            inputTitle = (string)EditorGUILayout.TextField("Title", inputTitle);
            taskType = (TaskType)EditorGUILayout.EnumPopup("Task Type", taskType);
            switch (taskType)
            {
                case TaskType.CompositeTask:
                    compositeTask = (ECompositeTask)EditorGUILayout.EnumPopup("CompositeTask", compositeTask);
                    windowTitle = compositeTask.ToString();
                    break;
                case TaskType.DecorateTask:
                    decorateTask = (EDecorateTask)EditorGUILayout.EnumPopup("DecorateTask", decorateTask);
                    windowTitle = decorateTask.ToString();
                    break;
                case TaskType.ActionTask:
                    actionTask = (EActionTask)EditorGUILayout.EnumPopup("ActionTask", actionTask);
                    windowTitle = actionTask.ToString();
                    break;
            }
            if (inputTitle != "")
                windowTitle = inputTitle;
            if (decorateTask == EDecorateTask.DistanceDecorate || decorateTask == EDecorateTask.TimeDecorate)
            {
                behaviorCondition = (BehaviorCondition)EditorGUILayout.EnumPopup("BehaviorCondition", behaviorCondition);
            }
            value1 = EditorGUILayout.FloatField("Value1", value1);
            value2 = EditorGUILayout.FloatField("Value2", value2);

            if (e.type == EventType.Repaint)
            {
                inputRect = GUILayoutUtility.GetLastRect();

            }
        }

        public void NodeDeleted(TaskNode node)
        {
            if (node.Equals(input))
            {
                input = null;
            }

        }

        public void SetInput(TaskNode input, Vector2 clickPos)
        {
            clickPos.x -= windowRect.x;
            clickPos.y -= windowRect.y;
            if (inputRect.Contains(clickPos))
            {
                this.input = input;
                if (!input.childrens.Contains(this))
                    input.childrens.Add(this);
                parents.Add(input);
            }
            return;
        }

        public Task CreateBehaviorNode()
        {
            //TODO: AI 노드 추가마다 설정
            switch (taskType)
            {
                case TaskType.CompositeTask:
                    switch (compositeTask)
                    {
                        case ECompositeTask.Service:
                            return ScriptableObject.CreateInstance<Service>().Set(value1);
                        case ECompositeTask.Selector:
                            return ScriptableObject.CreateInstance<Selector>();
                        case ECompositeTask.Sequence:
                            return ScriptableObject.CreateInstance<Sequence>();
                        case ECompositeTask.SubSelector:
                            return ScriptableObject.CreateInstance<SubSelector>();
                    }
                    break;
                case TaskType.DecorateTask:
                    switch (decorateTask)
                    {
                        case EDecorateTask.Root:
                            return ScriptableObject.CreateInstance<Root>();
                        case EDecorateTask.Bool:
                            return ScriptableObject.CreateInstance<BoolDecorate>();
                        case EDecorateTask.DistanceDecorate:
                            return ScriptableObject.CreateInstance<DistanceDecorate>().Set(behaviorCondition, value1);
                        case EDecorateTask.TimeDecorate:
                            return ScriptableObject.CreateInstance<TimeDecorate>().Set(behaviorCondition, value1);
                        case EDecorateTask.HealthDecorate:
                            return ScriptableObject.CreateInstance<HealthDecorate>().Set(behaviorCondition, value1);
                    }
                    break;
                case TaskType.ActionTask:
                    switch (actionTask)
                    {
                        case EActionTask.CharacterDeadAction:
                            return ScriptableObject.CreateInstance<CharacterDeadAction>();
                        case EActionTask.AStarTrackAtion:
                            return ScriptableObject.CreateInstance<AStarTrackAtion>().Set(value1);
                        case EActionTask.RoundingTrackAction:
                            return ScriptableObject.CreateInstance<RoundingTrackAction>().Set(value1,value2);
                        case EActionTask.RushTrackAtion:
                            return ScriptableObject.CreateInstance<RushTrackAtion>().Set(value1);
                        case EActionTask.SkillAction:
                            return ScriptableObject.CreateInstance<SkillAction>().Set((int)value1);
                        case EActionTask.StopAction:
                            return ScriptableObject.CreateInstance<StopTrackAction>();
                        case EActionTask.RunawayAction:
                            return ScriptableObject.CreateInstance<RunawayTrackAction>().Set(value1);
                        case EActionTask.ShotAction:
                            return ScriptableObject.CreateInstance<ShotAction>().Set((int)value1);

                    }
                    break;
            }
            return null;
        }

        public void AddChild(TaskNode taskNode)
        {
            childrens.Add(taskNode);
            taskNode.parents.Add(this);
        }
    }
}
