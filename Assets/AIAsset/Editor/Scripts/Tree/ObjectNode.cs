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

        private Rect inputRect;
        private TaskNode input;
        public List<TaskNode> parents;

        BehaviorCondition behaviorCondition;
        float value;

        private TaskType taskType;
        private ECompositeTask compositeTask;
        private EDecorateTask decorateTask;
        private EActionTask actionTask;

        public TaskNode()
        {
            childrens = new List<TaskNode>();
            parents = new List<TaskNode>();
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
            if (decorateTask == EDecorateTask.DistanceDecorate || decorateTask == EDecorateTask.TimeDecorate)
            {
                behaviorCondition = (BehaviorCondition)EditorGUILayout.EnumPopup("BehaviorCondition", behaviorCondition);
            }
            value = EditorGUILayout.FloatField("Value", value);

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
                            return ScriptableObject.CreateInstance<Service>().Set(value);
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
                        case EDecorateTask.TimeDecorate:
                            return ScriptableObject.CreateInstance<TimeDecorate>().Set(behaviorCondition, value);
                        case EDecorateTask.DistanceDecorate:
                            return ScriptableObject.CreateInstance<DistanceDecorate>().Set(behaviorCondition, value);
                        case EDecorateTask.Root:
                            return ScriptableObject.CreateInstance<Root>();
                        case EDecorateTask.Bool:
                            return ScriptableObject.CreateInstance<BoolDecorate>();
                    }
                    break;
                case TaskType.ActionTask:
                    switch (actionTask)
                    {
                        case EActionTask.CharacterDeadAction:
                            return ScriptableObject.CreateInstance<CharacterDeadAction>();
                        case EActionTask.AStarTrackAtion:
                            return ScriptableObject.CreateInstance<AStarTrackAtion>();
                        case EActionTask.RoundingTrackAction:
                            return ScriptableObject.CreateInstance<RoundingTrackAction>().Set(value);
                        case EActionTask.RushTrackAtion:
                            return ScriptableObject.CreateInstance<RushTrackAtion>();
                        case EActionTask.SkillAction:
                            return ScriptableObject.CreateInstance<SkillAction>().Set((int)value);
                        case EActionTask.StopAction:
                            return ScriptableObject.CreateInstance<StopTrackAction>();
                        case EActionTask.RunawayAction:
                            return ScriptableObject.CreateInstance<RunawayTrackAction>();
                        case EActionTask.ShotAction:
                            return ScriptableObject.CreateInstance<ShotAction>().Set((int)value);

                    }
                    break;
            }
            return null;
        }

    }
}
