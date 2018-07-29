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
        SubSelector
    }

    public enum EDecorateTask
    {
        Root,
        Bool,
        DistanceDecorate,
        TimeDecorate
    }

    public enum EActionTask
    {
        CharacterDeadAction,
        AStarTrackAtion,
        RoundingTrackAction,
        RushTrackAtion,
        AttackAction,
        StopAction,
        RunawayAction
    }
}
