namespace BT
{
    public enum TaskType
    {
        CompositeTask,
        DecorateTask,
        ActionTask
    }

    public enum ECompositeTask
    {
        Service,
        Selector,
        Sequence,
        SubSelector,
        RandomSelector
    }

    public enum EDecorateTask
    {
        Root,
        Bool,
        DistanceDecorate,
        TimeDecorate,
        HealthDecorate
    }

    public enum EActionTask
    {
        CharacterDeadAction,
        AStarTrackAtion,
        RoundingTrackAction,
        RushTrackAtion,
        SkillAction,
        StopTrackAction,
        RunawayTrackAction,
        ShotAction,
        PositionTrackAction
    }
}
